//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.UI;
using DDK.Base.Statics;


namespace DDK.Base.ScriptableObjects {

	/// <summary>
	/// This should be used to ease the process of creating multi-language apps.
	/// </summary>
	[CreateAssetMenu( fileName = "LanguagesDictionaries", menuName = "Scriptable Objects/Language Dictionary List")] 
	public class LanguagesDictionaries : ScriptableObject {

		[NotLessThan( -1, "If higher or equal to 0 the selected language will be overridden" )]
        [SerializeField]
        protected int _overrideLanguageIndex = -1;
		[ShowIfAttribute( "_OverrideLanguage", 1 )]
		[Tooltip("If true, all the active Text components in the scene will be automatically updated to the overridden language")]
		public bool autoUpdateAllActiveTextsInScene = true;
		/*[InspectorButtonAttribute( "UpdateAllActiveTextsInScene", true, "_OverrideLanguage" )]
		public bool updateAllActiveTextsInScene;*/
		public int defaultLanguageIndex = 0;
		public LanguageDictionary[] dictionaries;		
		

		#if UNITY_EDITOR
		protected bool _OverrideLanguage()
		{
			bool overriding = _overrideLanguageIndex >= 0;
			if( overriding )
				_UpdateAllActiveTextsInScene();
			return overriding;
		}            
		#endif

        public int OverrideLanguageIndex 
		{
            get
			{
                if( PlayerPrefs.HasKey( Constants.LANGUAGE_OVERRIDE ) )
                    _overrideLanguageIndex = PlayerPrefs.GetInt( Constants.LANGUAGE_OVERRIDE );
                return _overrideLanguageIndex;
            }
        }
		/// <summary>
		/// The dictionaries count. Each should be a different language.
		/// </summary>
        public int Length 
		{
            get{ return dictionaries.Length; }
        }


        protected void _UpdateAllActiveTextsInScene()
        {
            var texts = GameObject.FindObjectsOfType<TextFromLanguagesDictionary>();
            if( texts == null )
                return;
            for( int i=0; i<texts.Length; i++ )
            {
                texts[i].UpdateText();
            }
        }



        /// <summary>
        /// Sets the override language and updates all text objects in scene holding a TextFromLanguagesDictionary component.
        /// </summary>
        public void SetOverrideLanguage( int index )
        {
            _overrideLanguageIndex = index.Clamp( -1, dictionaries.Length );

            PlayerPrefs.SetInt( Constants.LANGUAGE_OVERRIDE, _overrideLanguageIndex );
            PlayerPrefs.Save();

            _UpdateAllActiveTextsInScene();
        }
		public LanguageDictionary GetCurrentLanguageDictionary()
		{
            if( OverrideLanguageIndex >= 0f && OverrideLanguageIndex < dictionaries.Length )
			{
                return dictionaries[ OverrideLanguageIndex ];
			}
			return dictionaries.GetCurrentLanguageDictionary( defaultLanguageIndex );
		}
		public LanguageDictionary GetLanguageDictionary( SystemLanguage language )
		{
			if( OverrideLanguageIndex >= 0f && OverrideLanguageIndex < dictionaries.Length )
			{
				return dictionaries[ OverrideLanguageIndex ];
			}
			return dictionaries.GetLanguageDictionary( language, defaultLanguageIndex );
		}
		public SystemLanguage GetCurrentLanguage()
		{
            if( OverrideLanguageIndex >= 0f && OverrideLanguageIndex < dictionaries.Length )
			{
                return dictionaries[ OverrideLanguageIndex ].language;
			}
			return dictionaries.GetCurrentLanguageDictionary( defaultLanguageIndex ).language;
		}
    }    
}
