//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.SoundFX;
using DDK.Base.Managers;


namespace DDK.Base.Extensions 
{	
    /// <summary>
    /// Sfx class extension.
    /// </summary>
	public static class SfxExt 
    {		
		#region MISC
		/// <summary>
		/// Creates a List of sound effects.
		/// </summary>
		/// <param name="sfx">Sfx.</param>
		/// <param name="clips">Clips.</param>
		public static List<Sfx> Create( this List<Sfx> sfx, IList<AudioClip> clips )
		{
			sfx = new List<Sfx>();
			for( int i=0; i<clips.Count; i++ )
			{
				if( clips[i] != null )
				{
					Sfx _sfx = new Sfx( clips[i] );
					sfx.Add( _sfx );
				}
			}
			return sfx;
		}
		/// <summary>
		/// Creates a List of sound effects.
		/// </summary>
		/// <param name="sfx">Sfx.</param>
		/// <param name="clips">Clips.</param>
		public static List<Sfx> Create( this List<Sfx> sfx, IList<Object> clips )
		{
			sfx = new List<Sfx>();
			for( int i=0; i<clips.Count; i++ )
			{
				if( clips[i] != null )
				{
					Sfx _sfx = new Sfx( clips[i] as AudioClip );
					sfx.Add( _sfx );
				}
			}
			return sfx;
		}
		#endregion MISC

		#region GET
		public static List<AudioClip> GetClips( this IList<Sfx> sfx )
		{
			List<AudioClip> clips = new List<AudioClip>();
			for( int i=0; i<sfx.Count; i++ )
			{
				clips.Add( sfx[i].clip );
			}
			return clips;
		}
		public static List<float> GetVolumes( this IList<Sfx> sfx )
		{
			List<float> vols = new List<float>();
			for( int i=0; i<sfx.Count; i++ )
			{
				vols.Add( sfx[i].volume );
			}
			return vols;
		}
		
		public static List<float> GetPitches( this IList<Sfx> sfx )
		{
			List<float> pitches = new List<float>();
			for( int i=0; i<sfx.Count; i++ )
			{
				pitches.Add( sfx[i].pitch );
			}
			return pitches;
		}
		
		public static List<bool> GetPlayOnAwakes( this IList<Sfx> sfx )
		{
			List<bool> poa = new List<bool>();
			for( int i=0; i<sfx.Count; i++ )
			{
				poa.Add( sfx[i].playOnAwake );
			}
			return poa;
		}
		/// <summary>
		/// Get the total length (duration) of the specified sfx's clips.
		/// </summary>
		/// <returns>The total lenght.</returns>
		/// <param name="sfx">Sound Effects.</param>
		/// <param name="includeDelays">If true, the /delay/ and /nextDelay/ will be included in the Total Length.</param>
		public static float GetClipsTotalLength( this IList<Sfx> sfx, bool includeDelays = false )
		{
			return sfx.GetClipsTotalLength( 0, sfx.Count, includeDelays );
		}
		/// <summary>
		/// Gets the total length ( duration ) of the specified clips. If any of the specified indexes is wrong it will be clamped to a valid value.
		/// </summary>
		/// <param name="from">From ( clip index).</param>
		/// <param name="to">To ( clip index ).</param>
		public static float GetClipsTotalLength( this IList<Sfx> sfx, int from, int to, bool includeDelays = false )
		{
			float delays = 0f;
			to = to.Clamp( 1, sfx.Count );
			from = from.Clamp( 0, to - 1 );
			if( includeDelays )
			{
				for( int i=from; i<to; i++ )
				{
					delays += sfx[i].delay + sfx[i].nextDelay;
				}
			}
			return sfx.GetClips().GetClipsTotalLength( from, to ) + delays;
		}
		/// <summary>
		/// Gets the total delays of the specified sound effects. If any of the specified indexes is wrong it will be clamped to a valid value.
		/// </summary>
		/// <param name="from">From ( clip index).</param>
		/// <param name="to">To ( clip index ).</param>
		public static float GetDelays( this IList<Sfx> sfx, int from, int to )
		{
			float delays = 0f;
			to = to.Clamp( 1, sfx.Count );
			from = from.Clamp( 0, to - 1 );
			for( int i=from; i<to; i++ )
			{
				delays += sfx[i].delay + sfx[i].nextDelay;
			}
			return delays;
		}
		/// <summary>
		/// Gets the total delays of the specified sound effects. If any of the specified indexes is wrong it will be clamped to a valid value.
		/// </summary>
		/// <param name="from">From ( clip index).</param>
		/// <param name="to">To ( clip index ).</param>
		public static float GetAllDelays( this IList<Sfx> sfx )
		{
			return sfx.GetDelays( 0, sfx.Count );
		}
		#endregion GET

		#region SET
		public static void SetClips( this IList<Sfx> sfx, IList<AudioClip> clips )
		{
			if( sfx == null )
			{
				Debug.LogWarning("No Sfx");
				return;
			}
			if( sfx.Count != clips.Count )
			{
				Debug.LogWarning( "The clips count doesn't match the sfx count" );
				return;
			}
			for( int i=0; i<sfx.Count; i++ )
			{
				sfx[i].clip = clips[i];
			}
		}
		/// <summary>
		/// Set the source for all this Sfx.
		/// </summary>
		public static void SetSources( this IList<Sfx> sfx, string source )
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				sfx[i].source = source;
			}
		}
		/// <summary>
		/// Set the delay for all this Sfx.
		/// </summary>
		public static void SetDelays( this IList<Sfx> sfx, float delay )
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				sfx[i].delay = delay;
			}
		}
		/// <summary>
		/// Set the volume for all this Sfx.
		/// </summary>
		public static void SetVolumes( this IList<Sfx> sfx, float volume )
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				sfx[i].volume = volume;
			}
		}
		/// <summary>
		/// Set the pitch for all this Sfx.
		/// </summary>
		public static void SetPitches( this IList<Sfx> sfx, float pitch )
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				sfx[i].pitch = pitch;
			}
		}
		#endregion SET

		#region PLAY/STOP MANAGEMENT
		/// <summary>
		/// Plays the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		public static AudioSource Play( this Sfx sfx, string audioSourceHolder )
		{
			if( sfx != null )
			{
				return sfx.clip.Play( audioSourceHolder );
			}
			return null;
		}
		/// <summary>
		/// Plays the specified Sfx. NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify
		/// if you want to apply or ommit the pitche and volume values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		public static AudioSource Play( this Sfx sfx, Behaviour pauser = null )	
		{
			sfx._Play( pauser ).Start();
			return sfx.Source;
		}
		/// <summary>
		/// Plays the specified Sfx in the specified source. If the audio source is null, the /sfx/'s audio source will be used instead.
	    /// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or ommit the pitche and volume values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		public static AudioSource Play( this Sfx sfx, AudioSource source, Behaviour pauser = null )	
		{
			if( !source )
			{
				source = sfx.Source;
			}
			sfx._Play( source, pauser ).Start();
			return source;
		}
		/// <summary>
		/// Plays the specified Sfxs. NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify
		/// if you want to apply or ommit the pitche and volume values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		public static void Play( this IList<Sfx> sfx, Behaviour pauser = null )	
		{
			sfx._Play( pauser ).Start();
		}
		/// <summary>
		/// Plays the specified Sfx in the SfxManager's specified source. If the audio source doesn't exist, the SfxManager's 
		/// Effects audio source will be used instead. NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values
		/// to specify if you want to apply or ommit the pitche and volume values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		public static void Play( this IList<Sfx> sfx, AudioSource source, Behaviour pauser = null )	
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				sfx[i].Play( source, pauser );
			}
		}
		/// <summary>
		/// If a clip matches the specified /name/ it will be played in its SfxManager's source.
		/// </summary>
		public static bool PlayClip( this IList<Sfx> sfx, string name ) 
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				if( sfx[i].clip && sfx[i].clip.name.Equals( name, System.StringComparison.CurrentCultureIgnoreCase ) )
				{
					sfx[i].Play();
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Plays the specified Sfx in the SfxManager's specified source. If the audio source doesn't exist, the SfxManager will create it
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitche and volume values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		private static IEnumerator _Play( this Sfx sfx, Behaviour pauser = null )
		{
			if( sfx == null )
				yield break;
			yield return sfx._Play( sfx.Source, pauser ).Start();
		}
		/// <summary>
		/// Plays the specified Sfx in the specified source. If the audio source is null, the /sfx/'s 
		/// audio source will be used instead. NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values
		/// to specify if you want to apply or ommit the pitche and volume values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		/// <param name="currentRef">This will keep a reference of the currently playing audio source.</param>
		private static IEnumerator _Play( this Sfx sfx, AudioSource source, Behaviour pauser = null )
		{
			if( sfx == null )
				yield break;
			if( !string.IsNullOrEmpty( sfx._source ) || !source )
				source = sfx.Source;

			var clip = sfx.clip;
			var volume = sfx.volume;
			var pitche = sfx.pitch;
			while( pauser && !pauser.enabled )
				yield return null;
			if( clip != null && sfx != null && source.gameObject.IsActiveInHierarchy() )
			{
				sfx.OnClipPlay();
				source.clip = clip;
				if( Sfx.m_applyVolumes )
				{
					source.volume = volume;
				}
				if( Sfx.m_applyPitches )
				{
					source.pitch = pitche;
				}
				source.Play ();
				#region PREVENT COROUTINE FROM CONTINUING IF SOURCE.PLAY() IS BEING CALLED AGAIN
                yield return new WaitForSeconds( clip.length * 0.8f );
                while( source.isPlaying )
                {
                    if( source.time == 0f )
                    {
                       yield break;
                    }
                    yield return null;
					if( !source )
						yield break;
                }
                #endregion
                yield return new WaitForSeconds( sfx.nextDelay );
            }
        }
		/// <summary>
		/// Enables the spcified source and plays the specified sfxs, to stop the playing queue just deactivate the 
		/// gameoobject who holds it. If the audio source is null the SfxManager's Effects audio source will be used instead.
		/// If it doesn't exist the SfxManager will create it.
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		public static void Play( this IList<Sfx> sfx, AudioSource source )
		{
			sfx.PlayAfter( 0f, source );
		}
		/// <summary>
		/// Enables the spcified source and plays the specified sfxs, to stop the playing queue just deactivate the 
		/// gameoobject who holds it. If the Sfx has a source specified, it will override this function's source.
		/// If this function's source is null the SfxManager's Effect audio source will be used instead.
		/// If it doesn't exist the SfxManager will create it.
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		public static void PlayAfter( this IList<Sfx> sfx, float after, AudioSource source, Behaviour pauser = null )
		{
			sfx._Play( source, pauser ).Start( after );
		}	
		/// <summary>
		/// Enables the spcified source and plays the specified sfxs, to stop the playing queue just deactivate the 
		/// gameoobject who holds it.
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		public static void Play( this IList<Sfx> sfx, float after, Behaviour pauser = null )
		{
			sfx._Play( pauser ).Start( after );
		}
		/// <summary>
		/// Enables the spcified source and plays the specified sfxs, to stop the playing queue just deactivate the 
		/// gameoobject who holds it.
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		/// <param name="source">If not null, this will override all sfxs sources.</param>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		private static IEnumerator _Play( this IList<Sfx> sfx, Behaviour pauser = null )
		{
			if( sfx == null ) yield break;
			for( int i=0; i<sfx.Count; i++ )
			{
				yield return sfx[i]._Play( pauser ).Start();
			}
		}
		/// <summary>
		/// Enables the spcified source and plays the specified sfxs, to stop the playing queue just deactivate the 
		/// gameobject who holds it. If the Sfx has a source specified, it will override this function's source.
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		/// <param name="source">If not null, this will override all sfxs sources.</param>
		/// <param name="pauser">If not null, and the behaviour is disabled, the source will be paused.</param>
		private static IEnumerator _Play( this IList<Sfx> sfx, AudioSource source, Behaviour pauser = null )
		{
			if( sfx == null ) yield break;
			for( int i=0; i<sfx.Count; i++ )
			{
				yield return sfx[i]._Play( source, pauser ).Start();
			}
			/*if( source != null )
			{
				source.enabled = true;
				var clips = sfx.GetClips();
				var volumes = sfx.GetVolumes();
				var pitches = sfx.GetPitches();
				for( int i=0; i<sfx.Count; i++ )
				{
					while( pauser && !pauser.enabled )
						yield return null;
					if( clips[i] != null && sfx[i] != null && source.gameObject.IsActiveInHierarchy() )
					{
						#region ACTIVATE, DEACTIVATE, ENABLE, DISABLE..
						if( sfx[i].onClipPlay != null )
						{
							if( sfx[i].onClipPlay.activate != null )//ACTIVATE..
							{
								sfx[i].onClipPlay.activate.SetActiveInHierarchy();
							}
							if( sfx[i].onClipPlay.deactivate != null )//DEACTIVATE..
							{
								sfx[i].onClipPlay.deactivate.SetActiveInHierarchy( false );
							}
							if( sfx[i].onClipPlay.enable != null )//ENABLE..
							{
								sfx[i].onClipPlay.enable.SetEnabled( true );
							}
							if( sfx[i].onClipPlay.disable != null )//DISABLE..
							{
								sfx[i].onClipPlay.disable.SetEnabled( false );
							}
						}
						#endregion
						source.clip = clips[i];
						if( Sfx.m_applyVolumes )
						{
							source.volume = volumes[i];
						}
						if( Sfx.m_applyPitches )
						{
							source.pitch = pitches[i];
						}
						source.Play ();
						#region PREVENT COROUTINE FROM CONTINUING IF SOURCE.PLAY() IS BEING CALLED AGAIN
						yield return new WaitForSeconds( clips[i].length * 0.8f );
						bool _break = false;
						while( source.isPlaying )
						{
							if( source.time == 0f )
							{
								_break = true;
								break;
							}
							yield return null;
						}
						if( _break )	break;
						#endregion
						yield return new WaitForSeconds( sfx[i].nextDelay );
					}
				}
			}
			else
			{
				GameObject srcHolder = "Sfx".Find();
				var src = srcHolder.GetAddAudioSource();
				yield return sfx._Play( src ).Start();
			}*/
		}
        /*
		/// <summary>
		/// Stop all sources and play the specified clip in the specified object. If the obj doesn't have an audio source one will be added, 
		/// and if the obj doesn't exist it will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs 
		/// inactiveObjs array.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		public static AudioSource StopAllAndPlay( this Sfx sfx, string audioSourceHolder = "Sfx", IList<string> omit = null )
		{
			Sfx.StopAll( omit );
			if( sfx != null )
			{
				return sfx.Play( audioSourceHolder );
			}
			return null;
		}
		
		/// <summary>
		/// Plays the specified sfxs in the specified object, to stop the playing queue just deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// NOTE: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		public static AudioSource Play( this IList<Sfx> sfx, string audioSourceHolder = "Sfx" )
		{
			return sfx.Play( 0f, audioSourceHolder );
		}
		
		/// <summary>
		/// Plays the specified sfxs in the specified object, to stop the playing queue just deactivate the 
		/// gameoobject who holds it. If the obj doesn't have an audio source one will be added, and if the obj doesn't exist it
		/// will be created. NOTE: If the object is inactive make sure to added to the InactivesHolder.cs inactiveObjs array.
		/// NOTE 2: Call Sfx.m_applyVolumes or Sfx.m_applyPitches and set their values to specify if you want to apply or 
		/// ommit the pitches and volumes values.
		/// </summary>
		public static AudioSource Play( this IList<Sfx> sfx, float after, string audioSourceHolder = "Sfx" )
		{
			if( sfx != null )
			{
				if( !string.IsNullOrEmpty( audioSourceHolder ) )
				{
					GameObject srcHolder = audioSourceHolder.Find();
					var src = srcHolder.GetAddAudioSource();
					sfx.Play( after, src );
				}
			}
			return null;
		}
		*/
		/// <summary>
		/// Play the specified sfx in this source.
		/// </summary>
		/// <returns> Returns the playing clip's length. </returns>
		public static IEnumerable Play( this AudioSource src, IList<Sfx> sfx )
		{
			if( sfx == null )
			{
				yield break;
			}
			for( int i=0; i<sfx.Count; i++ )
			{
				if( sfx[i] == null )
				{
					continue;
				}
				sfx[i].OnClipPlay();
				if( Sfx.m_applyVolumes )
				{
					src.volume = sfx[i].volume;
				}
				if( Sfx.m_applyPitches )
				{
					src.pitch = sfx[i].pitch;
				}
				src.clip = sfx[i].clip;
				src.Play ();
				yield return new WaitForSeconds( src.clip.length );
			}

			/*var clips = sfx.GetClips();
			if( src != null && clips != null )
			{
				for( int i=0; i<clips.Count; i++ )
				{
					if( clips[i] != null )
					{
						src.volume = sfx[i].volume;
						src.pitch = sfx[i].pitch;
						src.clip = clips[i];
						src.Play ();
						yield return new WaitForSeconds( clips[i].length );
					}
				}
			}*/
		}	
		#endregion PLAY/STOP MANAGEMENT
		
	}

}