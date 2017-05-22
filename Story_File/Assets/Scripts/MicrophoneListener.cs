using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	[RequireComponent( typeof( AudioSource ) )]
	public class MicrophoneListener : MonoBehaviour {

		// Use this for initialization
		void Start () {
			AudioSource aud = GetComponent<AudioSource>();
			aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
			aud.Play();
		}	
	}
}
