using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	[RequireComponent( typeof( AudioSource ) )]
	public class MicrophoneHandler : MonoBehaviour {

		private AudioSource _source;
		private const string MIC_NAME = "Built-in Microphone";


		// Use this for initialization
		void Start () {
			_source = GetComponent<AudioSource>();
		}	


		public void Record( bool record = true )
		{
			if( record )
			{
				_source.clip = Microphone.Start( MIC_NAME, false, 10, 44100);
			}
			else { 
				Microphone.End( MIC_NAME );
			}
		}
		public void PlayLastRecording()
		{
			_source.Play();
		}
	}
}
