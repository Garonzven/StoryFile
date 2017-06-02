//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.UI;
using DDK.Base.Classes;


namespace DDK.Base.UI.Managers 
{
	/// <summary>
	/// Attach to an object to manage a karaoke behaviour. The text component's properties in this object will be used 
	/// to spawn the karaoke words.
	/// </summary>
	[RequireComponent( typeof( HorizontalLayoutGroup ), typeof( CanvasGroupFader ) )]
	[ExecuteInEditMode]
	public class KaraokeManager : MonoBehaviour 
    {
		[Tooltip("The UI Text prefab that will represent each word")]
		public GameObject textPrefab;
		[Multiline]
		public string text;
		[ReadOnlyAttribute(1)]
		[Tooltip("If this is typed into the -text- then a pause will be made for the specified amount of seconds. This also " +
		         "forces the sentence to end. NOTE: This must follow the specified convention <p= /floatValue/>")]
			public string pauseIdentifier = "<p= 2.5>";
		[ReadOnlyAttribute(1)]
		[Tooltip("If this is typed into the -text- then current sentence will be forced to end no matter how many characters " +
		         "are being shown")]
			public string forceEndIdentifier = "<f>";
		[Tooltip("The maximum characters that each sentence/line can have")]
		public int charactersLimit = 20;
		/// <summary>
		/// If true, the last word will be included even if its length causes the charactersLimit to be exceeded.
		/// </summary>
		[Tooltip("If true, the last word will be included even if its length causes the charactersLimit to be exceeded")]
		[Indent(1)]
			public bool overflow = true;
		public bool disableWhenDone = true;
		[Header("Copy To Each Word")]
		[Tooltip("This will be copied to the specified /textPrefab/'s instances")]
		public GraphicEnabledAnimsController toNormalWords;
		[Header("Events")]
		public EnableDisableState onDone;


		protected int _currentWord;
		protected List<string> _words;
		protected CanvasGroupFader _fader;
        protected WaitForSeconds _waitForFader;
		
		
		// Use this for initialization
	    protected void OnEnable () 
        {
			gameObject.AddGetComponent<CanvasGroup>();
			if( !toNormalWords )
			{
				var animController = gameObject.AddGetComponent<GraphicEnabledAnimsController>();
				if( !animController )
				{
					Debug.LogWarning( "Make sure you add a Graphic (Image, Text...) component to this object" );
					return;
				}
				animController.enabled = true;
				animController.enableOnNextSiblingAfterDone = true;
				toNormalWords = GetComponent<GraphicEnabledAnimsController>();
			}

			if( Application.isPlaying )
			{
				if( !textPrefab )
				{
					Debug.LogWarning("Text Prefab hasn't been set, disabling component");
					enabled = false;
					return;
				}
				else if( !textPrefab.GetComponent<Text>() )
				{
					Debug.LogWarning("Text Prefab has no Text component, disabling component");
					enabled = false;
					return;
				}

				_fader = GetComponent<CanvasGroupFader>();
				_fader.enabled = false;
                _waitForFader = new WaitForSeconds( _fader.duration );
				_words = text.GetWords().ToList();
				_currentWord = 0;
				
				StartCoroutine( _Begin() );
			}
		}

		
        /// <summary>
        /// Starts the karaoke.
        /// </summary>
		protected IEnumerator _Begin()
		{
			//SPAWN EACH WORD UNTIL LIMIT IS REACHED
			while( _currentWord < _words.Count )
			{
				int currentCharsCount = 0;
				bool overflowed = false;
				bool nextSentence = false;
				bool forced = false;//true if a word equals the force end identifier
				bool paused = false;//true if a word equals the pause identifier
				int wordsForThisSentence = 0;
                bool isFirstWord;//caching
                GameObject word;//caching
				while( true )//Spawn Sentence
				{
					if( currentCharsCount + _words[ _currentWord ].Length > charactersLimit )//WILL OVERFLOW WITH NEXT WORD
					{
						if( currentCharsCount == charactersLimit || ( overflowed ^ !overflow ) )
						{
							//WHEN LAST WORD ANIMATION ENDS, NEXT SENTENCE WILL BEGIN
                            _LastWordEnded( () => nextSentence = true );

							break;
						}
						overflowed = true;
					}
					//CHECK SPECIAL IDENTIFIERS
					if( _words[ _currentWord ].Equals( pauseIdentifier.Substring( 0, 3 ) ) )
					{
						paused = true;
						break;
					}
					else if ( _words[ _currentWord ].Equals( forceEndIdentifier ) )
					{
						forced = true;
						break;
					}

					word = textPrefab.SetActiveInHierarchy();//SPAWN
					word.SetActive( false );//Prevent undesired behaviour when copying the GraphicEnableanimController (toNormalWords)
					word.SetParent( transform );//ADD TO LAYOUT
					word.GetComponent<Text>().text = word.name = _words[ _currentWord ];//SET WORD'S TEXT

					isFirstWord = currentCharsCount == 0;
					toNormalWords.CopyTo( word, isFirstWord );
					//INCREMENTS
					currentCharsCount += _words[ _currentWord++ ].Length;
					wordsForThisSentence++;

					if( _currentWord >= _words.Count )
					{
						if( _words.Count == 1 )//WHEN ONLY WORD ANIMATION ENDS, NEXT SENTENCE WILL BEGIN
						{
							var onlyWord = gameObject.GetChild( 0 ).GetComponent<GraphicEnabledAnimsController>();
							onlyWord.onAnimationEnd += () => nextSentence = true;
						}
						else //WHEN LAST WORD ANIMATION ENDS, NEXT SENTENCE WILL BEGIN
						{
                            _LastWordEnded( () => nextSentence = true );
						}
						break;
					}
				}
				//FADE IN
				_fader.enabled = true;
                yield return _waitForFader;
				//START KARAOKE
				//Each word will enable the next, on animation end. Activate word after disabling GraphicEnableAnimController to prevent undesired behaviour
				gameObject.GetChildren(true, true).SetActiveInHierarchy();
				//FORCED ?
				if( _currentWord < _words.Count && forced )
				{
					_words.RemoveAt( _currentWord );
					yield return new WaitForSeconds( toNormalWords.animDuration * wordsForThisSentence + 0.5f );
				}
				//PAUSED ?
				else if( _currentWord < _words.Count && paused )
				{
					_words.RemoveAt( _currentWord );
					float pause = float.Parse( _words[ _currentWord ].Substring( 0, _words[ _currentWord ].Length - 1 ) );
					_words.RemoveAt( _currentWord );
					yield return new WaitForSeconds( pause );
				}
				else while( !nextSentence )//WAIT FOR LAST WORD'S ANIMATION TO END
				{
					yield return null;
				}
				//FADE OUT
				_fader.enabled = false;
                yield return _waitForFader;
				gameObject.DestroyChildren();
			}
			onDone.SetStates();
			if( disableWhenDone )
			{
				enabled = false;
			}
		}
        /// <summary>
        /// Invoked when the last word in a karaoke sentence is reached.
        /// </summary>
        protected void _LastWordEnded( System.Action action )
        {
            var lastWord = gameObject.GetChild( gameObject.ChildCount() - 1 ).GetComponent<GraphicEnabledAnimsController>();
            lastWord.onAnimationEnd += () => action();
        }
	}
}
