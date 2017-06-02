//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


#if USE_FINAL_IK
using DDK.GamesDevelopment.Characters;
using RootMotion.FinalIK;
#endif


namespace DDK.GamesDevelopment.Characters.FinalIK {

	[System.Serializable]
	public class FBBIKTargetHandler : IKTargetHandler {
		
		#if USE_FINAL_IK
		[System.Serializable]
		public class TargetReachedActivator : Activator {

			[Tooltip("Parent To Effector")]
			[ShowIfAttribute( "_IsObjValid", 1 )]
			public bool parentToEffector;
			

			/// <summary>
			/// Activates or spawns the specified /obj/ gameobject.
			/// </summary>
			public void Activate( FullBodyBipedIK character, FullBodyBipedEffector fbbEffector )
			{
				if( !character || !obj )
				{
					return;
				}
				Transform effector = character.GetBone( fbbEffector );
				var _obj = obj.SetActiveInHierarchy(); //(GameObject) GameObject.Instantiate( activateOnTargetReached, effector.position, effector.rotation );
				if( onTarget )
				{
					_obj.transform.position = effector.position;
					_obj.transform.rotation = effector.rotation;
				}
				if( parentToEffector )
				{
					_obj.transform.SetParent( effector );
				}
			}
		}
		[System.Serializable]
		public class Effector {

			public FullBodyBipedEffector fbbEffector;
			[Tooltip("If the specified effector's bend goal is not null, this will be multiplied by the effector's weight and applied to the bend goal's weight")]
			[ShowIfAttribute( 0f, 1f, "_DoesEffectorHaveBendGoal", 0 )]
			public float bendGoalMultiplier;
			[Tooltip("The delay will be the /animDuration/ times this value.")]
			[ShowIfAttribute( 0f, 0.9f, "_IsBendGoalValid", 1 )]
            public float bendGoalDelay;


			#region VALIDATION FUNCTIONS
			protected bool _DoesEffectorHaveBendGoal()
			{
				switch( fbbEffector )
				{
				case FullBodyBipedEffector.LeftFoot: return true;
				case FullBodyBipedEffector.RightFoot: return true;
				case FullBodyBipedEffector.LeftHand: return true;
				case FullBodyBipedEffector.RightHand: return true;
                }
                return false;
            }
			protected bool _IsBendGoalValid()
			{
				if( !_DoesEffectorHaveBendGoal() )
					return false;
				return bendGoalMultiplier > 0f;
			}
            #endregion
		}



		[ShowIfAttribute( "_IsIkTargetNull", true )]
		public TargetReachedActivator activateOnTargetReached;
		[ShowIfAttribute( "_IsIkTargetNull", true )]
		public TargetReachedActivator activateOnTargetReachedAndStayed;
		[Space(10f)]
		[ShowIfAttribute( "_IsIkTargetNull", true )]
		public Effector effector = new Effector();



		internal void SetAnimationReturn( bool _return )
		{
			animation._return = _return;
		}
		internal void MoveToTarget( FullBodyBipedIK character )
		{
			ActionToTarget( character );
		}		
		internal void OnTargetReached( FullBodyBipedIK character )
		{
			base.OnTargetReached();
			activateOnTargetReached.Activate( character, effector.fbbEffector );
		}
		internal void OnTargetReachedAndStayed( FullBodyBipedIK character )
		{
			base.OnTargetReachedAndStayed();
			activateOnTargetReachedAndStayed.Activate( character, effector.fbbEffector );
		}


		#region PUBLIC MULTI PARAM FUNCTIONS
		/// <summary>
		/// Animates the effector until it reaches the specified target.
		/// </summary>
		/// <param name="target">If null, the FullBodyBipedIK's solver's effector's target will be used instead.</param>
		public void MoveEffector( FullBodyBipedIK fbbik, Transform target, float posWeight = 1f, float duration = 1f, 
		                         FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
		{
			if( !fbbik )
				return;
			var solver = fbbik.solver;
			solver.AnimEffectorTo( target, duration, effector, posWeight, -1f );
		}
		/// <summary>
		/// Animates the bend goal until it reaches the specified goal.
		/// </summary>
		/// <param name="goal">If null, the FullBodyBipedIK's solver's chain's bend goal will be used instead.</param>
		public IEnumerator MoveBendGoalCo( FullBodyBipedIK fbbik, Transform goal, float weight = 1f, float duration = 1f,
		                                  FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float delay = 0f )
		{
			if( !fbbik )
				yield break;
			var solver = fbbik.solver;
			yield return solver.AnimBendToCo( goal, duration, effector, weight ).Start( delay );
		}
		/// <summary>
		/// Animates the bend goal until it reaches the specified goal.
		/// </summary>
		/// <param name="goal">If null, the FullBodyBipedIK's solver's chain's bend goal will be used instead.</param>
		public void MoveBendGoal( FullBodyBipedIK fbbik, Transform goal, float weight = 1f, float duration = 1f, 
		                         FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float delay = 0f )
		{
			fbbik.MoveBendGoalCo( goal, weight, duration, effector, delay ).Start();
		}
		/// <summary>
		/// Animates the effector until it reaches the specified target.
		/// </summary>
		/// <param name="target">If null, the FullBodyBipedIK's solver's effector's target will be used instead.</param>
		public void RotateEffector( FullBodyBipedIK fbbik, Transform target, float rotWeight = 1f, float duration = 1f, 
		                           FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
		{
			if( !fbbik )
				return;
			var solver = fbbik.solver;
			solver.AnimEffectorTo( target, duration, effector, -1f, rotWeight );
		}
		/// <summary>
		/// Animates the effector until it is aligned with the specified target.
		/// </summary>
		/// <param name="target">If null, the FullBodyBipedIK's solver's effector's target will be used instead.</param>
		public void AlignEffector( FullBodyBipedIK fbbik, Transform target, float posWeight = 1f, float rotWeight = 1f, 
		                          float duration = 1f, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
		{
			fbbik.MoveEffector( target, posWeight, duration, effector );
			fbbik.RotateEffector( target, rotWeight, duration, effector );
		}
		#endregion
		

		/// <summary>
		/// Look At
		/// </summary>
		protected override IEnumerator _ActionToTarget( IK character )
		{
			yield return _ActionToTarget( character as FullBodyBipedIK );
        }
        /// <summary>
		/// Move to
		/// </summary>
		protected IEnumerator _ActionToTarget( FullBodyBipedIK character )
		{
			float duration = animation.animDuration;
			float stayDuration = animation.stayAtTargetDuration;

			MoveEffector( character, m_IKTarget, 1f, duration, effector.fbbEffector );
			if( character.solver.GetBendGoal( effector.fbbEffector ) )
				MoveBendGoal( character, null, 1f * effector.bendGoalMultiplier, duration * ( 1 - effector.bendGoalDelay ),
				                       effector.fbbEffector, duration * effector.bendGoalDelay );
			yield return new WaitForSeconds( duration );
			OnTargetReached( character );
			//RETURN OR STAY PERMANENTLY
			yield return new WaitForSeconds( stayDuration );
			if( stayDuration > 0f )
			{
				if( animation._return )
				{
					MoveEffector( character, m_IKTarget, 0f, duration, effector.fbbEffector );
					if( character.solver.GetBendGoal( effector.fbbEffector ) )
						yield return MoveBendGoalCo( character, null, 0f * effector.bendGoalMultiplier, duration * effector.bendGoalDelay,
						                                      effector.fbbEffector, duration * ( 1 - effector.bendGoalDelay ) ).Start();
				}
				OnTargetReachedAndStayed( character );
			}
		}
		
		#else
		[HelpBoxAttribute( MessageType.Warning )]
		public string msg = "The USE_FINAL_IK scripting symbol must be defined";
		#endif
		
	}


}