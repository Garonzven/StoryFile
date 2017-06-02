//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Misc;
using DDK.Base.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DDK.Base.Statics;
using DDK.Base.UI;
using DDK.Base.Classes;

namespace DDK.GamesDevelopment.MemoryGames.Cards 
{
	/// <summary>
	/// This assigns the replacements for the specified FlipAndSwap components.
	/// </summary>
	/// <seealso cref="FlipAndSwap.cs"/>
	public class Initializer : MonoBehaviour 
    {
		[System.Serializable]
		public class Replacement
		{
			[Tooltip("If the FlipAndSwap has a Text component, this will set its value. If any value is null or empty " +
			         "then the Text's value will be equal to the replacement's name.")]
			public string label;
			public Sprite replacement;
			public Color color = Color.white;
			public AudioClip descriptionClip;


			/// <summary>
			/// The name of the /replacement/ (sprite).
			/// </summary>
			public string name 
            {
				get
                {
					if( !replacement )
						return null;
					return replacement.name;
				}
			}


			public Replacement( Sprite replacement )
			{
				this.replacement = replacement;
			}
			public Replacement(){}


			/// <summary>
			/// Adds the sprites for the specified replacements.
			/// </summary>
			public static void AddReplacements( IList<Replacement> replacements, IList<Sprite> sprites )
			{
				if( replacements == null )
				{
					replacements = new List<Replacement>();
				}
				for( int i=0; i<sprites.Count; i++ )
				{
					replacements.Add( new Replacement( sprites[i] ) );
				}
			}
			/// <summary>
			/// Returns a new list without the -objs- list contained elements.
			/// </summary>
			/// <returns>The new list without the elements from the -objsToRemove- list that are in the speified list.</returns>
			/// <param name="list">List.</param>
			/// <param name="objsToRemove">The name of the elements that must be removed.</param>
			public static List<Replacement> RemoveNamed( IList<Replacement> replacements, IList<string> objsToRemove, 
                                               StringComparison comparison = StringComparison.CurrentCultureIgnoreCase )
			{
				List<Replacement> newList = new List<Replacement>(replacements);
				for( int i=0; i<objsToRemove.Count; i++ )
				{
					for( int j=0; j<newList.Count; j++ )
					{
						if( newList[j].name.Equals( objsToRemove[i], comparison ) )
						{
							newList.RemoveAt( j );
						}
					}
				}
				return newList;
			}
		}



		[Tooltip("If true, the replacements will be assigned OnStart(); otherwise, you'll have to call the " +
			"AssignReplacements() from some event or another script")]
		public bool assignReplacementsForFlipAndSwapsOnStart = true;
		[Tooltip("If true, the FlipAndSwap components must be attached to the children; otherwise, they will be searched in hierarchy")]
		public bool replaceOnChildren = true;

		[Header("Replacements")]
		[Tooltip("If true, the replacements will be applied to the FlipAndSwaps children")]
		public bool doOnChildren;
		public bool loadFromResources = true;
		[HelpBoxAttribute]
		public string msg = "If using 2nd replacements, their NAMES MUST MATCH with the first's";
		//TRUE
        [DisableIfAttribute( "loadFromResources", true )]
		public PathHolder.Index replacementsPath;
        [DisableIfAttribute( "loadFromResources", true )]
		[Tooltip("If this is set, the second pairs will be considered to be different from the first (different sprites)")]
		public PathHolder.Index replacementsPath2;
        [DisableIfAttribute( "loadFromResources", true )]
		public string[] skipSpritesNamed = new string[]{ "CardBack" };
		//FALSE
		[Space(5f)]
        [DisableIfAttribute( "loadFromResources" )]
		public List<Replacement> replacements = new List<Replacement>();
        [DisableIfAttribute( "loadFromResources" )]
		[Tooltip("If this is set, the second pairs will be considered to be different from the first (different sprites)")]
		public List<Replacement> replacements2 = new List<Replacement>();
		[Space(5f)]
		public bool destroyThisAfterInit = true;
		public bool setGameEnd;
		public DelayedAction[] onGameEnd;
		public DelayedAction[] onCorrect;
		public DelayedAction[] onIncorrect;
		private OnCardClick[] onCardClicks;
		private FlipAndSwap[] flipAndSwaps;


		public int FlippedCards
		{
			get
			{ 
				int flippedCards = 0;
				if (flipAndSwaps != null)
					for (int i = 0; i < flipAndSwaps.Length; i++)
						if (flipAndSwaps[i]._flipped)
							flippedCards++;
				return flippedCards;
			}
		}

		public static bool _assigningReplacements;



		// Use this for initialization
		void Start () 
        {
			if( assignReplacementsForFlipAndSwapsOnStart )
			{
				AssignReplacements();
			}
			FlipAndSwap.willBeFlippedCount = 0;  //Proper restart of the game
		}



		public void AssignReplacements()
		{
			flipAndSwaps = null;
			onCardClicks = null;
			if( replaceOnChildren )
			{
				flipAndSwaps = GetComponentsInChildren<FlipAndSwap>();
				onCardClicks = GetComponentsInChildren<OnCardClick>();
			}
			else
			{
				flipAndSwaps = GameObject.FindObjectsOfType<FlipAndSwap>();
				onCardClicks = GameObject.FindObjectsOfType<OnCardClick>();
			}

			if( loadFromResources )
			{
				Replacement.AddReplacements( replacements, Resources.LoadAll<Sprite>( replacementsPath.path ) );
				if( replacementsPath2.pathIndex >= 0 )
				{
					var allReplacements = replacements;
					Replacement.AddReplacements( allReplacements, Resources.LoadAll<Sprite>( replacementsPath2.path ) );
					replacements = allReplacements;
				}
			}
			else if( replacements2.Count == replacements.Count )
			{
				replacements.AddRange( replacements2 );
            }
			else
			{
				replacements.AddRange( replacements );
			}
            
            if( replacements == null )
			{
                Utilities.LogError( "Replacements path is wrong or there are no replacement sprites in the specified path." );
				return;
			}
			replacements = Replacement.RemoveNamed( replacements, skipSpritesNamed );
			if( replacements.Count == 0 )
			{
				Utilities.LogError( "Replacements path is wrong or there are no replacement sprites in the specified path." );
				return;
			}
			//ASSIGN!
			_assigningReplacements = true;
			
			List<int> indexes = new List<int>( replacements.Count );//Each index will be removed after it is assigned
			for( int i=0; i<replacements.Count; i++ )
			{
				indexes.Add( i );
			}
			indexes.Shuffle();
			var flipAndSwapsShuffled = flipAndSwaps.ToList();
			flipAndSwapsShuffled.Shuffle();
			
			for( int i=0; i<flipAndSwaps.Length; i++ )
			{
				//SET GAME SETTINGS
				onCardClicks[i].SetGameEnd = setGameEnd;
				onCardClicks[i].OnGameEnd = onGameEnd;
				onCardClicks[i].OnCorrect = onCorrect;
				onCardClicks[i].OnIncorrect = onIncorrect;
				//GET INDEX OF THE SPRITE TO ASSIGN
				int randomIndex = indexes[ 0 ];
				if( (i+1) % 2 == 0 || replacements.Count == flipAndSwaps.Length )//REMOVE INDEX IF ALREADY ASSIGNED
				{
                    indexes.RemoveAt( 0 );
                }
                
				//ASSIGN
				if( doOnChildren && flipAndSwapsShuffled[ i ]._imgChild )
				{
					flipAndSwapsShuffled[ i ].childReplacement = replacements[ randomIndex ].replacement;
					flipAndSwapsShuffled[ i ]._imgChild.color = replacements[ randomIndex ].color;
				}
				else 
				{
					flipAndSwapsShuffled[ i ].replacement = replacements[ randomIndex ].replacement;
					flipAndSwapsShuffled[ i ]._img.color = replacements[ randomIndex ].color;
				}
				if( flipAndSwapsShuffled[ i ]._text )
					flipAndSwapsShuffled[ i ]._text.text = replacements[ randomIndex ].label;
				if( !flipAndSwapsShuffled[ i ].cardDescription )
					flipAndSwapsShuffled[ i ].cardDescription = replacements[ randomIndex ].descriptionClip;

            }
            
            _assigningReplacements = false;
			if( destroyThisAfterInit )
				Destroy ( this );
        }

		public void _SetColor( FlipAndSwap card, Color color )
		{
			if( doOnChildren )
				card._imgChild.color = color;
			else card._img.color = color;
		}
		public void _SetText( FlipAndSwap card, string value )
		{
			var text = card.GetComponentInChildren<Text>();
			if( !text )
				return;
			text.text = value;
		}
        
    }

}
