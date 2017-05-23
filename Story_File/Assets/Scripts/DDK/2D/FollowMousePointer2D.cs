//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Statics;



namespace DDK._2D {

	/// <summary>
	/// Attach to a gameobject to make it follow the mouse pointer.
	/// </summary>
	public class FollowMousePointer2D : MonoBehaviour {

		public bool onMouseDownOnly;
        [Tooltip( "If no reference, the main camera will be used" )]
		public Camera cam;
		public float zDepth = 0f;
		public bool hideMousePointer = true;
		public bool showMousePointerOnDisable = false;
		/// <summary>
		/// If this object triggers a collision with an object from any of the specified layers, the -target- will 
		/// reference it. Useful for 2d games
		/// </summary>
		[Tooltip("If this object triggers a collision with an object from any of the specified layers, the -target- will reference it. Useful for 2d games")]
		public int[] setTargetWhenCollideWithObjFromLayer;
		[Tooltip("This helps preventing the object from teleporting when the user taps in different locations, this doesn't happen on platforms were touch is not supported")]
		public bool doNotTeleportOnTouchableDevices = true;
		[ShowIfAttribute( "_DoNotTeleport", 1 )]
		public float towardsSpeed;	


		protected bool _UpdateCam()
		{
			if( !cam )
				cam = Camera.main;
			return true;
		}
		protected bool _DoNotTeleport()
		{
			return doNotTeleportOnTouchableDevices;
		}
		
		
		/// <summary>
		/// The object that collides with the one following the mouse pointer. Useful for 2d games. Example: Firing projectiles that should
		/// follow an enemy.
		/// </summary>
		internal GameObject _target;

		private bool _isUI;
		private Canvas _thisCanvas;
		protected bool _pointerReached;		
		
		
		void Awake()
		{
			if( !cam )
				cam = Camera.main;
			if( hideMousePointer ){
				Cursor.visible = false;
			}
            _isUI = ( GetComponent<RectTransform>() ) ? true : false;
		}
		
		// Use this for initialization
		void Start () {

            if( _isUI )
            {
                _thisCanvas = gameObject.GetCompInParent<Canvas>();
                if( _thisCanvas.renderMode == RenderMode.ScreenSpaceOverlay )
                {
                    Debug.LogError( "The Canvas must NOT be in Screen Space Overlay", gameObject );
                }
            }
		}		
		// Update is called once per frame
		void Update () {

			if( Input.GetMouseButtonUp(0) && Device.isTouchable )
			{
				_pointerReached = false;
			}

			SetPos();
		}		
		void OnDisable()
		{
			if( showMousePointerOnDisable && hideMousePointer )
			{
				Cursor.visible = true;
			}
		}		
		void OnTriggerEnter2D( Collider2D coll )
		{
			for( int i=0; i<setTargetWhenCollideWithObjFromLayer.Length; i++ )
			{
				if( coll.gameObject.layer == setTargetWhenCollideWithObjFromLayer[i] )
				{
					_target = coll.gameObject;
					break;
				}
			}
		}		
		void OnTriggerExit2D( Collider2D coll )
		{
			for( int i=0; i<setTargetWhenCollideWithObjFromLayer.Length; i++ )
			{
				if( coll.gameObject.layer == setTargetWhenCollideWithObjFromLayer[i] )
				{
					_target = null;
				}
			}
		}


		public void SetPos()
		{

			if( onMouseDownOnly && !Input.GetMouseButton(0) )
				return;

			if( doNotTeleportOnTouchableDevices && Device.isTouchable )
			{
				if( _pointerReached )
				{
					TeleportToPointer();
				}
				else
				{
					MoveTowardsPointer();
				}
			}
			else
			{
				TeleportToPointer();
			}
		}
		public Vector3 GetPointerPos()
		{
			Vector3 mPos = default( Vector3 );
			if( _isUI )
			{
				Vector2 pos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(_thisCanvas.transform as RectTransform, Input.mousePosition, _thisCanvas.worldCamera, out pos);
				Vector3 finalPos = new Vector3( pos.x, pos.y, zDepth );
				mPos = _thisCanvas.transform.TransformPoint( finalPos );
			}
			else
			{
				mPos = cam.ScreenToWorldPoint( Input.mousePosition );
				mPos = new Vector3( mPos.x, mPos.y, zDepth );
			}
			return mPos;
		}		
		public Vector3 GetNextTowardPos()
		{
			Vector3 mPos = default( Vector3 );
			if( _isUI )
			{
				Vector2 pos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(_thisCanvas.transform as RectTransform, Input.mousePosition, _thisCanvas.worldCamera, out pos);
				Vector3 finalPos = new Vector3( pos.x, pos.y, zDepth );
				mPos = _thisCanvas.transform.TransformPoint( finalPos );
			}
			else
			{
				mPos = cam.ScreenToWorldPoint( Input.mousePosition );
				mPos = new Vector3( mPos.x, mPos.y, zDepth );
			}
			return Vector3.MoveTowards( transform.position, mPos, towardsSpeed * Time.deltaTime * 100f );
		}
		public void TeleportToPointer()
		{
			transform.position = GetPointerPos();
			_pointerReached = true;
		}
		public void MoveTowardsPointer()
		{
			transform.position = GetNextTowardPos();
			if( transform.position == GetPointerPos() )
			{
				_pointerReached = true;
			}
		}		
	}
}