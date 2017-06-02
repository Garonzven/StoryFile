using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
	/// <summary>
	/// Attach to a Toggle button or any object, to handle toggle events.
	/// </summary>
	public class OnToggle : MonoBehaviourExt 
    {
		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string msg = "If disabled, events won't be invoked";
		#endif
		[Tooltip("If true, the toggle's state will be checked, meaning the respective events will be invoked.")]
		public bool checkToggledOnEnable;
		[Tooltip("If true, and this toggle has Toggle siblings, then there will always be at least one toggled on")]
		public bool atLeastOneToggled;
		[Tooltip("If true, on toggled the siblings toggle array will be updated. This can be useful if the toggles are " +
			"dynamic (if toggles can be removed or added at runtime")]
		[IndentAttribute(1)]
		public bool updateSiblingToggles;

		[Header("Events")]
		[Tooltip("If true, this toggle is selected, and this toggle has a toggle group, then another toggle will be " +
			"automatically toggled ON after this is destroyed")]
		public bool onDestroyAutoToggleAnother = true;
		public ComposedEvent onToggleOn = new ComposedEvent();
		public ComposedEvent onToggleOff = new ComposedEvent();
		[Tooltip("This is invoked after the onToggleOn and/or onToggleOff")]
		public ComposedEvent onToggled = new ComposedEvent();


        public Toggle m_Toggle { get; private set; }
        /// <summary>
        /// This includes this toggle.
        /// </summary>
        public Toggle[] m_Siblings { get; private set; }


		void Awake() 
        {
			m_Toggle = GetComponent<Toggle>();
			if( m_Toggle )
			{
				m_Toggle.onValueChanged.AddListener( on => {

					if( on )
						OnToggleOn();
					else OnToggleOff();
					OnToggled();
				} );
            }
        }
		void Start() 
        {
			if( atLeastOneToggled && transform.parent )
			{
				m_Siblings = transform.parent.GetComponentsInChildren<Toggle>();
			}
		}
		void OnEnable()
		{
			if( checkToggledOnEnable )
			{
				m_Toggle.onValueChanged.Invoke( m_Toggle.isOn );
			}
		}
		void OnDestroy()
		{
			if( m_Toggle == null || m_Siblings == null || m_Siblings.Length == 1 || m_Toggle.group == null || 
				!m_Toggle.isOn || !onDestroyAutoToggleAnother )
				return;
            for( int i=0; i<m_Siblings.Length; i++ )
            {
                if( m_Siblings[ i ] == m_Toggle )
                    continue;
                m_Siblings[ i ].isOn = true;
            }
		}

		public void OnToggleOn()
		{
			if( !enabled )
				return;
			onToggleOn.Invoke ();
		}
		public void OnToggleOff()
		{
			if( atLeastOneToggled && updateSiblingToggles && transform.parent )
			{
				m_Siblings = transform.parent.GetComponentsInChildren<Toggle>();
			}
			if( !enabled )
				return;
			if( m_Siblings != null && m_Siblings.AreAllOff() )
			{
				m_Toggle.isOn = true;
				return;
			}
			onToggleOff.Invoke ();
        }
		public void OnToggled()
		{
			if( !enabled )
				return;
			onToggled.Invoke ();
		}
	}
}
