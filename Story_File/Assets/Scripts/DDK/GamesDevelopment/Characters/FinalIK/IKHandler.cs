//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.Events;
using DDK.Base.Classes;


#if USE_FINAL_IK
using DDK.GamesDevelopment.Characters;
using RootMotion.FinalIK;
#endif


namespace DDK.GamesDevelopment.Characters.FinalIK {
	
	/// <summary>
	/// This allows handling multiple IK Components for one character.
	/// </summary>
	public class IKHandler : MonoBehaviourExt {
		
		#if USE_FINAL_IK

		/// <summary>
		/// This variable will be removed in the future.
		/// </summary>
//		[ShowIfAttribute( "_FixReference" )]
		public SearchableGameObject character = new SearchableGameObject();
		[Tooltip("The character that will be handled. If null, an IK component will be searched in this object and its children")]
		public SearchableIK ik = new SearchableIK();
		[Tooltip("This will be used to split some functions's arguments")]
		public char argsSeparator = ':';
		public FBBIKTargetHandler[] fbbIkTargetHandlers = new FBBIKTargetHandler[0];
		public AimIKTargetHandler[] aimIkTargetHandlers = new AimIKTargetHandler[0];


		protected bool _FixReference()
		{
			if( !string.IsNullOrEmpty( character.objName ) )
			{
				ik.objName = character.objName;
			}
			return false;
		}
				

		private IK _character;
		protected IK _Character 
        {
			get
            {
				if( !_character )
				{
					_FixReference();
                    if( ik.m_gameObject )
                    {
                        _character = ik.m_gameObject.GetComponentInChildren<IK>();
                    }
				}
				if( !_character )
				{
					Debug.LogWarning("No /character/ named \""+ ik.objName +"\" was found..");
					_proceed = false;
				}
				else _proceed = true;
				return _character;
			}
		}
		protected bool _proceed;
		#if USE_FINAL_IK
		protected FullBodyBipedIK _fbbik;
		public FullBodyBipedIK fbbik {
			get{
				if( !_fbbik )
				{
					_fbbik = _Character.GetComponentInChildren<FullBodyBipedIK>();
				}
				if( !_fbbik && _Character.GetParent() )
				{
					_fbbik = _Character.GetParent().GetComponentInChildren<FullBodyBipedIK>();
				}
				return _fbbik;
			}
		}
		protected GrounderFBBIK _fbbikGrounder;
		public GrounderFBBIK fbbikGrounder {
			get{
				if( !_fbbikGrounder )
				{
					_fbbikGrounder = _Character.GetComponentInChildren<GrounderFBBIK>();
				}
				if( !_fbbikGrounder && _Character.GetParent() )
				{
					_fbbikGrounder = _Character.GetParent().GetComponentInChildren<GrounderFBBIK>();
				}
				return _fbbikGrounder;
			}
		}
		protected AimIK _aimIk;
		public AimIK aimIk {
			get{
				if( !_aimIk )
				{
					_aimIk = _Character.GetComponentInChildren<AimIK>();
				}
				if( !_aimIk && _Character.GetParent() )
				{
					_aimIk = _Character.GetParent().GetComponentInChildren<AimIK>();
				}
				return _aimIk;
			}
		}
		#endif
		
		
		
		// Use this for initialization
		void Start () {

			if( !_Character )
			{
				Debug.LogWarning( "There is no GenericCharacter", gameObject );
				enabled = false;
			}
		}



		#region PUBLIC FUNCTIONS
		public void MoveToTarget( int fbbIkIndex )
		{
			if( !_proceed )
				return;
			if( fbbIkIndex >= fbbIkTargetHandlers.Length )
				return;
			fbbIkTargetHandlers[ fbbIkIndex ].MoveToTarget( fbbik );
		}
		public void LookAtTarget( int aimIkIndex )
		{
			if( !_proceed )
				return;
			if( aimIkIndex >= aimIkTargetHandlers.Length )
				return;
			aimIkTargetHandlers[ aimIkIndex ].LookAtTarget( aimIk );
		}
		public void AnimAimWeight( string aimIkIndex_target_animDuration = "0:0f:1f" )
		{
			if( !_proceed )
				return;
			string[] args = aimIkIndex_target_animDuration.Split( argsSeparator );
			if( args.Length < 2 )
			{
				Debug.LogWarning ("The specified argument string has less than 2 values. It should have at least the first two. Pattern: aimIkIndex, targetWeight, animDuration");
				return;
			}
			if( args[0].ToInt() >= aimIkTargetHandlers.Length )
				return;
			if( args.Length == 3 )
			{
				aimIkTargetHandlers[ args[0].ToInt() ].AnimAimWeight( aimIk, args[1].ToFloat(), args[2].ToFloat() );
			}
			else aimIkTargetHandlers[ args[0].ToInt() ].AnimAimWeight( aimIk, args[1].ToFloat() );
		}
		public void SetAnimationReturnT( int fbbIkIndex )
		{
			_SetAnimationReturn( fbbIkIndex, true );
		}
		public void SetAnimationReturnF( int fbbIkIndex )
		{
			_SetAnimationReturn( fbbIkIndex, false );
		}
		public void SetFBBIKIterations( int iterations )
		{
			fbbik.solver.iterations = iterations;
		}
		public void SetAimMaxIterations( int iterations )
		{
			aimIk.solver.maxIterations = iterations;
		}
		#endregion
		

		#region PUBLIC MULTI PARAM FUNCTIONS
		public void MoveToTarget( Transform target, IKTargetHandler.IKAnimation animation, FBBIKTargetHandler.Effector effector )
		{
			MoveToTargetCo( target, animation, effector ).Start();
		}
		public IEnumerator MoveToTargetCo( Transform target, IKTargetHandler.IKAnimation animation, FBBIKTargetHandler.Effector effector )
		{
			if( !_proceed )
				yield break;
			
			float duration = animation.animDuration;
			float stayDuration = animation.stayAtTargetDuration;
			
            animation._initialWeight = fbbik.GetFBBIKPositionWeight( effector.fbbEffector );
            animation._initialRotWeight = fbbik.GetFBBIKRotationWeight( effector.fbbEffector );
			
            fbbik.MoveEffector( target, animation.targetWeight, duration, effector.fbbEffector );
            fbbik.RotateEffector( target, animation.targetRotationWeight, duration, effector.fbbEffector );
			if( fbbik.solver.GetBendGoal( effector.fbbEffector ) )
				fbbik.MoveBendGoal( null, animation.targetWeight * effector.bendGoalMultiplier, 
				                        duration * ( animation.targetWeight - effector.bendGoalDelay ),
				                        effector.fbbEffector, duration * effector.bendGoalDelay );
            while( fbbik.GetFBBIKPositionWeight( effector.fbbEffector ) != animation.targetWeight )
            {
                yield return null;
            }
			//RETURN OR STAY PERMANENTLY
			yield return new WaitForSeconds( stayDuration );
			if( stayDuration <= 0f )
			{
				yield break;
			}
            if( animation._return )
            {
                fbbik.MoveEffector( target, animation._initialWeight, duration, effector.fbbEffector );
                fbbik.RotateEffector( target, animation._initialRotWeight, duration, effector.fbbEffector );
                if( fbbik.solver.GetBendGoal( effector.fbbEffector ) )
                    yield return fbbik.MoveBendGoalCo( null, animation._initialWeight * effector.bendGoalMultiplier, 
                                                           duration * effector.bendGoalDelay,
                                                           effector.fbbEffector, 
                                                           duration * ( animation.targetWeight - effector.bendGoalDelay ) ).Start();
            }
		}
		public void AnimEffectorWeight( float target, float duration, FBBIKTargetHandler.Effector effector )
		{
			AnimEffectorWeightCo( target, duration, effector ).Start();
		}
		public IEnumerator AnimEffectorWeightCo( float target, float duration, FBBIKTargetHandler.Effector effector )
		{
			if( !_proceed )
				yield break;
			
			fbbik.MoveEffector( null, target, duration, effector.fbbEffector );
			if( fbbik.solver.GetBendGoal( effector.fbbEffector ) )
				fbbik.MoveBendGoal( null, target * effector.bendGoalMultiplier, 
				                   duration * ( target - effector.bendGoalDelay ),
				                   effector.fbbEffector, duration * effector.bendGoalDelay );
			yield return new WaitForSeconds( duration );
		}
		public void LookAtTarget( Transform target, IKTargetHandler.IKAnimation animation )
		{
			LookAtTargetCo( target, animation ).Start();
		}
		public IEnumerator LookAtTargetCo( Transform target, IKTargetHandler.IKAnimation animation )
		{
			if( !_proceed )
				yield break;

			animation._initialWeight = aimIk.solver.IKPositionWeight;
			yield return aimIk.LookAtCo( target, animation.targetWeight, animation.animDuration ).Start();
			//RETURN OR STAY PERMANENTLY
			yield return new WaitForSeconds( animation.stayAtTargetDuration );
			if( animation.stayAtTargetDuration > 0f )
			{
				if( animation._return )
				{
					yield return aimIk.LookAtCo( target, animation._initialWeight, animation.animDuration ).Start();
				}
			}
		}
		public void LookAtTarget( Transform target, float weightTarget, float animDuration )
		{
			if( !_proceed )
				return;
			aimIk.LookAt( target, weightTarget, animDuration );
		}
        public void LookAtTarget( string targetTag, IKTargetHandler.IKAnimation animation )
        {
            LookAtTargetCo( targetTag, animation ).Start();
        }
        public IEnumerator LookAtTargetCo( string targetTag, IKTargetHandler.IKAnimation animation )
        {
            yield return LookAtTargetCo( targetTag.Find<Transform>(), animation ).Start();
        }
        public void LookAtTarget( string targetTag, float weightTarget, float animDuration )
        {
            LookAtTarget( targetTag.Find<Transform>(), weightTarget, animDuration );
        }
		public void AnimAimWeight( float weightTarget, float animDuration )
		{
			AnimAimWeightCo( weightTarget, animDuration ).Start();
		}
		public IEnumerator AnimAimWeightCo( float weightTarget, float animDuration )
		{
			if( !_proceed )
				yield break;
			yield return aimIk.solver.AnimAimWeightCo( animDuration, weightTarget ).Start();
		}
        public void ChangeWeight(float finalWeight, float duration)
        {
            ChangeWeightCo(finalWeight, duration).Start();
        }
        private IEnumerator ChangeWeightCo(float finalWeight, float duration)
        {
            IKSolver ik = fbbik.GetIKSolver();
            float startWeight = ik.IKPositionWeight;
            float laspsedTime = duration;
            while (laspsedTime > 0)
            {
                ik.IKPositionWeight = Mathf.Lerp(finalWeight,startWeight, laspsedTime / duration);
                laspsedTime -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            ik.IKPositionWeight = finalWeight;
        }
        #endregion



        protected void _SetAnimationReturn( int fbbIkIndex, bool _return )
		{
			if( !_proceed )
				return;
			if( fbbIkIndex >= fbbIkTargetHandlers.Length )
				return;
			fbbIkTargetHandlers[ fbbIkIndex ].SetAnimationReturn( _return );
		}
		
		#else
		[HelpBoxAttribute( MessageType.Warning )]
		public string msg = "The USE_FINAL_IK scripting symbol must be defined";
		#endif
		
	}
	
}
