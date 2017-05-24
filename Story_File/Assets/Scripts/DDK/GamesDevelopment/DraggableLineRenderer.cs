//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.Managers;


namespace DDK.GamesDevelopment {

	/// <summary>
	/// Attach to a line renderer to make it draggable
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class DraggableLineRenderer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler //THIS HAS AN EDITOR CLASS
	{
		public bool autoDropOnTargetEnter;
		/// <summary>
		/// If true and the drop target has a DraggableLineRenderer attached, its drag will automatically begin when auto dropped.
		/// </summary>
		[TooltipAttribute("If true and the drop target has a DraggableLineRenderer attached, its drag will automatically begin when auto dropped")]
		public bool beginTargetDragOnEnter;
		public bool makeTargetInteractable = true;
		[Tooltip("This also affects their Image.enabled and DraggableLineRenderer.enabled")]
		public GameObject[] makeOthersInteractable = new GameObject[0];
		[Tooltip("This also affects their Image.enabled and DraggableLineRenderer.enabled")]
		public GameObject[] makeOthersNotInteractable = new GameObject[0];

		public bool dropOnLastTouchedTarget;
		public float minDistanceToLastTouchedTarget = 2f;

		public float lineRendererPosZ = 0;
		public bool snapToDropTargets = true;
		public bool snapOnlyToDropTargets = true;
		//public int[] snapSupportedAngles = new int[]{ 0, 90, 180, 270, 360, 45, 135, 225, 315 };

		public GameObject dropTarget;
		public bool dropTargetIsChild;
		public bool disableCompWhenDropOnTarget = true;
		public bool disableInteractableWhenDropOnTarget = false;
			public bool disableImageAlso = false;
		public bool allDropTargetsHaveMultipleParents;
		public GameObject allDropTargetsParent;
		public GameObject allDropTargetsParentsParent;
		[Tooltip("The clip (in SfxManager) that will be played when dropped on target")]
		public AudioClip onDropOnTarget;
		[Tooltip("The clip (in SfxManager) that will be played when dropped on other target")]
		public AudioClip onDropOnOtherTarget;
		[Tooltip("Name of the SfxManager's source that will play this audio clips")]
		[IndentAttribute(1)]
		public string srcName = "Effects";				
		
		

		protected Canvas _canvas;
		protected GameObject _lastTouchedTarget;
		protected bool _dropped;
		internal bool i_droppedCorrectly;
		/// <summary>
		/// LineRenderer's initial position.
		/// </summary>
		protected Vector3 _iniPos;
		/// <summary>
		/// LineRenderer's end position.
		/// </summary>
		protected Vector3 _endPos;
		protected List<GameObject> _allDropTargetsParents = new List<GameObject>();
		internal List<GameObject> _allDropTargets = new List<GameObject>();
		internal LineRenderer _lineRenderer;
		/// <summary>
		/// If true, another script is making this one execute its drag event.
		/// </summary>
		internal bool _isDraggingManually = false;
		/// <summary>
		/// The data that is making this drag to execute manually, null if not manually executed.
		/// </summary>
		internal PointerEventData _manualData;
		internal delegate void d_Action();
		/// <summary>
		/// Subscribe to this event to execute it on end drag.
		/// </summary>
		internal d_Action e_OnEndDrag;
		/// <summary>
		/// Subscribe to this event to execute it when dropped in the correct target (drop target).
		/// </summary>
		internal d_Action e_OnCorrectDrop;



		public static int correctDropsCount = 0;
		public static GameObject lastDroppedTarget;



		public void Start()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			if( dropTarget && dropTargetIsChild )
			{
				dropTarget = dropTarget.GetChild(0);
			}
			if( allDropTargetsHaveMultipleParents && allDropTargetsParentsParent )
			{
				_allDropTargetsParents = allDropTargetsParentsParent.GetChildren().ToList();
				for( int i=0; i<_allDropTargetsParents.Count; i++ )
				{
					_allDropTargets.AddRange( _allDropTargetsParents[i].GetChildren(true) );
				}
			}
			else _allDropTargets = allDropTargetsParent.GetChildren(true).ToList();
		}

		public void Update()
		{
			if( _isDraggingManually )
			{
				if( Input.GetMouseButton(0) )
				{
					OnDrag( _manualData );
				}
				else if( Input.GetMouseButtonUp(0) )
				{
					OnEndDrag( _manualData );
				}
			}
		}				
		
		
		public void OnBeginDrag(PointerEventData eventData)
		{
			if( enabled )
			{
				_dropped = false;
				
				_canvas = FindInParents<Canvas>(gameObject);
				if (_canvas == null)
					return;
				// We have clicked something that can be dragged.							
				_iniPos = gameObject.GetWorldPos( _canvas );//line renderer init position
				_iniPos = new Vector3( _iniPos.x, _iniPos.y, lineRendererPosZ );
				//iniPos[2] = canvas.GetComponent<RectTransform>().position.z;
				_lineRenderer.SetPosition( 0, _iniPos );
				_lineRenderer.SetPosition( 1, _iniPos );
				SetDraggedPosition(eventData);
			}
		}		
		
		public void OnDrag(PointerEventData data)
		{
			if( enabled && !_dropped )
			{
				SetDraggedPosition(data);
				if( _allDropTargets.Contains( data.pointerEnter ) && data.pointerEnter != gameObject )
				{
					if( autoDropOnTargetEnter )
					{
						Drop( data );
						if( beginTargetDragOnEnter && data.pointerEnter == dropTarget )
						{
							var targetDraggable = dropTarget.GetComponent<DraggableLineRenderer>();
							if( targetDraggable )
							{
								targetDraggable.Start();
								targetDraggable.enabled = true;
								targetDraggable.OnBeginDrag( data );
								targetDraggable._manualData = data;
								targetDraggable._isDraggingManually = true;
								targetDraggable.SetInteractable( makeTargetInteractable );
							}
							if( makeOthersInteractable.Length > 0 )
							{
								makeOthersInteractable.SetInteractables( true );
								makeOthersInteractable.Enable<Image>( true );
								makeOthersInteractable.Enable<DraggableLineRenderer>( true );
							}
							if( makeOthersNotInteractable.Length > 0 )
							{
								makeOthersNotInteractable.SetInteractables( false );
								makeOthersNotInteractable.Enable<Image>( false );
								makeOthersNotInteractable.Enable<DraggableLineRenderer>( false );
							}
						}
					}
					else if( dropOnLastTouchedTarget )
					{
						_lastTouchedTarget = data.pointerEnter;
					}
				}
			}
		}		
		
		public void OnEndDrag(PointerEventData eventData)
		{
			if( enabled )
			{
				Drop ( eventData );
			}
		}		


		
		private void SetDraggedPosition(PointerEventData data)
		{
			_endPos = data.pressEventCamera.ScreenToWorldPoint( data.position );//line renderer end position
			_endPos = new Vector3( _endPos.x, _endPos.y, lineRendererPosZ );
			//endPos[2] = canvas.GetComponent<RectTransform>().position.z;

			if( snapToDropTargets && snapOnlyToDropTargets )
			{
				if( _allDropTargets.Contains( data.pointerEnter ) )
				{
					_lineRenderer.SetPosition( 1, (Vector2)data.pointerEnter.Position() );
				}
			}
			else _lineRenderer.SetPosition( 1, _endPos );
		}

		public void Drop( PointerEventData eventData )
		{
			if( !_dropped )
			{
				if( ( eventData.pointerEnter == dropTarget && dropTarget ) || 
				   ( _lastTouchedTarget && _lastTouchedTarget == dropTarget && _endPos.Distance( _lastTouchedTarget.Position() ) <= minDistanceToLastTouchedTarget ) )//true = attach to target
				{
					_lineRenderer.SetPosition( 1, (Vector2)dropTarget.Position() );
					if( onDropOnTarget )
					{
						SfxManager.PlayClip( srcName, onDropOnTarget );
					}
					if( disableCompWhenDropOnTarget )
					{
						enabled = false;
					}
					if( disableInteractableWhenDropOnTarget )
					{
						gameObject.SetInteractable(false);
						if( disableImageAlso )
						{
							gameObject.GetComponent<Image>().SetEnabled( false );
						}
					}
					_dropped = true;
					i_droppedCorrectly = true;
					correctDropsCount++;
					lastDroppedTarget = _lastTouchedTarget;
					_lastTouchedTarget = null;
					if( e_OnCorrectDrop != null )
					{
						e_OnCorrectDrop();
					}
				}
				else//return
				{
					if( ( _allDropTargets.Contains( eventData.pointerEnter ) && eventData.pointerEnter != gameObject ) ||
					   ( _lastTouchedTarget && _endPos.Distance( _lastTouchedTarget.Position() ) <= minDistanceToLastTouchedTarget ) )
					{
						if( onDropOnOtherTarget )
						{
							SfxManager.PlayClip( srcName, onDropOnOtherTarget );
						}
					}
					_lineRenderer.SetPosition( 1, (Vector2)gameObject.Position() );
					_dropped = true;
					_lastTouchedTarget = null;
				}
				//Reset, execute event
				_isDraggingManually = false;
				_manualData = null;
				if( e_OnEndDrag != null )
				{
					e_OnEndDrag();
				}
			}
		}		
		
		
		
		public static T FindInParents<T>(GameObject go) where T : Component
		{
			if (go == null) return null;
			var comp = go.GetComponent<T>();
			if (comp != null)
				return comp;
			
			Transform t = go.transform.parent;
			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
			return comp;
		}
	}
	
}

