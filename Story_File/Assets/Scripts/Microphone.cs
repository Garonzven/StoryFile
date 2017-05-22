using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( AudioSource ) )]
public class Microphone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioSource aud = GetComponent<AudioSource>();
		aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
		aud.Play();
	}	
}
