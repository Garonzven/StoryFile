//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Statics;


namespace DDK._3D 
{
	/// <summary>
	/// Animates a transform to look at a target. The transform will animate-look at a target even if it 
	/// changes at runtime.
	/// </summary>
	[ExecuteInEditMode]
	public class LookAt : MonoBehaviourExt 
    {
		/// <summary>
		/// If null, it will be searched by the specified name.
		/// </summary>
		[Tooltip("If null, it will be searched by the specified name.")]
		public Transform _target;
		/// <summary>
		/// If empty, the Main Camera will be used
		/// </summary>
		[Tooltip("If empty, the Main Camera will be used")]
		public string target = "Main Camera";
		/// <summary>
		/// The initial duration of the transition/animation in seconds
		/// </summary>
		[Tooltip("The initial duration of the transition/animation in seconds")]
		[Space(5f)]
		public float period;
		/// <summary>
		/// If false, the object will keep its Z axis rotation value at 0
		/// </summary>
		[Tooltip("If false, the object will keep its Z axis rotation value at 0")]
		public bool lockZ = true;
		/// <summary>
		/// If true, the component will be disabled after the object looks at the target
		/// </summary>
		[Tooltip("If true, the component will be disabled after the object looks at the target")]
		public bool disableAfterDone;
		[Tooltip("If true, the component will be destroyed after the object looks at the target")]
		public bool destroyAfterDone;
		[Space(10f)]
		[Tooltip("Any condition that must be fulfilled for the look animation to start")]
        public When when = new When( true );



		protected Quaternion _startRotation, _endRotation;
		protected float _time;



		// Use this for initialization
		void Start () 
        {
			if( _target )
				return;
			if( string.IsNullOrEmpty( target ) )
			{
				target = Camera.main.name;
			}
			_target = target.Find<Transform>();
		}
		
		// Update is called once per frame
		void Update () 
        {
#if UNITY_EDITOR
			if( !Application.isPlaying ) //IF EXECUTING IN EDIT MODE
			{
				if( !string.IsNullOrEmpty( target ) )
				{
					_target = target.Find<Transform>();
				}
			}
#endif
			if( !_target )
				return;
			
			if( !target.Equals( _target.name ) )//If the target was changed
			{
				ResetTimer();
				target = _target.name;
			}

			if( when.IsConditionFulfilled( gameObject ) )
			{
				//LOOK AT
				_time += Time.deltaTime;
				if( _time >= period ) 
				{
					//END LOOK
					if( lockZ )
					{
                        _endRotation = Quaternion.LookRotation( _target.position - transform.position );
                        transform.rotation = _endRotation;
					}
					else transform.rotation = Quaternion.LookRotation( _target.position.ZeroZ() - transform.position.ZeroZ() );
                    //DISABLE / DESTROY COMP ?
                    if( disableAfterDone )
                    {
                        enabled = false;
                    }
					else if( destroyAfterDone )
					{
						Destroy( this );
					}
                }
                else { //ANIMATE LOOK
                    _LookAt();
				}
                transform.rotation = Quaternion.Euler( transform.rotation.eulerAngles.ClampOpposite( 359f ) );
			}

		}

		
		void _LookAt() 
		{
			_startRotation = transform.rotation;
			_endRotation = Quaternion.LookRotation( _target.position - transform.position );

			if( lockZ )
			{
				_startRotation = Quaternion.Euler( _startRotation.eulerAngles.Zero( false, false, true ) );
				_endRotation = Quaternion.Euler( _endRotation.eulerAngles.Zero( false, false, true ) );
			}

			transform.rotation = Quaternion.Slerp( _startRotation, _endRotation, _time/period );
		}

		/// <summary>
		/// Resets the timer allowing the look at animation to be affected by its period again.
		/// </summary>
		public void ResetTimer()
		{
			_time = 0f;
		}
		public void SetTarget( Transform target )
		{
			if( !target )
				return;
			_target = target;
		}
		/// <summary>
		/// The specified tag will be searched, and the closest object will be set as the /target/
		/// </summary>
		public void SetTargetAsClosestTag( string tag )
		{
			Transform[] targets = tag.FindComponentsByTag<Transform>();
			Transform target = transform.GetClosest( targets );
			if( !target )
				return;
			_target = target;
		}
		public void SetPeriod( float period )
		{
			if( period < 0f )
				period = 0f;
			this.period = period;
		}


		/// <summary>
		/// Adds a component of type /LookAt/ to the specified gameObject.
		/// </summary>
		/// <param name="addTarget">The gameObject into which the component will be added.</param>
		/// <param name="lookTarget">The /target/ of the LookAt component.</param>
		public static LookAt AddAndLookAt( GameObject addTarget, Transform lookTarget )
		{
			if( !addTarget )
			{
				Utilities.LogWarning ("The specified gameObject is null..");
				return null;
			}
			var lookAt = addTarget.AddGetComponent<LookAt>();
			lookAt._target = lookTarget;
			return lookAt;
		}
	}
}
