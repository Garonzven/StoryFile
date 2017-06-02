//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DDK.Base.Classes;
using DDK.Base.Extensions;
using UnityEngine.UI;
#if USE_SVG_IMPORTER
using SVGImporter;
#endif


namespace DDK.Base.Events 
{
    /// <summary>
    /// Similar to Unity's EventTrigger monobehaviour class, but with extra functionality and more control.
    /// </summary>
    #if USE_SVG_IMPORTER
    [RequireComponent( typeof( SVGImage ) )]
    #else
    [RequireComponent( typeof( Image ) )]
    #endif
	public class EventTriggersPointers : MonoBehaviourExt, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, 
    IPointerExitHandler, IPointerClickHandler
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If this component is disabled, the events won't be invoked";
        #endif
		public ComposedEvent onPointerDown = new ComposedEvent();
		public ComposedEvent onPointerUp = new ComposedEvent();
		[Tooltip("If true, the OnPointerUp events won't be invoked unless the OnPointerDown events were invoked first")]
		[IndentAttribute(1)]
		public bool requiresPointerDown;
		[Tooltip("The minimum time that has to pass before the /onPointerUp/ event can be invoked")]
        [ShowIfAttribute( "requiresPointerDown", 2 )]
		public float minTime;
		[Tooltip("If true, the /onPointerUp/ events will be invoked after the specified /minTime/ even if the pointer" +
			"was up before the time passed.")]
		[ShowIfAttribute( "_UsingMinTime", 3 )]
		public bool invokeAfterTime;
		public ComposedEvent onPointerEnter = new ComposedEvent();
        [Tooltip("If true, the OnPointerEnter events won't be invoked unless a touch is detected on the screen first")]
        [DisplayNameAttribute( "Requires Pointer Down Anywhere", 1 )]
        public bool enterRequiresPointerDown;
		public ComposedEvent onPointerExit = new ComposedEvent();
		public ComposedEvent onPointerClick = new ComposedEvent();


		protected bool _UsingMinTime()
		{
			return minTime > 0;
		}


		internal UnityAction<BaseEventData> m_onPointerDown;
		internal UnityAction<BaseEventData> m_onPointerUp;
		internal UnityAction<BaseEventData> m_onPointerEnter;
		internal UnityAction<BaseEventData> m_onPointerExit;
		internal UnityAction<BaseEventData> m_onPointerClick;

		/// <summary>
		/// True if the OnPointerDown events were invoked.
		/// </summary>
		protected bool _downInvoked;
		protected float _downTime;
        protected CanvasGroup _canvasGroup;
        protected Button _bt;

        /// <summary>
        /// If this gameObject has a Button or CanvasGroup, this will return the speficied's interactable state.
        /// </summary>
        public bool IsInteractable 
        {
            get
            {
                if( _bt )
                {
                    if( !_bt.enabled )
                        return false;
                    else if( _canvasGroup )
                        return _canvasGroup.interactable & _bt.interactable;
                    return _bt.interactable;
                }
                if( _canvasGroup )
                    return _canvasGroup.interactable;
                return true;
            }
        }


		void Start()
		{
            _bt = GetComponent<Button>();
            _canvasGroup = GetComponentInParent<CanvasGroup>();
		}	
		
		
        /// <summary>
        /// Raises the pointer down event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
		public void OnPointerDown (PointerEventData eventData) 
        {

            if( !enabled || !IsInteractable )
				return;
			// Do action
			if( m_onPointerDown != null )
			{
				m_onPointerDown( eventData );
			}
			onPointerDown.Invoke();
			_downInvoked = true;
			_downTime = Time.time;
		}		
        /// <summary>
        /// Raises the pointer up event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
		public void OnPointerUp (PointerEventData eventData) 
        {

            if( !enabled || !IsInteractable )
				return;
			if( requiresPointerDown && !_downInvoked )
				return;
			// Do action
			if( minTime + _downTime > Time.time )
			{
				if( invokeAfterTime )
				{
					Invoke( "InvokeOnPointerUp", minTime );
				}
				return;
			}
			if( m_onPointerUp != null )
			{
				m_onPointerUp( eventData );
			}
			onPointerUp.Invoke();
		}		
        /// <summary>
        /// Raises the pointer enter event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
		public void OnPointerEnter (PointerEventData eventData) 
        {

            if( !enabled || !IsInteractable )
				return;
            if( enterRequiresPointerDown && !Input.GetMouseButton( 0 ) )
                return;
			// Do action
			if( m_onPointerEnter != null )
			{
				m_onPointerEnter( eventData );
			}
			onPointerEnter.Invoke();
		}		
        /// <summary>
        /// Raises the pointer exit event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
		public void OnPointerExit (PointerEventData eventData) 
        {

            if( !enabled || !IsInteractable )
				return;
			// Do action
			if( m_onPointerExit != null )
			{
				m_onPointerExit( eventData );
			}
			onPointerExit.Invoke();
		}		
        /// <summary>
        /// Raises the pointer click event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
		public void OnPointerClick (PointerEventData eventData) 
        {

            if( !enabled || !IsInteractable )
				return;
			// Do action
			if( m_onPointerClick != null )
			{
				m_onPointerClick( eventData );
			}
			onPointerClick.Invoke();
		}

		#region UNITY EVENT compatible functions
		public void InvokeOnPointerDown()
		{
			OnPointerDown( null );
		}
		public void InvokeOnPointerUp()
		{
			OnPointerUp( null );
		}
		public void InvokeOnPointerEnter()
		{
			OnPointerEnter( null );
		}
		public void InvokeOnPointerExit()
		{
			OnPointerExit( null );
		}
		public void InvokeOnPointerClick()
		{
			OnPointerClick( null );
		}
		#endregion

		/// <summary>
		/// This resets the pointer down flag used to prevent OnPointerup events execution if /requiresPointerDown/ is true.
		/// </summary>
		public void ResetDownInvoked()
		{
			_downInvoked = false;
		}
	}	
}
