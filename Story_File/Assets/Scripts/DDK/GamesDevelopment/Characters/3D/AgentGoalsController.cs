//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Statics;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif


namespace DDK.GamesDevelopment.Characters._3D 
{
	[RequireComponent( typeof( Animator ), typeof( NavMeshAgent ) )]
	public class AgentGoalsController : MonoBehaviour 
    {
		[Tooltip("If true, the NavMeshAgent will be enabled when the agent need to move, and disabled when it doesn't")]
		public bool handleAgent = false;
		[Tooltip("The objects representing the agent goals. If empty, the goals (names) array will be used to search the transforms")]
		public Transform[] _goals = new Transform[0];
		[Tooltip("Name of the objects representing the agent goals")]
		public string[] goals = new string[0];
		[Indent(1)]
		public bool moveToNextGoal;

		[Header("Turn")]
        [Tooltip("The animator's param name (and character's animation name), it must be a float param. If the character " +
            "has 2 turn animations, ensure their names end with 'Left' and 'Right', this might be necessary with non-humanoid" +
            " or non-biped characters where mirroring the animation doesn't work.")]
        /// <summary>
        /// The animator's param name (and character's animation name), it must be a float param. If the character has 2 
        /// turn animations, ensure their names end with Left and Right, this might be necessary with non-humanoid or 
        /// non-biped characters where mirroring the animation doesn't work.
        /// </summary>
		public string turn = "Turn";
		public AnimationClip turnClip;
		[IndentAttribute(1)]
		[Tooltip("The total amount of degrees that the animation applies")]
		public float turnAnimationTotalDegrees = 90f;
		public float turnSpeed = 1f;
		public float maxSpeed = 2f;

		[Header("Move")]
		[Tooltip("The animator's param name. It must be a float param")]
		public string forward = "Forward";
		[Indent(1)]
		public float acceleration = 0.5f;
		public float minDistanceToSlowDown = 8f;
		public float distanceTolerance = 0.5f;
		[Indent(1)]
		public float deceleration = 0.1f;
		[Indent(1)]
		public float minSpeed = 0.1f;

		internal delegate void GoalReachedEvent();
		internal GoalReachedEvent onGoalReached;

		/// <summary>
		/// True if the agent is moving to the next goal.
		/// </summary>
		public bool m_IsMoving { get; protected set; }
		public bool m_IsTurning { get; protected set; }
		public AnimationClip m_TurnClip
		{
			get
            {
				if( !turnClip )
					turnClip = (AnimationClip) Character.runtimeAnimatorController.animationClips.GetNamed( turn );
                if( !turnClip )
                    turnClip = (AnimationClip) Character.runtimeAnimatorController.animationClips.GetNamed( turn + LEFT );
				if( !turnClip )
					turnClip = (AnimationClip) Character.runtimeAnimatorController.animationClips.GetNamed( turn + "-L" );
                if( !turnClip )
                    Debug.LogWarning( "The Turn clip hasn't been set/found. This might cause turning issues", gameObject );
				return turnClip;
			}
		}
		public NavMeshAgent Agent
		{
			get
			{
				if ( _agent == null )
				{
					_agent = GetComponent<NavMeshAgent>();
					_agent.updatePosition = false;
					_agent.updateRotation = false;
					_agent.acceleration = acceleration;
				}
				
				return _agent;
			}
			private set { _agent = value; }
		}		
		public Animator Character
		{
			get
			{
				if ( _character == null )
				{
					_character = GetComponent<Animator>();
				}
				
				return _character;
			}
			private set { _character = value; }
		}


		protected int _currentGoal;
		protected float forwardVel;

		private Transform _temporalGoal;
		private Animator _character;		
		private NavMeshAgent _agent;

        private const string LEFT = "Left";

		private bool _cancelTurning = false;

		void Start () 
        {			
			if( _goals.Length == 0 )
				_goals = goals.Find<Transform>().ToArray();
			
			if( _goals.Length > 0 )
			{
				for( int i=0; i<_goals.Length; i++ )
				{
					if( _goals[i] == null )
					{
						Debug.LogWarning( name + ": Goal index ["+i+"] is null, disabling component.." );
						enabled = false;
						return;
					}
				}
			}
			if( _goals.Length == 0 )
			{
				Debug.LogWarning( name + ": There are no goals, disabling component..", gameObject );
				enabled = false;
				return;
			}
		}
		
		void Update()
		{
			if( moveToNextGoal )
			{
				MoveToNextGoal();
				moveToNextGoal = false;
			}
		}
		

		#region MOVE
		/// <summary>
		/// Moves the character to the specified goal.
		/// </summary>
		public void MoveToGoal( Transform goal )
		{
			StartCoroutine( _MoveToGoal( goal ) );
		}		
		public void MoveThroughPath( Transform[] path )
		{
			StartCoroutine( MoveThroughPathCo( path ) );
		}		
		public IEnumerator MoveThroughPathCo( Transform[] path )
		{
			var bDeceleration = deceleration;
			
			for (int i = 0; i < path.Length; i++)
			{
				yield return StartCoroutine( _MoveToGoal( path[i] ) );
				
				if (i > 0 && i < path.Length - 2)
				{
					deceleration = 0f;
				}
				
				if ( i == path.Length - 2 )
				{
					deceleration = bDeceleration;
					
				}
			}
			
			while ( m_IsMoving )
			{
				yield return null;
			}
		}		
		public IEnumerator MoveToGoalCo( Transform goal )
		{
			yield return StartCoroutine( _MoveToGoal(goal) );
			
			while (m_IsMoving)
			{
				yield return null;
			}
		}		
		public void MoveToNextGoal()
		{
			if( _goals.Length < _currentGoal + 1 )
			{
				Debug.Log ( "There are no more goals" );
				return;
			}
			StartCoroutine( _MoveToGoal( _goals[ _currentGoal ] ) );
		}		
		public void DisableWhileMoving( Behaviour comp )
		{
			_DisableWhileMoving( comp ).Start();
		}		
		public void DeactivateWhileMoving( GameObject obj )
		{
			_DeactivateWhileMoving( obj ).Start();
		}	
		#endregion

		#region TURNING
		public IEnumerator TurnToNextPoint()
		{
			yield return StartCoroutine( TurnToPoint( _goals[ _currentGoal ] ) );
		}
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public IEnumerator TurnToPoint( Transform point )
		{
			yield return StartCoroutine( TurnWithSpeedToPointCo( point, turnSpeed ) );
		}		
		/// <summary>
		/// Turns the character the specified degrees (euler)
		/// </summary>
		public IEnumerator Turn( float degrees )
		{
			//TURN
			TurnWithSpeed( degrees, turnSpeed );
			while( m_IsTurning )
				yield return null;
		}
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public void TurnWithSpeedToPoint( Transform point, float speed )
		{
			StartCoroutine( TurnWithSpeedToPointCo( point, speed ) );
		}
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public IEnumerator TurnWithSpeedToPointCo( Transform point, float speed )
		{
			if( !point )
				yield break;
			//Angle between game object and point
			float turnTarget = gameObject.GetSignedAngleTo( point );
			//TURN
			TurnWithSpeed( turnTarget, speed );
			while( m_IsTurning )
				yield return null;
			
            if ( !_cancelTurning )
				transform.LookAt (Vector3.up, point);//Fix small offset
			else
				_cancelTurning = false;
		}
		/// <summary>
		/// Amount should go from 0 to degrees (right) and 0 to -degrees (left)
		/// </summary>
		public void TurnWithSpeed( float degrees, float speed = 1f )
		{
			if ( !m_IsTurning )
			{
				if( !m_TurnClip )
				{
					return;
				}
				StartCoroutine( TurnWithSpeedCo( degrees, speed ) );
			}
			else Debug.Log( name + " already turning", gameObject );
		}		
		/// <summary>
		/// Amount should go from 0 to degrees (right) and 0 to -degrees (left)
		/// </summary>
		public IEnumerator TurnWithSpeedCo( float degrees, float speed = 1f )
		{
            if( m_IsTurning )
            {
                Debug.Log( name + " already turning", gameObject );
                yield break;
            }
            if( !m_TurnClip )
                yield break;
			m_IsTurning = true;
			if( degrees < 0f )
			{
				speed *= -1f;
			}
			Character.SetFloatValue( turn, speed );
            float duration = ( degrees.Abs() * m_TurnClip.length / turnAnimationTotalDegrees ) / speed.Abs();
            float time = 0f;
			if( Character.updateMode == AnimatorUpdateMode.UnscaledTime )
			{
				while( time < duration && m_IsTurning)
                {
                    time += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
			else while( time < duration && m_IsTurning)
            {
                time += Time.deltaTime;
                yield return null;
            }
			Character.SetFloatValue( turn, 0f );
			m_IsTurning = false;
		}
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public void TurnToPoint( Transform point, float duration )
		{
			StartCoroutine( TurnToPointCo( point, duration ) );
        }
        /// <summary>
        /// Turns the character to the specified point
		/// </summary>
		public IEnumerator TurnToPointCo( Transform point, float duration )
		{
			if( !point )
				yield break;
			//Angle between game object and point
			float turnTarget = gameObject.GetSignedAngleTo( point );
			//TURN
			Turn( turnTarget, duration );
			while( m_IsTurning )
                yield return null;
            transform.LookAt( Vector3.up, point );//Fix small offset
        }
        /// <summary>
        /// Turns the character to the specified point
        /// </summary>
        public void TurnToPoint( string pointTag, float duration )
        {
            StartCoroutine( TurnToPointCo( pointTag, duration ) );
        }
        /// <summary>
        /// Turns the character to the specified point
        /// </summary>
        public IEnumerator TurnToPointCo( string pointTag, float duration )
        {
            yield return StartCoroutine( TurnToPointCo( pointTag.FindWithTag<Transform>(), duration ) );
        }
		public void Turn( float degrees, float duration )
		{
			if ( !m_IsTurning )
			{
				if( !m_TurnClip )
				{
					return;
				}
				StartCoroutine( TurnCo( degrees, duration ) );
			}
			else Debug.Log( name + " already turning", gameObject );
		}		
		public IEnumerator TurnCo( float degrees, float duration )
		{
			m_IsTurning = true;
			if( degrees < 0f )
			{
				duration *= -1f;
			}
			Character.SetFloatValue( turn, ( degrees.Abs() * m_TurnClip.length / turnAnimationTotalDegrees ) / duration );//SET SPEED
            float time = 0f;
            duration = duration.Abs();
			if( Character.updateMode == AnimatorUpdateMode.UnscaledTime )
			{
				while( time < duration && m_IsTurning)
                {
                    time += Time.unscaledDeltaTime;
                    yield return null;
                }
			}
			else while( time < duration && m_IsTurning)
            {
                time += Time.deltaTime;
                yield return null;
            }
			Character.SetFloatValue( turn, 0f );
			m_IsTurning = false;
		}
		public void DisableWhileTurning( Behaviour comp )
		{
			_DisableWhileTurning( comp ).Start();
		}		
		public void DeactivateWhileTurning( GameObject obj )
		{
			_DeactivateWhileTurning( obj ).Start();
		}
		public void CancelCurrentMovement()
		{
			m_IsMoving = false;
			m_IsTurning = false;
			_cancelTurning = true;
		}
		#endregion


		#region PROTECTED
		protected IEnumerator _MoveToGoal( Transform goal )
		{
			if( !goal )
				yield break;
			
			// early stop for multiple calls, resets 
			// the destination for running coroutine
			if ( m_IsMoving )
			{
				Agent.destination = goal.position;
				_temporalGoal = goal;
				yield break;
			}
			
			HandleAgent( true );
			m_IsMoving = true;
			yield return StartCoroutine( TurnToPoint( goal ) );
			//MOVE FORWARD UNTIL GOAL IS REACHED
			Agent.destination = goal.position;
			_temporalGoal = goal;
			
			while( /*_agent.remainingDistance*/  transform.position.Distance( Agent.destination ) > distanceTolerance && m_IsMoving )//If point hasn't been reached: keep moving
			{
				transform.LookAt( Vector3.up, _temporalGoal );//Fix small offset
				if( transform.position.Distance( Agent.destination ) /*_agent.remainingDistance*/ < minDistanceToSlowDown )//SLOW DOWN, BUT DON'T STOP
				{
					forwardVel -= deceleration;
				}
				else forwardVel = Agent.velocity.sqrMagnitude;
				Character.SetFloatValue( forward, forwardVel.Clamp( minSpeed, maxSpeed ) );
				
				yield return null;
				Agent.nextPosition = transform.position;//Fix Agent Position
				float angle = Vector3.Angle(transform.forward, _temporalGoal.position - transform.position);
				if (Mathf.Abs (angle) >= 90)
					m_IsMoving = false;
			}
			Character.SetFloatValue( forward, 0f );
			m_IsMoving = false;
			if( onGoalReached != null )
			{
				onGoalReached();
			}
			Debug.Log ( "Next Goal \"" + goal.name + "\" Reached!" );
			HandleAgent( false );
			_currentGoal++;
		}			
		protected IEnumerator _DisableWhileMoving( Behaviour comp )
		{
			comp.enabled = false;
			while( m_IsMoving )
				yield return null;
			comp.enabled = true;
		}		
		protected IEnumerator _DeactivateWhileMoving( GameObject obj )
		{
			obj.SetActive( false );
			while( m_IsMoving )
				yield return null;
			obj.SetActive( true );
		}		
		protected void HandleAgent( bool enable )
		{
			if( !handleAgent )
				return;
			
			Agent.enabled = enable;
		}		
		protected IEnumerator _DisableWhileTurning( Behaviour comp )
		{
			comp.enabled = false;
			while( m_IsTurning )
				yield return null;
			comp.enabled = true;
		}		
		protected IEnumerator _DeactivateWhileTurning( GameObject obj )
		{
			obj.SetActive( false );
			while( m_IsTurning )
				yield return null;
			obj.SetActive( true );
		}
		#endregion
	}
}