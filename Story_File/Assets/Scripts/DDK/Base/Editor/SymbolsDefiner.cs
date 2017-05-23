//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using DDK.Base.Extensions;


namespace DDK.Misc 
{
	/// <summary>
	/// This allows to setup the common Scripting Symbols in an Editor Window.
	/// </summary>
	public static class SymbolsDefiner 
    {
		[Flags]
		public enum Symbols 
        {
			PLAYFAB = 0x1,
			FB_MANAGER = 0x2,
			ADMOB = 0x4,
			APPLOVIN = 0x8,
			FB_ADS = 0x10,
			REVMOB = 0x20,
			IAD = 0x40,
			CHARTBOOST = 0x80,
			IOS_NATIVE = 0x100,
			DIALOGS_MANAGER = 0x200,
			ANDROID_SHARE = 0x400,
			MEGABOOK = 0x800,
			FINAL_IK = 0x1000,
			SUPRESS_ADS = 0x2000,
			ASSET_BUNDLES = 0x4000,
			PAINTING = 0x8000,
			OBJ_MANAGEMENT = 0x10000,
			GAMES_DEVELOPMENT = 0x20000,
			TWO_D = 0x40000,
			THREE_D = 0x80000,
			BEST_HTTP = 0x100000,
			NODE_CANVAS = 0x200000,
			ONE_SIGNAL = 0x400000,
            SVG_IMPORTER = 0x800000,
			ANDROID_NATIVE = 0x1000000,
		}



		public static BuildTargetGroup targetGroups = 0x0;
		public static Symbols symbols = 0x0;
		public const string BuildTargetGroups = "SCRIPTING_SYMBOLS_BUILD_TARGET_GROUPS";
		public const string ScriptingSymbols = "SCRIPTING_SYMBOLS";


		private static CustomWindow symbolsWindow;


		[MenuItem("Custom/Scripting Symbols..")]
		public static void ScriptingSymbolsMenuItem()
		{
			if( symbolsWindow )
				symbolsWindow.Focus();
			//NOTE: IF YOU CALL ScriptableObject.CreateInstance() THE WINDOW CAN'T BE RESIZEABLE
			else symbolsWindow = new CustomWindow( SymbolsOnGUI, "Symbols Definer", 250f );
		}
		[MenuItem("Custom/Preferences/Clear Scripting Symbols Prefs")]
		public static void ClearScriptingSymbolsPrefsMenuItem()
		{
			EditorPrefs.DeleteKey( BuildTargetGroups );
			EditorPrefs.DeleteKey( ScriptingSymbols );
			Debug.Log ("Scripting Symbols Preferences Cleared");
		}
		private static void SymbolsOnGUI()
		{
			if( !EditorPrefs.HasKey( BuildTargetGroups ) )//IF FIRST TIME THIS FUNCTION IS CALLED IT SHOULDN'T REMOVE THE CURRENT DEFINED SYMBOLS
            {
				targetGroups = (BuildTargetGroup) targetGroups.SetFlag( (BuildTargetGroup) (EditorUserBuildSettings.selectedBuildTargetGroup.ToInt64() << 1), true );
			}
			else targetGroups = (BuildTargetGroup) EditorPrefs.GetInt( BuildTargetGroups );
			symbols = (Symbols) symbols.SetFlags( GetCurrentBuildTargetScriptingSymbols(), true );

			symbols = (Symbols) EditorGUILayout.EnumMaskPopup( new GUIContent( "Symbols" ), symbols );
			targetGroups = (BuildTargetGroup) EditorGUILayout.EnumMaskPopup( new GUIContent( "Build Target Groups" ), targetGroups );
			var _targetGroups = (BuildTargetGroup)( targetGroups.ToInt64() >> 1 );

			#region SYMBOLS DEFINITION
			if( symbols.IsFlagged( Symbols.PLAYFAB ) )
			{
				AddSymbol( _targetGroups, Symbols.PLAYFAB );
			}
			else RemoveSymbol( _targetGroups, Symbols.PLAYFAB );
			if( symbols.IsFlagged( Symbols.FB_MANAGER ) )
			{
				AddSymbol( _targetGroups, Symbols.FB_MANAGER );
			}
			else RemoveSymbol( _targetGroups, Symbols.FB_MANAGER );
			if( symbols.IsFlagged( Symbols.ADMOB ) )
			{
				AddSymbol( _targetGroups, Symbols.ADMOB );
			}
			else RemoveSymbol( _targetGroups, Symbols.ADMOB );
			if( symbols.IsFlagged( Symbols.APPLOVIN ) )
			{
				AddSymbol( _targetGroups, Symbols.APPLOVIN );
			}
			else RemoveSymbol( _targetGroups, Symbols.APPLOVIN );
			if( symbols.IsFlagged( Symbols.FB_ADS ) )
			{
				AddSymbol( _targetGroups, Symbols.FB_ADS );
			}
			else RemoveSymbol( _targetGroups, Symbols.FB_ADS );
			if( symbols.IsFlagged( Symbols.REVMOB ) )
			{
				AddSymbol( _targetGroups, Symbols.REVMOB );
			}
			else RemoveSymbol( _targetGroups, Symbols.REVMOB );
			if( symbols.IsFlagged( Symbols.IAD ) )
			{
				AddSymbol( _targetGroups, Symbols.IAD );
			}
			else RemoveSymbol( _targetGroups, Symbols.IAD );
			if( symbols.IsFlagged( Symbols.CHARTBOOST ) )
			{
				AddSymbol( _targetGroups, Symbols.CHARTBOOST );
			}
			else RemoveSymbol( _targetGroups, Symbols.CHARTBOOST );
			if( symbols.IsFlagged( Symbols.IOS_NATIVE ) )
			{
				AddSymbol( _targetGroups, Symbols.IOS_NATIVE );
			}
			else RemoveSymbol( _targetGroups, Symbols.IOS_NATIVE );
			if( symbols.IsFlagged( Symbols.DIALOGS_MANAGER ) )
			{
				AddSymbol( _targetGroups, Symbols.DIALOGS_MANAGER );
			}
			else RemoveSymbol( _targetGroups, Symbols.DIALOGS_MANAGER );
			if( symbols.IsFlagged( Symbols.ANDROID_SHARE ) )
			{
				AddSymbol( _targetGroups, Symbols.ANDROID_SHARE );
			}
			else RemoveSymbol( _targetGroups, Symbols.ANDROID_SHARE );
			if( symbols.IsFlagged( Symbols.MEGABOOK ) )
			{
				AddSymbol( _targetGroups, Symbols.MEGABOOK );
			}
			else RemoveSymbol( _targetGroups, Symbols.MEGABOOK );
            if( symbols.IsFlagged( Symbols.FINAL_IK ) )
            {
                AddSymbol( _targetGroups, Symbols.FINAL_IK );
            }
            else RemoveSymbol( _targetGroups, Symbols.FINAL_IK );
            if( symbols.IsFlagged( Symbols.SUPRESS_ADS ) )
            {
                AddSymbol( _targetGroups, Symbols.SUPRESS_ADS );
            }
            else RemoveSymbol( _targetGroups, Symbols.SUPRESS_ADS );
			if( symbols.IsFlagged( Symbols.ASSET_BUNDLES ) )
			{
				AddSymbol( _targetGroups, Symbols.ASSET_BUNDLES );
			}
			else RemoveSymbol( _targetGroups, Symbols.ASSET_BUNDLES );
			if( symbols.IsFlagged( Symbols.PAINTING ) )
			{
				AddSymbol( _targetGroups, Symbols.PAINTING );
			}
			else RemoveSymbol( _targetGroups, Symbols.PAINTING );
			if( symbols.IsFlagged( Symbols.OBJ_MANAGEMENT ) )
			{
				AddSymbol( _targetGroups, Symbols.OBJ_MANAGEMENT );
			}
			else RemoveSymbol( _targetGroups, Symbols.OBJ_MANAGEMENT );
			if( symbols.IsFlagged( Symbols.GAMES_DEVELOPMENT ) )
			{
				AddSymbol( _targetGroups, Symbols.GAMES_DEVELOPMENT );
			}
			else RemoveSymbol( _targetGroups, Symbols.GAMES_DEVELOPMENT );
			if( symbols.IsFlagged( Symbols.TWO_D ) )
			{
				AddSymbol( _targetGroups, Symbols.TWO_D );
			}
			else RemoveSymbol( _targetGroups, Symbols.TWO_D );
			if( symbols.IsFlagged( Symbols.THREE_D ) )
			{
				AddSymbol( _targetGroups, Symbols.THREE_D );
			}
			else RemoveSymbol( _targetGroups, Symbols.THREE_D );
			if( symbols.IsFlagged( Symbols.BEST_HTTP ) )
			{
				AddSymbol( _targetGroups, Symbols.BEST_HTTP );
			}
			else RemoveSymbol( _targetGroups, Symbols.BEST_HTTP );
			if( symbols.IsFlagged( Symbols.NODE_CANVAS ) )
			{
				AddSymbol( _targetGroups, Symbols.NODE_CANVAS );
			}
			else RemoveSymbol( _targetGroups, Symbols.NODE_CANVAS );
			if( symbols.IsFlagged( Symbols.ONE_SIGNAL ) )
			{
				AddSymbol( _targetGroups, Symbols.ONE_SIGNAL );
			}
			else RemoveSymbol( _targetGroups, Symbols.ONE_SIGNAL );
            if( symbols.IsFlagged( Symbols.SVG_IMPORTER ) )
            {
                AddSymbol( _targetGroups, Symbols.SVG_IMPORTER );
            }
            else RemoveSymbol( _targetGroups, Symbols.SVG_IMPORTER );
			if( symbols.IsFlagged( Symbols.ANDROID_NATIVE ) )
			{
				AddSymbol( _targetGroups, Symbols.ANDROID_NATIVE );
			}
			else RemoveSymbol( _targetGroups, Symbols.ANDROID_NATIVE );
			#endregion

			EditorPrefs.SetInt( BuildTargetGroups, (int)targetGroups.ToInt64() );
			EditorPrefs.SetInt( ScriptingSymbols, (int)symbols.ToInt64() );
		}


		/// <summary>
		/// Returns the current Editor Scripting Symbols as they are named in the /symbols/ enum. This includes symbols 
		/// that are not in the enum.
		/// </summary>
		/// <returns>The editor scripting symbols.</returns>
		/// <param name="targetGroup">Target group.</param>
		public static string[] GetCurrentBuildTargetScriptingSymbols()
		{
			string defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup( EditorUserBuildSettings.selectedBuildTargetGroup );
			string[] symbols = defines.Split( ';' );
			symbols = symbols.Replace( "USE_", "" ).ToArray();
			return symbols;
		}
		public static void AddSymbol( UnityEditor.BuildTargetGroup[] targetGroups, string symbol )
		{
			for( byte i=0; i<targetGroups.Length; i++ )
			{
				string defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGroups[i] );
				if( !defines.Contains( symbol ) )
				{
					if( targetGroups[i] == BuildTargetGroup.Unknown )
						continue;
					UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGroups[i], defines + ";" + symbol );
					if( i == targetGroups.Length - 1 )
						Debug.Log ( symbol + " symbol defined" );
				}
			}
		}
		public static void RemoveSymbol( UnityEditor.BuildTargetGroup[] targetGroups, string symbol )
		{
			for( byte i=0; i<targetGroups.Length; i++ )
			{
				string defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGroups[i] );
				if( defines.Contains( symbol ) )
				{
					if( targetGroups[i] == BuildTargetGroup.Unknown )
                        continue;
					UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGroups[i], defines.Replace( symbol, "" ) );
					if( i == targetGroups.Length - 1 )
						Debug.Log ( symbol + " symbol removed" );
				}
			}
		}
		public static void AddSymbol( UnityEditor.BuildTargetGroup[] targetGroups, Symbols symbol )
		{
			string prefix = "USE_";
			if( symbol.ToString().StartsWith("_") )
			{
				prefix = "USE";
			}
			AddSymbol( targetGroups, prefix + symbol.ToString() );
		}
		public static void RemoveSymbol( UnityEditor.BuildTargetGroup[] targetGroups, Symbols symbol )
		{
			string prefix = "USE_";
			if( symbol.ToString().StartsWith("_") )
			{
				prefix = "USE";
			}
			RemoveSymbol( targetGroups, prefix + symbol.ToString() );
		}
		public static void AddSymbol( UnityEditor.BuildTargetGroup flaggedTargetGroups, Symbols symbol )
		{
			AddSymbol( flaggedTargetGroups.GetFlaggedValuesAsArray<BuildTargetGroup>(), symbol );
		}
		public static void RemoveSymbol( UnityEditor.BuildTargetGroup flaggedTargetGroups, Symbols symbol )
		{
			RemoveSymbol( flaggedTargetGroups.GetFlaggedValuesAsArray<BuildTargetGroup>(), symbol );
		}
		
	}

}