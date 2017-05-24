//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.Handlers {

	public class TimeHandler : MonoBehaviour {

        [HelpBoxAttribute]
        public string msg = "Disable to prevent Update() from being called each frame. Events will still be invoked if this component is disabled";
		[Tooltip("If true, this component is disabled after the time scale is set")]
		public bool onStart;
		[Tooltip("If true the /timeScale/ will be added instead of set")]
		[ShowIfAttribute( "_OnStart", 1 )]
		public bool addAsIncrement;
		[ShowIfAttribute( "_OnStart", 1 )]
		public bool destroyThisAfter;
		public float timeScale = 1f;
		[Tooltip("When true, the AudioSources array will be updated will all active AudioSources found. This is " +
			"disabled after the update is done")]
		public bool updateAudioSources;


		protected bool _OnStart()
		{
			return onStart;
		}


		protected AudioSource[] allAudioSources;


		// Use this for initialization
		IEnumerator Start () {

			if( onStart )
			{
				if( addAsIncrement )
				{
					Time.timeScale += timeScale;
					UpdateAudioSourcesArray();
					UpdateAudioSourcesPitches();
				}
				else Update();
				if( destroyThisAfter )
					Destroy ( this );
				enabled = false;
				yield break;
			}
			yield return null;
			UpdateAudioSourcesArray();
		}

		// Update is called once per frame
		void Update () {

			Time.timeScale = timeScale;
			UpdateAudioSourcesArray();
			UpdateAudioSourcesPitches();
		}
			

		public void UpdateAudioSourcesArray()
		{
			if( !updateAudioSources )
				return;
			allAudioSources = GameObject.FindObjectsOfType<AudioSource>();
			updateAudioSources = false;
		}
		/// <summary>
		/// Set the audio sources pitches according to the current Time.timeScale value.
		/// </summary>
		public void UpdateAudioSourcesPitches()
		{
			allAudioSources.SetPitches( Time.timeScale );
		}
		/// <summary>
		/// Updates the audio sources array and then sets their pitches according to the current Time.timeScale value.
		/// </summary>
		public void UpdateAudioSourcesArrayAndPitches()
		{
			allAudioSources = GameObject.FindObjectsOfType<AudioSource>();
			updateAudioSources = false;
			UpdateAudioSourcesPitches();
		}

        public void SetTimeScale( float timeScale )
        {
            Time.timeScale = timeScale;
            UpdateAudioSourcesArray();
            UpdateAudioSourcesPitches();
        }

		public void AnimateTimeScale( float target, float duration )
		{
			AnimateTimeScaleCo( target, duration ).Start();
		}
		public IEnumerator AnimateTimeScaleCo( float target, float duration )
		{
			float time = 0;
			float initialTimeScale = Time.timeScale;
			UpdateAudioSourcesArray();
			while( time < duration )
			{
				time += Time.unscaledDeltaTime;
				Time.timeScale = Mathf.Lerp( initialTimeScale, target, time / duration );
				UpdateAudioSourcesPitches();
				yield return null;
			}
		}
	}
}
