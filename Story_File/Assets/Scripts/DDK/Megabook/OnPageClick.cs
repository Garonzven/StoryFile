//By: Daniel Soto
//4dsoto@gmail.com
using System.Collections;
using DDK.Base.Managers;
using DDK.Base.Statics;
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.SoundFX;


#if USE_GAMES_DEVELOPMENT
using DDK.GamesDevelopment;
#endif


namespace DDK.MegaBook {
	

	[RequireComponent( typeof( BoxCollider ) )]
	public class OnPageClick : MonoBehaviour {
		
#if USE_MEGABOOK
		[Header("Clips")]
		public Sfx flipPage = new Sfx();
        public Sfx ClosePage = new Sfx();
        [Tooltip("The sounds that will play when each page is shown. The first index represents the cover")]
		public Sfx[] pages;
		[IndentAttribute(1)]
		[Tooltip("If 0, the clips won't be repeated")]
		public float repeatInterval = 10f;

        [Tooltip("If true, the first page (cover) clip will be played immediately; otherwise, it will be played after the /repeatInterval/")]
		[IndentAttribute(1)]
		public bool playFirstPageImmediately;
        [Tooltip("Delay of the actions after next page triggered")]
        [IndentAttribute(1)]
        public float delayAfterNextPage = 0;
        [Tooltip("Set if in the end of the book it is going to close from last page to front")]
        [IndentAttribute(1)]
        public bool isCloseFromBackToFront = false;
        [Tooltip("Set if theres is going to be alternate repeatInstruction with a Mouse Pointer")]
        [IndentAttribute(1)]
        public bool isRepeatWPointer = false;
        [IndentAttribute(1)]
        [Tooltip("Reference to Object with pointerAid")]
        public GameObject pointer;

        [Space(10f)]
		[Tooltip("This object will be activate when the user stays to long without changing the page.")]
		public SearchableGameObject nextPageIndicator = new SearchableGameObject();
		[ShowIfAttribute( "_IsNextPageIndicatorValid", 1 )]
		public float delay = 4f;
		[Tooltip("When this page is reached, Game.ended will be set to true and the OnBookEnd() event will be called. NOTE: The starting page will be considered to be the current page when this script" +
			"is enabled so it will always start at 0")]
		[NotLessThan(2)]
		public int lastPageIndex = 7;
#if USE_GAMES_DEVELOPMENT
		[IndentAttribute(1)]
		[Tooltip("If true, a message will be displayed in the console when the OnBookEnd() event is called")]
		public bool logBookEnded = true;
#endif

    [Header("Events")]
#if USE_GAMES_DEVELOPMENT
		[Tooltip("USE_GAMES_DEVELOPMENT scripting symbol must be defined. The Game.ended value when the book ends")]
		public bool setGameEnded = true;
#endif
		[Tooltip("If true, this gameObject will be deactivated On Book End")]
		public bool deactivateThis = true;
		[Tooltip("If true, this gameObject will be destroyed On Book End")]
		public bool destroyThis;
		[Tooltip("If true, the events will be executed after the specified delay in seconds. This helps preventing multiple issues such as using the Game.ended variable in another script" +
			"and getting an unexpected result")]
		public ComposedEvent onEnd = new ComposedEvent();


		protected bool _IsNextPageIndicatorValid()
		{
			return nextPageIndicator != null;
		}
#else
	//	[HelpBoxAttribute( MessageType.Warning )]
	//	public string msg = "USE_MEGABOOK scripting symbol hasn't been defined. See Menu Item: Custom/Scripting Symbols..";
#endif
		
		
#if USE_MEGABOOK
		private const string PLAY_PAGE = "PlayPage";
		private MegaBookBuilder _book;
		private int _startingPage = 0;
		private bool _bookWasOpened;
		private bool _lastPageReached;
        private bool _lastRepeatedInstruction;

		/// <summary>
		/// The time when a page was last flipped
		/// </summary>
		private float _lastFlip;
		internal int _currentPage {
			get {
				return (int)_book.page - _startingPage;
			}
		}
		/// <summary>
		/// The final Sfx path from the SfxManager.
		/// </summary>
		/// <value>The _sfx path.</value>
		protected string _sfxPath {
			get{
				return SfxManager.Instance.allClipsPath;
			}
		}
		
		
		
		// Use this for initialization
		protected void Start () {

            if (isRepeatWPointer)
            {
                if (pointer)
                {
                    _lastRepeatedInstruction = false;
                }
                else
                {
                    isRepeatWPointer = false;
                    Debug.Log("No pointer Assigned to OnPageClick");
                }

            }

            _book = GameObject.FindObjectOfType<MegaBookBuilder>();
			if( !_book )
			{
				Debug.LogWarning("No MegaBookbuilder was found.. Disabling component..", gameObject);
				enabled = false;
			}

			if( pages.Length == 0 )
			{
				return;
			}
			_startingPage = _currentPage;
			float firstDelay = ( playFirstPageImmediately ) ? 0f : repeatInterval;
			float clipLength = 0f;
			if (pages [_currentPage].clip != null && pages.Length > _currentPage)
				clipLength = pages [_currentPage].clip.length;
			InvokeRepeating( PLAY_PAGE, firstDelay, repeatInterval + clipLength );	
			if( playFirstPageImmediately )
			{
				GetComponent<BoxCollider>().DisableFor( clipLength );
			}

		}
		
		// Update is called once per frame
		protected void Update () {

			if( _lastFlip + delay < Time.time && !_lastPageReached )//SHOW NEXT PAGE INDICATOR
			{
				ShowNextPageIndicator();
			}
			else ShowNextPageIndicator( false );
		}

        protected void OnMouseUpAsButton()
		{
			if( !SfxManager.IsPlaying( flipPage.source ) && !SfxManager.IsAnyPlaying( pages ) && !_lastPageReached )
			{
				_lastFlip = Time.time;
				//FLIP PAGE
				CancelInvoke( PLAY_PAGE );
				if( _currentPage < lastPageIndex )
				{
					float clipLength = ( pages.Length > _currentPage ) ? SfxManager.GetClipLength( _sfxPath + pages[ _currentPage ] ) : 0f;
					InvokeRepeating( PLAY_PAGE, _book.GetTurnTime() + delayAfterNextPage, repeatInterval + clipLength );
				}
				else//LAST PAGE REACHED
				{
					CancelInvoke( PLAY_PAGE );
					_lastPageReached = true;
                    if (isCloseFromBackToFront)
                    {
                        SfxManager.PlaySfx(ClosePage, true, gameObject);
                        _book.SetTurnTime(_book.GetTurnTime()/3);
                        _book.SetPage(_startingPage, false);
                    }
					OnBookEnd();
                    return;
				}
                NextPage();
			}
		}

		/// <summary>
		/// This flips the page.
		/// </summary>
		protected void NextPage()
		{
			PlayFlipPage();
            _book.NextPage();
		}
		/// <summary>
		/// This is called when the specified last book page is reached.
		/// </summary>
		protected void OnBookEnd()
		{
#if USE_GAMES_DEVELOPMENT
			Game.ended = setGameEnded;
#endif
			onEnd.Invoke ();

			ShowNextPageIndicator( false );
			if( destroyThis )
			{
				gameObject.Destroy();
			}
			else gameObject.SetActiveInHierarchy( !deactivateThis );

			if( !logBookEnded )
				return;
			Debug.Log ("Book Ended!");
		}
		
		
		
		public void ShowNextPageIndicator( bool show = true )
		{
			if( nextPageIndicator.m_gameObject )
			{
				nextPageIndicator.m_gameObject.SetActive( show );
			}
		}
				
		
		#region PLAY SFX		
		public void PlayFlipPage()
		{
            _lastRepeatedInstruction = false;
			SfxManager.PlaySfx( flipPage, true, gameObject );
		}
		
		public void PlayPage()
		{

			if( _currentPage >= pages.Length )
				return;

            if (isRepeatWPointer)
            {
                if (_lastRepeatedInstruction)
                {
                    pointer.GetComponent<PointerAid>().StartMove();
                    _lastRepeatedInstruction = false;
                    return;
                }
                _lastRepeatedInstruction = true;
            }
            SfxManager.PlaySfx(pages[_currentPage], true, gameObject);
            
        }
        #endregion

#endif
    }
	
}
