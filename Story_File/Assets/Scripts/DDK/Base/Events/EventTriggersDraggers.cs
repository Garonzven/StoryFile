//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DDK.Base.Classes;
using UnityEngine.UI;


namespace DDK.Base.Events 
{
    /// <summary>
    /// Similar to Unity's EventTrigger monobehaviour class, but with extra functionality and more control.
    /// </summary>
	public class EventTriggersDraggers : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
	IInitializePotentialDragHandler 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "This might give issues if using ScrollRect. If this component is disabled, the events won't be invoked";
        #endif
		public ComposedEvent onBeginDrag = new ComposedEvent();
		public ComposedEvent onDrag = new ComposedEvent();
		public ComposedEvent onEndDrag = new ComposedEvent();
		public ComposedEvent onInitializePotentialDrag = new ComposedEvent();


		internal UnityAction<BaseEventData> m_onBeginDrag;
		internal UnityAction<BaseEventData> m_onDrag;
		internal UnityAction<BaseEventData> m_onEndDrag;	
		internal UnityAction<BaseEventData> m_onInitializePotentialDrag;


        protected CanvasGroup _canvasGroup;
        protected Button _bt;


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
		
		
		public void OnBeginDrag ( PointerEventData eventData ) 
		{
			if( !enabled )
				return;
			// Do action
			if( m_onBeginDrag != null )
			{
				m_onBeginDrag( eventData );
			}
			onBeginDrag.Invoke();
		}	
		public void OnDrag ( PointerEventData eventData ) 
		{
			if( !enabled )
				return;
			// Do action
			if( m_onDrag != null )
			{
				m_onDrag( eventData );
			}
			onDrag.Invoke();
		}	
		public void OnEndDrag ( PointerEventData eventData ) 
		{
			if( !enabled )
				return;
			// Do action
			if( m_onEndDrag != null )
			{
				m_onEndDrag( eventData );
			}
			onEndDrag.Invoke();
		}	
		public void OnInitializePotentialDrag ( PointerEventData eventData ) 
		{
			if( !enabled )
				return;
			// Do action
			if( m_onInitializePotentialDrag != null )
			{
				m_onInitializePotentialDrag( eventData );
			}
			onInitializePotentialDrag.Invoke();
		}	
		
	}
	
}
