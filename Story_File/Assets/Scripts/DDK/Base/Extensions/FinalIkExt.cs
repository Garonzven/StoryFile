//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


#if USE_FINAL_IK
using RootMotion.FinalIK;
#endif


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Final IK plugin extension class.
    /// </summary>
	public static class FinalIkExt 
    {
#if USE_FINAL_IK

		#region IK FULL BODY BIPED EFFECTORS
		/// <summary>
		/// Animates the effector weights
		/// </summary>
		/// <param name="solver">Solver.</param>
		/// <param name="effector">Effector.</param>
		/// <param name="duration">The animation's duration. If below 0, it will be clamped to 0</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		/// <param name="rotWeight">Rotation weight. If below 0, its current value will be kept.</param>
		public static IEnumerator AnimEffectorWeightsCo( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector, float duration, float posWeight = -1f, float rotWeight = -1f )
		{			
			if( !solver.IsValid( true ) )
				yield break;
			duration = duration.Clamp( 0f, float.MaxValue );
			float iniPosWeight = solver.GetEffector( effector ).positionWeight;
			float iniRotWeight = solver.GetEffector( effector ).rotationWeight;
			float targetPosWeight = ( posWeight < 0f ) ? iniPosWeight : posWeight;
			float targetRotWeight = ( rotWeight < 0f ) ? iniRotWeight : rotWeight;
			
			float time = 0f;
			while( ( solver.GetEffector( effector ).positionWeight != targetPosWeight || solver.GetEffector( effector ).rotationWeight != targetRotWeight )
			      && duration > 0f )
			{
				time += Time.deltaTime;
				solver.GetEffector( effector ).positionWeight = iniPosWeight.Lerp( targetPosWeight, time / duration );
				solver.GetEffector( effector ).rotationWeight = iniRotWeight.Lerp( targetRotWeight, time / duration );
				yield return null;
			}
			solver.SetEffectorWeights( effector, targetPosWeight, targetRotWeight );
		}
		/// <summary>
		/// Animates the effector weights
		/// </summary>
		/// <param name="solver">Solver.</param>
		/// <param name="effector">Effector.</param>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		/// <param name="rotWeight">Rotation weight. If below 0, its current value will be kept.</param>
		public static void AnimEffectorWeights( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector, float duration, float posWeight = -1f, float rotWeight = -1f )
		{			
			solver.AnimEffectorWeightsCo( effector, duration, posWeight, rotWeight ).Start();
		}
		/// <summary>
		/// Animates the Solver's effector.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="weight">Weight. If below 0, its current value will be kept.</param>
		public static IEnumerator AnimEffectorToCo( this IKSolverFullBodyBiped solver, Transform target, float duration, 
		                                           FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float posWeight = -1f, float rotWeight = -1f )
		{
			/*float _duration = duration;
			if( target )
			{
				/*if( target != solver.GetEffector( effector ).target )//IF DIFFERENT TARGET
				{
					if( solver.GetEffector( effector ).positionWeight != 0f )//IF WEIGHT IS NOT CERO IT WILL FLICKER
						_duration /= solver.GetEffector( effector ).positionWeight * 2;
					else if( solver.GetEffector( effector ).rotationWeight != 0f )//IF WEIGHT IS NOT CERO IT WILL FLICKER
						_duration /= solver.GetEffector( effector ).rotationWeight * 2;
				}
				yield return solver.AnimEffectorWeightsCo( effector, _duration, 0f, 0f ).Start();//TO CERO IN HALF THE DURATION
				solver.GetEffector( effector ).target = target;
				yield return solver.GetEffector( effector ).target.MoveTowardsCo( duration, target ).Start();
			}
			yield return solver.AnimEffectorWeightsCo( effector, _duration, posWeight, rotWeight ).Start();*/

			if( !solver.GetEffector( effector ).bone )
				yield break;
			solver.AnimEffectorWeights( effector, duration, posWeight, rotWeight );
			if( target )
			{
				var _target = new GameObject( solver.GetEffector( effector ).bone.name + " EffectorAnimator to: " + target.name );
				if( solver.GetEffector( effector ).target )//PREVENT ANIMATION JUMP/FLICKER
					_target.transform.position = solver.GetEffector( effector ).target.position;
				solver.GetEffector( effector ).target = _target.transform;//REPLACE BEFORE ANIMATING

				yield return solver.GetEffector( effector ).target.MoveTowardsCo( duration, target ).Start();
				solver.GetEffector( effector ).target = target;
				_target.Destroy();
			}
		}
		/// <summary>
		/// Animates the Solver's effector.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="weight">Weight. If below 0, its current value will be kept.</param>
		public static void AnimEffectorTo( this IKSolverFullBodyBiped solver, Transform target, float duration, 
		                                  FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float posWeight = -1f, float rotWeight = -1f )
		{			
			solver.AnimEffectorToCo( target, duration, effector, posWeight, rotWeight ).Start();
		}
		#endregion


		#region FULL BODY BIPED IK
		/// <summary>
		/// Animates the effector until it reaches the specified target.
		/// </summary>
		/// <param name="target">If null, the FullBodyBipedIK's solver's effector's target will be used instead.</param>
		public static void MoveEffector( this FullBodyBipedIK fbbik, Transform target, float posWeight = 1f, float duration = 1f, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
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
		public static IEnumerator MoveBendGoalCo( this FullBodyBipedIK fbbik, Transform goal, float weight = 1f, float duration = 1f, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float delay = 0f )
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
		public static void MoveBendGoal( this FullBodyBipedIK fbbik, Transform goal, float weight = 1f, float duration = 1f, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float delay = 0f )
		{
			fbbik.MoveBendGoalCo( goal, weight, duration, effector, delay ).Start();
		}
		/// <summary>
		/// Animates the effector until it reaches the specified target.
		/// </summary>
		/// <param name="target">If null, the FullBodyBipedIK's solver's effector's target will be used instead.</param>
		public static void RotateEffector( this FullBodyBipedIK fbbik, Transform target, float rotWeight = 1f, float duration = 1f, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
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
		public static void AlignEffector( this FullBodyBipedIK fbbik, Transform target, float posWeight = 1f, float rotWeight = 1f, float duration = 1f, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
		{
			fbbik.MoveEffector( target, posWeight, duration, effector );
			fbbik.RotateEffector( target, rotWeight, duration, effector );
		}
		#endregion


		#region BEND GOALS
		/// <summary>
		/// Animates the effector weights
		/// </summary>
		/// <param name="solver">Solver.</param>
		/// <param name="effector">Effector.</param>
		/// <param name="duration">The animation's duration. If below 0, it will be clamped to 0</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		/// <param name="rotWeight">Rotation weight. If below 0, its current value will be kept.</param>
		public static IEnumerator AnimBendWeightCo( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector, float duration, float weight = -1f)
		{			
			if( !solver.IsValid( true ) )
				yield break;
			duration = duration.Clamp( 0f, float.MaxValue );
			float iniWeight = solver.GetChain( effector ).bendConstraint.weight;
			float targetWeight = ( weight < 0f ) ? iniWeight : weight;
			
			float time = 0f;
			while( ( solver.GetChain( effector ).bendConstraint.weight != targetWeight ) && duration > 0f )
			{
				time += Time.deltaTime;
				solver.GetChain( effector ).bendConstraint.weight = iniWeight.Lerp( targetWeight, time / duration );
				yield return null;
			}
			solver.SetBendWeight( effector, targetWeight );
		}
		/// <summary>
		/// Animates the effector weights
		/// </summary>
		/// <param name="solver">Solver.</param>
		/// <param name="effector">Effector.</param>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		/// <param name="rotWeight">Rotation weight. If below 0, its current value will be kept.</param>
		public static void AnimBendWeight( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector, float duration, float weight = -1f )
		{			
			solver.AnimBendWeightCo( effector, duration, weight ).Start();
		}
		/// <summary>
		/// Animates the Solver's bend goal.
		/// </summary>
		/// <param name="goal">If null, the solver's chain's bend goal will be used.</param>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="weight">Weight. If below 0, its current value will be kept.</param>
		public static IEnumerator AnimBendToCo( this IKSolverFullBodyBiped solver, Transform goal, float duration, 
		                                       FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float weight = -1f)
		{	
			float _duration = duration;
			if( goal )
			{
				if( goal != solver.GetBendGoal( effector ) )//IF DIFFERENT TARGET
				{
					if( solver.GetBendWeight( effector ) != 0f )//IF WEIGHT IS NOT CERO IT WILL FLICKER
						_duration /= solver.GetBendWeight( effector ) * 2;
				}
				yield return solver.AnimBendWeightCo( effector, _duration, 0f ).Start();//TO CERO IN HALF THE DURATION
				solver.GetEffector( effector ).target = goal;
			}
			yield return solver.AnimBendWeightCo( effector, _duration, weight ).Start();
		}
		/// <summary>
		/// Animates the Solver's bend goal.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="weight">Weight. If below 0, its current value will be kept.</param>
		public static void AnimBendTo( this IKSolverFullBodyBiped solver, Transform goal, float duration, 
		                                       FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand, float weight = -1f, float delay = 0f )
		{			
			solver.AnimBendToCo( goal, duration, effector, weight ).Start( delay );
		}
		#endregion


		#region IK SOLVER AIM
		/// <summary>
		/// Animates the AimIK's weight
		/// </summary>
		/// <param name="duration">The animation's duration. If below 0, it will be clamped to 0</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		public static IEnumerator AnimAimWeightCo( this IKSolverAim solver, float duration, float weight = -1f )
		{			
			if( !solver.IsValid( true ) )
				yield break;
			duration = duration.Clamp( 0f, float.MaxValue );
			float targetPosWeight = ( weight < 0f ) ? solver.IKPositionWeight : weight;
			
			float time = 0f;
			float iniWeight = solver.IKPositionWeight;
			if( duration > 0f )
			{
				while( solver.IKPositionWeight != targetPosWeight )
				{
					time += Time.deltaTime;
					solver.IKPositionWeight = iniWeight.Lerp( targetPosWeight, time / duration );
					yield return null;
				}
			}
			solver.SetIKPositionWeight( targetPosWeight );
		}
		/// <summary>
		/// Animates the AimIK's position weight
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		public static void AnimAimWeight( this IKSolverAim solver, float duration, float weight = -1f )
		{			
			solver.AnimAimWeightCo( duration, weight ).Start();
		}
		/// <summary>
		/// Animates the AimIK's weight
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		public static IEnumerator AnimAimToCo( this IKSolverAim solver, Transform target, float duration, float weight = -1f )
		{			
			if( !solver.IsValid( true ) )
				yield break;
			if( !target.Validate( solver, true ) )
				yield break;
			string targetName = solver.transform.name + "_AimTarget";
			Transform aimTarget = new GameObject( targetName ).transform;
			if( !solver.target )
			{
				solver.IKPositionWeight = 0f;
				Vector3 extra = solver.transform.forward;
				if( solver.axis == Vector3.up )
				{
					extra = solver.transform.up;
				}
				else if( solver.axis == Vector3.down )
				{
					extra = -solver.transform.up;
				}
				else if( solver.axis == Vector3.right )
				{
					extra = solver.transform.right;
				}
				else if( solver.axis == Vector3.left )
				{
					extra = -solver.transform.right;
				}
				else if( solver.axis == Vector3.back )
				{
					extra = -solver.transform.forward;
				}
				else
				{
					Debug.LogError( "The solver's axis must be set to 1 axis only" );
				}
				aimTarget.position = solver.transform.position + extra * 10;
			}
			else
			{
				aimTarget.position = solver.target.position;//To avoid flicker if pos weight is 1
			}
			solver.target = aimTarget;
			/*if( !solver.target )//CREATE
			{
				solver.IKPositionWeight = 0f;
				aimTarget = new GameObject( targetName ).transform;
				aimTarget.position = solver.transform.position;
				solver.target = aimTarget;
			}
			else //FIND AND SET
			{
				aimTarget = targetName.Find<Transform>();
				if( !aimTarget )
					aimTarget = new GameObject( targetName ).transform;
				aimTarget.position = solver.target.position;//To avoid flicker if pos weight is 1
				solver.target = aimTarget;
			}*/
			//solver.target.AnimMoveTowards( duration, target );
			solver.target.AnimMoveTowards( duration, target );
			solver.AnimAimWeight( duration, weight );
			yield return new WaitForSeconds( duration );
			/*while( solver.target.position.Distance( target.position ) > 0.01f )
			{
				Debug.Log ( solver.target.position.Distance( target.position ) );
				yield return null;
				if( solver == null || !solver.target )
					break;
			}*/
			if( solver != null && solver.target )
				solver.target = target;

			if ( aimTarget != null )
				GameObject.DestroyImmediate( aimTarget.gameObject );
		}
		/// <summary>
		/// Animates the AimIK's position weight
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="posWeight">Position weight. If below 0, its current value will be kept.</param>
		public static void AnimAimTo( this IKSolverAim solver, Transform target, float duration, float posWeight = -1f )
		{			
			solver.AnimAimToCo( target, duration, posWeight ).Start();
		}
		#endregion


		#region AIM IK
		/// <summary>
		/// The Character must have an AimIK component attached and setup.
		/// </summary>
		/// <param name="target">If null, the AimIK's solver's target will be used instead.</param>
		public static IEnumerator LookAtCo( this AimIK aimIk, Transform target, float weight = 1f, float duration = 1f )
		{
			if( !aimIk )
				yield break;
			var solver = aimIk.solver;
			yield return solver.AnimAimToCo( target, duration, weight ).Start();
		}
		/// <summary>
		/// The Character must have an AimIK component attached and setup.
		/// </summary>
		/// <param name="target">If null, the AimIK's solver's target will be used instead.</param>
		public static void LookAt( this AimIK aimIk, Transform target, float weight = 1f, float duration = 1f )
		{
			aimIk.LookAtCo( target, weight, duration ).Start();
		}
		#endregion


		#region VALIDATIONS
		/// <summary>
		/// Returns false, if the solver is null, or its /transform/ hasn't been set ( IKSolverAim ).
		/// </summary>
		public static bool IsValid( this IKSolver solver, bool logWarning = false )
		{
			bool valid = true;
			string msg = "The solver is null..";
			if( solver == null )
				valid = false;
			if( solver is IKSolverAim && !( (IKSolverAim) solver ).transform )
				msg = "The solver is null.. Or its Transform hasn't been specified";
			if( !valid && logWarning )
				Debug.LogWarning ( msg );
			return valid;
		}
		/// <summary>
		/// If this target is null, it will be set as the specified solver's target. If the solver's target is null, /false/ is returned.
		/// </summary>
		public static bool Validate( this Transform target, IKSolverHeuristic solver, bool logWarning = false )
		{
			if( !target )
			{
				if( solver.target )
					target = solver.target;
				else 
				{
					Debug.LogWarning("No target has been specified" );
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// If this target is null, it will be set as the specified solver's effector's target. If the solver's target is null, /false/ is returned.
		/// </summary>
		public static bool ValidateEffector( this IKSolverFullBodyBiped solver, ref Transform target, FullBodyBipedEffector effector, bool logWarning = false )
		{
			if( !solver.GetEffector( effector ).bone )
			{
				if( logWarning )
					Debug.LogWarning("The specified solver's bone is null");
				return false;
			}
			if( !target )
			{
				if( solver.GetEffector( effector ).target )
					target = solver.GetEffector( effector ).target;
				else 
				{
					Debug.LogWarning("No target has been specified" );
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// If this target is null, it will be set as the specified solver's chain's bend goal. If the solver's chain's bend goal is null, /false/ is returned.
		/// </summary>
		public static bool ValidateChain( this IKSolverFullBodyBiped solver, ref Transform goal, FullBodyBipedEffector effector, bool logWarning = false )
		{
			if( !solver.GetEffector( effector ).bone )
			{
				if( logWarning )
					Debug.LogWarning("The specified solver's bone is null");
				return false;
			}
			if( !goal )
			{
				Transform _goal = solver.GetBendGoal( effector );
				if( _goal )
					goal = _goal;
				else 
				{
					Debug.LogWarning("No bend goal has been specified" );
					return false;
				}
			}
			return true;
		}
		#endregion


		#region GET
		/// <summary>
		/// Returns the solver's effector's bend constraint's bend goal, or null if any object is null or the bend goal is.
		/// </summary>
		public static Transform GetBendGoal( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector )
		{
			if( solver == null || solver.GetChain( effector ) == null || solver.GetChain( effector ).bendConstraint == null )
				return null;
			return solver.GetChain( effector ).bendConstraint.bendGoal;
		}
		/// <summary>
		/// Returns the solver's effector's bend constraint's weight, or 0 if any object is null.
		/// </summary>
		public static float GetBendWeight( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector )
		{
			if( solver == null || solver.GetChain( effector ) == null || solver.GetChain( effector ).bendConstraint == null )
				return 0f;
			return solver.GetChain( effector ).bendConstraint.weight;
		}
		public static Transform GetBone( this FullBodyBipedIK fbbik, FullBodyBipedEffector effector = FullBodyBipedEffector.RightHand )
		{
			if( !fbbik )
				return null;
			return fbbik.solver.GetEffector( effector ).bone;
		}
		/// <summary>
		/// Returns AimIK's current target transform. The Character must have an AimIK component attached and setup.
		/// </summary>
		public static Transform GetAimTarget( this AimIK aimIk )
		{
			if( !aimIk )
				return null;
			return aimIk.solver.target;
		}
		/// <summary>
		/// Returns the target the AimIK is looking at, the first object hit by a raycast from the aim's transform to the target's transform.
		/// If the AimIK's target transform is null, this returns null. If no Transform is hit, the aim's target will be returned.
		/// The Character must have an AimIK component attached and setup.
		/// </summary>
		/// <param name="extraLength">The raycast's extra length. If higher than 0 the raycast's target will exceed the targets position.</param>
		public static Transform GetLookTarget( this AimIK aimIk, float extraLength = 0f, LayerMask layerMask = default(LayerMask) )
		{
			if( !aimIk )
				return null;
			var solver = aimIk.solver;
			if( !solver.target )
				return null;
			Vector3 lookDir = solver.target.position - solver.transform.position;
			RaycastHit hit;
			if( Physics.Raycast( solver.transform.position, lookDir.normalized, out hit, lookDir.sqrMagnitude * lookDir.sqrMagnitude + extraLength, layerMask.value ) )
			{
				return hit.transform;
			}
			return solver.target;
		}
		/// <summary>
		/// Returns the direction the AimIK is looking at. If the AimIK's target transform is null, the IKPosition is used
		/// to calculate the direction. The Character must have an AimIK component attached and setup.
		/// </summary>
		/// <param name="extraLength">The raycast's extra length. If higher than 0 the raycast's target will exceed the targets position.</param>
		public static Vector3 GetLookDirection( this AimIK aimIk, float extraLength = 0f, LayerMask layerMask = default(LayerMask) )
		{
			if( !aimIk )
				return default( Vector3 );
			var solver = aimIk.solver;
			if( !solver.target )
				return solver.IKPosition - solver.transform.position;
			return solver.target.position - solver.transform.position;
		}
        public static float GetIKPositionWeight( this IK ik )
        {
            if( ik == null || ik.GetIKSolver() == null )
                return 0f;
            return ik.GetIKSolver().IKPositionWeight;
        }
        public static float GetFBBIKPositionWeight( this FullBodyBipedIK ik, FullBodyBipedEffector effector )
        {
            if( ik == null || ik.GetIKSolver() == null )
                return 0f;
            return ik.solver.GetEffector( effector ).positionWeight;
        }
        public static float GetFBBIKRotationWeight( this FullBodyBipedIK ik, FullBodyBipedEffector effector )
        {
            if( ik == null || ik.GetIKSolver() == null )
                return 0f;
            return ik.solver.GetEffector( effector ).rotationWeight;
        }
		#endregion


		#region SET
		/// <summary>
		/// Sets the solver's effector's bend constraint's bend goal.
		/// </summary>
		public static void SetBendGoal( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector, Transform goal )
		{
			if( !goal || solver == null || solver.GetChain( effector ) == null || solver.GetChain( effector ).bendConstraint == null )
				return;
			solver.GetChain( effector ).bendConstraint.bendGoal = goal;
		}
		/// <summary>
		/// Returns the solver's effector's bend constraint's weight, or 0 if any object is null.
		/// </summary>
		public static void SetBendWeight( this IKSolverFullBodyBiped solver, FullBodyBipedEffector effector, float weight )
		{
			if( solver == null || solver.GetChain( effector ) == null || solver.GetChain( effector ).bendConstraint == null )
				return;
			solver.GetChain( effector ).bendConstraint.weight = weight;
		}
		#endregion
#endif
		
	}
}
