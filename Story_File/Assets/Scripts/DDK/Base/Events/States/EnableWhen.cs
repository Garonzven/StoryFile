//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.SceneManagement;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Enables the specified when the conditions are fulfilled
	/// </summary>
	public class EnableWhen : MonoBehaviour 
    {		
		public int frameRate = 10;
		[Space(5f)]
		public Behaviour[] willBeEnabled;
		public Renderer[] willBeEnabledR;
        [Tooltip("Checked on Update()")]
		public bool whenAnySoundIsPlaying;
        [Tooltip("Checked on Update()")]
		public bool whenMouseDown;
		[Space(2f)]
        [Tooltip("If equal or below cero, this is not taken into account")]
		public float reDisableAfter = 0f;	
        [Space(5f)]
        [Tooltip("Names of the scenes where the condition WON'T be fulfilled. NOTE: This is checked on Start()")]
        public string[] whenNotInScenes;
		
		
		bool _areEnabled = false;
		/// <summary>
		/// The time when the objects were enabled.
		/// </summary>
        float _enabledTime;
		/// <summary>
		/// This helps control enable-disable times related to whenAnySoundIsPlaying = true. It is not perfect but works okey.
		/// </summary>
        float _resetAreEnabledAfter;	
        bool _scenesConditionFulfilled;
		
		
		// Use this for initialization
		void Start () 
        {			
            if( !whenAnySoundIsPlaying && !whenMouseDown )
            {
                enabled = false;
                return;
            }
            _scenesConditionFulfilled = IsScenesConditionFulfilled();
            if( !_scenesConditionFulfilled || whenAnySoundIsPlaying || whenMouseDown )
                return;

            Enable();
		}		
		// Update is called once per frame
		void Update () 
        {			
			if( Time.frameCount % frameRate != 0 )
                return;
            if( !_areEnabled )
            {
                if( CheckMouseDown() && _scenesConditionFulfilled )
                {
                    Enable();
                }
                if( IsAnySoundPlaying() && _scenesConditionFulfilled )
                {
                    Enable();
                    var lastPlayingSound = AudioSourceExt.FindPlayingSound();
                    _resetAreEnabledAfter = lastPlayingSound.clip.length;
                }
            }
            else if( reDisableAfter > 0f )
            {
                if( _enabledTime + reDisableAfter > Time.time || CheckMouseDown() )
                    return;

                Enable( false );
                if( _resetAreEnabledAfter + _enabledTime < Time.time )
                {
                    _areEnabled = false;
                }
            }
        }

        protected bool IsAnySoundPlaying()
        {
            return whenAnySoundIsPlaying && AudioSourceExt.IsAnySoundPlaying();
        }
        protected bool IsScenesConditionFulfilled()
        {
            return !whenNotInScenes.Contains( SceneManager.GetActiveScene().name );
        }

		public void Enable()
		{
			_enabledTime = Time.time;
			_areEnabled = true;
			Enable( true );
		}		
		public void Enable( bool enable )
		{
			willBeEnabled.SetEnabled( enable );
			willBeEnabledR.SetEnabled( enable );
		}
		public bool CheckMouseDown()
		{
			if( whenMouseDown )
			{
				return Input.GetMouseButton(0);
			}
			return false;
		}		
	}
}
