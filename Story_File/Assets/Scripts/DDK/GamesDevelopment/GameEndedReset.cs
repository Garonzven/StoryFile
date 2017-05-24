//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.GamesDevelopment {

	/// <summary>
	/// This resets the Game.ended variable, from the Statics namespace, to false.
	/// </summary>
	public class GameEndedReset : MonoBehaviour {

		[Tooltip("If false, it will be reset on Start()")]
		public bool onAwake = true;


		void Awake()
		{
			if( onAwake )
				Reset();
		}

		// Use this for initialization
		void Start () {

			if( !onAwake )
				Reset();
		}


		public void Reset()
		{
			Game.ended = false;
		}
	}
}
