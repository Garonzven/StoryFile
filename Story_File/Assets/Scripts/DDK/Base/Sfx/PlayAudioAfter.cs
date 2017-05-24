//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Managers;
using DDK.Base.Classes;
using DDK.Base.Statics;


namespace DDK.Base.SoundFX 
{
	/// <summary>
	/// Attach to an object to make an Audio Source play after something happens
	/// </summary>
	public class PlayAudioAfter : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "clipOverride.playOnAwake won't work in this component. The clip will always be played " +
            "after the specified delays";
        #endif
		[Tooltip("The time of the source when it starts playing")]
		public float time;
		[Tooltip("If the clip is not empty/null then it will override the specified source's current clip")]
		public Sfx clipOverride = new Sfx( 1f, 1f );

		[Header("Play After:")]
		public float delay = 0.2f;
		[Tooltip("Name of the source that must end playing for the specified source to play. If no source with the " +
            "specified name exists in the SfxManager this will be omitted")]
		public string finishedPlaying = "";
		[Tooltip("If false, Play() will be called OnEnable()")]
		public bool onMouseUpAsButton = false;

		[Space(10f)]
		[Tooltip("If true, the audio will be played even if its playing already")]
		public bool evenIfItsPlaying;
		public bool stopAllSounds;
		[Tooltip("Sources to omit from Stop All")]
        [ShowIfAttribute( "stopAllSounds", 1 )]
		public string[] omit;

		[Header("On Clip Ended - Events")]
		public bool disable;
		public bool destroyThis;
		public bool destroyObj;
		[Tooltip("This states are applied after the clip ends playing")]
		public EnableDisableState onClipEndedPlaying = new EnableDisableState();		
		
		

		void OnEnable () 
        {
			if( !onMouseUpAsButton )
			{
				StartCoroutine( _PlayAfter() );
			}
		}
		public void OnMouseUpAsButton()
		{
			if( enabled )
			{				
				Play();
			}
		}

        void _OnClipPlayed()
		{
			onClipEndedPlaying.SetStates();
			if( disable )
			{
				enabled = false;
			}
			if( destroyThis )
			{
				Destroy( this );
			}
			if( destroyObj )
			{
				Destroy ( gameObject );
			}
		}

        IEnumerator _PlayAfter()
		{
            if( delay > 0f )
                yield return new WaitForSeconds( delay );
			if( SfxManager.HasSource( finishedPlaying ) )
			{
				while( SfxManager.IsPlaying( finishedPlaying ) )
				{
					yield return null;
				}
			}
			Play();
		}
        void _SetTime()
        {
            SfxManager.SetTime( clipOverride.source, time );
        }

		
		/// <summary>
		/// Stops all playing sounds if specified.
		/// </summary>
		public void StopAllPlayingSounds()
		{
			if( stopAllSounds )
				AudioSourceExt.StopAll( omit );
		}		
		public void Play()
		{
			Sfx.m_context = gameObject;//for debugging

			StopAllPlayingSounds();
            clipOverride.onClipPlay.events.AddListener( _OnClipPlayed );
			if( clipOverride.clip )
			{
				SfxManager.PlaySfx( clipOverride, evenIfItsPlaying, gameObject );
			}
			else SfxManager.Play( clipOverride.source, evenIfItsPlaying );
            Invoke( "_SetTime", clipOverride.delay );
		}		
	}
}