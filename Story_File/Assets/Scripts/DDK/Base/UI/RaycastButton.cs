//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Classes;
using DDK.Base.Extensions;


namespace DDK.Base.UI 
{	
	/// <summary>
	/// Attach to an image object to use it as a button that doesn't get affected by the event system. This can also 
	/// prevent onClick from executing its events when clicking on a transparent area.
	/// </summary>
	[RequireComponent( typeof(RectTransform), typeof(Image) )]
	public class RaycastButton : MonoBehaviourExt, ICanvasRaycastFilter
	{
		[HelpBoxAttribute]
		public string msg = "If disabled, onClick events won't be invoked";
		[Tooltip("If true, the image will mask the button preventing clicks on the image's sprite's transparent pixels")]
		public bool useMask;
		[Tooltip("If null, the main camera will be used")]
		public Camera eventCamera;
		[Header("Events")]
		public ComposedEvent onMouseEnter = new ComposedEvent();
		public ComposedEvent onMouseExit = new ComposedEvent();
		public ComposedEvent onClick = new ComposedEvent();


		private Image _image;
		private Sprite _sprite;
		private bool _mouseWasDownOnButton;
		private bool _mouseWasUp = true;
		private bool _mouseEntered;


		void Start ()
		{
			_image = GetComponent<Image>();
			if( !eventCamera )
				eventCamera = Camera.main;
		}
		void Update() 
        {
			if( CheckClick() )
			{
				OnClick();
			}
			else if( IsRaycastLocationValid() && !_mouseEntered )//MOUSE ENTER
			{
				_mouseEntered = true;
				onMouseEnter.Invoke();
			}
			else if( !IsRaycastLocationValid() && _mouseEntered )//MOUSE EXIT
			{
				_mouseEntered = false;
				onMouseExit.Invoke();
			}
		}
		bool CheckClick()
		{
			if( Input.GetMouseButtonDown( 0 ) && _mouseWasUp )
			{
				_mouseWasUp = false;
				if( IsRaycastLocationValid() )
				{
					_mouseWasDownOnButton = true;
				}
			}
			else if( Input.GetMouseButtonUp( 0 )  )
			{
				_mouseWasUp = true;
				if( _mouseWasDownOnButton && IsRaycastLocationValid() )
				{
					return true;
				}
				_mouseWasDownOnButton = false;
			}
			return false;
		}



		public void OnClick()
		{
			onClick.Invoke();
		}
		public bool IsRaycastLocationValid()
		{
			return IsRaycastLocationValid( Input.mousePosition, eventCamera );
		}
		public bool IsRaycastLocationValid(Vector2 sp, Camera evtCamera) //FROM ICanvasRaycastFilter
		{
			if( !enabled )
				return false;
			_sprite = _image.sprite;
			
			var rectTransform = (RectTransform)transform;
			if( !RectTransformUtility.RectangleContainsScreenPoint( rectTransform, sp, evtCamera ) )
				return false;
			if( !useMask )
				return true;

			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( rectTransform, sp, evtCamera, out localPositionPivotRelative);
			// convert to bottom-left origin coordinates
			var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x*rectTransform.rect.width,
			                                localPositionPivotRelative.y + rectTransform.pivot.y*rectTransform.rect.height);
			
			var spriteRect = _sprite.textureRect;
			var maskRect = rectTransform.rect;
			
			int x;
			int y;
			// convert to texture space
			switch (_image.type)
			{				
    			case Image.Type.Sliced:
    				var border = _sprite.border;
    				// x slicing
    				if (localPosition.x < border.x)
    				{
    					x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
    				}
    				else if (localPosition.x > maskRect.width - border.z)
    				{
    					x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
    				}
    				else
    				{
    					x = Mathf.FloorToInt(spriteRect.x + border.x +
    					                     ((localPosition.x - border.x)/
    					 (maskRect.width - border.x - border.z)) *
    					                     (spriteRect.width - border.x - border.z));
    				}
    				// y slicing
    				if (localPosition.y < border.y)
    				{
    					y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
    				}
    				else if (localPosition.y > maskRect.height - border.w)
    				{
    					y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
    				}
    				else
    				{
    					y = Mathf.FloorToInt(spriteRect.y + border.y +
    					                     ((localPosition.y - border.y) /
    					 (maskRect.height - border.y - border.w)) *
    					                     (spriteRect.height - border.y - border.w));
    				}
    				break;
    			default:
    				// conversion to uniform UV space
    				x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
    				y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
    				break;
			}
			
			// destroy component if texture import settings are wrong
			try
			{
				return _sprite.texture.GetPixel(x,y).a > 0;
			}
			catch (UnityException)
			{
				Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
				Destroy(this);
				return false;
			}
		}
	}
	
}