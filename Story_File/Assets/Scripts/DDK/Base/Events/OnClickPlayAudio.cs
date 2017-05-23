using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;
using DDK.Base.Managers;


namespace DDK.Base.Events {

	public class OnClickPlayAudio : MonoBehaviour {//THIS HAS AN EDITOR CLASS

        #if UNITY_EDITOR
        public string msg = "If this object has a Button component, the event is automatically added to the onClick Listener";
        #endif
		[Space(15f)]
		public bool useSfxManager = true;

		[Space(5f)]
		public float delay;
		[Tooltip("If true, and the Source is playing when the specified clip is about to be played, the source will be interupted; otherwise the clip won't be played")]
		public bool interrupt = true;
		public AudioSource source;
		[Tooltip("The name of the SfxManager's source that will play the clip")]
		public string sourceName = "Voices";
		public AudioClip clip;
		[Tooltip("This name will be searched inside the SfxManager loaded clips, if found, it will be played.")]
		public string clipName;



		// Use this for initialization
		void Start () 
        {
			var bt = GetComponent<Button>();
			if( bt )
			{
				bt.onClick.AddListener( Play );
			}
		}


		public void Play()
		{
			if( useSfxManager )
			{
				if( clip )
				{
					SfxManager.PlayClip( sourceName, clip, delay, interrupt );
				}
				else SfxManager.PlayClip( sourceName, clipName, delay, interrupt, gameObject );
			}
			else if( source )
			{
				if( clip )
				{
					source.Play( clip, delay, interrupt );
				}
				else Debug.LogWarning("The clip to be played hasn't been specified");
			}
			else if( clip )
			{
				AudioSource.PlayClipAtPoint( clip, transform.position );
			}
		}
    }
}
