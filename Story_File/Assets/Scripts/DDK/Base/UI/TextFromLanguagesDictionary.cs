//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.ScriptableObjects;
using UnityEngine.UI;
using DDK.Base.Classes;


namespace DDK.Base.UI
{
	/// <summary>
	/// Attach to a UIText object to allow setting its /text/ value from a StringsObj index's value.
	/// </summary>
	[ExecuteInEditMode]
	public class TextFromLanguagesDictionary : MonoBehaviourExt 
    {
		[System.Serializable]
		public class FontSizeOverrider
		{
			public SystemLanguage language;
			[Tooltip("If higher than 0, it will override the /text/'s font size for the specified language")]
			public int fontSize = 0;
		}


		#region INSPECTOR
		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string helpMsg = "SetTextFromCurrentLanguage() is called on OnEnable()";
		#endif
		public float delay;
        [Tooltip("If no Text is referenced, the component will be searched in this object and its children")]
        public SearchableGraphic text = new SearchableGraphic();
        public LanguagesDictionaries languagesDictionaries;

        [NotLessThan( 0 )]
        public int index;

        [ShowIfAttribute( "_controlledByAnimatorParam", 1 )]
        [Tooltip("The name of an integer animator's param that will control this value. The animator must be on this object or any of its parents")]
        public string paramName;
        [ShowIfAttribute( "_controlledByAnimatorParam", 1 )]
        [Tooltip("If below 0, the animator's param must be checked manually by calling UpdateIndexWithAnimatorParam()")]
        public float paramCheckRate = -1f;

        [ShowIfAttribute( 0, int.MaxValue, "_controlledBySiblingIndex", 1 )]
        [Tooltip("A value of 1 will represent the direct parent, 2 the grandparent, and so on...")]
        public int ancestorOffset = 0;
        [ShowIfAttribute( "_controlledBySiblingIndex", 1 )]
        [Tooltip("This value will be added to the specified sibling's sibling index's value before setting the /index/'s value")]
        public int siblingOffset;

        public FontSizeOverrider[] fontSizeOverriders = new FontSizeOverrider[0];

        [EditorButtonAttribute]
        public void UpdateText()
        {
            #if UNITY_EDITOR
            Awake ();
            if( !_Text )
                return;
            //UPDATE
            _Text.enabled = !_Text.enabled;
            _Text.enabled = !_Text.enabled;
            #endif
            OnEnable();
        }
        [ContextMenu("Toggle: Index Controlled By Animator Param")]
        public void ToggleAnimatorParam()
        {
            if( _controlledBySiblingIndex )
            {
                Debug.LogError( "The index is already being controlled by the sibling index" );
                return;
            }
            _controlledByAnimatorParam = !_controlledByAnimatorParam;
        }
        [ContextMenu("Toggle: Index Controlled By Sibling Index")]
        public void ToggleSiblingIndex()
        {
            if( _controlledByAnimatorParam )
            {
                Debug.LogError( "The index is already being controlled by an Animator's Parameter" );
                return;
            }
            _controlledBySiblingIndex = !_controlledBySiblingIndex;
        }
        #endregion


		public Text _Text { get; protected set; }
        [HideInInspector]
        public bool _controlledByAnimatorParam;
        [HideInInspector]
        public bool _controlledBySiblingIndex;
        protected Animator _animator;
		[HideInInspector]
		[SerializeField]
		protected int _originalFontSize;
		protected bool _isFontSizeOverridden;


		void Awake()
		{
			if( !text.m_object || !(text.m_object is Text) )
			{
				text.m_object = GetComponentInChildren<Text>();
			}
			_Text = (Text) text.m_object;
			if( !_Text )
			{
				return;
			}
			UpdateOriginalFontSize();
			_SetFontSizeFromOverrider();
		}
        void OnEnable()
        {
			if( delay > 0f )
				Invoke( "SetTextFromCurrentLanguage", delay );
			else SetTextFromCurrentLanguage();
        }
		// Use this for initialization
		void Start () 
        {
            _animator = gameObject.GetCompInParent<Animator>();
            if( _controlledByAnimatorParam && !string.IsNullOrEmpty( paramName ) )
            {
                if( paramCheckRate >= 0f && _animator && _animator.HasParam( paramName ) )
                {
                    Invoke( "UpdateIndexWithAnimatorParam", paramCheckRate );
                }
            }
            else if( _controlledBySiblingIndex )
            {
                UpdateIndexWithSiblingIndex( ancestorOffset, siblingOffset );
            }
		}
		//KEEP ALL ACTIVE INSTANCES OF THIS CLASS WITH THE SAME REFERENCE OF THE LANGUAGES DICTIONARIES
#if UNITY_EDITOR
		void Update()
		{
            if( Application.isPlaying )
				return;
            if( languagesDictionaries == null )
            {
                var activeDictionaries = FindObjectsOfType<TextFromLanguagesDictionary>();
                foreach( var dic in activeDictionaries )
                {
                    if( dic == this || dic.languagesDictionaries == null )
                        continue;
                    languagesDictionaries = dic.languagesDictionaries;
                    break;
                }   
            }			
            if( _controlledBySiblingIndex )
                UpdateIndexWithSiblingIndex( ancestorOffset, siblingOffset );
		}
#endif


		/// <summary>
		/// Sets the text from current language.
		/// </summary>
		public void SetTextFromCurrentLanguage()
		{
			SetTextFromCurrentLanguage( index );
		}
		/// <summary>
		/// Sets the text from current language.
		/// </summary>
		/// <param name="textIndex">The LanguageDictionary's Texts array's index to get the text from.</param>
		public void SetTextFromCurrentLanguage( int textIndex )
		{
			if( !languagesDictionaries )
				return;
			LanguageDictionary currentDic = languagesDictionaries.GetCurrentLanguageDictionary();
			if( !currentDic || !_Text )
				return;
			_Text.text = currentDic.GetTextAt( textIndex );
		}
        /// <summary>
        /// Updates the index with animator's parameter value.
        /// </summary>
        public void UpdateIndexWithAnimatorParam()
        {
            index = _animator.GetInteger( paramName );
            SetTextFromCurrentLanguage ( index );
        }
        /// <summary>
        /// Updates the index with the sibling index value.
        /// </summary>
        /// <param name="ancestorOffset">Cero is this gameObject, 1 is the parent, and so on...</param>
        public void UpdateIndexWithSiblingIndex( int ancestorOffset, int siblingIndexOffset )
        {
            GameObject ancestor = gameObject.GetAncestor( (byte)ancestorOffset );
            if( !ancestor )
                return;
            index = Mathf.Clamp( ancestor.transform.GetSiblingIndex() + siblingIndexOffset, 0, int.MaxValue );
        }
		public void SetFontSize( int size )
		{
			if( !_Text )
				return;
			if( _Text.resizeTextForBestFit )
			{
				_Text.resizeTextMaxSize = size;
			}
			else _Text.fontSize = size;
		}
        /// <summary>
        /// Updates the /_originalFontSize/'s value. This is called on Awake()
        /// </summary>
		public void UpdateOriginalFontSize()
		{
			if( !_Text )
				return;
			if( _Text.resizeTextForBestFit )
			{
				if( !_isFontSizeOverridden )
					_originalFontSize = _Text.resizeTextMaxSize;
			}
			else if( !_isFontSizeOverridden ) _originalFontSize = _Text.fontSize;
		}
        /// <summary>
        /// Resets the font size to the original size (the it had before this component was added).
        /// </summary>
		public void SetFontSizeToOriginal()
		{
			SetFontSize( _originalFontSize );
			_isFontSizeOverridden = false;
		}


        /// <summary>
        /// Sets the font size from the overrider if any.
        /// </summary>
		void _SetFontSizeFromOverrider()
		{
			for( int i=0; i<fontSizeOverriders.Length; i++ )
			{
				if( fontSizeOverriders[i].language == languagesDictionaries.GetCurrentLanguage() && fontSizeOverriders[i].fontSize > 0 )
				{
					SetFontSize( fontSizeOverriders[i].fontSize );
					_isFontSizeOverridden = true;
					return;
				}
			}
			SetFontSizeToOriginal();
		}
	}
}
