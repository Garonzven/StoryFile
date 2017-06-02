//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Misc;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK._2D 
{
	[RequireComponent( typeof( Image ) )]
	public class RandomSpriteAssigner : MonoBehaviour 
    {
		public Sprite[] sprites = new Sprite[0];
		[Tooltip("This will be used if the sprites array is empty")]
		public PathHolder.Index pathHolder;


		// Use this for initialization
		void Start () 
        {
			Sprite randomSprite = null;
			if( sprites.Length > 0 )
				randomSprite = sprites.GetRandom<Sprite>();
			else if( pathHolder.isValid ) randomSprite = Resources.LoadAll<Sprite>( pathHolder.path ).GetRandom<Sprite>();
			GetComponent<Image>().sprite = randomSprite;
		}
	}
}
