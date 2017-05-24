//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.GamesDevelopment.Characters;
using DDK.Base.Extensions;


namespace DDK.GamesDevelopment.Characters._3D 
{
	[RequireComponent( typeof( Animator ), typeof(Rigidbody) )]
	public class CharacterAutoClimbController : MonoBehaviour 
    {		
        [Tooltip("The animator's bool parameter name that controls the climb state")]
        public string climb = "Climb";
        [Tooltip("The animator's float parameter name that controls the horizontal climb")]
        public string climbX = "ClimbX";
        [Tooltip("The animator's float parameter name that controls the vertical climb")]
        public string climbY = "ClimbY";
        [Tooltip("Animator's params that will be set to false before /climb/ is set to true")]
        public string[] boolParamsToReset = new string[]{ "Walk", "Run", "Sit", "SitFloor", "CoverFace" };
        [Space( 5f )]
		public bool climbOnStart = true;
		public Transform[] goals;
		[Indent(1)]
		public bool moveToNextGoal;
		[Tooltip("This value will be added to the character's destination (current goal's position)")]
		public Vector2 goalOffset;
		public Vector2 maxSpeed = new Vector2( 2f, 2f );
		[Indent(1)]
		public float acceleration = 0.1f;
		public float minDistanceToSlowDown = 1f;
		[Indent(1)]
		public float deceleration = 0.1f;
		
		
		
		protected Vector2 _velocity;
        private Animator _animator;
        internal Animator _Animator
        {
            get
            {
                if( !_animator )
                    _animator = GetComponent<Animator>();
                return _animator;
            }
        }
		internal Rigidbody _rigidbody;
		internal int _currentGoal;
		internal Vector3 _destination;
		internal delegate void GoalReachedEvent();
		internal GoalReachedEvent onGoalReached;
		#region UNIQUE GOAL
		private Transform _goal;
		/// <summary>
		/// This will only be used if the goals array is empty or null. There will be a unique goal and the character will
		/// always move to it unless it is already there.
		/// </summary>
		internal Transform _Goal 
        {
			get
            {
				return _goal;
			}
			set
            {
				_isMovingToGoal = false;
				_goal = value;
			}
		}
		/// <summary>
		/// This is applies only when using the unique goal, when no goals are specified in the array or an array element is null.
		/// </summary>
		protected bool _isMovingToGoal;
		#endregion
			

		
		void Start () 
        {			
			StartCoroutine( _Start () );
		}
		IEnumerator _Start()
		{
			yield return null;
			if( goals.Length > 0 )
			{
				for( int i=0; i<goals.Length; i++ )
				{
					if( goals[i] == null )
					{
						Debug.Log( "Goal index ["+i+"] is null, using unique goal instead.." );
						_Goal = transform;
					}
				}
			}
			if( goals.Length == 0 )
			{
				Debug.Log( "There are no goals, using unique goal instead.." );
				_Goal = transform;
			}
			
			if( climbOnStart )
			{
				Climb();
			}
			_rigidbody = GetComponent<Rigidbody>();
		}		
		void Update()
		{
			if( moveToNextGoal )
			{
				MoveToNextGoal();
				moveToNextGoal = false;
			}
			else if( _Goal && !_isMovingToGoal )
			{
				_isMovingToGoal = true;
				MoveToNextGoal();
			}
		}		
		
		
		/// <summary>
        /// Moves the character to the next goal.
        /// </summary>
		public void MoveToNextGoal()
		{
			if( goals.Length < _currentGoal + 1 && _Goal == null )
			{
				Debug.Log ( "There are no more goals" );
				return;
			}
			
			_rigidbody.isKinematic = true;
			
			if( _Goal )
			{
				StartCoroutine( _MoveToGoal( _Goal ) );
			}
			else StartCoroutine( _MoveToNextGoal() );
		}		
		/// <summary>
		/// Returns the character's position taking into account its goal offset.
		/// </summary>
		/// <returns>The position.</returns>
		public Vector3 RealPosition()
		{
			return (Vector3)(transform.position - (Vector3)goalOffset);
		}
        public void Climb( bool climbing = true )
        {
            _ResetBools();
            if( _rigidbody )
            {
                _rigidbody.isKinematic = climbing;
            }
            _Animator.SetBoolValue( climb, climbing );
        }
        /// <summary>
        /// Directly sets the climb X and Y values.
        /// </summary>
        public void SetClimb(float xSpeed, float ySpeed)
        {
            _Animator.SetFloatValue( climbX, xSpeed );
            _Animator.SetFloatValue( climbY, ySpeed );
        }
        /// <summary>
        /// Directly sets the climb values.
        /// </summary>
        public void SetClimb( Vector2 speed )
        {
            _Animator.SetFloatValue(climbX, speed.x );
            _Animator.SetFloatValue( climbY, speed.y );
        }
		
		
		#region PRIVATE / PROTECTED
        /// <summary>
        /// Moves the character to the next goal, as specified in the /goals/ array.
        /// </summary>
		protected IEnumerator _MoveToNextGoal()
		{
			yield return StartCoroutine( _MoveToGoal( goals[ _currentGoal ] ) );
			_currentGoal++;
			
			if( goals.Length < _currentGoal + 1 )
			{
				_rigidbody.isKinematic = false;
			}
		}		
        /// <summary>
        /// Moves the character to the specified /goal/.
        /// </summary>
		protected IEnumerator _MoveToGoal( Transform goal )
		{
			//MOVE UNTIL GOAL IS REACHED
			_velocity = Vector2.zero;
			_destination = goal.position + (( goal == transform ) ? Vector3.zero : (Vector3)goalOffset);
			float distance = transform.position.Distance (_destination);
			while( distance > minDistanceToSlowDown * 0.5f  )//If point hasn't been reached: keep moving
			{
				distance = transform.position.Distance( _destination );
				if( distance < minDistanceToSlowDown )//SLOW DOWN
				{
					if( _velocity.sqrMagnitude > 0.2f )
					{
						_velocity = Vector2.MoveTowards( _velocity, Vector2.zero, deceleration * Time.deltaTime );
					}
				}
				else _velocity +=  (Vector2)transform.InverseTransformDirection( (_destination - transform.position) ).normalized * acceleration;
				_velocity.Normalize();
				SetClimb( _velocity.Clamp2( maxSpeed, -maxSpeed ) );
				yield return null;
			}
			SetClimb( Vector2.zero );

			if( goal != transform )
			{
				if( onGoalReached != null )
				{
					onGoalReached();
				}
				Debug.Log ( "Next Goal Reached!" );
			}
		}
        /// <summary>
        /// Resets the animator's specified bool parameters.
        /// </summary>
        protected void _ResetBools()
        {
            for( int i=0; i<boolParamsToReset.Length; i++ )
            {
                _animator.SetBoolValue( boolParamsToReset[ i ], false );
            }
        }
		#endregion PRIVATE / PROTECTED		
	}
}