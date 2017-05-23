//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.Misc;
using DDK.Base.Managers;
using DDK.Base.SoundFX;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Attach to an image to make it draggable.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class DraggableImage : MonoBehaviour	, IDragHandler, IPointerDownHandler, IPointerUpHandler //THIS HAS AN EDITOR CLASS
	{
		[System.Serializable]
		public class CopyToIcon 
		{
			public MonoBehaviour component;
			public bool enableAfterDroppedOnTarget;
		}


		/// <summary>
		/// The path to the icons (sprites). If null this image's sprite will be used; otherwise, a sprite containing
		/// this image's sprite's name (substring) will be used if found.
		/// </summary>
		[Tooltip("The path to the icons (sprites). If null this image's sprite will be used; otherwise, a sprite containing " +
			"this image's sprite's name (substring) will be used if found.")]
		public PathHolder.Index iconPath;
		[Tooltip("If false, the Ancestor's Canvas will be used as the drag surface")]
		public bool dragOnSurfaces = true;
		public bool setNativeSize = false;
		[IndentAttribute(1)]
		public Vector2 size = new Vector2( 50f, 50f );
	    public bool setDeltaSize = false;
        public CopyToIcon[] copyToIcon;

		[Range( 0f, 1f )]
		public float beenDraggedSourceOpacity = 0.5f;
		public GameObject target;
		[Tooltip("If true, the icon will be parented to the target when dropped on it")]
		public bool parentIconToTarget;
		/// <summary>
		/// The icon's sibling index when dropped on target (target is set as parent), if left at -1 it will be last
		/// index (on top of everything).
		/// </summary>
		[Tooltip( "The icon's sibling index when dropped on target (target is set as parent), if left at -1 it will be last "+
		"index (on top of everything)." )]
		public int iconTargetSiblingIndex = -1;
		public MonoBehaviour[] enableWhenDroppedOnTarget;

		public float destroyIconDelay = 0f;
		public bool destroyObjIfDroppedOnTarget;
		public float destroyDelay;

		[Tooltip("The sfx that will be played when dropping the item in the correct target")]
		public Sfx playWhenDroppedOnTarget;
		//INCORRECT TARGETS
		public GameObject[] incorrectTargets;
		[Tooltip("The sfx that will be played when dropping the item in an incorrect target")]
		public Sfx playWhenDroppedOnIncorrectTargets;
		public MonoBehaviour[] enableWhenDroppedOnIncorrectTarget;


		private GameObject _draggingIcon;
		private RectTransform _draggingPlane;
		private Image _this;
		private float _iniOpacity;
		private Button _bt;
		/// <summary>
		/// This is used to add extra validation in the dragging events 
		/// </summary>
		private bool _proceed = true;
		internal bool _isDragging;
		private PointerEventData _lastData;
		/// <summary>
		/// If copyToIcon, this will hold the new components that were copied to the icon.
		/// </summary>
		private List<MonoBehaviour> copiedToIcon = new List<MonoBehaviour>();


		/// <summary>
		/// The amount of items that have been dropped correctly on the specified target.
		/// </summary>
		public static int _correctDrops;


		public void Start()
		{
			_this = GetComponent<Image>();
			_bt = GetComponent<Button>();
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			_CheckInteractable();

			if( enabled && _proceed )
			{
				_iniOpacity = _this.color.a;
				_this.SetAlpha( beenDraggedSourceOpacity );

				var canvas =  gameObject.GetCompInParent<CanvasRenderer>();
				if (canvas == null)
                {
                    Debug.LogWarning("No CanvasRenderer in any ancestor, image can't be dragged");
                    return;
                }
				// We have clicked something that can be dragged.
				// What we want to do is create an icon for this.
				if (_draggingIcon != null)
				{
					DestroyImmediate( _draggingIcon );
				}
				_draggingIcon = new GameObject("icon");
				_draggingIcon.transform.SetParent (canvas.transform, false);
				_draggingIcon.transform.SetAsLastSibling();//ICON ON TOP OF EVERYTHING
				for( int i=0; i<copyToIcon.Length; i++ )
				{
					copiedToIcon.Add( (MonoBehaviour) copyToIcon[i].component.CopyTo( _draggingIcon, false ) );
				}

				var image = _draggingIcon.AddComponent<Image>();
				// The icon will be under the cursor.
				// We want it to be ignored by the event system.
				_draggingIcon.IgnoreElement();

				//GET ICON'S SPRITE
                if( iconPath.pathIndex != -1 )
				    image.sprite = iconPath.path.GetResourceWithSubstring<Sprite>( _this.sprite.name );
				if( !image.sprite )
				{
					image.sprite = _this.sprite;
				}
				//SET ICONS SIZE
				if( setNativeSize )
				{
					image.SetNativeSize();
				}
				else image.SetSize( size );

				if (dragOnSurfaces)
					_draggingPlane = (RectTransform) transform;
				else
					_draggingPlane = (RectTransform) canvas.transform;

				_SetDraggedPosition(eventData);
			}
			else if (_draggingIcon != null)
			{
				DestroyImmediate( _draggingIcon );
				_this.SetAlpha( _iniOpacity );
			}
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			_isDragging = false;
			if( enabled && _proceed )
			{
				Drop( eventData );
			}
			else if (_draggingIcon != null)
			{
				Destroy( _draggingIcon );
				_this.SetAlpha( _iniOpacity );
			}
		}
		public void OnDrag(PointerEventData data)
		{
			_isDragging = true;
			if( enabled && _proceed )
			{
				_lastData = data;
				if (_draggingIcon != null)
					_SetDraggedPosition(data);
			}
			else if (_draggingIcon != null)
            {
				Destroy( _draggingIcon );
				_this.SetAlpha( _iniOpacity );
			}
		}
		public void OnApplicationPause( bool paused )
		{
			if( !paused && _lastData != null )
			{
				OnPointerUp( _lastData );
			}
		}


		public void Drop( PointerEventData data )
		{
			bool droppedOnTarget = data.pointerEnter == target && target != null;
			_this.SetAlpha( _iniOpacity );
			if( droppedOnTarget )
			{
				if( parentIconToTarget )
				{
					//SET ICON'S SIBLING INDEX - THIS GIVE MORE CONTROL OVER THE ICON'S SPRITE ORDER
                    _draggingIcon.SetOrderInTarget( target, iconTargetSiblingIndex.Clamp( 0, target.ChildCount() - 1 ), 
                        destroyDelay ).Start();
				}

				_correctDrops++;
				enableWhenDroppedOnTarget.SetEnabled();
				SfxManager.PlaySfx( playWhenDroppedOnTarget, true, gameObject );
                DisableEventSystem( playWhenDroppedOnTarget );
				for( int i=0; i<copiedToIcon.Count; i++ )
				{
					if( copyToIcon[i].enableAfterDroppedOnTarget ) 
					{
						copiedToIcon[i].enabled = true;
					}
				}
                if( !parentIconToTarget && _draggingIcon != null )
					Destroy( _draggingIcon, destroyIconDelay );
			}
			else if (_draggingIcon != null)
			{
				Destroy( _draggingIcon );
				Sfx.m_context = gameObject;//for debugging
				if( incorrectTargets.Contains( data.pointerEnter ) )//DROPPED ON INCORRECT TARGET ?
				{
					enableWhenDroppedOnIncorrectTarget.SetEnabled();
					SfxManager.PlaySfx( playWhenDroppedOnIncorrectTargets, true, gameObject );
					DisableEventSystem( playWhenDroppedOnIncorrectTargets );
				}
			}
			copiedToIcon.Clear();
						
			if( destroyObjIfDroppedOnTarget && droppedOnTarget )
			{
				Destroy( gameObject, destroyDelay );
			}
		}

		/// <summary>
		/// Updates the dragging plane and the draggable item's dragging icon's rect transform.
		/// </summary>
		private void _SetDraggedPosition( PointerEventData data )
		{
			if( dragOnSurfaces && data.pointerEnter != null && ( (RectTransform)data.pointerEnter.transform ) != null )
				_draggingPlane = (RectTransform) data.pointerEnter.transform;
			
			var rt = _draggingIcon.GetComponent<RectTransform>();
			Vector3 globalMousePos;
			if ( RectTransformUtility.ScreenPointToWorldPointInRectangle( _draggingPlane, data.position, data.pressEventCamera, out globalMousePos ) )
			{
				rt.position = globalMousePos;
				rt.rotation = _draggingPlane.rotation;

			    if ( setDeltaSize )
			    {
			        rt.sizeDelta = size;
			    }
			}
		}
		/// <summary>
		/// If this has a Button component, it checks the state of the interactable property.
		/// </summary>
		private void _CheckInteractable()
		{
			if( !_bt )
				return;
			_proceed = _bt.interactable;
		}


		/// <summary>
		/// Disables the event system for the specified SfxManager's sfx's clip's length.
		/// </summary>
		public void DisableEventSystem( Sfx sfx )
		{
			Sfx.m_context = gameObject;//for debugging
			if (sfx.clip != null)
				EventSystem.current.DisableFor( sfx.clip.length );
		}
	}
}
