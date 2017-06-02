using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Managers;
using DDK.Base.SoundFX;

#if USE_NODE_CANVAS
using NodeCanvas.DialogueTrees;
using DDK.NodeCanvas;
#endif


namespace DDK.Base.Extensions
{
    /// <summary>
    /// DelayedAudioClip class extension.
    /// </summary>
	public static class DelayedAudioClipExt
	{
		/// <summary>
		/// Plays the specified clips and wait for them to end playing.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this DelayedAudioClip[] delayedClips )
		{
            yield return delayedClips.PlayAll ( null ).Start();
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing. If your list is to large, consider using an 
		/// array instead
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this List<DelayedAudioClip> delayedClips )
		{
            yield return delayedClips.PlayAll ( null ).Start();
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this DelayedAudioClip[] delayedClips, Object context )
		{
			if( delayedClips == null )
				yield break;
			for( int i=0; i<delayedClips.Length; i++ )
			{
				yield return new WaitForSeconds( delayedClips[i].delay );
				SfxManager.PlayClip( delayedClips[i].sfxManagerSource, delayedClips[i].clip, false, context );
				if( !delayedClips[i].clip )
				{
					Debug.LogWarning ("Missing clip..", context );
					yield break;
				}
				yield return new WaitForSeconds( delayedClips[i].clip.length );
			}
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing. If your list is to large, consider using an 
		/// array instead
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this List<DelayedAudioClip> delayedClips, Object context )
		{
			if( delayedClips == null )
				yield break;
			for( int i=0; i<delayedClips.Count; i++ )
			{
				yield return new WaitForSeconds( delayedClips[i].delay );
				SfxManager.PlayClip( delayedClips[i].sfxManagerSource, delayedClips[i].clip, false, context );
				yield return new WaitForSeconds( delayedClips[i].clip.length );
			}
		}
		#if USE_NODE_CANVAS
		/// <summary>
		/// Plays the specified clips and wait for them to end playing.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this DelayedAudioClip[] delayedClips, DialogueActor actor )
		{
            yield return delayedClips.PlayAll ( actor, null ).Start();
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing. If your list is to large, consider using an 
		/// array instead
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this List<DelayedAudioClip> delayedClips, DialogueActor actor )
		{
            yield return delayedClips.PlayAll ( actor, null ).Start();
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this DelayedAudioClip[] delayedClips, DialogueActor actor, Object context )
		{
			if( delayedClips == null )
				yield break;
			for( int i=0; i<delayedClips.Length; i++ )
			{
				if( !delayedClips[i].clip )
					continue;
				yield return new WaitForSeconds( delayedClips[i].delay );
				SfxManager.PlayClip( delayedClips[i].sfxManagerSource, delayedClips[i].clip, false, context );
				actor.TalkWithRate( true, delayedClips[i].talkingRate );
				yield return new WaitForSeconds( delayedClips[i].clip.length );
				actor.TalkWithRate( false, delayedClips[i].talkingRate );
			}
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing. If your list is to large, consider using an 
		/// array instead
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		/// <param name="context">The context object for console messages.</param>
        public static IEnumerator PlayAll( this List<DelayedAudioClip> delayedClips, DialogueActor actor, Object context )
		{
			if( delayedClips == null )
				yield break;
			for( int i=0; i<delayedClips.Count; i++ )
			{
				if( !delayedClips[i].clip )
					continue;
				yield return new WaitForSeconds( delayedClips[i].delay );
				SfxManager.PlayClip( delayedClips[i].sfxManagerSource, delayedClips[i].clip, false, context );
				actor.TalkWithRate( true, delayedClips[i].talkingRate );
				yield return new WaitForSeconds( delayedClips[i].clip.length );
				actor.TalkWithRate( false, delayedClips[i].talkingRate );
			}
		}
		#endif
		/// <summary>
		/// Gets the specified clips total length including their delays.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		public static float GetTotalDuration( this DelayedAudioClip[] delayedClips )
		{
			if( delayedClips == null )
                return 0f;
            float totalDuration = 0f;
            for( int i=0; i<delayedClips.Length; i++ )
            {
                if( !delayedClips[i].clip )
                    continue;
                totalDuration += delayedClips[i].delay;
                totalDuration += delayedClips[i].clip.length;
            }
            return totalDuration;
        }
        /// <summary>
        /// Gets the specified clips total length including their delays. If your list is to large, consider using an 
        /// array instead
        /// </summary>
        /// <param name="delayedClips">Delayed clips.</param>
        public static float GetTotalDuration( this List<DelayedAudioClip> delayedClips )
        {
            if( delayedClips == null )
                return 0f;
            float totalDuration = 0f;
            for( int i=0; i<delayedClips.Count; i++ )
            {
                if( !delayedClips[i].clip )
                    continue;
                totalDuration += delayedClips[i].delay;
                totalDuration += delayedClips[i].clip.length;
            }
            return totalDuration;
        }
    }
}
