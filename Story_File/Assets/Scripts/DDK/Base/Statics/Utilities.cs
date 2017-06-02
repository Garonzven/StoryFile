using System.Collections;

using UnityEngine;
using DDK.Base.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace DDK.Base.Statics
{
    /// <summary>
    /// Contains multiple utility functions.
    /// </summary>
    public static class Utilities
    {
		public static bool _IsTakingScreenshot { get; private set; }


		/// <summary>
		/// This yields until the specified level is loaded.
		/// </summary>
        public static IEnumerator WaitLevelLoaded ( string levelName )
        {
            #if UNITY_5_3_OR_NEWER
            while ( !SceneManager.GetActiveScene().name.Equals ( levelName ) )
            #else
            while ( !Application.loadedLevelName.Equals ( levelName ) )
            #endif
            {
                yield return null;
            }
        }
		/// <summary>
		/// Checks if a level is available (added to Build Settings and enabled). This returns true if not in the Editor.
		/// </summary>
        /// <param name="levelName"> The name of the level/scene to check </param>
		/// <param name="logMsgs">If set to <c>true</c> a message will be displayed if the scene hasn't been added
		/// to the Build Settings, or if it's disabled.</param>
		/// <param name="context"> The context object for the logged messages </param>
		public static bool IsLevelAvailable( string levelName, bool logMsgs = false, Object context = null )
		{
#if UNITY_EDITOR
			var scenes = EditorBuildSettings.scenes;
			foreach( var scene in scenes )
			{
				if( scene.path.GetLastEndPoint().RemoveFileExtension().Equals( levelName ) )
				{
					if( scene.enabled )
						return true;
					else if( logMsgs )
					{
						LogWarning ("The specified scene \"" + levelName + "\" has been added to the Build" +
							"Settings but it has been disabled", context );
						return false;
					}
				}
			}
			if( logMsgs )
			{
				LogWarning("Scene: \"" + levelName + "\" hasn't been added to the Build Settings", context );
			}
			return false;
#else
			return true;
#endif
		}
		/// <summary>
		/// Waits for real seconds, taking into account the Time.timeScale.
		/// </summary>
		public static IEnumerator WaitForRealSeconds( float seconds )
		{
			if( seconds > 0 )
				yield return new WaitForSeconds( seconds * Time.timeScale );
		}
        /// <summary>
        /// Waits for the specified amount of frames.
        /// </summary>
        public static IEnumerator<float> WaitForFrames( int frames )
        {
            while( frames > 0 )
            {
                yield return 0f;
                frames--;
            }
        }
		public static void TakeScreenshot( Texture2D screenshot )
		{
			TakeScreenshotAndWait( screenshot ).Start();
		}
		/// <summary>
		/// Takes a screenshot and waits for it to be ready. Get the screenshot from this classes /_LastScreenshot/ public variable.
		/// </summary>
		public static IEnumerator TakeScreenshotAndWait( Texture2D screenshot )
		{
			_IsTakingScreenshot = true;
			yield return null;// wait for graphics to render

			#region SCREENSHOT
			screenshot = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
			screenshot.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
			screenshot.Apply();
			#endregion

			_IsTakingScreenshot = false;
		}
		/// <summary>
		/// Log the specified message. This is the same as calling Debug.Log(message) with the difference that the 
		/// messages will only compile on debug and/or development builds/
		/// </summary>
		public static void Log( object message, Object context = null )
		{
			#if DEBUG || DEVELOPMENT_BUILD
			Debug.Log ( message, context );
			#endif
		}
		/// <summary>
		/// Log the specified message. This is the same as calling Debug.Log(message) with the difference that the 
		/// messages will only compile on debug and/or development builds/
		/// </summary>
		public static void LogWarning( object message, Object context = null )
		{
			#if DEBUG || DEVELOPMENT_BUILD
			Debug.LogWarning ( message, context );
			#endif
		}
		/// <summary>
		/// Log the specified message. This is the same as calling Debug.Log(message) with the difference that the 
		/// messages will only compile on debug and/or development builds/
		/// </summary>
		public static void LogError( object message, Object context = null )
		{
			#if DEBUG || DEVELOPMENT_BUILD
			Debug.LogError ( message, context );
			#endif
		}
        /// <summary>
        /// Log the specified message. This is the same as calling Debug.Log(message) with the difference that the 
        /// messages will only compile on debug and/or development builds/
        /// </summary>
        public static void Log( Color color, object message, Object context = null )
        {
            #if DEBUG || DEVELOPMENT_BUILD
            message = string.Format( "<color=#{0}>{1}</color>", color.ToHex(), message );
            Debug.Log ( message, context );
            #endif
        }
        /// <summary>
        /// Log the specified message. This is the same as calling Debug.Log(message) with the difference that the 
        /// messages will only compile on debug and/or development builds/
        /// </summary>
        public static void LogWarning( Color color, object message, Object context = null )
        {
            #if DEBUG || DEVELOPMENT_BUILD
            message = string.Format( "<color=#{0}>{1}</color>", color.ToHex(), message );
            Debug.LogWarning ( message, context );
            #endif
        }
        /// <summary>
        /// Log the specified message. This is the same as calling Debug.Log(message) with the difference that the 
        /// messages will only compile on debug and/or development builds/
        /// </summary>
        public static void LogError( Color color, object message, Object context = null )
        {
            #if DEBUG || DEVELOPMENT_BUILD
            message = string.Format( "<color=#{0}>{1}</color>", color.ToHex(), message );
            Debug.LogError ( message, context );
            #endif
        }
		/// <summary>
		/// Gets the country codes (two letter code). Eg: English = en, Spanish = es...
		/// <see href="https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.getcultures(v=vs.110).aspx"/>
		/// </summary>
		public static List<string> GetCountryCodes()
		{
			// Get all available cultures on the current system.
			CultureInfo[] cultures = CultureInfo.GetCultures( CultureTypes.AllCultures );
			int length = cultures.Length;
			List<string> codes = new List<string>( length );

			for( int j=0; j<length; j++ ) 
			{				
				string code =  cultures[j].Name;
				// Only all two-letter codes.
				if( code.Length != 2 )
					continue;
				codes.Add( code );
			}  
			return codes;
		}
		/// <summary>
		/// Gets the country two letter code for the specified language. Eg: English = en (default), Spanish = es...
		/// <see href="https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.getcultures(v=vs.110).aspx"/>
		/// </summary>
		public static string GetCountryCodeFor( SystemLanguage language )
		{
			// Get all available cultures on the current system.
			CultureInfo[] cultures = CultureInfo.GetCultures( CultureTypes.AllCultures );
			int length = cultures.Length;
			for( int i=0; i<length; i++ ) 
			{				
				string code =  cultures[i].Name;
				// Only two-letter codes are valid
				if( code.Length != 2 || !cultures[i].DisplayName.Contains( language.ToString() ) )
					continue;
				return code;
			}  
			return "en";
		}

        #if UNITY_EDITOR
        #if UNITY_5_3_OR_NEWER
        /// <summary>
        /// EDITOR ONLY. Show a Folder Selection Window to pick the export location and file name.
        /// </summary>
        public static void ExportJSON( string defaultFileName, object obj, bool prettyPrint )
        {
            string json = JsonUtility.ToJson (obj, prettyPrint);
            string savePath = EditorPrefs.GetString ( Application.productName + Constants.LAST_SAVE_PATH, "");
            savePath = EditorUtility.SaveFilePanel ("Export JSON..", savePath, defaultFileName, Constants.JSON);
            if (string.IsNullOrEmpty (savePath)) 
            {
                Debug.Log ("JSON Export canceled..");
                return;
            }
            File.WriteAllText (savePath, json);
            Debug.Log ( "JSON file exported at path: " + savePath );
            EditorPrefs.SetString( Application.productName + Constants.LAST_SAVE_PATH, savePath.RemoveLastEndPoint() );
        }
        /// <summary>
        /// EDITOR ONLY. Show a File Selection Window to pick the file to import.
        /// </summary>
        public static void ImportJSON( object toOverwrite )
        {
            int import = 0;//YES
            if (EditorPrefs.GetBool ( Constants.ASK_IMPORT_JSON, true)) 
            {
                import = EditorUtility.DisplayDialogComplex ("Import JSON..", "This will override this object's values. " +
                    "Are you sure you want to continue?", "Yes", "Cancel", "Yes, Don't Ask Again");
            }
            if (import == 1) //NO
            {
                return;
            }
            if (import == 2) 
            {
                EditorPrefs.SetBool ( Constants.ASK_IMPORT_JSON, false);
            }
            string filePath = EditorPrefs.GetString ( Application.productName + Constants.LAST_SAVE_PATH, "");
            filePath = EditorUtility.OpenFilePanel ("Import JSON..", filePath, Constants.JSON);
            if (string.IsNullOrEmpty (filePath)) 
            {
                Debug.Log ("JSON Import canceled..");
                return;
            }
            string json = File.ReadAllText (filePath);
            JsonUtility.FromJsonOverwrite (json, toOverwrite);
            Debug.Log ("JSON file imported from path: " + filePath);
        }
        #endif
		/// <summary>
		/// You can search for names, labels and types (classnames). 
		/// 'name': filter assets by their filename (without extension). Words separated by whitespace are treated as 
		/// separate name searches. Use quotes for grouping multiple words into a single search. 
		/// 
		/// 'labels': Use the keyword 'l'. Filtering by more than one label will return assets if just one asset label 
		/// is matched (OR'ed)
		/// 
		/// 'types': Use the keyword 't'. Filtering by more than one type will return assets if just one type is matched 
		/// (OR'ed). Types can be either builtin types e.g 'Texture2D' or user script class names. If all assets are 
		/// wanted: use 'Object' as all assets derive from Object.
		/// </summary>
		/// <see cref="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html"/>
		public static T[] LoadAssets<T>( string filter, string[] searchPaths = null ) where T : Object
		{
			List<string> paths = new List<string>();
			string[] guids = AssetDatabase.FindAssets( filter, searchPaths );
            int length = guids.Length;
            for( int i=0; i<length; i++ )
            {
				paths.Add( AssetDatabase.GUIDToAssetPath( guids[i] ) );
			}

			T[] assets = new T[ paths.Count ];
			for( int i=0; i<assets.Length; i++ )
			{
				assets[i] = AssetDatabase.LoadAssetAtPath<T>( paths[i] );
			}
			return assets;
		}
        #endif
    }
}