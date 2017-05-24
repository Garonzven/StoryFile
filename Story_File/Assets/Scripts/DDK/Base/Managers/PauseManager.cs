//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using UnityEngine.Events;
using DDK.Base.Fx.Transitions;
using System.Collections.Generic;


namespace DDK.Base.Managers 
{
    /// <summary>
    /// Allows pausing the game/app.
    /// </summary>
	public class PauseManager : MonoBehaviourExt 
    {
        [System.Serializable]
        public struct AnimatorsStateHandler
        {
            public float onPauseDelay;
            public bool namesAreTags;
            public string[] names;

            [HideInInspector]
            public Animator[] _animators;
        }


		[Tooltip("The time scale when paused. Lower values slow down enough to make it look as if it was paused")]
		public float timeScale = 0f;
		public bool pauseOnEnable = true;
        [Tooltip("If true, on standalone builds the player will be able to pause with Esc.")]
        public bool pauseWithEsc;
		[Space(5f)]
		public bool pauseAudioListener = true;
        [Tooltip("If a layer is enabled, all animator components from that layer will be disabled on pause, and enabled on resume")]
        public AnimatorsStateHandler animatorsStateHandler;
		[Header("Events")]
		public EnableDisableState onPauseStates;
		public EnableDisableState onResumeStates;
		public UnityEvent onPause;
		public UnityEvent onResume;


		protected float _timeScl;
		protected bool _timeStored;
        /// <summary>
        /// The last enabled instance.
        /// </summary>
		public static PauseManager ActiveInstance;
		public static bool m_Paused;
		public static float m_TimeScale 
        {
			get
            {
				if( !ActiveInstance )
				{
					Debug.LogWarning ("No PauseManager has been instantiated. Returning: Time.timeScale");
					return Time.timeScale;
				}
				return ActiveInstance.timeScale;
			}
		}
		private bool _audioListenerWasPausedBySomethingElse;



		void Awake()
		{
			if( AudioListener.pause )
				_audioListenerWasPausedBySomethingElse = true;
		}

		// Use this for initialization
		void Start () { }
        #if UNITY_STANDALONE
        void Update()
        {
            if( pauseWithEsc && !m_Paused && Input.GetKeyDown( KeyCode.Escape ) )
            {
                Pause();
            }
        }
        #endif
		void OnApplicationQuit()
		{
			if( !_audioListenerWasPausedBySomethingElse )
				AudioListener.pause = false;
		}
		protected void OnEnable()
		{
            ActiveInstance = this;
			if( pauseOnEnable )
			{
				Pause ();
			}
		}
		protected void OnDestroy()
		{
			if( !m_Paused )
				return;
			Time.timeScale = _timeScl;
			m_Paused = false;			
			if( !_audioListenerWasPausedBySomethingElse )
				AudioListener.pause = !pauseAudioListener;
		}
        protected IEnumerator<float> _ValidateAnimators()
        {
            if( animatorsStateHandler.names.Length == 0 )
                yield break;
            if( animatorsStateHandler._animators.Length == 0 )//FIND THEM
            {
                if( animatorsStateHandler.namesAreTags )
                {
                    animatorsStateHandler._animators = animatorsStateHandler.names.FindWithTags<Animator>().ToArray();
                }
                else animatorsStateHandler._animators = animatorsStateHandler.names.Find<Animator>().ToArray();
            }
            if( m_Paused )
            {
                float time = Time.unscaledTime;
                while( Time.unscaledTime < time + animatorsStateHandler.onPauseDelay )
                {
                    yield return 0f;
                }
            }
            animatorsStateHandler._animators.Enable( !m_Paused );
        }



		public void Pause()
		{
			if( m_Paused )
				return;
			_timeScl = Time.timeScale;
			Time.timeScale = timeScale;
			m_Paused = true;

			if( !_audioListenerWasPausedBySomethingElse )
                AudioListener.pause = pauseAudioListener;
			onPauseStates.SetStates();
			onPause.Invoke();
            _ValidateAnimators().Run();
		}
		public void Resume()
		{
			if( !m_Paused )
				return;
			Time.timeScale = _timeScl;
			m_Paused = false;

			if( !_audioListenerWasPausedBySomethingElse )
				AudioListener.pause = !pauseAudioListener;
			onResumeStates.SetStates();
			onResume.Invoke();
            _ValidateAnimators().Run();
		}

	}
}
