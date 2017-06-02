//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using UnityEngine.Events;
using DDK.Base.Statics;


namespace DDK.Base.UI
{
    /// <summary>
    /// Allows fading a CanvasGroup.
    /// </summary>
    public class CanvasGroupFader : MonoBehaviourExt
    {
		[System.Serializable]
		public class FaderEvents 
        {
			public UnityEvent onEnabled = new UnityEvent();
			public UnityEvent onDisabled = new UnityEvent();
		}



		[Tooltip("If empty, this or the parent's CanvasGroup will be used")]
        public CanvasGroup[] canvasGroups = new CanvasGroup[0];
		[Space(10f)]
		[Tooltip("If true, OnEnable() won't be called if the Start() hasn't been called")]
		public bool waitForInit;
		[Tooltip("If true, the delays won't take into account the Time.timeScale")]
		public bool unscaledTime;
		public float onEnableDelay;
        [Tooltip("The target alpha when this component is enabled")]
        [Range(0f, 1f)]
        public float onEnableTarget = 1f;
		public float onDisableDelay;
        [Tooltip("The target alpha when this component is disabled")]
        [Range(0f, 1f)]
        public float onDisableTarget;
		/// <summary>
		/// The fade animation's duration.
		/// </summary>
		[Tooltip("The fade animation's duration")]
        public float duration = 0.5f;
		[Header("Events")]
		[Tooltip("If true, this component will be destroyed after this component is enabled, or disabled")]
		public bool destroyThisAfterDone;
		[Tooltip("If true, this gameobject will be destroyed after this component is enabled, or disabled")]
        public bool destroyObjAfterDone;
		public EnableDisableStates states = new EnableDisableStates();
		public FaderEvents onDone = new FaderEvents();



		/// <summary>
		/// The total duration of the on enable fade animation ( onEnableDelay + duration  ).
		/// </summary>
		internal float _TotalOnEnableDuration 
        {
			get
            {
				return onEnableDelay + duration;
			}
		}
		/// <summary>
		/// The total duration of the on disable fade animation ( onDisableDelay + duration  ).
		/// </summary>
		internal float _TotalOnDisableDuration 
        {
			get
            {
				return onDisableDelay + duration;
			}
		}

		/// <summary>
		/// True if this component or this object is being destroyed.
		/// </summary>
        private bool _isBeingDestroyed;
		/// <summary>
		/// This prevents OnEnable from being called if the object hasn't being initialized.
		/// </summary>
		private bool _started;


        // Use this for initialization
        void Awake()
        {
			if( canvasGroups.Length == 0 )
			{
				canvasGroups = new CanvasGroup[]{ GetComponentInParent<CanvasGroup>() };
			}
        }
		void Start() 
        {
			_started = true;
		}
        void OnEnable()
        {
			if( !_started && waitForInit )
				return;
			AnimateCanvasGroups( onEnableDelay, onEnableTarget ).Start();
            states.SetStates( true );
			onDone.onEnabled.Invoke();
        }
        void OnDisable()
        {
			if( _isBeingDestroyed )
				return;
            AnimateCanvasGroups( onDisableDelay, onDisableTarget ).Start();
            states.SetStates( false );
			onDone.onDisabled.Invoke();
        }
		void OnApplicationQuit()
		{
			_isBeingDestroyed = true;
		}



		public IEnumerator AnimateCanvasGroups( float delay, float alphaTarget )
		{
            if( delay > 0f )
            {
                if( unscaledTime )
                    yield return Utilities.WaitForRealSeconds( delay ).Start();
                else yield return new WaitForSeconds( delay );
            }

			if( !_isBeingDestroyed && canvasGroups != null && canvasGroups.Length > 0 )
			{
				canvasGroups.AlphaTo( alphaTarget, duration );
				Done().Start();
			}
		}
        IEnumerator Done()
        {
			if( unscaledTime )
				yield return Utilities.WaitForRealSeconds( duration + 0.1f ).Start();
			else yield return new WaitForSeconds( duration + 0.1f );
            if( destroyObjAfterDone && gameObject != null )
            {
                _isBeingDestroyed = true;
                Destroy( gameObject );
            }
			if( destroyThisAfterDone )
			{
				_isBeingDestroyed = true;
				Destroy ( this );
			}
        }
    }
}
