using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Misc;

namespace DDK.GamesDevelopment.Characters._3D {

	public class AgentGoalsControllerRef : MonoBehaviourExt {

		[Tooltip("The name of the object holding the T component")]
		public string objName;
		[IndentAttribute(1)]
		[Tooltip("Must be true if the specified name is a Tag")]
		public bool nameIsTag;
		[Header("Events")]
		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string msg = "The string param for the Call() function must be in the following format:" +
			"methodName/methodSplitter/value/argsSplitter/value/argsSplitter.... Ex: Talk#T:1. " +
			"NOTE: The allowed types are int, float, and bool (T, True, F, False).";
		#endif
		[Tooltip("This will allow splitting the method Name for the Call() function")]
		public char methodSplitter = '#';
		[Tooltip("This will allow splitting the arguments for the Call() function")]
		public char argsSplitter = ':';
		public ComposedEvent onGoalReached;
		public ComposedEvent onTurnedToTarget;
		protected Transform _currentGoal;
		protected Transform _currentPoint;

		private AgentGoalsController _comp;

		public bool IsMoving {
			get{
				return m_Comp.m_IsMoving;
			}
		}
			
		public AgentGoalsController m_Comp
		{
			get
			{
				#if !UNITY_EDITOR
				if ( _comp == null || _comp.GetType() != typeof(AgentGoalsController) )
				{
				if( !string.IsNullOrEmpty( objName ) )
				{
				if( nameIsTag )
				{
				_comp = objName.FindWithTag<AgentGoalsController>();
				}
				else _comp = objName.Find<AgentGoalsController>();
				}
				}
				#endif
				return _comp;
			}
			set { _comp = value; }
		}		
			
		void Start() 
		{			
			if( !string.IsNullOrEmpty( objName ) )
			{
				if( nameIsTag )
				{
					m_Comp = objName.FindWithTag<AgentGoalsController>();
				}
				else m_Comp = objName.Find<AgentGoalsController>();
			}
		}		
			
		public void Call( string args = "methodName#arg0:arg1" )
		{
			Call( m_Comp, args, methodSplitter, argsSplitter );
		}		
			
		#region MOVING
		/// <summary>
		/// Moves the character to the specified goal.
		/// </summary>
		public void MoveToGoal( Transform goal )
		{
			if( !goal )
				return;
			m_Comp.MoveToGoal( goal );
			_OnGoalReached().Start();
		}		
		/// <summary>
		/// Moves the character to the specified goal.
		/// </summary>
		public IEnumerator MoveToGoalCo( Transform goal )
		{
			if( !goal )
				yield break;
			yield return StartCoroutine( m_Comp.MoveToGoalCo( goal ) );
			_OnGoalReached().Start();
		}	
		public void MoveToNextGoal()
		{
			m_Comp.MoveToNextGoal();
			_OnGoalReached().Start();
		}
        public void MoveThroughPath( Transform[] path )
        {
            StartCoroutine( MoveThroughPathCo( path ) );
        }       
        public IEnumerator MoveThroughPathCo( Transform[] path )
        {
            yield return StartCoroutine( m_Comp.MoveThroughPathCo( path ) );
        }   
		public void SetCurrentGoal( Transform goal )
		{
			if( !goal )
				return;
			_currentGoal = goal;
		}
		public void MoveToCurrentGoal()
		{
			if( !_currentGoal )
			{
				Debug.LogWarning ("CurrentGoal hasn't been set. Call SetCurrentGoal()", gameObject );
				return;
			}
			MoveToGoal( _currentGoal );
			_OnGoalReached().Start();
		}
		public void CancelCurrentMovement()
		{
			m_Comp.CancelCurrentMovement();
		}
		
		public void DisableWhileMoving( Behaviour comp )
		{
			m_Comp.DisableWhileMoving( comp );
		}
		public void DeactivateWhileMoving( GameObject obj )
		{
			m_Comp.DeactivateWhileMoving( obj );
		}
		#endregion
		        
        
		#region TURNING
		/// <summary>
		/// Turns the character the specified degrees
		/// </summary>
		public void Turn( float degrees )
		{
			TurnCo( degrees ).Start();
		}
		/// <summary>
		/// Turns the character the specified degrees
		/// </summary>
		public IEnumerator TurnCo( float degrees )
		{
			yield return m_Comp.Turn( degrees ).Start();
			yield return _OnTurned().Start();
		}
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public void TurnToPoint( Transform point )
		{
			TurnToPointCo( point ).Start();
		}	
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public IEnumerator TurnToPointCo( Transform point )
		{
			if( !point )
				yield break;
			yield return m_Comp.TurnToPoint( point ).Start();
			yield return _OnTurned().Start();
		}	
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public void TurnToPoint( string point )
		{
			Transform _point = point.Find<Transform>();
			if( !_point )
			{
				Debug.LogWarning ("SetCurrentPoint() -> The specified point couldn't be found..", gameObject );
				return;
			}
			TurnToPoint( _point );
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
			yield return m_Comp.TurnWithSpeedToPointCo( point, speed ).Start();
		}
		/// <summary>
		/// Amount should go from 0 to degrees (right) and 0 to -degrees (left)
		/// </summary>
		public void TurnWithSpeed( float degrees, float speed = 1f )
		{
			m_Comp.TurnWithSpeed( degrees, speed );
		}		
		/// <summary>
		/// Amount should go from 0 to degrees (right) and 0 to -degrees (left)
		/// </summary>
		public IEnumerator TurnWithSpeedCo( float degrees, float speed = 1f )
		{
			yield return m_Comp.TurnWithSpeedCo( degrees, speed ).Start();
        }
		/// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public void TurnToPoint( Transform point, float duration )
        {
            m_Comp.TurnToPoint( point, duration );
        }	
        /// <summary>
		/// Turns the character to the specified point
		/// </summary>
		public IEnumerator TurnToPointCo( Transform point, float duration )
		{
            yield return m_Comp.TurnToPointCo( point, duration ).Start();
		}
        /// <summary>
        /// Turns the character to the specified point
        /// </summary>
        public void TurnToPoint( string pointTag, float duration )
        {
            m_Comp.TurnToPoint( pointTag, duration );
        }   
        /// <summary>
        /// Turns the character to the specified point
        /// </summary>
        public IEnumerator TurnToPointCo( string pointTag, float duration )
        {
            yield return m_Comp.TurnToPointCo( pointTag, duration ).Start();
        }
		public void Turn( float degrees, float duration )
		{
            m_Comp.Turn( degrees, duration );
		}		
		public IEnumerator TurnCo( float degrees, float duration )
		{
            yield return m_Comp.TurnCo( degrees, duration ).Start();
        }
        public void TurnToNextPoint()
		{
			m_Comp.TurnToNextPoint().Start();
			_OnTurned().Start();
		}
		public void SetCurrentPoint( Transform point )
		{
			if( !point )
				return;
			_currentPoint = point;
		}
		public void SetCurrentPoint( string point )
		{
			Transform _point = point.Find<Transform>();
			if( !_point )
			{
				Debug.LogWarning ("SetCurrentPoint() -> The specified point couldn't be found..", gameObject );
				return;
			}
			_currentPoint = _point;
		}
		public void TurnToCurrentPoint()
		{
			if( !_currentPoint )
			{
				Debug.LogWarning ("CurrentPoint hasn't been set. Call SetCurrentPoint()", gameObject );
				return;
			}
			TurnToPoint( _currentPoint );
			_OnTurned().Start();
		}
		public void DisableWhileTurning( Behaviour comp )
		{
			m_Comp.DisableWhileTurning( comp );
		}
		public void DeactivateWhileTurning( GameObject obj )
		{
			DeactivateWhileTurning( obj );
		}
		#endregion


		#region EVENTS
		protected IEnumerator _OnGoalReached()
		{
			while( m_Comp != null && m_Comp.m_IsMoving )
				yield return null;
			if ( onGoalReached != null )
				onGoalReached.Invoke();

			if ( onGoalReached == null )
				Debug.Log( "onGoalReached is NULL!!! Check this!" );
			if ( m_Comp == null ) 
				Debug.Log( "m_Comp is NULL!!! Check this!" );
		}
		protected IEnumerator _OnTurned()
		{
			while( m_Comp.m_IsTurning )
				yield return null;
			onTurnedToTarget.Invoke();
		}
		#endregion


		#region MISC
		public void EnableNavAgent( bool enable )
		{
			m_Comp.Agent.enabled = enable;
		}
		#endregion
    }
}
