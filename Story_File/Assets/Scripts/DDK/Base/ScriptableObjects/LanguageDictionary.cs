//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.IO;
using DDK.Base.Extensions;
using DDK.Base.Statics;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DDK.Base.ScriptableObjects
{
    /// <summary>
    /// This should be used to ease the process of creating multi-language apps.
    /// </summary>
    /// <see cref="LanguagesDictionaries"/>
	[CreateAssetMenu( fileName = "LanguageDictionary", menuName = "Scriptable Objects/Language Dictionary")] 
    public class LanguageDictionary : ScriptableObject
    {
        #if UNITY_EDITOR && UNITY_5_3_OR_NEWER
        [InspectorButtonAttribute ("_ExportJSON", true, "_IsArrayValid")]
        public bool exportJSON;
        [InspectorButtonAttribute ("_ImportJSON", true)]
        public bool importJSON;
        #endif
        public SystemLanguage language = SystemLanguage.English; //https://docs.unity3d.com/ScriptReference/SystemLanguage.html
        //public string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		[InspectorButtonAttribute ("_OrderByAscending", true)]
		public bool orderByAscending;
		[InspectorButtonAttribute ("_OrderByDescending", true)]
		public bool orderByDescending;
        [TextArea]
        public string[] texts;


		/// <summary>
		/// Gets the /texts/ string in specified index.
		/// </summary>
        public string this[ int i ]
        {
            get{ return texts[ i ]; }
            set{ texts[i] = value; }
        }


        protected bool _IsArrayValid ()
        {
            if (texts == null)
                return false;
            return texts.Length > 0;
        }

        public int Length 
		{
            get{ return texts.Length; }
        }
		/// <summary>
		/// Gets the specified language's two letter country code. Eg: Englush = en, Spanish = es...
		/// </summary>
		public string LanguageCode
		{
			get{ return Utilities.GetCountryCodeFor( language ); }
		}


        #if UNITY_EDITOR
        #if UNITY_5_3_OR_NEWER
        protected void _ExportJSON ()
        {
            Utilities.ExportJSON( language.ToString(), this, true );
        }

        protected void _ImportJSON ()
        {
            Utilities.ImportJSON( this );
			EditorUtility.SetDirty( this );
        }
		protected void _OrderByAscending()
		{
			texts = texts.OrderByAscending();
			EditorUtility.SetDirty( this );
		}
		protected void _OrderByDescending()
		{
			texts = texts.OrderByDescending();
			EditorUtility.SetDirty( this );
		}
        #endif
        #endif
            

        public string GetTextAt (int index)
        {
            if (texts == null || texts.Length == 0) 
			{
                Utilities.LogWarning ("This /texts/ array is null or empty. Returning null");
                return null;
            }
            if (texts.Length <= index) 
			{
                Utilities.LogWarning ("The specified index exceeds the /texts/ array's length. Returning text at index [0]");
                return texts [0];
            }
            return texts [index];
        }
    }

}
