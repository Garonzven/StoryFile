//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.Events;
using DDK.Base.Extensions;
using System.Collections;
using DDK.GamesDevelopment.Characters;
using DDK.Base.Classes;
#if USE_FINAL_IK
using RootMotion.FinalIK;
#endif


namespace DDK.GamesDevelopment.Characters.FinalIK {

	[System.Serializable]
	public abstract class IKTargetHandler {

#if USE_FINAL_IK
		[System.Serializable]
		public abstract class Activator {

			[Tooltip("If not empty, the name of the spawned/activated object will be set to the specified")]
			[ShowIfAttribute( "_IsObjValid" )]
			public string nameReplacer = "";
			[Tooltip("If it's a prefab, it will be spawned when the effector's weight reaches 1; otherwise, it will just be activated")]
			public GameObject obj;
			[Tooltip("If true, the object will be positioned and rotated as the effector, in the activation/spawning frame")]
			[ShowIfAttribute( "_IsObjValid", 1 )]
			public bool onTarget;


			#region VALIDATION FUNCTIONS
			protected bool _IsObjValid()
			{
				return obj != null;
            }
			#endregion
		}
		[System.Serializable]
		public class IKAnimation {

			[Range( 0f, 1f )]
			public float targetWeight = 1f;
            [Range( 0f, 1f )]
            public float targetRotationWeight = 0f;
			[NotLessThan( 0f )]
			public float animDuration = 1f;
			[Tooltip( "If 0, the effector won't come back from the target (its weight will stay at 1)" )]
			[DisableIfAttribute( 0f, float.MaxValue, "IsStayForNull" )]
			public float stayAtTargetDuration = 1f;
			[DisableIfAttribute( "_IsStayHigherThan0" )]
			public AudioClip stayAtTargetFor;
			[Tooltip("If false, the effector will stay on the target (weight = 1) even after /stayAtTargetDuration/ passes")]
			[ShowIfAttribute( "_IsStayHigherThan0", 1 )]
			public bool _return = true;


			internal float _initialWeight;
            internal float _initialRotWeight;


			protected bool _IsStayHigherThan0()
			{
				return stayAtTargetDuration > 0f || stayAtTargetFor != null;
			}
			protected bool _IsStayForNull()
			{
				return stayAtTargetFor != null;
			}
		}



		[ShowIfAttribute( "_IsIkTargetNull" )]
		[Tooltip("The name of the target")]
		public string ikTargetName = "";
		[ShowIfAttribute( "_IsIkTargetNull", 1 )]
		[Tooltip("If true, the ikTargetName will be considered as a tag for the search")]
		public bool isTag;
		[Tooltip("The target object that will be used as the animation's IK target. If null, it will be searched by the " +
			"specified name/tag. If not null, the name to search will be automatically set to the current reference's name")]
		public Transform ikTarget;

		[ShowIfAttribute( "_IsIkTargetNull", true )]
		public IKAnimation animation = new IKAnimation();
		
		[Header("Events")]
		[ShowIfAttribute( "_IsIkTargetNull", true )]
		public ComposedEvent onTargetReachedEvents = new ComposedEvent();
		[ShowIfAttribute( "_IsIkTargetNull", true )]
		public ComposedEvent onTargetReachedAndStayedEvents = new ComposedEvent();

		
		public Transform m_IKTarget {
			get{
				if( !ikTarget )
					ikTarget = ( isTag ) ? ikTargetName.FindWithTag<Transform>() : ikTargetName.Find<Transform>();
				return ikTarget;
			}
		}


		#region VALIDATION FUNCTIONS		
		protected bool _IsIkTargetNull()
		{
			if( ikTarget && !isTag )
			{
				ikTargetName = ikTarget.name;
			}
			return m_IKTarget == null;
        }
		#endregion
		
		

		internal void ActionToTarget( IK ik )
		{
			if( !ik )
			{
				Debug.LogWarning("No Character has been specified.. Call SetCharacter( character ), or send a valid character/parameter");
				return;
			}
			_ActionToTarget( ik ).Start();
		}		
		internal virtual void OnTargetReached()
		{
			onTargetReachedEvents.Invoke();
		}
		internal virtual void OnTargetReachedAndStayed()
		{
			onTargetReachedAndStayedEvents.Invoke();
		}
		
		


		/// <summary>
		/// This represents an IK action, such as: MoveTo, LookAt, RotateTo...
		/// </summary>
		/// <param name="character">The character that will execute the action.</param>
		protected abstract IEnumerator _ActionToTarget( IK ik );
#endif
	}

}