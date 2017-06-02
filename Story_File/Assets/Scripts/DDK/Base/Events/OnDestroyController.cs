//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.EventSystems;
using DDK.Base.Classes;


namespace DDK.Base.Events 
{
	/// <summary>
	/// Attach to an object that will get destroyed at some point. This allows to control when to destroy it or what 
	/// will happen afterwards.
	/// </summary>
    public class OnDestroyController : MonoBehaviour //THIS HAS AN EDITOR CLASS
    {		
		[System.Serializable]
		public class Substitute
		{
            /// <summary>
            /// Object will be randomly selected.
            /// </summary>
            [Tooltip("object will be randomly selected")]
			public GameObject[] obj;
            public SearchableGameObject parent = new SearchableGameObject();
			public float destroyAfter = -1f;
			public bool useRandomRotation;
			[IndentAttribute(1)]
			public bool x;
			[IndentAttribute(1)]
			public bool y;
			[IndentAttribute(1)]
			public bool z = true;
		}		
		

        [NotLessThan( -1f, "If -1, it won't be destoyed by this component")]
		public float destroyAfter = -1f;
        [NotLessThan( 0f, "The amount of seconds to advance the show subtitute. E.g: If 1 then the substitute will be shown 1 " +
            "second before destroying the object")]
        public float showBeforeDestruction = 0f;
		//--------------------------------------------------------------
		public Substitute[] substitutes = new Substitute[0];
		/// <summary>
		/// The chances of showing last substitute. It might be some enemy destroyed price as coins, points, life refill...
		/// </summary>
		[Tooltip("The chances of showing last substitute. It might be some -enemy destroyed- price as coins, points, life refill...")]
		public float chancesOfShowingLastSubstitute = 0.05f;
		/// <summary>
		/// The last object random pick chances. The sum of each element inside this array should be equal to 1 (100%).
		/// </summary>
		[Tooltip("The last object random pick chances. The sum of each element inside this array should be equal to 1 (100%). " +
            "Each element represents an index from the obj substitutes array")]
		public float[] lastObjRandomPickChances = new float[]{ 1f };
		//--------------------------------------------------------------
		//TRANSFER TAG
		public bool toNextSibling;
			//TRUE
			public bool mustBeActive;
		//--------------------------------------------------------------
		[Tooltip("This Happens Immediately!")]
		public bool destroyOnMouseDown;
			//TRUE
			public bool uiEventSystemMustBeActive = true;
						

		/// <summary>
		/// Prevents all onDestroyControllers to execute its OnDestroy() function.
		/// </summary>
        private static bool _appIsShuttingDown;
		/// <summary>
		/// Prevents this OnDestroyController to execute its OnDestroy() function.
		/// </summary>
		internal bool _block;
		
				
		
		// Use this for initialization
		void Start () 
        {			
			gameObject.Destroy( destroyAfter );
            if( showBeforeDestruction > destroyAfter )
            {
                Debug.LogWarning("Show Before Destruction is higher than the destroy delay (Destroy After)");
                return;
            }
            if( destroyAfter >= 0f )
            {
                Invoke( "_DoSubstitutions", Mathf.Clamp( destroyAfter - showBeforeDestruction, 0f, float.MaxValue ) - 0.01f );
            }
		}		
		// Update is called once per frame
		void Update () 
        {
			_CheckAndDestroy();
		}		
		void OnDestroy()
		{
			if( !_appIsShuttingDown && !_block )
			{
                if( destroyAfter.CloseTo( 0f ) )
				{
					_DoSubstitutions();
				}
				_TransferTag();
			}
		}
        void OnApplicationQuit()
        {
            _appIsShuttingDown = true;
        }   


		protected void _CheckAndDestroy()
		{
			#region ON MOUSE DOWN
			if( Input.GetMouseButtonDown(0) && destroyOnMouseDown )
			{
				if( uiEventSystemMustBeActive && !EventSystem.current )
				{
					return;
				}
				Destroy(gameObject);
			}
			#endregion ON MOUSE DOWN
		}
		protected void _TransferTag()
		{
			if( toNextSibling )
			{
				GameObject nextSibling = gameObject.GetNextSibling( mustBeActive );
				if( nextSibling )
				{
					nextSibling.tag = tag;
				}
			}
		}
		protected void _DoSubstitutions()
		{
			for( int i=0; i<substitutes.Length; i++ )
			{
				int objToInstantiate = Random.Range( 0, substitutes[i].obj.Length );//pick a random object
				if( i == substitutes.Length - 1 )//LAST
				{
					objToInstantiate = lastObjRandomPickChances.GetIndexFromRandomChances();
				}
				
				if( substitutes[i].obj[objToInstantiate] != null )
				{
					if( i == substitutes.Length - 1 )//LAST
					{
						if( Random.value > chancesOfShowingLastSubstitute )
							break;
						_InstantiateSub( i, objToInstantiate );
					}
					else _InstantiateSub( i, objToInstantiate );
				}
			}
		}

		void _InstantiateSub( int i, int objIndex )
		{
			Vector3 rotEuler = substitutes[i].obj[objIndex].transform.rotation.eulerAngles;
			var rot = substitutes[i].useRandomRotation ? Quaternion.Euler( 
                ( substitutes[i].x ? Random.Range( 0f, 360f ) : rotEuler.x ),
                ( substitutes[i].y ? Random.Range( 0f, 360f ) : rotEuler.y ),
                ( substitutes[i].y ? Random.Range( 0f, 360f ) : rotEuler.y ) ) : substitutes[i].obj[objIndex].transform.rotation;
            var obj = (GameObject)Instantiate( substitutes[i].obj[objIndex], transform.position, rot, substitutes[i].parent.m_transform );
			obj.SetActiveInHierarchy( true );
			if( substitutes[i].destroyAfter > 0f )
			{
				obj.Destroy( substitutes[i].destroyAfter );
			}
		}	
	}
}
