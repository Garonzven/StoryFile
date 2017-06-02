using UnityEngine;
using System.Collections;
using DDK.Base.Classes;


namespace DDK.GamesDevelopment
{
	public class OnGameEnded : MonoBehaviour {

		[Tooltip("This gets called when Game.ended is set to true")]
		public ComposedEvent onGameEnded;
		
		
		
		// Use this for initialization
		void Start () {

			if( onGameEnded != null )
			{
				Game.onGameEnded = onGameEnded;
			}
		}
	}
}
