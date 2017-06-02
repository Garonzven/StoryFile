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
	public class AimIKTargetHandler : IKTargetHandler {
		
		#if USE_FINAL_IK
		[System.Serializable]
		public class AimedActivator : Activator {

			[ShowIfAttribute( "_IsObjValid", 1 )]
			[Tooltip("Parent To the Target that was Hit when aiming")]
			public bool parentToAimedHitTarget;
			
			
			/// <summary>
			/// Activates or spawns the specified /obj/ gameobject.
			/// </summary>
			public void Activate( AimIK character )
			{
				if( !character || !obj )
				{
					return;
				}
				Transform target = character.GetAimTarget();
				var spawn = obj.SetActiveInHierarchy();
				if( onTarget )
				{
					spawn.transform.position = target.position;
					spawn.transform.rotation = target.rotation;
				}
				if( parentToAimedHitTarget )
				{
					Transform hitTarget = character.GetLookTarget();
					spawn.SetParent( hitTarget );
				}
			}
		}




		[ShowIfAttribute( "_IsIkTargetNull", true )]
		[Tooltip("If it's a prefab, it will be spawned when the aim's weight reaches 1; otherwise, it will just be activated")]
		public AimedActivator activateOnTargetReached;
		[ShowIfAttribute( "_IsIkTargetNull", true )]
		[Tooltip("If it's a prefab, it will be spawned when the aim's weight reaches 1 and the animation's stay duration passes; otherwise, it will just be activated")]
		public AimedActivator activateOnTargetReachedAndStayed;

		
		internal void LookAtTarget( AimIK character )
		{
			ActionToTarget( character );
		}
		internal void AnimAimWeight( AimIK character, float target = 0f )
		{
			character.LookAt( m_IKTarget, target, animation.animDuration );
		}
		internal void AnimAimWeight( AimIK character, float target, float animDuration )
		{
			character.LookAt( m_IKTarget, target, animDuration );
		}
		internal void OnTargetReached( AimIK character )
		{
			base.OnTargetReached();
			activateOnTargetReached.Activate( character );
		}
		internal void OnTargetReachedAndStayed( AimIK character )
		{
			base.OnTargetReachedAndStayed();
			activateOnTargetReachedAndStayed.Activate( character );
		}
		

		/// <summary>
		/// Look At
		/// </summary>
		protected override IEnumerator _ActionToTarget( IK character )
		{
			yield return _ActionToTarget( character as AimIK );
        }
		/// <summary>
		/// Look At
		/// </summary>
		protected IEnumerator _ActionToTarget( AimIK character )
		{
			yield return character.LookAtCo( m_IKTarget, 1f, animation.animDuration ).Start();
			//yield return new WaitForSeconds( animation.animDuration );
			OnTargetReached( character );
			//RETURN OR STAY PERMANENTLY
			yield return new WaitForSeconds( animation.stayAtTargetDuration );
			if( animation.stayAtTargetDuration > 0f )
			{
				if( animation._return )
					yield return character.LookAtCo( m_IKTarget, 0f, animation.animDuration );
				OnTargetReachedAndStayed( character );
			}
		}
		
		#else
		[HelpBoxAttribute( MessageType.Warning )]
		public string msg = "The USE_FINAL_IK scripting symbol must be defined";
		#endif
		
	}


}