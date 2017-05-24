//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using DDK.Base.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;


namespace DDK {

	public static class AssetsCustomContexts {
		
		#region FONT
		[MenuItem ("Assets/Font/Apply to All Active -Text- in Hierarchy")]
		static void Font_ApplyToAllInH() {
			
			Font font = Selection.activeObject as Font;
			if( font != null )
			{
				ApplyToAllInH( font );
			}
		}
		[MenuItem ("Assets/Font/Apply to All Active -Text- in Hierarchy", true)]
		static bool Font_ApplyToAllInH_Val() {
			
			return Selection.activeObject.GetType() == typeof(Font);
		}
		#endregion

		#region MISC
		[MenuItem("Assets/Misc/Clean Cache")]
		public static void CleanCache()
		{
			Caching.CleanCache ();
			Debug.Log ("Cache has been cleaned!");
		}
		[MenuItem ("Assets/Misc/Select All Active GameObjects In Hierarchy Containing This Script")]
		static void SelectAllWithThisScript() {

			MonoScript script = Selection.activeObject as MonoScript;

			if( script != null )
			{
				_SelectAllWithThisScript( script );
			}
		}
		[MenuItem ("Assets/Misc/Log Total Clips Length")]
		static void LogClipsLength() {
			
			Object[] clips = Selection.objects;
			float total = 0f;
			foreach( var clip in clips )
			{
				total += ( clip as AudioClip ).length;
			}
			Debug.Log ( total );
		}
		[MenuItem ("Assets/Play Clip &p")]
		static void PlayClip() {
			
			AudioClip clip = Selection.activeObject as AudioClip;

			if( AudioUtility.IsClipPlaying( clip ) )
			{
				AudioUtility.StopClip( clip );
			}
			else AudioUtility.PlayClip( clip );
		}
        [MenuItem ("Assets/Misc/Enable Script(s)")]
        static void EnableScripts() {

            Object[] scripts = Selection.objects;
            ToggleScripts( scripts, true );
        }
        [MenuItem ("Assets/Misc/Disable Script(s)")]
        static void DisableScripts() {

            Object[] scripts = Selection.objects;
            ToggleScripts( scripts, false );
        }


		//VALIDATIONS
		[MenuItem ("Assets/Select All Active GameObjects In Hierarchy Containing This Script", true)]
		static bool IsScriptSelected_Val() {

			return Selection.activeObject.GetType() == typeof(MonoScript);
		}
        [MenuItem ("Assets/Misc/Enable Script(s)", true)]
		static bool AreDisabledScriptsSelected_Val() {
			
			return IsSelectionADisabledScript();
		}
        [MenuItem ("Assets/Misc/Disable Script(s)", true)]
        static bool AreEnabledScriptsSelected_Val() {

            return IsSelectionAnEnabledScript();
        }
		[MenuItem ("Assets/Misc/Log Total Clips Length", true)]
		static bool LogClipsLength_Val() {

			Object[] clips = Selection.objects;
			foreach( var clip in clips )
			{
				if( !(clip is AudioClip) )
				{
					return false;
				}
			}
			return true;
			//return Selection.activeObject.GetType() == typeof(MonoScript);
		}
		[MenuItem ("Assets/Play Clip &p", true)]
		static bool PlayClip_Val() {
			
			AudioClip clip = Selection.activeObject as AudioClip;
			if( !clip )
				return false;
			return true;
		}
		#endregion
				

		const string DISABLED_CS = ".disabled_CS";
		const string DISABLED_JS = ".disabled_JS";
		const string CS = ".cs";
		const string JS = ".js";
		const string META = ".meta";


        /// <summary>
        /// Returns false if any of the selected objects is not a script.
        /// </summary>
        public static bool IsSelectionAScript() {

            Object[] selection = Selection.objects;
            if( selection != null && selection.Length > 1 )
            {
                for( int i=0; i<selection.Length; i++ )
                {
                    if( selection[i].GetType() != typeof(MonoScript) )
                    {
						string path = AssetDatabase.GetAssetPath( selection[i] );
						if( !path.EndsWith( DISABLED_CS ) && !path.EndsWith( DISABLED_JS ) )
						{
							return false;
						}
                    }
                }
                return true;
            }
            return Selection.activeObject.GetType() == typeof(MonoScript);
        }
		/// <summary>
		/// Returns false if any of the selected objects is not a script.
		/// </summary>
		public static bool IsSelectionAnEnabledScript() {
			
			Object[] selection = Selection.objects;
			if( selection != null && selection.Length > 1 )
			{
				for( int i=0; i<selection.Length; i++ )
				{
					if( selection[i].GetType() != typeof(MonoScript) )
					{
						return false;
					}
				}
				return true;
			}

			//Added by Germain, this case is never considered and is needed to avoid error building assetbundles
			if ( selection == null || selection.Length == 1 ){
				return false;
			}

            if( !Selection.activeObject )
                return false;
			return Selection.activeObject.GetType() == typeof(MonoScript);
		}
		/// <summary>
		/// Returns false if any of the selected objects is not a disabled script.
		/// </summary>
		public static bool IsSelectionADisabledScript() {
			
			Object[] selection = Selection.objects;
			if( selection != null && selection.Length > 1 )
			{
				for( int i=0; i<selection.Length; i++ )
				{
					string path = AssetDatabase.GetAssetPath( selection[i] );
					if( !path.EndsWith( DISABLED_CS ) && !path.EndsWith( DISABLED_JS ) 
					   && selection[i].GetType() == typeof(MonoScript) )
					{
						return false;
					}
				}
			}
			return true;
		}
        public static void ToggleScripts ( Object[] scripts, bool enable )  
        {       
            if( scripts == null )
                return;
            for( int i=0; i<scripts.Length; i++ )
            {
				string path = AssetDatabase.GetAssetPath( scripts[i] );
				string newPath = null;
				if( enable )
                {
					if( path.EndsWith( DISABLED_CS ) )
					{
						newPath = AssetDatabase.MoveAsset( path, path.RemoveFileExtension() + CS );
					}
					else if( path.EndsWith( DISABLED_JS ) )
					{
						newPath = AssetDatabase.MoveAsset( path, path.RemoveFileExtension() + JS );
					}
                }
                else if( !enable )
                {
					if( path.EndsWith( CS ) )
					{
						newPath = AssetDatabase.MoveAsset( path, path.RemoveFileExtension() + DISABLED_CS );
					}
					else if( path.EndsWith( JS ) )
					{
						newPath = AssetDatabase.MoveAsset( path, path.RemoveFileExtension() + DISABLED_JS );
					}
                }
				if( string.IsNullOrEmpty( newPath ) )
					continue;
				System.IO.File.Move( path, newPath );
            }
            Debug.Log ( scripts.Length + " Scripts have been " + (enable ? "ENABLED" : "DISABLED") );
        }

		

		static void ApplyToAllInH ( Font font )	
		{		
			Text[] objs = GameObject.FindObjectsOfType<Text>();
			if( objs != null )
			{
				objs.SetFont( font );
				int textCount = 0;
				for( int i=0; i<objs.Length; i++ )
				{
					EditorUtility.SetDirty(objs[i]);
					textCount++;
				}
				Debug.Log ( "Font ("+font.name+") applied to "+textCount+" active texts in hierarchy." );
			}		
		}


		static void _SelectAllWithThisScript ( MonoScript script )	
		{		
			GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
			List<Object> selection = new List<Object>();
			for( int i=0; i<objs.Length; i++ )
			{
				if( objs[i].GetComponent( script.GetClass() ) )
				{
					selection.Add( objs[i] as Object );
				}
			}
			if( objs != null )
			{
				Selection.objects = selection.ToArray();
				Debug.Log ( selection.Count+" Objects have been selected in the Hierarchy" );
			}		
		}
		
		
		
		/*static Object[] GetSelectedAudioclips()
		
	{
		
		return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		
	}*/
	}

}