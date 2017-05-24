//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using DDK.Base.Events;


namespace DDK.Base.Animations
{
    /// <summary>
    /// Controls an animator's tilt param's value, and handles tilt related events.
    /// </summary>
	public class AnimatorTiltController : AnimatorActions 
	{		
        [System.Serializable]
        public struct LimitsEvents
        {
            public bool onAnyLimitReachedDisable;
            public ComposedEvent onAnyLimitReached;
            public ComposedEvent onMaxReached;
            public ComposedEvent onMinReached;
        }


		[Space(10f)]
		#if UNITY_EDITOR
		[Tooltip("EDITOR ONLY. Left tilt is done by default")]
		public KeyCode rightTilt = KeyCode.Space;
		#endif
		public string tiltParam = "tilt";
		[Tooltip("Left")]
		public float min = -1f;
		[Tooltip("Right")]
		public float max = 1f;
		[Tooltip("The amount that will be added to the tilt param's value when tilting right or left")]
		public float increment = 0.01f;
		[Header("Events")]
        public LimitsEvents events = new LimitsEvents();
		
		
        /// <summary>
        /// Gets or sets the tilt amount of the animato's param.
        /// </summary>
		public float Tilt
        {
			get{ return Controller.GetFloat( tiltParam ); }
			set
            {
				value = Mathf.Clamp( value, min, max );
				Controller.SetFloat( tiltParam, value );
				if( value == max )
				{
                    events.onMaxReached.Invoke();
                    events.onAnyLimitReached.Invoke();
                    if( events.onAnyLimitReachedDisable )
						enabled = false;
				}
				else if( value == min )
				{
                    events.onMinReached.Invoke();
                    events.onAnyLimitReached.Invoke();
                    if( events.onAnyLimitReachedDisable )
						enabled = false;
				}
			}
		}
		
		
		
		// Use this for initialization
		void Start () 
        {			
			if( !Controller )
			{
				Controller = GetComponentInChildren<Animator>();
			}
			increment *= 10f;
		}
		
		// Update is called once per frame
		void Update () 
        {			
#if UNITY_EDITOR
			if( Input.GetKey( rightTilt ) )
			{
				Tilt += increment * Time.deltaTime;
			}
			else Tilt -= increment * Time.deltaTime;
			return;
#endif
#pragma warning disable 0162
			if( Input.acceleration.x > 0 )
			{
				Tilt += increment * Time.deltaTime;
			}
			else Tilt -= increment * Time.deltaTime;
#pragma warning restore 0162
		}
	}

}