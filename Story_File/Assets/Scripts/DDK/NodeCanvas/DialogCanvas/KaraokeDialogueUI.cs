//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using DDK.Base.Events;
using DDK.Base.Extensions;
using DDK.Base.Managers;
using MovementEffects;


#if USE_NODE_CANVAS
using NodeCanvas.DialogueTrees;
#endif


namespace DDK.NodeCanvas.DialogCanvas 
{
	/// <summary>
	/// * Multiple lines. The line should end, and when the karaoke effect reaches the end the text should fade out and the next line should fade in.
	/// * Audio per line. If null, no audio plays in that line.
	/// * The canvas should fade in/out.
	/// * 
    /// </summary>
	public class KaraokeDialogueUI : MonoBehaviour 
    {
		#if USE_NODE_CANVAS
		[System.Serializable]
		public class SubtitleDelays
		{
			public float characterDelay = 0.05f;
			[Tooltip("This gets added if a '.', '!', or '?' is found in the speech text")]
			public float sentenceDelay  = 0.5f;
			public float commaDelay     = 0.1f;
			public float finalDelay     = 1.2f;
		}


		//Options...
		[Header("Options")]
		public bool skipOnInput;
		public UnityEvent onSkipOnInput;
		public bool waitForInput;
		[Tooltip("Valid only if the subtitles or the options have a CanvasGroup component")]
		public float fadeDuration = 0.3f;

		//Group...
		[Header("Subtitles Group")]
		public RectTransform subtitlesGroup;
		public Text actorSpeech;
		public Text actorName;
		public string[] boysNames = new string[0];
		public Sprite boyNamePlate;
		public string[] girlsNames = new string[0];
		public Sprite girlNamePlate;
		public Image actorPortrait;
		[Tooltip("This just applies when there is no audio in the statement")]
		public SubtitleDelays subtitleDelays = new SubtitleDelays();

		//Karaoke..
		[Header("Karaoke Options")]
		[Tooltip("The amount of time it takes for a line to fade in or out")]
		public float lineFadeDuration = 0.15f;
		[Tooltip("The font color will be animated to this value when highlighted, and then back to the original")]
		public Color highlightedSpeechColor = Color.red;
		/*[Tooltip("The font size will be multiplied by this value when highlighted, and then it will be set back the original")]
		public float highlightedSpeechFontScale = 1.2f; THIS IS BEEN CONTROLLED BY THE ACTOR SPEECH'S MIN AND MAX FONT SIZE*/
		public float highlightDuration = 0.2f;

		//Group...
		[Header("Options Group")]
		public RectTransform optionsGroup;
		public Button optionButton;
		private Dictionary<Button, int> cachedButtons;
		private Vector2 _originalSubsPosition;
		private bool _isWaitingChoice;

		private AudioSource _mainAudioSource;
        private AudioSource _MainAudioSource{
			get {
                if( !_mainAudioSource )
                    _mainAudioSource = SfxManager.GetSource( "Voices" );
                return _mainAudioSource;
            }
		}

		protected CanvasGroup _subtitlesCanvasGroup;
		protected CanvasGroup _optionsCanvasGroup;
		protected bool _IsKaraokeAnimating;

		public static KaraokeDialogueUI Instance;


		void Awake()
        {
			if( Instance )
			{
				Debug.LogWarning( "There MUST only be a single instance of this class. Destroying the other one..", gameObject );
				DestroyImmediate( gameObject );
			}
			Instance = this;
			DialogueTree.OnDialogueStarted       += OnDialogueStarted;
			DialogueTree.OnDialoguePaused        += OnDialoguePaused;
			DialogueTree.OnDialogueFinished      += OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest      += OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
		}

		void Start()
        {
			//GET CANVAS GROUPS
			_subtitlesCanvasGroup = subtitlesGroup.GetComponentInChildren<CanvasGroup>();
			_optionsCanvasGroup = optionsGroup.GetComponentInChildren<CanvasGroup>();
			if( !_subtitlesCanvasGroup )
				_subtitlesCanvasGroup = GetComponent<CanvasGroup>();
			if( !_optionsCanvasGroup )
				_optionsCanvasGroup = GetComponent<CanvasGroup>();

			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
			optionButton.gameObject.SetActive(false);
			_originalSubsPosition = subtitlesGroup.transform.position;
		}

		void OnDialogueStarted( DialogueTree dlg )
        {
			//nothing special...
		}
		void OnDialoguePaused( DialogueTree dlg )
        {
			ShowSubtitlesAndOptions( false );
		}
		void OnDialogueFinished( DialogueTree dlg )
        {
			ShowSubtitlesAndOptions( false );
		}
		void OnSubtitlesRequest( SubtitlesRequestInfo info )
        {
            Internal_OnSubtitlesRequestInfo( info ).Run();
		}


		public void ShowSubtitles( bool show )
		{
			if( this == null || !subtitlesGroup )
				return;
			if( !_subtitlesCanvasGroup )
			{
				subtitlesGroup.gameObject.SetActive( show );
				return;
			}
			float targetAlpha = show ? 1f : 0f;
			if( _subtitlesCanvasGroup.alpha != targetAlpha )
				_subtitlesCanvasGroup.AlphaTo( targetAlpha, fadeDuration ).Start();
		}
		public void ShowOptions( bool show )
		{
			if( this == null || !optionsGroup )
				return;
			if( !_optionsCanvasGroup )
			{
				optionsGroup.gameObject.SetActive( show );
				return;
			}
			float targetAlpha = show ? 1f : 0f;
			if( _optionsCanvasGroup.alpha != targetAlpha )
				_optionsCanvasGroup.AlphaTo( targetAlpha, fadeDuration ).Start();
		}
		public void ShowSubtitlesAndOptions( bool show )
		{
			if( this == null || !subtitlesGroup )
				return;
			ShowSubtitles( show );
			if( !optionsGroup )
				return;
			ShowOptions( show );
		}


        IEnumerator<float> Internal_OnSubtitlesRequestInfo( SubtitlesRequestInfo info )
		{
			var text = info.statement.text;
			var audio = info.statement.audio;
			var actor = info.actor;
			//Show dialogue box with no text
			subtitlesGroup.gameObject.SetActive(true);
			actorSpeech.text = "";

			actorName.text = actor.name;
			actorSpeech.color = actor.dialogueColor;
			actorSpeech.SetAlpha( 0f );

			actorPortrait.gameObject.SetActive( actor.portraitSprite != null );
			actorPortrait.sprite = actor.portraitSprite;
			#region NAME PLATE'S COLOR
			//CHECK IF ´ACTOR NAME´ HAS A PARENT DIFFERENT FROM ´SUBTITLE GROUP´
			if( ( (RectTransform) actorName.transform.parent ) != subtitlesGroup )
			{
				//THE PARENT'S IMAGE IS THE NAME PLATE
				Image namePlate = actorName.GetComponentInParent<Image>();
				//DIFFERENCE IF ACTOR IS BOY OR GIRL
				if( boysNames.ContainsIgnoreCase( actor.name ) )//IT'S A BOY
				{
					namePlate.sprite = boyNamePlate;
				}
				else if( girlsNames.ContainsIgnoreCase( actor.name ) )
				{
					namePlate.sprite = girlNamePlate;
				}
			}
			#endregion

			if( audio != null )
			{
                _MainAudioSource.clip = audio;
                _MainAudioSource.Play();
				var statement = info.statement as MultiAudioStatement;
				if( statement != null )
				{
					( actor as DialogueActor ).TalkWithRate( true, statement.talkingRate, audio.length );
				}
				else ( actor as DialogueActor ).Talk( true, audio.length );
                Timing.RunCoroutine( StartKaraoke( text, actor, statement ) );

				yield return Timing.WaitForSeconds( audio.length );
				if( statement != null )
				{
                    statement.secondaryAudios.PlayAll( actor as DialogueActor ).Start();
                    yield return Timing.WaitForSeconds( statement.secondaryAudios.GetTotalDuration() );
				}
				while( _IsKaraokeAnimating )
					yield return 0f;
			}
			else
			{
                yield return Timing.WaitUntilDone( StartKaraoke( text, actor, null ).Run() );

				if ( !waitForInput )
				{
					yield return Timing.WaitForSeconds( subtitleDelays.finalDelay );
				}
			}
			if( waitForInput )
			{
				while( !Input.anyKeyDown )
				{
					yield return 0f;
				}
			}

			yield return 0f;
			//subtitlesGroup.gameObject.SetActive(false);
			info.Continue();
		}

        IEnumerator<float> CheckInput( System.Action Do )
        {
			while( !Input.anyKeyDown )
            {
				yield return 0f;
			}
			Do();
		}

        IEnumerator<float> StartKaraoke( string text, IDialogueActor actor, MultiAudioStatement statement )
		{
			string[] speechLines = text.Split( '\n' );
			bool statementAudiosMatchLines = false;
			if( statement != null )
			{
				statementAudiosMatchLines = ( statement.secondaryAudios.Count == speechLines.Length - 1 );
			}
			_IsKaraokeAnimating = true;
			for( int i=0; i<speechLines.Length; i++ )
			{
				//SET & SHOW CURRENT LINE'S TEXT
				actorSpeech.text = speechLines[ i ];
				if( i > 0 && statementAudiosMatchLines )
				{
					yield return Timing.WaitForSeconds( statement.secondaryAudios[ i-1 ].delay );
				}
                actorSpeech.AnimAlphaCo( 1f, lineFadeDuration ).Run();
				//KARAOKE ANIMATION
                yield return Timing.WaitUntilDone( actorSpeech.AnimTextWordsColor( actor.dialogueColor, 
                    highlightedSpeechColor, highlightDuration, highlightDuration * 0.5f ).Run() );
				//FADE OUT
				while( actorSpeech.color.a != 1f )
					yield return 0f;
                yield return Timing.WaitUntilDone( actorSpeech.AnimAlphaCo( 0f, lineFadeDuration ).Run() );
			}
			_IsKaraokeAnimating = false;
		}


		#region MULTIPLE CHOICE
		void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info){
			
			optionsGroup.gameObject.SetActive(true);
			var buttonHeight = optionButton.GetComponent<RectTransform>().rect.height;
			optionsGroup.sizeDelta = new Vector2(optionsGroup.sizeDelta.x, (info.options.Values.Count * buttonHeight) + 20);
			
			cachedButtons = new Dictionary<Button, int>();
			int i = 0;
			
			foreach (KeyValuePair<IStatement, int> pair in info.options){
				var btn = (Button)Instantiate(optionButton);
				btn.gameObject.SetActive(true);
				btn.transform.SetParent(optionsGroup.transform, false);
				btn.transform.localPosition = (Vector2)optionButton.transform.localPosition - new Vector2(0, buttonHeight * i);
				btn.GetComponentInChildren<Text>().text = pair.Key.text;
				cachedButtons.Add(btn, pair.Value);
				btn.onClick.AddListener( ()=> { Finalize(info, cachedButtons[btn]);	});
				i++;
			}
			
			if (info.showLastStatement){
				subtitlesGroup.gameObject.SetActive(true);
				var newY = optionsGroup.position.y + optionsGroup.sizeDelta.y + 1;
				subtitlesGroup.position = new Vector2(subtitlesGroup.position.x, newY);
			}
			
			if (info.availableTime > 0){
				StartCoroutine(CountDown(info));
			}
		}
		
		IEnumerator CountDown(MultipleChoiceRequestInfo info){
			_isWaitingChoice = true;
			var timer = 0f;
			while (timer < info.availableTime){
				if (_isWaitingChoice == false){
					yield break;
				}
				timer += Time.deltaTime;
				SetMassAlpha(optionsGroup, Mathf.Lerp(1, 0, timer/info.availableTime));
				yield return null;
			}
			
			if (_isWaitingChoice){
				Finalize(info, info.options.Values.Last());
			}
		}
		
		void Finalize(MultipleChoiceRequestInfo info, int index){
			_isWaitingChoice = false;
			SetMassAlpha(optionsGroup, 1f);
            optionsGroup.gameObject.SetActive(false);
            if (info.showLastStatement){
                subtitlesGroup.gameObject.SetActive(false);
                subtitlesGroup.transform.position = _originalSubsPosition;
            }
            foreach (var tempBtn in cachedButtons.Keys){
                Destroy(tempBtn.gameObject);
            }
            info.SelectOption(index);
        }
        
        void SetMassAlpha(RectTransform root, float alpha){
            foreach(var graphic in root.GetComponentsInChildren<CanvasRenderer>()){
                graphic.SetAlpha(alpha);
			}
		}
		#endregion

#else
		[HelpBoxAttribute()]
		public string msg = "USE_NODE_CANVAS scripting symbol must be defined";
#endif
	}
}