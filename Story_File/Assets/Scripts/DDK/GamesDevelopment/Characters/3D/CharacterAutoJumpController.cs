using System;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;

using UnityEngine;
using DDK.Base.Statics;

#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace DDK.GamesDevelopment.Characters._3D
{
    public class CharacterAutoJumpController : MonoBehaviour
    {
        public enum DelegateUsage
        {
            PreviousTarget,
            CurrentPosition
        }           


        public SearchableAnimator animator = new SearchableAnimator();
        [Tooltip("The animator's trigger param that makes the character jump")]
        public string jump = "Jump";
        public AgentGoalsController agentGoalsController;
        [Space( 5f )]
        public bool useRigidbody = true;
        public bool isNavMeshAgent = true;

        public Transform[] goals;

        [InspectorButton ( "JumpToNextGoal", true )]
        public bool jumpToNextGoal = false;

        [InspectorButton ( "JumpWithVelocity", true )]
        public bool jumpWithVelocityVector = false;

        public Vector3 velocityVector = Vector3.up;

        [Space ( 10f )]
        public float jumpTime = 1f;

        public float turningSpeed = 1f;
        public float maxTargetDistance = 5f;
        public float velocityZeroTolerance = Mathf.Epsilon;

        [Space ( 10f )]
        public bool matchJumpClipSpeed = true;

        [Range ( 0f, 1f )]
        public float jumpingAnimationFactor = 1f;
        public float jumpClipLength;

        public string jumpAnimationName = "Jump";

        public DelayedAction[] afterJumpEvent;
        public DelayedAction[] beforeJumpEvent;
        public Action<Transform> beforeJumpAction;
        public Action<Transform> afterJumpAction;

        private bool _triedToGetAnimator;
        private bool _triedToGetAgentGoalsController;

        private Transform _previousTarget;
        private Rigidbody _rigidBody;
        private int _goalIndex = 0;

        private NavMeshAgent _navMeshAgent;



        public DelegateUsage ParamAfterJump { get; set; }
        public DelegateUsage ParamBeforeJump { get; set; }

        private Transform _character;
        public Transform _Character 
        { 
            get
            {
                if ( _character == null || _character.GetType() != typeof ( Transform ) )
                {
                    _character = transform;
                }
                return _character;
            }
            set
            {
                _character = value;
            }
        }
        private Animator _animator;
        protected Animator _Animator
        {
            get
            {
                if( !_animator && !_triedToGetAnimator )
                {
                    if( !animator.m_object )
                        _animator = animator.m_object = GetComponent<Animator>();
                    else return _animator = animator.m_object;
                    _triedToGetAnimator = true;
                }
                if ( !_animator )
                {
                    Utilities.LogError( "The character has no Animator controller", gameObject );
                }
                return _animator;
            }
        }
        private AgentGoalsController _agentGoalsController;
        protected AgentGoalsController _AgentGoalsController
        {
            get
            {
                if( !_agentGoalsController && !_triedToGetAgentGoalsController )
                {
                    if( !agentGoalsController )
                        _agentGoalsController = agentGoalsController = GetComponent<AgentGoalsController>();
                    else return _agentGoalsController = agentGoalsController;
                    _triedToGetAgentGoalsController = true;
                }
                if ( !_agentGoalsController )
                {
                    Utilities.LogError( "The character has no Agent Goals Controller", gameObject );
                }
                return _agentGoalsController;
            }
        }



        void Awake()
        {
			if ( _Character == null )
            {
                _Character = transform;
            }                

            if ( useRigidbody )
            {
                _rigidBody = _Character.GetComponent<Rigidbody>();
            }

            if ( useRigidbody )
            {
                _navMeshAgent = _Character.GetComponent<NavMeshAgent>();
            }
        }
        void Start()
        {
            ParamAfterJump = DelegateUsage.PreviousTarget;
            ParamBeforeJump = DelegateUsage.PreviousTarget;
        }


        public void JumpWithVelocity()
        {
            StartCoroutine ( _JumpVelocity ( velocityVector ) );
        }
        public void JumpToNextGoal()
        {
            if ( _goalIndex >= goals.Length )
            {
                _goalIndex = 0;
            }

            StartCoroutine ( JumpToGoalCo ( goals[_goalIndex++] ) );
        }
        private IEnumerator _JumpVelocity ( Vector3 velocity )
        {
            if ( !useRigidbody )
            {
                yield break;
            }

            bool trueUPosition = false, trueURotation = false;

            if ( isNavMeshAgent && _navMeshAgent.enabled )
            {
                if ( _navMeshAgent.updatePosition )
                {
                    _navMeshAgent.updatePosition = false;
                    trueUPosition = true;
                }

                if ( _navMeshAgent.updateRotation )
                {
                    _navMeshAgent.updateRotation = false;
                    trueURotation = true;
                }

				_navMeshAgent.isStopped = true;
            }

            if ( _Animator )
            {
                Jump();
            }

            _rigidBody.AddForce ( velocity, ForceMode.VelocityChange );

            while ( Mathf.Abs ( _rigidBody.velocity.magnitude ) >
                    velocityZeroTolerance )
            {
                yield return null;
            }

            if ( isNavMeshAgent && _navMeshAgent.enabled )
            {
                _navMeshAgent.nextPosition = _Character.position;

                if ( trueURotation )
                {
                    _navMeshAgent.updatePosition = true;
                }

                if ( trueUPosition )
                {
                    _navMeshAgent.updateRotation = true;
                }

				_navMeshAgent.isStopped = false;
            }
        }



        public void Jump()
        {
            _Animator.SetTriggerValue( jump );
        }
        public void JumpToGoal ( Transform target )
        {
            StartCoroutine ( JumpToGoalCo ( target ) );
        }
        public IEnumerator JumpToGoalCo ( Transform target )
        {
            Vector3 targetPos = target.position;

            if ( targetPos.Distance ( _Character.position ) > maxTargetDistance )
            {
                Debug.LogWarning ( this + " jump target is too far away." );
                yield break;
            }

            float angle = _Character.gameObject.GetSignedAngleTo ( target );
            bool trueUPosition = false;
            bool trueURotation = false;

            if ( isNavMeshAgent && _navMeshAgent.enabled )
            {
                if ( _navMeshAgent.updatePosition )
                {
                    _navMeshAgent.updatePosition = false;
                    trueUPosition = true;
                }
                if ( _navMeshAgent.updateRotation )
                {
                    _navMeshAgent.updateRotation = false;
                    trueURotation = true;
                }
				_navMeshAgent.isStopped = true;
            }

            if ( _AgentGoalsController )
            {
                _AgentGoalsController.TurnWithSpeed ( angle, turningSpeed );
            }
            else
            {
                _Character.TurnToDirectionCo ( targetPos, turningSpeed ).Start();
            }

            if ( beforeJumpEvent != null )
            {
                beforeJumpEvent.InvokeAll();
            }

            if ( beforeJumpAction != null )
            {
                switch ( ParamBeforeJump )
                {
                    case DelegateUsage.PreviousTarget:
                        if ( _previousTarget != null )
                        {
                            beforeJumpAction ( _previousTarget );
                        }

                        break;
                    case DelegateUsage.CurrentPosition:
                        beforeJumpAction ( _Character.transform );
                        break;
                }
            }

            if ( useRigidbody )
            {
                Vector3 velocity = ThrowSpeed ( _Character.position, targetPos, jumpTime );
                _rigidBody.AddForce ( velocity, ForceMode.VelocityChange );

                while ( Mathf.Abs ( _rigidBody.velocity.magnitude ) > velocityZeroTolerance )
                {
                    yield return null;
                }
            }
            else
            {
                if ( _Animator )
                {
                    if ( matchJumpClipSpeed )// match jump speed with animation clip
                    {
                        _Animator.speed = ( jumpClipLength / jumpTime ) * jumpingAnimationFactor;
                    }

                    Jump();
                    yield return StartCoroutine ( _Character.MoveToTargetCo( targetPos, jumpTime ) );

                    if ( matchJumpClipSpeed )
                    {
                        _Animator.speed = 1f;
                    }
                }
                else
                {
                    yield return StartCoroutine ( _Character.MoveToTargetCo( targetPos, jumpTime ) );
                }
            }

            if ( isNavMeshAgent && _navMeshAgent.enabled )
            {
                _navMeshAgent.nextPosition = _Character.position;

                if ( trueURotation )
                {
                    _navMeshAgent.updatePosition = true;
                }

                if ( trueUPosition )
                {
                    _navMeshAgent.updateRotation = true;
                }

                _navMeshAgent.nextPosition = targetPos;
				_navMeshAgent.isStopped = false;
            }

            if ( afterJumpEvent != null )
            {
                afterJumpEvent.InvokeAll();
            }

            if ( afterJumpAction != null )
            {
                switch ( ParamAfterJump )
                {
                    case DelegateUsage.PreviousTarget:
                        if ( _previousTarget != null )
                        {
                            afterJumpAction ( _previousTarget );
                        }
                        break;
                    case DelegateUsage.CurrentPosition:
                        afterJumpAction ( _Character.transform );
                        break;
                }
            }

            _previousTarget = target;
        }

        public static Vector3 ThrowSpeed ( Vector3 src, Vector3 dst, float t )
        {
            var direction = dst - src;
            var directionXZ = direction;
            directionXZ.y = 0;
            var y = direction.y;
            var xz = directionXZ.magnitude;
            // compute starting speed for xz and y
            // deltaX = v0 * t + 1/2 * a * t * t (where a is -gravity)
            var v0Y = y / t + 0.5f * Physics.gravity.magnitude * t;
            var v0XZ = xz / t;
            var result = directionXZ.normalized;
            result *= v0XZ;
            result.y = v0Y;
            return result;
        }
    }
}