using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public bool test;
	[SerializeField]
	private bool otherTest;

	#if MY_SYMBOL
	void Awake()
	{
		
	}
	#endif
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnEnable(){
		//GameObject.find
	}
}
