//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.UI;
using System.Collections.Generic;
using DDK.Base.Extensions;
using UnityEngine.UI;
using DDK.Base.Managers;
using DDK.Base.Statics;
using DDK.Base.SoundFX;
using DDK.Base.Classes;


namespace DDK.GamesDevelopment.MemoryGames.Cards 
{
	[RequireComponent ( typeof( FlipAndSwap ) )]
	public class OnCardClick : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If this object contains a UI Button component, its OnClick() event will be automatically " +
            "added to the button's onClick Listener. NOTE: If the FlipAndSwap has a /childReplacemen/t, the match will" +
            "be done with it instead of the /replacement/";
        #endif
		public Sfx matched;
		public Sfx wrong;
		public Sfx completedSfx;
		public Sfx wrongAnswer;
		public Sfx matchedAnswer;


		[Header("Corrects / Incorrects")]
		[Range( 0f, 1f )]
		public float chancesOfPlayingCorrectsIncorrects = 0.5f;
		[Tooltip("The name of the Sfx Manager's Audio Source that will be used to play the correct and incorrect sounds")]
		public string sourceName = "Voices";
		public string correctClipsPath = "Sfx/Global/Voice Overs/Corrects/Arturo";
		public string incorrectClipsPath = "Sfx/Global/Voice Overs/Incorrects";
		[Header("Events - On Complete (All Cards Flipped)")]
		[Tooltip("If true, the static class's Game.ended variable will be set to true")]
		private bool setGameEnd;
		private DelayedAction[] onGameEnd;
		private DelayedAction[] onCorrect;
		private DelayedAction[] onIncorrect;

		/// <summary>
		/// This FlipAndSwap (card).
		/// </summary>
		protected FlipAndSwap _thisCard;
		internal FlipAndSwap[] allCards;
		public bool shouldSoundBePlayed
		{
			get 
			{
				if( Random.value <= chancesOfPlayingCorrectsIncorrects  )
				{
					return true;
				}
				return false;
			}
		}
		public bool SetGameEnd
		{
			get{ return setGameEnd; }
			set{ setGameEnd = value; }
		}
		public DelayedAction[] OnGameEnd
		{
			get { return onGameEnd; }
			set { onGameEnd = value; }
		}
		public DelayedAction[] OnCorrect
		{
			get { return onCorrect; }
			set { onCorrect = value; }
		}
		public DelayedAction[] OnIncorrect
		{
			get { return onIncorrect; }
			set { onIncorrect = value; }
		}

		/// <summary>
		/// True, if all cards were flipped.
		/// </summary>
		public static bool _completed;
		/// <summary>
		/// Some of these sounds might be played when a pair has been matched or not.
		/// </summary>
        public static AudioClip[] _corrects =  new AudioClip[0];
        public static AudioClip[] _incorrects = new AudioClip[0];

		// Use this for initialization
		void Start ()
		{
			_thisCard = GetComponent<FlipAndSwap>();
            allCards = FlipAndSwap.allFlipAndSwap.ToArray();
			_thisCard._Bt.onClick.AddListener( () => OnClick() );
		}


		public GameObject GetPair()
		{
			for( int i=0; i<allCards.Length; i++ ) //Iterate on all the cards
			{
				if( _thisCard != allCards[i] ) //Not the same card
				{
					if( allCards[i].childReplacement != null )
					{
						if( allCards[i].childReplacement.name.Equals( _thisCard.childReplacement.name ) ||
						   allCards[i].childReplacement == _thisCard.childReplacement )
						{
							return allCards[i].gameObject;
						}
					}
					else if( allCards[i].replacement.name.Equals( _thisCard.replacement.name ) ||
					        allCards[i].replacement == _thisCard.replacement )
					{
						return allCards[i].gameObject;
					}
				}
			}
			return null;
		}
		public GameObject GetNotFlippedCard()
		{
			for( int i=0; i<allCards.Length; i++ )
			{
				if( allCards[i]._Bt.enabled && !allCards[i].WillBeFlipped )
				{
					return allCards[i].gameObject;
				}
			}
			return null;
		}
		public GameObject GetPreviousCard()
		{
			for( int i=0; i<allCards.Length; i++ )
			{
				if( allCards[i]._Bt.enabled && allCards[i].WillBeFlipped && _thisCard != allCards[i] )
				{
					return allCards[i].gameObject;
				}
			}
			return null;
		}

		public void EnableNotFlippedCards( bool value )
		{
			for (int i = 0; i < allCards.Length; i++)//GET PREVIOUS CARD
			{
				if (!allCards [i]._flipped && !allCards[i]._flipping)
				{
					allCards [i]._Bt.interactable = value;
				}
			}
		}
		public IEnumerator CheckCard()
		{
			EnableNotFlippedCards (false);
			while( !_thisCard._flipped )
			{
				yield return null;
			}
			//SEARCH FOR THE FLIPPED CARD AND COMPARE
			if (FlipAndSwap.willBeFlippedCount == 2)
			{
				//GET ENABLED FlipAndSwaps
				FlipAndSwap previousCard = null;
				for (int i = 0; i < allCards.Length; i++) {//GET PREVIOUS CARD
					if (allCards [i]._Bt.enabled && allCards [i].WillBeFlipped && _thisCard != allCards [i])
					{
						previousCard = allCards [i];
						break;
					}
				}
				if (previousCard.childReplacement != null)
				{
					if (previousCard.childReplacement.name.Equals (_thisCard.childReplacement.name) || previousCard.childReplacement == _thisCard.childReplacement)
					{
						yield return StartCoroutine (Correct (previousCard));
					}
					else
					{
						yield return StartCoroutine (Incorrect (previousCard));
					}
				}
				else
				{
					if (previousCard.replacement.name.Equals (_thisCard.replacement.name) || previousCard.replacement == _thisCard.replacement)
					{
						yield return StartCoroutine (Correct (previousCard));
					}
					else
					{
						yield return StartCoroutine (Incorrect (previousCard));
					}
				}
			}
			else
			{
				EnableNotFlippedCards (true);
			}
		}
		public void OnClick()
		{
			StartCoroutine (CheckCard ());
		}
		public IEnumerator Correct( FlipAndSwap previousCard )
		{
            Utilities.Log ( "Correct!" );
			SfxManager.PlaySfx( matched, true, gameObject );
			onCorrect.InvokeAll ();
			if( CheckIfAllCardsFlipped() )
			{
				OnComplete();
			}
			else
				yield return StartCoroutine( PlayCorrect() );
			previousCard._Bt.enabled = false;
			_thisCard._Bt.enabled = false; //Temporal Fix to avoid highlighted color when disabled
			ColorBlock cb = _thisCard._Bt.colors;
			cb.highlightedColor = cb.normalColor;
			_thisCard._Bt.colors = cb;
			enabled = false;
			FlipAndSwap.willBeFlippedCount = 0;//reset
			EnableNotFlippedCards (true);
			yield return null;
		}
		public IEnumerator Incorrect( FlipAndSwap previousCard )
		{
            Utilities.Log ( "Incorrect!" );
			SfxManager.PlaySfx( wrong, true, gameObject );
			onIncorrect.InvokeAll ();
			yield return StartCoroutine( PlayIncorrect() );
			FlipAndSwap.blockOnClick = true;
			FlipBack( previousCard );
			FlipBack( _thisCard );
			while (_thisCard._flipped || previousCard._flipped)
				yield return null;
			FlipAndSwap.blockOnClick = false;
			EnableNotFlippedCards (true);
		}
		public bool CheckIfAllCardsFlipped()
		{
			for( int i=0; i<allCards.Length; i++ )
			{
				if( !allCards[i]._flipped )
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Call when all cards have been flipped.
		/// </summary>
		public void OnComplete()
		{
            Utilities.Log ( "All Cards have been flipped!" );
			SfxManager.PlaySfx( completedSfx, true, gameObject );
			//EVENTS
			if( setGameEnd )
			{
				Game.ended = true;
			}
			onGameEnd.InvokeAll ();
		}
		public void FlipBack( FlipAndSwap card )
		{
			StartCoroutine( card.Flip_Swap() );
		}
		/// <summary>
		/// Plays a random correct sound, unless index is specified and valid.
		/// </summary>
		/// <param name="index">Index.</param>
		public IEnumerator PlayCorrect( int index = -1 )
		{
			Sfx.m_context = gameObject;//for debugging
			if( matched.clip )
			{
				yield return Utilities.WaitForRealSeconds( matched.delay / matched.pitch ).Start();
				yield return null;
				while( SfxManager.IsPlaying( matched ) )
				{
					yield return null;
				}
			}
			SfxManager.PlaySfx( matchedAnswer, true, gameObject );
		}		
		/// <summary>
		/// Plays a random correct sound, unless index is specified and valid.
		/// </summary>
		/// <param name="index">Index.</param>
		public IEnumerator PlayIncorrect( int index = -1 )
		{
            Utilities.Log ( "IncorrectCoRoutine" );
			Sfx.m_context = gameObject;//for debugging
			if( wrong.clip )
			{
				yield return new WaitForSeconds( wrong.delay / wrong.pitch );
				yield return null;
				while( SfxManager.IsPlaying( wrong ) )
				{
					yield return null;
				}
			}
			SfxManager.PlaySfx( wrongAnswer, true, gameObject );
		}
	}

}
