//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DDK.Base.Statics;
using DDK.Base.SoundFX;
using MovementEffects;
using System.IO;



namespace DDK.Base.Extensions 
{
    /// <summary>
    /// AudioSource class extension.
    /// </summary>
	public static class AudioSourceExt 
    {		
		/// <summary>
		/// This can be used to temporary hold the a sound's time.
		/// </summary>
		public static float time;
		private static float _lastSpeed, _lastStart, _lastTarget, _lastDuration;
		private static Dictionary<int, float> _timeSavers = new Dictionary<int, float>();



		/// <summary>
		/// Stores the audio source time with the specified id.
		/// </summary>
		public static void StoreTime( this AudioSource audio, int id )
		{
			_timeSavers[ id ] = audio.time;
		}
		/// <summary>
		/// Gets the audio source time as the specified id's stored time.
		/// </summary>
		public static float GetStoredTime( this AudioSource audio, int id )
		{
			if( _timeSavers.ContainsKey( id ) )
			{
				return _timeSavers[ id ];
			}
			else return 0f;
		}
		/// <summary>
		/// Sets the audio source time as the specified id's stored time.
		/// </summary>
		public static void SetStoredTime( this AudioSource audio, int id )
		{
			if( _timeSavers.ContainsKey( id ) )
			{
				audio.time = _timeSavers[ id ];
			}
		}

		
		#region FADE
		public static void Fade( this AudioSource audio, float targetVol, float duration = 1f, bool autoPlayOrPause = true )
		{
			FadeCo( audio, targetVol, duration, autoPlayOrPause ).Start();
		}	
		public static IEnumerator FadeCo( this AudioSource audio, float targetVol, float duration = 1f, bool autoPlayOrPause = true )
		{
			_StoreLast( audio.volume, duration, targetVol );
			if( targetVol != 0f && autoPlayOrPause && !audio.isPlaying ){
				audio.PlaySound();
			}
			float time = 0f;
			float initialVol = audio.volume;
			while( audio.volume != targetVol )
			{
				time += Time.deltaTime;
				yield return null;
				audio.volume = initialVol.Lerp( targetVol, time / duration );
			}
			if( audio.volume == 0f && autoPlayOrPause )
				audio.Pause();
		}	
		/// <summary>
		/// Fades back from the last faded sound.
		/// </summary>
		/// <param name="audio">Audio.</param>
		public static IEnumerator FadeBack( this AudioSource audio, bool autoPlayPause = true )
		{
			yield return audio.FadeBack( _lastTarget, autoPlayPause ).Start();
		}		
		/// <summary>
		/// Fades back from the last faded sound.
		/// </summary>
		/// <param name="audio">Audio.</param>
		/// <param name="newTargetVol">New target volume.</param>
		public static IEnumerator FadeBack( this AudioSource audio, float newTargetVol, bool autoPlayPause = true )
		{
			_StoreLast( audio.volume, _lastDuration, newTargetVol );
			if( newTargetVol != 0f && autoPlayPause && !audio.isPlaying ){
				audio.PlaySound();
			}
			float time = 0f;
			float initialVol = _lastStart;
			while( audio.volume != newTargetVol )
			{
				yield return null;
				audio.volume = initialVol.Lerp( newTargetVol, time / _lastDuration );
			}
			if( audio.volume == 0f && autoPlayPause )
				audio.Pause();
		}
		private static void _StoreLast( float vol, float duration, float targetVol )
		{
			_lastDuration = duration;
			_lastStart = vol;
			_lastTarget = targetVol;
		}
		#endregion

		#region PLAY
		/// <summary>
		/// Play the sound. Verifies if the audio source is null and/or a prefab instance.
		/// </summary>
		/// <param name="audio">Audio.</param>
		public static void PlaySound( this AudioSource audio, bool evenIfItsPlaying = false )
		{
			if( audio != null )
			{
				if( !audio.gameObject.IsActiveInHierarchy() )
				{
					var obj = GameObject.Find( audio.name );
					if( obj )
					{
						var clip = audio.clip;
						audio = obj.GetComponent<AudioSource>();
						audio.clip = clip;
					}
					else 
					{
						obj = GameObject.Find( audio.name+"(Clone)" );
						if( obj )
						{
							var clip = audio.clip;
							audio = obj.GetComponent<AudioSource>();
							audio.clip = clip;
						}
					}
				}
			}
			if( audio != null )
			{
				if( !audio.isPlaying && !evenIfItsPlaying )
					audio.Play ();
				else if( evenIfItsPlaying ) audio.Play();
			}
		}		
		/// <summary>
		/// Play the sound after -ended- has finished playing. Verifies if the audio source is null and/or a prefab instance.
		/// </summary>
		public static IEnumerator PlaySoundAfter( this AudioSource audio, AudioSource ended, bool evenIfItsPlaying = false )
		{
			while( ended.isPlaying )
				yield return null;
			audio.PlaySound( evenIfItsPlaying );
		}		
		/// <summary>
		/// Play the sound after -delay-. Verifies if the audio source is null and/or a prefab instance.
		/// </summary>
        public static IEnumerator<float> PlaySoundAfter( this AudioSource audio, float delay, bool evenIfItsPlaying = false )
		{
            yield return Timing.WaitForSeconds( delay );
			audio.PlaySound( evenIfItsPlaying );
		}		
		/// <summary>
		/// Plays a sound every -x- secons; the sound starts after -every-. To stop this coroutine just destroy/disable the object in which is started.
		/// </summary>
		/// <returns>The cyclic.</returns>
		/// <param name="sound">Sound.</param>
		/// <param name="every">Every.</param>
		/// <param name="evenIfItsPlaying">If set to <c>true</c> even if its playing.</param>
		/// <param name="waitForOtherSoundsToEnd"> If another sound is playing and this is true, this will wait for it to end. </param>
		public static IEnumerator PlayCyclic( this AudioSource sound, float every, bool evenIfItsPlaying = false, bool waitForOtherSoundsToEnd = true )
		{
			while( true )
			{
				yield return new WaitForSeconds( every );
				while( IsAnySoundPlaying() && waitForOtherSoundsToEnd )
				{
					yield return new WaitForSeconds( 0.4f );//the lowest duration a sound might last
					yield return null;
				}
				sound.PlaySound( evenIfItsPlaying );
				if( !sound.gameObject.IsActiveInHierarchy() )
				{
					break;
				}
				yield return null;
			}
		}		
		/// <summary>
		/// Plays a sound every -x- secons; the sound starts immediately. To stop this coroutine just destroy/disable the object in which is started.
		/// </summary>
		/// <returns>The cyclic.</returns>
		/// <param name="sound">Sound.</param>
		/// <param name="every">Every.</param>
		/// <param name="evenIfItsPlaying">If set to <c>true</c> even if its playing.</param>
		/// <param name="waitForOtherSoundsToEnd"> If another sound is playing and this is true, this will wait for it to end. </param>
		public static IEnumerator PlayThenRepeat( this AudioSource sound, float every, bool evenIfItsPlaying = false, bool waitForOtherSoundsToEnd = true )
		{
			while( IsAnySoundPlaying() && waitForOtherSoundsToEnd )
			{
				yield return new WaitForSeconds( 0.4f );//the lowest duration a sound might last
				yield return null;
			}
			sound.PlaySound( evenIfItsPlaying );
			yield return sound.PlayCyclic( every, evenIfItsPlaying, waitForOtherSoundsToEnd );
		}
		/// <summary>
		/// Plays the sounds one after another.
		/// </summary>
		/// <returns>The sounds.</returns>
		/// <param name="sounds">Sounds.</param>
		/// <param name="delay">Initial delay.</param>
		/// <param name="evenIfAnyPlaying">If set to <c>true</c> the sound is played even if any other sound is playing.</param>
		/// <param name="inAdvance">This makes the sound play before the previous one ends.</param>
		/// <param name="volumes">The override volumes array.</param>
		/// <param name="overrideVolume0">If false the sounds volumes equal to = 0 won't be overriden.</param>
		public static IEnumerable PlaySounds( this IList<AudioSource> srcs, float delay, bool evenIfAnyPlaying = true, float[] inAdvance = null,
		                                     float[] volumes = null, bool overrideVolume0 = true )
		{
			yield return new WaitForSeconds( delay );
			for( int i=0; i<srcs.Count; i++ )
			{
				srcs[i].PlaySound( evenIfAnyPlaying );
				float delay0 = srcs[i].clip.length;
				if( inAdvance != null )
				{
					if( inAdvance.Length > i )
					{
						if( delay0 >= inAdvance[i] )
						{
							delay0 -= inAdvance[i];
						}
					}
				}
				if( volumes != null )
				{
					if( volumes.Length > i )
					{
						if( volumes[i] != -1 && !( !overrideVolume0 && srcs[i].volume == 0 ) )
						{
							srcs[i].volume = volumes[i];
						}
					}
				}
				yield return srcs[i];
				int currentFrame = Time.frameCount;
				yield return new WaitForSeconds( delay0 );
				if( _stopAllFrame > currentFrame && _stopAllFrame < Time.frameCount )
				{
					break;
				}
			}			
		}
		/// <summary>
		/// Plays the sources one after another.
		/// </summary>
		/// <returns>The sounds.</returns>
		/// <param name="sounds">Sounds.</param>
		/// <param name="delay">Initial delay.</param>
		/// <param name="evenIfAnyPlaying">If set to <c>true</c> the sound is played even if any other sound is playing.</param>
		/// <param name="inAdvance">This makes the sound play before the previous one ends.</param>
		/// <param name="volumes">The override volumes array.</param>
		/// <param name="overrideVolume0">If false the sounds volumes equal to = 0 won't be overriden.</param>
		public static IEnumerable PlaySounds( this IList<AudioSource> sources, bool evenIfAnyPlaying = true, float[] inAdvance = null, float[] volumes = null, 
		                                     bool overrideVolume0 = true )
		{
			yield return sources.PlaySounds( 0f, evenIfAnyPlaying,inAdvance, volumes, overrideVolume0 );
		}
		/// <summary>
		/// Play the specified clip in the specified source.
		/// </summary>
		/// <param name="evenIfItsPlaying">If set to <c>true</c> the source will be played even if it's already playing.</param>
		public static void Play( this AudioSource source, AudioClip clip, bool evenIfItsPlaying = true )
		{
			if( source != null )
			{
				if( !source.gameObject.IsActiveInHierarchy() )
				{
					var obj = GameObject.Find( source.name );
					if( obj )
					{
						source = obj.GetComponent<AudioSource>();
					}
					else 
					{
						obj = GameObject.Find( source.name+"(Clone)" );
						if( obj )
						{
							source = obj.GetComponent<AudioSource>();
						}
					}
				}
			}
			if( source == null )
				return;
			if( source.clip != clip )
				source.clip = clip;
			if( !source.isPlaying && !evenIfItsPlaying )
				source.Play ();
			else if( evenIfItsPlaying ) source.Play();
		}
		/// <summary>
		/// Play the specified clip in the specified source.
		/// </summary>
		/// <param name="evenIfItsPlaying">If set to <c>true</c> the source will be played even if it's already playing.</param>
		public static void Play( this AudioSource source, AudioClip clip, float delay, bool evenIfItsPlaying = true )
		{
			source._Play( clip, delay, evenIfItsPlaying ).Start();
		}
		/// <summary>
		/// Play the specified clip in the specified source.
		/// </summary>
		/// <param name="evenIfItsPlaying">If set to <c>true</c> the source will be played even if it's already playing.</param>
		private static IEnumerator _Play( this AudioSource source, AudioClip clip, float delay, bool evenIfItsPlaying = true )
		{
			yield return new WaitForSeconds( delay );
			source.Play( clip, evenIfItsPlaying );
		}
		
		/// <summary>
		/// Plays the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		public static AudioSource Play( this AudioClip clip, string audioSourceHolder = "Sfx" )
		{
			if( clip != null )
			{
				GameObject srcHolder;
				if( string.IsNullOrEmpty( audioSourceHolder ) )
				{
					audioSourceHolder = "Sfx";
				}
				srcHolder = audioSourceHolder.Find();
				var src = srcHolder.GetAddAudioSource( clip );
				src.Play();
				return src;
			}
			return null;
		}
		
		/// <summary>
		/// Plays the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		public static AudioSource Play( this AudioClip clip, AudioSource source )
		{
			if( source != null )
			{
				return clip.Play( source.name );
			}
			else return clip.Play();
		}	
		/// <summary>
		/// Plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		public static AudioSource Play( this IList<AudioClip> clips, string audioSourceHolder = "Sfx" )
		{
			if( clips != null )
			{
				GameObject srcHolder;
				if( string.IsNullOrEmpty( audioSourceHolder ) )
				{
					audioSourceHolder = "Sfx";
				}
				srcHolder = audioSourceHolder.Find();
				var src = srcHolder.GetAddAudioSource();
				src.Play( clips ).Start();
				return src;
			}
			return null;
		}
		
		/// <summary>
		/// Plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		public static AudioSource PlayAndEnableWithEachClip( this IList<AudioClip> clips, IList<Behaviour> enableWithEachClip, string audioSourceHolder = "Sfx" )
		{
			if( clips != null )
			{
				GameObject srcHolder;
				if( string.IsNullOrEmpty( audioSourceHolder ) )
				{
					audioSourceHolder = "Sfx";
				}
				srcHolder = audioSourceHolder.Find();
				var src = srcHolder.GetAddAudioSource();
				src.PlayAndEnableWithEachClip( clips, enableWithEachClip ).Start();
				return src;
			}
			return null;
		}		
		/// <summary>
		/// Play the specified clips in the specified source.
		/// </summary>
		/// <returns> Returns the playing clip's length. </returns>
		public static IEnumerable Play( this AudioSource src, IList<AudioClip> clips )
		{
			if( src != null && clips != null )
			{
				src.enabled = true;
				for( int i=0; i<clips.Count; i++ )
				{
					if( clips[i] != null )
					{
						src.clip = clips[i];
						src.Play ();
						#region PREVENT COROUTINE FROM CONTINUING IF SOURCE.PLAY() IS BEING CALLED AGAIN
						yield return new WaitForSeconds( clips[i].length * 0.5f );
						bool _break = false;
						while( src != null && src.isPlaying )
						{
							if( src.time == 0f )
							{
								_break = true;
								break;
							}
							yield return null;
						}
						if( _break )	break;
						#endregion
					}
				}
			}
		}		
		/// <summary>
		/// Play the specified clips in the specified source.
		/// </summary>
		/// <returns> Returns the playing clip's length. </returns>
		public static IEnumerable PlayAndEnableWithEachClip( this AudioSource src, IList<AudioClip> clips, IList<Behaviour> enableWithEachClip )
		{
			if( src != null && clips != null )
			{
				src.enabled = true;
				for( int i=0; i<clips.Count; i++ )
				{
					if( clips[i] != null )
					{
						if( enableWithEachClip != null )
						{
							if( i < enableWithEachClip.Count )
							{
								if( enableWithEachClip[i] != null )
								{
									enableWithEachClip[i].enabled = true;
								}
							}
						}
						src.clip = clips[i];
						src.Play ();
						#region PREVENT COROUTINE FROM CONTINUING IF SOURCE.PLAY() IS BEING CALLED AGAIN
						yield return new WaitForSeconds( clips[i].length * 0.5f );
						bool _break = false;
						while( src.isPlaying )
						{
							if( src.time == 0f )
							{
								_break = true;
								break;
							}
							yield return null;
						}
						if( _break )	break;
						#endregion
					}
				}
			}
		}		
		/// <summary>
		/// Plays the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable PlayAfter( this AudioClip clip, float after, string audioSourceHolder = "Sfx" )
		{
			yield return new WaitForSeconds( after );
			yield return clip.Play( audioSourceHolder );
		}		
		/// <summary>
		/// Plays the specified clip in the specified object after the specified audio source has ended playing. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable PlayAfter( this AudioClip clip, AudioSource ended, string audioSourceHolder )
		{
			while( ended.isPlaying )
			{
				yield return null;
			}
			yield return clip.Play( audioSourceHolder );
		}		
		/// <summary>
		/// Plays the specified clip in the specified audio source after it ends playing. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable PlayAfter( this AudioClip clip, AudioSource ended )
		{
			if( ended )
			{
				while( ended.isPlaying )
				{
					yield return null;
				}
				yield return clip.Play( ended );
			}
		}		
		/// <summary>
		/// Plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable PlayAfter( this IList<AudioClip> clips, float after, string audioSourceHolder = "Sfx" )
		{
			yield return new WaitForSeconds( after );
			yield return clips.Play( audioSourceHolder );
		}		
		/// <summary>
		/// Play the specified clips in the specified source.
		/// </summary>
		/// <returns> WaitForSeconds, the playing clip's length. </returns>
		public static IEnumerable PlayAfter( this AudioSource src, float after, IList<AudioClip> clips )
		{
			yield return new WaitForSeconds( after );
			yield return src.Play( clips );
		}		
		/// <summary>
		/// Adds and audio source to the specified object (if necessary) and plays it..
		/// </summary>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		public static AudioSource AddAndPlay( this AudioClip clip, GameObject obj = null )
		{
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				return src;
			}
			return null;
		}				
		/// <summary>
		/// Adds and audio source to the specified object (if necessary) and plays it..
		/// </summary>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		public static AudioSource AddAndPlay( this string clipPath, GameObject obj = null )
		{
			var clip = Resources.Load<AudioClip>( clipPath );
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				return src;
			}
			return null;
		}
		#endregion

		#region STOP
		public static void Stop( this AudioSource[] stop )
		{
			for( int i=0; i<stop.Length; i++ )
			{
				stop[i].Stop();
			}
		}

		private static bool doNotStopAllThisFrame;
		private static IEnumerator _DoNotStopAllThisFrame()
		{
			doNotStopAllThisFrame = true;
			yield return null;
			doNotStopAllThisFrame = false;
		}
		/// <summary>
		/// Prevents StopAll() from executing this frame.
		/// </summary>
		/// <seealso cref="StopAll"/>
		public static void DoNotStopAllThisFrame()
		{
			_DoNotStopAllThisFrame().Start();
		}
		/// <summary>
		/// This allows more control over other extension methods.
		/// </summary>
		private static int _stopAllFrame;
		/// <summary>
		/// Stops all playing sounds.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		public static void StopAll( IList<string> omit = null )
		{
			_stopAllFrame = Time.frameCount;
			if( doNotStopAllThisFrame )
				return;
			var sounds = GameObject.FindObjectsOfType<AudioSource>();
			if( sounds != null )
			{
				for( int i=0; i<sounds.Length; i++ )
				{
					if( sounds[i].isPlaying )
					{
						if( omit != null )
						{
							if( omit.Contains( sounds[i].name ) )
							{
								continue;
							}
						}
						sounds[i].Stop();
					}
				}
			}
		}
		#endregion

		#region STOP AND PLAY
		/// <summary>
		/// Stop all sources and play the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		public static AudioSource StopAllAndPlay( this AudioClip clip, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			StopAll( omit );
			return clip.Play( audioSourceHolder );
		}
		/// <summary>
		/// Stops all sources and plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		public static AudioSource StopAllAndPlay( this IList<AudioClip> clips, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			StopAll( omit );
			if( clips != null )
			{
				GameObject srcHolder;
				if( string.IsNullOrEmpty( audioSourceHolder ) )
				{
					audioSourceHolder = "Sfx";
				}
				srcHolder = audioSourceHolder.Find();
				var src = srcHolder.GetAddAudioSource();
				src.Play( clips ).Start();
				return src;
			}
			return null;
		}		
		/// <summary>
		/// Stops all sources and plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		public static AudioSource StopAll_PlayAndEnableWithEachClip( this IList<AudioClip> clips, IList<Behaviour> enableWithEachClip, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			StopAll( omit );
			return clips.PlayAndEnableWithEachClip( enableWithEachClip, audioSourceHolder );
		}
		/// <summary>
		/// Stop all sources and play the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable StopAllAndPlayAfter( this AudioClip clip, float after, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			yield return new WaitForSeconds( after );
			StopAll( omit );
			yield return clip.Play( audioSourceHolder );
		}
		/// <summary>
		/// Stops all sources and plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable StopAllAndPlayAfter( this IList<AudioClip> clips, float after, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			yield return new WaitForSeconds( after );
			yield return clips.StopAllAndPlay( audioSourceHolder, omit );
		}		
		/// <summary>
		/// Stops all sources and plays the specified clips in the specified object, to stop the playing queue just disable the source or deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		/// <returns> WaitForSeconds, AudioSource </returns>
		public static IEnumerable StopAll_PlayAfterAndEnableWithEachClip( this IList<AudioClip> clips, float after,
		             IList<Behaviour> enableWithEachClip, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			yield return new WaitForSeconds( after );
			yield return clips.StopAll_PlayAndEnableWithEachClip( enableWithEachClip, audioSourceHolder, omit );
		}
		/// <summary>
		/// Stops all playing sounds, adds and audio source to the specified object (if necessary) and plays it..
		/// </summary>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		public static AudioSource StopAllAddAndPlay( this AudioClip clip, GameObject obj = null )
		{
			StopAll();
			return clip.AddAndPlay( obj );
		}
		/// <summary>
		/// Stops all playing sounds, adds and audio source to the specified object (if necessary) and plays it..
		/// </summary>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		public static AudioSource StopAllAddAndPlay( this string clipPath, GameObject obj = null )
		{
			StopAll();
			return clipPath.AddAndPlay( obj );
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clip">Clip to add and/or play.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="after">The audio source that must end playing before playing the specified clip.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the specified -after- clip ends. </param>
		public static IEnumerator StopAllAddAndPlayAfter( this AudioClip clip, GameObject obj, AudioSource after, float secsBeforeItEnds = 0f )
		{
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				if( secsBeforeItEnds <= 0f )
				{
					if( after )
					{
						while( after.isPlaying )
							yield return null;
					}
				}
				else if( after )
				{
					if( after.isPlaying )
					{
						yield return new WaitForSeconds( (after.clip.length - secsBeforeItEnds) / after.pitch );
					}
				}
				StopAll();
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				yield return src;
			}
			yield return null;
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clip">Clip to add and/or play.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="after">The audio source that must end playing before playing the specified clip.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the specified -after- clip ends. </param>
		public static IEnumerator StopAllAddAndPlayAfter( this string clipPath, GameObject obj, AudioSource after, float secsBeforeItEnds = 0f )
		{
			var clip = Resources.Load<AudioClip>( clipPath );
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				if( secsBeforeItEnds <= 0f )
				{
					if( after )
					{
						while( after.isPlaying )
							yield return null;
					}
				}
				else if( after )
				{
					if( after.isPlaying )
					{
						yield return new WaitForSeconds( (after.clip.length - secsBeforeItEnds) / after.pitch );
					}
				}
				StopAll();
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				yield return src;
			}
			yield return null;
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clip">Clip to add and/or play.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="after">The audio clip that must end playing before playing the specified clip. It must be contained in an audio source in the specified obj.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the specified -afterClip- ends. </param>
		public static IEnumerator StopAllAddAndPlayAfter( this AudioClip clip, GameObject obj, AudioClip after, float secsBeforeItEnds = 0f )
		{
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				var afterSrc = after.GetSource( obj );
				if( secsBeforeItEnds <= 0f )
				{
					if( afterSrc )
					{
						while( afterSrc.isPlaying )
							yield return null;
					}
				}
				else if( afterSrc )
				{
					if( afterSrc.isPlaying )
					{
						yield return new WaitForSeconds( (after.length - secsBeforeItEnds) / afterSrc.pitch );
					}
				}
				StopAll();
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				yield return src;
			}
			yield return null;
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clipPath">Clip to add and/or play, if multiple clips in the specified path, a random one is picked.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="after">The audio clip that must end playing before playing the specified clip. It must be contained in an audio source in the specified obj.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the specified -after- clip ends. </param>
		public static IEnumerator StopAllAddAndPlayAfter( this string clipPath, GameObject obj, AudioClip after, float secsBeforeItEnds = 0f )
		{
			var clip = GetRandomClip(clipPath);
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				var afterSrc = after.GetSource( obj );
				if( secsBeforeItEnds <= 0f )
				{
					if( afterSrc )
					{
						while( afterSrc.isPlaying )
							yield return null;
					}
				}
				else if( afterSrc )
				{
					if( afterSrc.isPlaying )
					{
						yield return new WaitForSeconds( (after.length - secsBeforeItEnds) / afterSrc.pitch );
					}
				}
				StopAll();
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				yield return src;
			}
			yield return null;
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clipPath">Clip to add and/or play, if multiple clips in the specified path, a random one is picked.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="afterClipPath">The audio clip's path that must end playing before playing the specified clip. It must be contained in an audio source in the specified obj.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the specified -afterClip- ends. </param>
		public static IEnumerator StopAllAddAndPlayAfter( this string clipPath, GameObject obj, string afterClipPath, float secsBeforeItEnds = 0f )
		{
			var clip = GetRandomClip(clipPath);
			if( clip != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				var after = Resources.Load<AudioClip>( afterClipPath );
				var afterSrc = after.GetSource( obj );
				if( secsBeforeItEnds <= 0f )
				{
					if( afterSrc )
					{
						while( afterSrc.isPlaying )
							yield return null;
					}
				}
				else if( afterSrc )
				{
					if( afterSrc.isPlaying )
					{
						yield return new WaitForSeconds( (after.length - secsBeforeItEnds) / afterSrc.pitch );
					}
				}
				else Debug.Log ( "no source found for clip path: "+ afterClipPath );
				StopAll();
				var src = obj.AddGetAudioSource( clip );
				src.PlaySound();
				yield return src;
			}
			yield return null;
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clipsPath">Clips path to add and/or play.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the previous clip ends. </param>
		public static IEnumerator StopAllAddAndPlayMultiple( this string clipsPath, GameObject obj, float[] secsBeforeItEnds = null )
		{
			var clips = Resources.LoadAll<AudioClip>( clipsPath );
			if( clips != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				StopAll();
				AudioSource src = null;
				for( int i=0; i<clips.Length; i++ )
				{
					if( i == 0 )
					{
						src = obj.AddGetAudioSource( clips[i] );
						src.PlaySound();
						yield return src;
					}
					else
					{
						if( secsBeforeItEnds == null )
						{
							secsBeforeItEnds = new float[]{0f};
						}
						var secsBefore = secsBeforeItEnds.Length - 1;
						if( secsBefore < 0 ){
							secsBefore = 0;
						}
						if( secsBeforeItEnds[ Mathf.Clamp( i-1, 0, secsBefore ) ] <= 0f )
						{
							if( src )
							{
								while( src.isPlaying )
									yield return null;
							}
						}
						else if( src )
						{
							if( src.isPlaying )
							{
								yield return new WaitForSeconds( (src.clip.length - secsBeforeItEnds[ Mathf.Clamp( i-1, 0, secsBefore ) ]) / src.pitch );
							}
						}
						src = obj.AddGetAudioSource( clips[i] );
						src.PlaySound();
						yield return src;
					}
				}
			}
			else yield return null;
		}		
		/// <summary>
		/// After the specified audio source stops playing, all playing sounds are stopped and an audio source is added to the specified object (if necessary) and played afterwards..
		/// </summary>
		/// <param name="clips">Clips to add and/or play.</param>
		/// <param name="obj">The object to which the audio source with the specified clip will be added if necessary.</param>
		/// <param name="secsBeforeItEnds"> if higher than 0, the specified clip will be played -secsBeforeItEnds- seconds before the previous clip ends. </param>
		private static IEnumerable _StopAllAddAndPlayMultiple( this IList<AudioClip> clips, GameObject obj, 
		                                                      IList<float> secsBeforeItEnds = null )
		{
			if( clips != null )
			{
				if( !obj )
				{
					obj = _GetSfx();
				}
				if( clips.Count > 0 )
					StopAll();
				AudioSource src = null;
				for( int i=0; i<clips.Count; i++ )
				{
					src = obj.AddGetAudioSource( clips[i] );
					src.PlaySound();
					float delay = clips[i].length;//delay before next sound gets played
					
					if( secsBeforeItEnds != null )
					{
						if( secsBeforeItEnds.Count > i )
						{
							if( delay >= secsBeforeItEnds[i] )
							{
								delay -= secsBeforeItEnds[i];
							}
						}
					}
					
					yield return src;
					int currentFrame = Time.frameCount;
					yield return new WaitForSeconds( delay );
					if( _stopAllFrame > currentFrame && _stopAllFrame < Time.frameCount )
					{
						break;
					}
					
				}
			}
			else yield return null;
		}		
		public static IEnumerable StopAllAddAndPlayMultiple( this IList<AudioClip> clips, GameObject gameObject = null, IList<float> startClipsAdvance = null,
		                                                    IList<float> volumes = null, IList<float> pitches = null )
		{
			if( !gameObject )
			{
				gameObject = _GetSfx();
			}
			
			int i=0;
			foreach( var obj in clips._StopAllAddAndPlayMultiple( gameObject, startClipsAdvance ) )
			{
				if( obj is AudioSource )
				{
					var src = obj as AudioSource;
					if( volumes != null )
					{
						if( volumes.Count > i )
						{
							src.volume = volumes[i];
						}
					}
					if( pitches != null )
					{
						if( pitches.Count > i )
						{
							src.pitch = pitches[i];
						}
					}
					yield return src;
					i++;
				}
				else if( obj is WaitForSeconds )
				{
					yield return obj;
				}
				yield return null;
			}
		}
		#endregion

		#region IS
		public static bool IsAnySoundPlaying()
		{
			var sounds = GameObject.FindObjectsOfType<AudioSource>();
			if( sounds != null )
			{
				for( int i=0; i<sounds.Length; i++ )
				{
					if( sounds[i].isPlaying )
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Determines if any sound is playing.
		/// </summary>
		/// <returns><c>true</c> if is any sound is playing; otherwise, <c>false</c>.</returns>
		/// <param name="omit">The name of the gameobjects that should be omitted from this check.</param>
		public static bool IsAnySoundPlaying( IList<string> omit )
		{
			var sounds = GameObject.FindObjectsOfType<AudioSource>();
			if( sounds != null )
			{
				for( int i=0; i<sounds.Length; i++ )
				{
					if( sounds[i].isPlaying )
					{
						if( omit.Contains( sounds[i].name ) )
						{
							continue;
						}
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Determines if any sound is playing. This audio source is not taken into account.
		/// </summary>
		/// <returns><c>true</c> if is any sound is playing; otherwise, <c>false</c>.</returns>
		/// <param name="omit">The name of the gameobjects that should be omitted from this check.</param>
		public static bool IsAnySoundPlaying( this AudioSource source, IList<string> omit )
		{
			List<string> OMIT = new List<string>( omit );
			OMIT.Add( source.name );
			return IsAnySoundPlaying( OMIT );
		}
		/// <summary>
		/// Returns true if any source inside the list is playing.
		/// </summary>
		/// <returns><c>true</c> if is any source playing the specified srcs; otherwise, <c>false</c>.</returns>
		/// <param name="srcs">Srcs.</param>
		public static bool IsAnyPlaying( this IList<AudioSource> srcs )
		{
			if( srcs == null )
				return false;
			for( int i=0; i<srcs.Count; i++ )
			{
				if( srcs[i].isPlaying )
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region GET
		/// <summary>
		/// Determines if any sound is playing.
		/// </summary>
		/// <returns> The first found playing source, or null if no source is playing. </returns>
		/// <param name="omit">The name of the gameobjects that should be omitted from this check.</param>
		public static AudioSource GetFirstFoundPlayingSource( IList<string> omit )
		{
			var sounds = GameObject.FindObjectsOfType<AudioSource>();
			if( sounds != null )
			{
				for( int i=0; i<sounds.Length; i++ )
				{
					if( sounds[i].isPlaying )
					{
						if( omit.Contains( sounds[i].name ) )
						{
							continue;
						}
						return sounds[i];
					}
				}
			}
			return null;
		}
		/// <summary>
		/// Determines if any sound is playing. This audio source is not taken into account.
		/// </summary>
		/// <returns> The first found playing source, or null if no source is playing. </returns>
		/// <param name="omit">The name of the gameobjects that should be omitted from this check.</param>
		public static AudioSource GetFirstFoundPlayingSource( this AudioSource source, IList<string> omit )
		{
			List<string> OMIT = new List<string>( omit );
			OMIT.Add( source.name );
			return GetFirstFoundPlayingSource( OMIT );
		}
		/// <summary>
		/// Returns the time remaining before the source stops playing.
		/// </summary>
		/// <returns>The remaining time.</returns>
		/// <param name="src">Source.</param>
		public static float GetRemainingTime( this AudioSource src )
		{
			float remaining = 0;
			if( src.isPlaying )
			{
				float clipLength = src.clip.length;
				remaining = clipLength - src.time;
			}
			return remaining;
		}
		/// Returns the audio source inside the specified object that contains the specified clip. If obj is null all
		/// audio sources are searched.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="clip">Clip.</param>
		/// <param name="obj">Object.</param>
		public static AudioSource GetSource( this AudioClip clip, GameObject obj = null )
		{
			if( clip )
			{
				AudioSource[] srcs;
				if( obj )
				{
					srcs = obj.GetComponents<AudioSource>();
				}
				else srcs = GameObject.FindObjectsOfType<AudioSource>();
				for( int i=0; i<srcs.Length; i++ )
				{
					if( srcs[i].clip == clip )
					{
						return srcs[i];
					}
				}
			}
			return null;
		}
		/// <summary>
		/// Returns the audio sources inside the specified object that contains the specified clips.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="clip">Clip.</param>
		/// <param name="obj">Object.</param>
		public static List<AudioSource> GetSources( this IList<AudioClip> clips, GameObject obj = null )
		{
			if( !obj )
			{
				obj = _GetSfx();
			}
			List<AudioSource> audioSrcs = new List<AudioSource>();
			if( clips != null )
			{
				var srcs = obj.GetComponents<AudioSource>();
				for( int i=0; i<srcs.Length; i++ )
				{
					for( int j=0; j<clips.Count; j++ )
					{
						if( srcs[i].clip == clips[j] )
						{
							audioSrcs.Add( srcs[i] );
							break;
						}
					}
				}
			}
			return audioSrcs;
		}
		/// <summary>
		/// Returns the audio sources inside the specified object that contain the clips located inside the specified path (folder).
		/// </summary>
		/// <returns>The audio sources.</returns>
		/// <param name="clipsPath">Clips path.</param>
		/// <param name="obj">Object.</param>
		public static List<AudioSource> GetSources( this string clipsPath, GameObject obj = null )
		{
			if( !obj )
			{
				obj = _GetSfx();
			}
			List<AudioSource> audioSrcs = new List<AudioSource>();
			AudioClip[] clips = Resources.LoadAll<AudioClip>( clipsPath );
			if( !string.IsNullOrEmpty( clipsPath ) && clips != null )
			{
				var srcs = obj.GetComponents<AudioSource>();
				for( int i=0; i<srcs.Length; i++ )
				{
					for( int j=0; j<clips.Length; j++ )
					{
						if( srcs[i].clip == clips[j] )
						{
							audioSrcs.Add( srcs[i] );
							break;
						}
					}
				}
			}
			return audioSrcs;
		}
		/// <summary>
		/// This will look for a random clip in the specified folder (path), if another folder with the same name as the picked
		/// clip exists in the specified folder's parent folder, then another random clip will be searched inside that folder and so on...
		/// </summary>
		/// <returns>Multiple audio clips which should compose a sentence.</returns>
		/// <param name="path">The ABResources Path. If ABResources is not being used or the path is not found the Resources folder will be searched instead.</param>
		public static List<AudioClip> GetSentence( this string path )
		{
			List<AudioClip> sentence = new List<AudioClip>();

			AudioClip[] clips = Resources.LoadAll<AudioClip>( path );
			if( clips != null )
			{
				if( clips.Length > 0 )
				{
					//GET A RANDOM CLIP AND SEARCH FOR ITS FOLDER IN PARENT FOLDER
					int ran = UnityEngine.Random.Range( 0, clips.Length );
					sentence.Add( clips[ran] );
					int lastFolderIndex = path.LastIndexOf( "/", path.Length - 1 );
					if( lastFolderIndex < 0 )
					{
						lastFolderIndex = path.Length;
					}
					path = path.Substring( 0, lastFolderIndex );
					path += "/" + clips[ran].name;
					sentence.AddRange( path.GetSentence() );
				}
			}
			return sentence;
		}
		
		private static string _previousPath="";
		/// <summary>
		/// This allows the below recursive method to know which execution is the last return (first call).
		/// </summary>
		private static int _i = 0;
		/// <summary>
		/// This will look for a random clip in the specified folder (path), if another folder with the same name as the picked
		/// clip exists in the specified folder's parent folder, then another random clip will be searched inside that folder 
		/// and so on... If -addRandomsFromLast- parameter is higher than 0, the specified amount of clips will be randomly
		/// added from the last valid directory, if that directory holds more or the same amount of clips that this value represents.
		/// </summary>
		/// <returns>Multiple audio clips which should compose a sentence.</returns>
		/// <param name="path">Path.</param>
		/// <param name="addRandomsFromLast">If higher than 0, the specified amount of clips will be randomly added from 
		/// the last valid directory, if that directory holds more or the same amount of clips that this value represents.</param>
		public static List<AudioClip> GetSentenceRepeatAddLast( this string path, int addRandomsFromLast = 0 )
		{ 
			List<AudioClip> sentence = new List<AudioClip>();
			int recursiveId = _i++;
			
			var clips = Resources.LoadAll<AudioClip>( path );
			if( clips != null )
			{
				if( clips.Length > 0 )
				{
					if( clips.Length >= addRandomsFromLast )
						_previousPath = path;
					//GET A RANDOM CLIP AND SEARCH FOR ITS FOLDER IN PARENT FOLDER
					int ran = Random.Range( 0, clips.Length );
					sentence.Add( clips[ran] );
					int lastFolder = path.LastIndexOf( "/" );
					path = path.Substring( 0, lastFolder );
					path += "/" + clips[ran].name;
					sentence.AddRange( path.GetSentenceRepeatAddLast( addRandomsFromLast ) );
				}
				//IF ELSE, NO CLIPS IN CURRENT PATH
				else if ( addRandomsFromLast > 0 )//IF TRUE, ADD RANDOMS FROM PREVIOUS.
				{
					var allPreviousClips = Resources.LoadAll<AudioClip>( _previousPath );
					if( allPreviousClips.Length >= addRandomsFromLast )
					{
						sentence.AddRange( allPreviousClips.GetRandoms<AudioClip>(addRandomsFromLast) );
					}
				}
			}
			if( recursiveId == 0 )
			{
				sentence.RemoveAt( sentence.Count - 1 - addRandomsFromLast );//Remove possibly duplicated element
				_i = 0;
			}
			return sentence;
		}
		/// <summary>
		/// If the specified path has more than 1 audio clip, this returns a random one.
		/// </summary>
		/// <returns>The random clip.</returns>
		/// <param name="path">Path.</param>
		public static AudioClip GetRandomClip( this string path )
		{
			var clips = Resources.LoadAll<AudioClip>( path );
			if( clips != null )
			{
				if( clips.Length > 0 )
				{
					int ran = Random.Range( 0, clips.Length );
					return clips[ran];
				}
			}
			return null;
		}
		/// <summary>
		/// Get the total length (duration) of the specified clips.
		/// </summary>
		/// <returns>The total length.</returns>
		/// <param name="clips">Clips.</param>
		public static float GetClipsTotalLength( this IList<AudioClip> clips )
		{
			return clips.GetClipsTotalLength( 0, clips.Count );
		}
		/// <summary>
		/// Gets the total length ( duration ) of the specified clips. If any of the specified indexes is wrong it will be clamped to a valid value.
		/// </summary>
		/// <param name="from">From ( clip index).</param>
		/// <param name="to">To ( clip index ).</param>
		public static float GetClipsTotalLength( this IList<AudioClip> clips, int from, int to )
		{
			float totalLength = 0f;
			to = to.Clamp( 1, clips.Count );
			from = from.Clamp( 0, to - 1 );
			for( int i=from; i<to; i++ )
			{
				if( !clips[i] )
					continue;
				totalLength += clips[i].length;
			}
			return totalLength;
		}
		/// <summary>
		/// Get the total length (duration) of the specified sources clips.
		/// </summary>
		/// <returns>The total lenght.</returns>
		/// <param name="clips">Clips.</param>
		public static float GetClipsTotalLength( this IList<AudioSource> srcs )
		{
			float totalLength = 0f;
			for( int i=0; i<srcs.Count; i++ )
			{
				totalLength += srcs[i].clip.length;
			}
			return totalLength;
		}
		public static AudioClip GetClip( this IList<AudioClip> clips, string name )
		{
			for( int i=0; i<clips.Count; i++ )
			{
				if( clips[i] )
				{
					if( clips[i].name.Equals( name, System.StringComparison.CurrentCultureIgnoreCase ) )
					{
						return clips[i];
					}
				}
			}
			return null;
		}
		public static List<AudioClip> GetClips( this IList<AudioClip> clips, IList<string> names )
		{
			List<AudioClip> _clips = new List<AudioClip>();
			for( int i=0; i<clips.Count; i++ )
			{
				for( int j=0; j<names.Count; j++ )
				{
					if( clips[i] )
					{
						if( clips[i].name.Equals( names[j], System.StringComparison.CurrentCultureIgnoreCase ) )
						{
							_clips.Add( clips[i] );
						}
					}
				}
			}
			return _clips;
		}
		private static GameObject _GetSfx()
		{
			var obj = GameObject.Find("Sfx");
			if( !obj )
			{
				obj = new GameObject("Sfx");
			}
			return obj;
		}
		#endregion

		#region SET
		public static void SetVolumes( this IList<AudioSource> sources, float[] volumes )
		{
			if( sources == null )
				return;
			for( int i=0; i<sources.Count; i++ )
			{
				if( volumes[i] != -1 )
				{
					sources[i].volume = volumes[i];
				}
			}
		}		
		public static void SetVolumes( this IList<AudioSource> sources, float volume )
		{
			if( sources == null )
				return;
			for( int i=0; i<sources.Count; i++ )
			{
				sources[i].volume = volume;
			}
		}		
		public static void SetPitches( this IList<AudioSource> sources, float[] pitches )
		{
			if( sources == null )
				return;
			for( int i=0; i<sources.Count; i++ )
			{
				if( sources[i] && pitches[i] != -1 )
				{
					sources[i].pitch = pitches[i];
				}
			}
		}
		public static void SetPitches( this IList<AudioSource> sources, float pitch )
		{
			if( sources == null )
				return;
			for( int i=0; i<sources.Count; i++ )
			{
				if( sources[i] )
				{
					sources[i].pitch = pitch;
				}
			}
		}
		#endregion

		#region MISC
		/// <summary>
		/// Returns the first found playing sound, null if no sound is playing.
		/// </summary>
		/// <returns>The playing sound.</returns>
		public static AudioSource FindPlayingSound()
		{
			var sounds = GameObject.FindObjectsOfType<AudioSource>();
			if( sounds != null )
			{
				for( int i=0; i<sounds.Length; i++ )
				{
					if( sounds[i].isPlaying )
					{
						return sounds[i];
					}
				}
			}
			return null;
		}
		public static bool ContainsClip( this IList<AudioClip> clips, string name )
		{
			for( int i=0; i<clips.Count; i++ )
			{
				if( clips[i] )
				{
					if( clips[i].name.Equals( name, System.StringComparison.CurrentCultureIgnoreCase ) )
					{
						return true;
					}
				}
			}
			return false;
		}
		/*public static byte[] ConvertToWav( this AudioClip clip )
		{
			

			/*var s2 = new FileStream("assets\\resources\\sounds\\s2.wav", FileMode.Create);
			var bw = new BinaryWriter(s2); 
			bw.Write(saveBytes);
			s2.Close();

			//Read s1.wav

			var sr = new StreamReader(  );
			var fileContents = sr.ReadToEnd();
			var saveBytes = System.Text.Encoding.UTF8.GetBytes(fileContents);
			var s = new System.IO.MemoryStream(saveBytes); 
			var br = new BinaryReader(s);
			sr.Close();
		}*/
		public static byte[] EncodeToWav( this AudioClip clip )
		{
			var samples = new float[clip.samples];
			
			clip.GetData(samples, 0);
			
			System.Int16[] intData = new System.Int16[samples.Length];
			//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]
			
			System.Byte[] bytesData = new System.Byte[samples.Length * 2];
			//bytesData array is twice the size of
			//dataSource array because a float converted in Int16 is 2 bytes.
			
			int rescaleFactor = 32767; //to convert float to Int16
			
			for (int i = 0; i<samples.Length; i++) {
				intData[i] = (short) (samples[i] * rescaleFactor);
				System.Byte[] byteArr = new System.Byte[2];
				byteArr = System.BitConverter.GetBytes(intData[i]);
				byteArr.CopyTo(bytesData, i * 2);
			}
			
			return bytesData;
		}
		public static AudioClip Decode( this byte[] clip, string clipName, string fileExt = ".wav" )
		{
			switch( fileExt )
			{
			case ".wav": return clip.DecodeFromWav( clipName );
			}
			return null;
		}
		public static AudioClip DecodeFromWav( this byte[] encodedClip, string clipName, bool stream = false )
		{
			WAV wav = new WAV( encodedClip );
			AudioClip audioClip = AudioClip.Create( clipName, wav.SampleCount, 1, wav.Frequency, stream );
			audioClip.SetData( wav.LeftChannel, 0 );
			return audioClip;
		}
		/// <summary>
		/// Combines this audio clips into one.
		/// </summary>
		/// <param name="clips">Clips.</param>
		public static AudioClip Combine( this IList<AudioClip> clips )
		{
			if( clips == null || clips.Count == 0 )
				return null;
			
			int length = 0;
			for (int i = 0; i < clips.Count; i++)
			{
				if (clips[i] == null)
					continue;
				
				length += clips[i].samples * clips[i].channels;
			}
			
			float[] data = new float[length];
			length = 0;
			for (int i = 0; i < clips.Count; i++)
			{
				if (clips[i] == null)
					continue;
				
				float[] buffer = new float[clips[i].samples * clips[i].channels];
				clips[i].GetData(buffer, 0);
				//System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
				buffer.CopyTo(data, length);
				length += buffer.Length;
			}
			
			if (length == 0)
				return null;
			
			AudioClip result = AudioClip.Create( "CombinedClip", length, 2, 44100, false );
			result.SetData(data, 0);
			
			return result;
		}
		#endregion

	}
}