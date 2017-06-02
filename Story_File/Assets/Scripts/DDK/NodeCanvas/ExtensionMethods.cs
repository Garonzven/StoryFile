//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Events;
using DDK.Base.Extensions;

#if USE_NODE_CANVAS
using NodeCanvas.DialogueTrees;
#endif

namespace DDK.NodeCanvas
{
	public static class ExtensionMethods 
	{		
		#if USE_NODE_CANVAS
		/// <summary>
		/// Make this actor talk if it has an AnimatorActions component.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /talk/.</param>
		public static void Talk( this DialogueActor actor, bool talk, float duration = 0f )
		{
			actor.TalkWithRate ( "Talking", talk, 0.1f, duration );
        }
		/// <summary>
		/// Make this actor talk if it has an AnimatorActions component.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /talk/.</param>
		public static void Talk( this DialogueActor actor, string talkingParam, bool talk, float duration = 0f )
		{
			if( actor == null )
				return;
			AnimatorActions actions = actor.GetComponent<AnimatorActions>();
			if( !actions )
			{
				Animator animator = actor.GetComponent<Animator>();
				animator.SetBoolForDuration( talkingParam, talk, duration );
				return;
			}
			actions.SetBool( talkingParam, talk, duration );
		}
		/// <summary>
		/// Make this actor talk if it has an AnimatorActions component.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /talk/.</param>
		public static void TalkWithRate( this DialogueActor actor, string talkingParam, bool talk, 
		                        float talkingRate = 0.1f, float duration = 0f )
		{
			actor.TalkWithRate ( talkingParam, talk, "TalkingRate", talkingRate, duration );
		}
		/// <summary>
		/// Make this actor talk if it has an AnimatorActions component.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /talk/.</param>
		public static void TalkWithRate( this DialogueActor actor, bool talk, float talkingRate = 0.1f, float duration = 0f )
		{
			actor.TalkWithRate ( "Talking", talk, "TalkingRate", talkingRate, duration );
		}
		/// <summary>
		/// Make this actor talk if it has an AnimatorActions component.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /talk/.</param>
		public static void TalkWithRate( this DialogueActor actor, string talkingParam, bool talk, 
		                        string talkingRateParam, float talkingRate = 0.1f, float duration = 0f )
		{
			if( actor == null)
				return;
			Animator anim = actor.GetComponent<Animator> ();
			AnimatorActions actions = actor.GetComponent<AnimatorActions>();
			if (actions)
			{
				actions.SetFloat (talkingRateParam, talkingRate);
				actions.SetBool (talkingParam, talk, duration);
			}
			else if (anim)
			{
				anim.SetFloat( talkingRateParam, talkingRate );
				anim.SetBoolForDuration( talkingParam, talk, duration );
			}
		}
#endif
    }
}
