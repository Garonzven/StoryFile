//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEditor;
using DDK.Base.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;
using DDK.Base.ScriptableObjects;
using DDK.Base.Managers;
using DDK.Base.SoundFX;


namespace DDK {

	public static class CustomMenuItems {


		#region CREATE
		[MenuItem( "Custom/Create/SfxManager" )]
		public static void CreateSfxManager()
		{
			SfxManager sfxManager = GameObject.FindObjectOfType<SfxManager>();
			if( !sfxManager )
			{
				new GameObject("SfxManager", typeof( SfxManager ) );
			}
		}
		[MenuItem( "Custom/Sound/Combine Selected Audio Clips" )]
		public static void CombineAudioClips()
		{
			AudioClip[] clips = GetSelectedAudioClips();
			if( clips == null || clips.Length == 0 )
			{
				Debug.LogError ( "No clips have been selected" );
				return;
			}

			clips.Reverse();
			AudioClip result = clips.Combine();

			string path = AssetDatabase.GetAssetPath( clips[ clips.Length - 1 ].GetInstanceID() ).RemoveLastEndPoint( true );

			SaveWav.Save( path, "CombinedClip", result );

			AssetDatabase.Refresh();
		}
		[MenuItem( "Custom/Sound/Combine Selected Audio Clips Reversed" )]
		public static void CombineAudioClipsReversed()
		{
			AudioClip[] clips = GetSelectedAudioClips();
			if( clips == null || clips.Length == 0 )
			{
				Debug.LogError ( "No clips have been selected" );
				return;
			}

			AudioClip result = clips.Combine();
			
			string path = AssetDatabase.GetAssetPath( clips[ 0 ].GetInstanceID() ).RemoveLastEndPoint( true );
			
			SaveWav.Save( path, "CombinedClip", result );
			
			AssetDatabase.Refresh();
		}


		//VALIDATIONS
		/*[MenuItem( "Custom/Sound/Combine Selected Audio Clips", true )] THIS IS FREEZING THE EDITOR
		public static bool AreSomeAudioClipsSelected()
		{
			AudioClip[] clips = GetSelectedAudioClips();
			if( clips == null )
				return false;
			return clips.Length > 1;
		}
		[MenuItem( "Custom/Sound/Combine Selected Audio Clips Reversed", true )] THIS IS FREEZING THE EDITOR
		public static bool AreSomeAudioClipsSelectedReversed()
		{
			return AreSomeAudioClipsSelected();
		}*/
		#endregion

		#region PREFERENCES
		[MenuItem("Custom/Preferences/Clear Player Prefs")]
		public static void ClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
			Debug.Log ("Player Prefs Cleared!");
		}
		[MenuItem("Custom/Preferences/Clear Editor Prefs")]
		public static void ClearEditorPrefs()
		{
			EditorPrefs.DeleteAll();
			Debug.Log ("Editor Prefs Cleared!");
		}
		private static CustomWindow _customPPWindow;
		public const string CUSTOMPP_TEXTURE_FORMAT = "CustomPP_TextureFormat";
		public const string CUSTOMPP_MAX_WIDTH = "CustomPP_MaxWidth";
		public const string CUSTOMPP_MAX_HEIGHT = "CustomPP_MaxHeight";
		public const string CUSTOMPP_MIPMAPS = "CustomPP_MipMaps";
		[MenuItem("Custom/Preferences/Custom Packer Policy..")]
		public static void CustomPackerPolicy()
		{
			if( _customPPWindow )
				_customPPWindow.Focus();
			else _customPPWindow = new CustomWindow( CustomPackerPolicyWindow, "Custom Packer Policy Preferences", 300f, 80f );
		}
		public static void CustomPackerPolicyWindow()
		{
			//GUILayout.Label ("Settings", EditorStyles.boldLabel);

			//GET
            TextureFormat textureFormat = (TextureFormat) EditorPrefs.GetInt( CUSTOMPP_TEXTURE_FORMAT + Application.platform, 
                #if UNITY_IOS
                (int)TextureFormat.PVRTC_RGBA4
                #else
                (int)TextureFormat.ETC2_RGBA8.ToInt32() 
                #endif
            );
			int maxWidth = EditorPrefs.GetInt( CUSTOMPP_MAX_WIDTH, 2048 );
			int maxHeight = EditorPrefs.GetInt( CUSTOMPP_MAX_HEIGHT, 2048 );
			bool generateMipMaps = EditorPrefs.GetBool( CUSTOMPP_MIPMAPS, true );
			
			textureFormat = (TextureFormat) EditorGUILayout.EnumPopup ( new GUIContent( "Texture Format" ), (System.Enum) textureFormat );
			maxWidth = EditorGUILayout.IntField ( new GUIContent( "Max Width" ), maxWidth );
			maxHeight = EditorGUILayout.IntField ( new GUIContent( "Max Height" ), maxHeight );
			generateMipMaps = EditorGUILayout.Toggle ( new GUIContent( "Generate Mip Maps" ), generateMipMaps );
			//SET
            EditorPrefs.SetInt( CUSTOMPP_TEXTURE_FORMAT + Application.platform, (int)textureFormat.ToInt32() );
			EditorPrefs.SetInt( CUSTOMPP_MAX_WIDTH, maxWidth );
			EditorPrefs.SetInt( CUSTOMPP_MAX_HEIGHT, maxHeight );
			EditorPrefs.SetBool( CUSTOMPP_MIPMAPS, generateMipMaps );
		}
        public const string COMPOSED_EVENTS_LOGS = "ComposedEventsLogs";
        [MenuItem("Custom/Preferences/Toggle Composed Events Console Logs")]
        public static void ToggleComposedEventsLogs()
        {
            EditorPrefs.SetBool( COMPOSED_EVENTS_LOGS, !EditorPrefs.GetBool( COMPOSED_EVENTS_LOGS, true ) );
            Debug.Log( "Show Composed Events Console Logs = " + EditorPrefs.GetBool( COMPOSED_EVENTS_LOGS ) );
        }            
		#endregion

		#region FBX
		[MenuItem("Custom/FBX/Import Materials")]
		public static void FbxImportMaterials()
		{
			EditorPrefs.SetBool( "FbxImportMaterials", true );
			Debug.Log ("FBX Materials will be imported");
		}
		[MenuItem("Custom/FBX/Do NOT Import Materials")]
		public static void FbxNotImportMaterials()
		{
			EditorPrefs.SetBool( "FbxImportMaterials", false );
			Debug.Log ("FBX Materials will NOT be imported");
		}
		[MenuItem("Custom/FBX/Do NOT Override Scale", false, 10)]
		public static void FbxScaleOverride()
		{
			EditorPrefs.SetBool( "FbxScaleOverride", false );
			Debug.Log ("FBXs scales will stay the same..");
		}
		[MenuItem("Custom/FBX/Scale to 1 (Default)", false, 10)]
		public static void FbxScaleTo1()
		{
			EditorPrefs.SetBool( "FbxScaleOverride", true );
			EditorPrefs.SetInt( "FbxScale", 1 );
			Debug.Log ("FBXs scales will be set to 1");
		}
		[MenuItem("Custom/FBX/Scale to 10", false, 10)]
		public static void FbxScaleTo10()
		{
			EditorPrefs.SetBool( "FbxScaleOverride", true );
			EditorPrefs.SetInt( "FbxScale", 10 );
			Debug.Log ("FBXs scales will be set to 10");
		}
		[MenuItem("Custom/FBX/Scale to 100", false, 10)]
		public static void FbxScaleTo100()
		{
			EditorPrefs.SetBool( "FbxScaleOverride", true );
			EditorPrefs.SetInt( "FbxScale", 100 );
			Debug.Log ("FBXs scales will be set to 100");
		}
		#endregion

		#region GAMEOBJECTS
		private const string REPLACE_KEEP = "ReplaceKeep";
		private const string REPLACE_OMIT = "ReplaceOmit";
		private static CustomWindow _renameWindow;
		private static string prefix, suffix;


        [MenuItem("GameObject/Rename.. &r")]
		public static void Rename()
		{
			if( _renameWindow )
				_renameWindow.Focus();
			else _renameWindow = new CustomWindow( RenameWindowOnGUI, "Rename Selected Objects", 300, 80 );
		}
		private static void RenameWindowOnGUI()
		{
			var objs = Selection.gameObjects;
			//WINDOW
			if( objs.Length == 0 )
			{
				EditorGUILayout.LabelField( "NO OBJECTS SELECTED.." );
				return;
			}
			prefix = EditorGUILayout.TextField( "Prefix: ", prefix );
			suffix = EditorGUILayout.TextField( "Suffix: ", suffix );
			if( GUILayout.Button( "Apply" ) )
			{
				foreach( var obj in objs )
				{
					obj.name = prefix + obj.name + suffix;
				}
				Debug.Log("Selected Objects Renamed");
			}
			if( GUILayout.Button( "Remove" ) )
			{
				foreach( var obj in objs )
				{
					if( !string.IsNullOrEmpty( prefix ) )
					{
						int prefixIndex = obj.name.IndexOf( prefix );
						if( prefixIndex != -1 )
							obj.name = obj.name.Remove( prefixIndex, prefix.Length );
					}
					if( !string.IsNullOrEmpty( suffix ) )
					{
						int suffixIndex = obj.name.IndexOf( suffix );
						if( suffixIndex != -1 )
							obj.name = obj.name.Remove( suffixIndex, suffix.Length );
					}
				}
				Debug.Log("Selected Objects Renamed");
			}
		}

        [MenuItem("GameObject/Select/Immediate Parents &,")]
		public static void SelectImmediateParents()
		{
			GameObject[] objs = Selection.gameObjects;
			List<GameObject> newSelections = new List<GameObject>();
			
			foreach( var obj in objs )
			{
				newSelections.Add( obj.GetParent() );
            }
            Selection.objects = newSelections.ToArray() as Object[];
        }
        [MenuItem("GameObject/Select/Immediate Children &.")]
		public static void SelectImmediateChildren()
		{
			GameObject[] objs = Selection.gameObjects;
			List<GameObject> newSelections = new List<GameObject>();

			foreach( var obj in objs )
			{
				if( obj.ChildCount() == 0 )
				{
					newSelections.Add( obj );
				}
				else newSelections.AddRange( obj.GetChildren() );
			}
			Selection.objects = newSelections.ToArray() as Object[];
		}
        [MenuItem("GameObject/Select/Similar/By Mesh &h")]
		public static void SelectSimilarByMesh()
		{
			GameObject obj = Selection.activeGameObject;
			var allMeshes = GameObject.FindObjectsOfType<MeshFilter>();
			List<GameObject> newSelections = new List<GameObject>();

			if( !obj )
			{
				Debug.LogWarning("No object has been selected");
				return;
			}
			MeshFilter current = obj.GetComponent<MeshFilter>();
			if( !current )
			{
				Debug.LogWarning("There is no MeshFilter in the selected object");
				return;
			}

			foreach( var filter in allMeshes )
			{
				if( filter.sharedMesh == current.sharedMesh )
				{
					newSelections.Add( filter.gameObject );
				}
			}
			Selection.objects = newSelections.ToArray() as Object[];
		}
        [MenuItem("GameObject/Select/Similar/By Material Name &m")]
		public static void SelectSimilarByMaterial()
		{
			GameObject obj = Selection.activeGameObject;
            Renderer[] allRenderers = GameObject.FindObjectsOfType<Renderer>();
			List<GameObject> newSelections = new List<GameObject>();
			
			if( !obj )
			{
				Debug.LogWarning("No object has been selected");
				return;
			}
			Renderer current = obj.GetComponent<Renderer>();
			if( !current )
			{
				Debug.LogWarning("There is no Renderer in the selected object");
				return;
			}
			
			foreach( var renderer in allRenderers )
			{
				bool similar = true;
				if( renderer.sharedMaterials.Length != current.sharedMaterials.Length )
					continue;
				for( int i=0; i<renderer.sharedMaterials.Length; i++ )
				{
                    if( renderer.sharedMaterials[i] == null )
                    {
                        Debug.LogWarning( "The Renderer component in gameObject '"+ renderer.name +"' has a null material" +
                            " at index: "+i, renderer.gameObject );
                        continue;
                    }
					if( !renderer.sharedMaterials[i].name.Equals( current.sharedMaterials[i].name ) )
					{
						similar = false;
						break;
					}
				}
				if( similar )
				{
					newSelections.Add( renderer.gameObject );
				}
			}
			Selection.objects = newSelections.ToArray() as Object[];
		}
		static CustomWindow _selectSimilarByNameWindow;
		static string _startingWith;
		static string _endingWith;
        [MenuItem("GameObject/Select/Similar/By Name... &n")]
		public static void SelectSimilarByName()
		{
			GameObject obj = Selection.activeGameObject;
			if( obj )
				_startingWith = obj.name;
			if( _selectSimilarByNameWindow )
				_selectSimilarByNameWindow.Focus();
			else _selectSimilarByNameWindow = new CustomWindow( _SelectSimilarByNameWindowOnGUI, "Selected Similar GameObjects By Name..", 300, 60 );
		}
		private static void _SelectSimilarByNameWindowOnGUI()
		{
			//WINDOW
			_startingWith = EditorGUILayout.TextField( "Starting With: ", _startingWith );
			_endingWith = EditorGUILayout.TextField( "Ending With: ", _endingWith );
			if( GUILayout.Button( "Select" ) )
			{
				var _objs = GameObject.FindObjectsOfType<GameObject>();
				List<GameObject> newSelections = new List<GameObject>();
				
				foreach( var go in _objs )
				{
					if( ( !string.IsNullOrEmpty( _startingWith ) && go.name.StartsWith( _startingWith ) ) || 
					   ( !string.IsNullOrEmpty( _endingWith ) && go.name.EndsWith( _endingWith ) ) )
					{
						newSelections.Add( go );
					}
				}
				Selection.objects = newSelections.ToArray() as Object[];
			}
		}

        [MenuItem("GameObject/Select/Text/All")]
		public static void SelectAllText()
		{
			var allText = GameObject.FindObjectsOfType<Text>();
			List<GameObject> newSelections = new List<GameObject>();
			
			foreach( var txt in allText )
			{
				newSelections.Add( txt.gameObject );
			}
			Selection.objects = newSelections.ToArray() as Object[];
		}
        [MenuItem("GameObject/Select/Text/All Children")]
		public static void SelectAllSiblingText()
		{
			GameObject obj = Selection.activeGameObject;
			var allChildrenText = obj.GetComponentsInChildren<Text>();
			List<GameObject> newSelections = new List<GameObject>();
			
			if( !obj )
			{
				Debug.LogWarning("No object has been selected");
				return;
			}
			
			foreach( var txt in allChildrenText )
			{
				newSelections.Add( txt.gameObject );
			}
			Selection.objects = newSelections.ToArray() as Object[];
		}

		static CustomWindow _replaceSelectionWindow;
		static Object _replacement;
		static bool _keepT = true;
		static bool _keepNames = true;
		static bool _keepRotX = true;
		static bool _keepRotY = true;
		static bool _keepRotZ = true;
        [MenuItem("GameObject/Replace Selections...")]
		public static void ReplaceSelectionsMenuItem()
		{
			GameObject obj = Selection.activeGameObject;
			if( obj )
				_startingWith = obj.name;
			if( _replaceSelectionWindow )
				_replaceSelectionWindow.Focus();
			else _replaceSelectionWindow = new CustomWindow( _ReplaceSelectionsWindowOnGUI, "Replace", 300, 135 );
		}
		private static void _ReplaceSelectionsWindowOnGUI()
		{
			//WINDOW
			EditorGUILayout.LabelField( "<b>Replace Selections...<b>" );
			_replacement = EditorGUILayout.ObjectField( new GUIContent( "Replacement: ", "The object that will replace all selections" ), _replacement, typeof( GameObject ), true );
			_keepT = EditorGUILayout.Toggle( new GUIContent( "Keep Transforms: ", "Keep the selections's Transform values intact" ), _keepT );
			_keepNames = EditorGUILayout.Toggle( new GUIContent( "Keep Names: ", "Keep the selections's names intact" ), _keepNames );
			_keepRotX = EditorGUILayout.Toggle( new GUIContent( "Keep Rot X: ", "Keep the selections's Rotation in the X axis intact" ), _keepRotX );
			_keepRotY = EditorGUILayout.Toggle( new GUIContent( "Keep Rot Y: ", "Keep the selections's Rotation in the X axis intact" ), _keepRotY );
			_keepRotZ = EditorGUILayout.Toggle( new GUIContent( "Keep Rot Z: ", "Keep the selections's Rotation in the X axis intact" ), _keepRotZ );
			if( GUILayout.Button( "Replace Selections" ) )
			{
				ReplaceSelections();
			}
		}
		public static void ReplaceSelections()
		{
			if( !_replacement )
			{
				EditorUtility.DisplayDialog( "Error", "No object has been specified as the replacement", "OK" );
				return;
			}
			var objs = Selection.gameObjects;
			var replacement = _replacement as GameObject;
			objs = objs.RemoveContained( new GameObject[]{ replacement } );

			Undo.SetCurrentGroupName( "ReplaceSelectionsWithFirstSelected" );
			int undoGroup = Undo.GetCurrentGroup();

			int leadingCeros = objs.Length.ToString().Length;
			for( int i=0; i<objs.Length; i++ )
			{
				GameObject obj = objs[ i ];
				GameObject duplicate = null;
				if( PrefabUtility.GetPrefabObject( replacement ) )
				{
					duplicate = PrefabUtility.InstantiatePrefab( replacement ) as GameObject;
				}
				else duplicate = GameObject.Instantiate( replacement );
				Undo.RegisterCreatedObjectUndo( duplicate, obj.name );
				duplicate.SetParent( obj.transform.parent );
				Vector3 rot = duplicate.transform.rotation.eulerAngles;
				if( _keepT )
				{
					duplicate.CopyTransformFrom( obj );
				}
				if( !_keepRotX )
				{
					duplicate.ApplyRot( new Vector3( rot.x, duplicate.transform.rotation.eulerAngles.y, duplicate.transform.rotation.eulerAngles.z ) );
				}
				if( !_keepRotY )
				{
					duplicate.ApplyRot( new Vector3( duplicate.transform.rotation.eulerAngles.x, rot.y, duplicate.transform.rotation.eulerAngles.z ) );
				}
				if( !_keepRotZ )
				{
					duplicate.ApplyRot( new Vector3( duplicate.transform.rotation.eulerAngles.x, duplicate.transform.rotation.eulerAngles.y, rot.z ) );
				}
				if( _keepNames )
				{
					duplicate.name = obj.name;
				}
				else duplicate.name += " " + i.ToString().PadLeft( leadingCeros, '0' );
				Undo.DestroyObjectImmediate( obj );
			}
			Undo.CollapseUndoOperations( undoGroup );
		}

        [MenuItem("GameObject/Order/Children/By Name Descending")]
		public static void OrderChildrenByNameDescending()
		{
			var objs = Selection.activeGameObject.GetChildren();
			objs.OrderSiblingsByNameDescending();
		}
        [MenuItem("GameObject/Order/Children/By Name Ascending")]
		public static void OrderChildrenByNameAscending()
		{
			var objs = Selection.activeGameObject.GetChildren();
			objs.OrderSiblingsByNameAscending();
		}

        [MenuItem("GameObject/Tris/Log Count Per Object %&t")]
		public static void TrisCountPerObj()
		{
			GameObject obj = Selection.activeGameObject;
			try {
				Debug.Log ( obj.name +": "+ obj.GetComponent<MeshFilter>().sharedMesh.triangles.Length/3, obj );
			}
			catch( MissingComponentException )
			{
				try {
					Debug.Log ( obj.name +": "+ obj.GetComponent<SkinnedMeshRenderer>().sharedMesh.triangles.Length/3, obj );
				}
				catch( MissingComponentException )
				{
					var mfilters = obj.GetComponentsInChildren<MeshFilter>();
					if( mfilters != null )
					{
						if( mfilters.Length > 0 )
						{
							for( int i=0; i<mfilters.Length; i++ )
							{
								Debug.Log ( mfilters[i].name+": "+mfilters[i].sharedMesh.triangles.Length/3, mfilters[i].gameObject );
							}
							return;
						}
					}
					var skinnedMeshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
					if( skinnedMeshes != null )
					{
						if( skinnedMeshes.Length > 0 )
						{
							for( int i=0; i<skinnedMeshes.Length; i++ )
							{
								Debug.Log ( skinnedMeshes[i].name+": "+skinnedMeshes[i].sharedMesh.triangles.Length/3, skinnedMeshes[i].gameObject );
							}
							return;
						}
					}
				}
			}
		}		
		[MenuItem("GameObject/Tris/Log Total Count %#t")]
		public static void TrisCount()
		{
			GameObject obj = Selection.activeGameObject;
			try {
				Debug.Log ( obj.name +": "+ obj.GetComponent<MeshFilter>().sharedMesh.triangles.Length/3, obj );
			}
			catch( MissingComponentException )
			{
				try {
					Debug.Log ( obj.name +": "+ obj.GetComponent<SkinnedMeshRenderer>().sharedMesh.triangles.Length/3, obj );
				}
				catch( MissingComponentException )
				{
					int total = 0;
					var mfilters = obj.GetComponentsInChildren<MeshFilter>();
					if( mfilters != null )
					{
						if( mfilters.Length > 0 )
						{
							for( int i=0; i<mfilters.Length; i++ )
							{
								total += mfilters[i].sharedMesh.triangles.Length/3;
							}
							Debug.Log ( obj.name +": "+ total, obj );
							return;
						}
					}
					var skinnedMeshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
					if( skinnedMeshes != null )
					{
						if( skinnedMeshes.Length > 0 )
						{
							for( int i=0; i<skinnedMeshes.Length; i++ )
							{
								total += skinnedMeshes[i].sharedMesh.triangles.Length/3;
							}
							Debug.Log ( obj.name +": "+ total, obj );
							return;
						}
					}
				}
			}
		}

        /*[MenuItem("Custom/LODGroups/Clean (Delete LODs with no renderers)")]
        public static void LODGroupsClean()
        {
            /*LODGroup lodGroup = Selection.activeGameObject.GetComponent<LODGroup>();
            LOD[] lods = lodGroup.GetLODs();
            for( int i=0; i<lods.Length; i++ )
            {
                Debug.Log( lods[i].renderers.Length );
            }//*
            int cleanedCount = 0;
            LODGroup[] lodGroups = GameObject.FindObjectsOfType<LODGroup>();
            LOD[] lods = null;//caching
            LOD[] _lods = null;//caching
            for( int i=0; i<lodGroups.Length; i++ )
            {
                lods = lodGroups[i].GetLODs();
                for( int j=0; j<lods.Length; j++ )
                {
                    if( lods[j].renderers.Length == 0 )
                    {
                        Debug.Log( lodGroups[i].name, lodGroups[i].gameObject );
                        _lods = new LOD[ lods.Length - 1 ];
                        for( int k=0, l=0; k<lods.Length; k++ )
                        {
                            if( k == j )
                                continue;
                            _lods[l] = lods[k];
                            l++;
                        }
                        lodGroups[i].SetLODs( _lods );
                        cleanedCount++;
                    }
                }
            }
            Debug.Log( cleanedCount + " LODs cleaned!" );
        }*/
        [MenuItem( "GameObject/LODGroups/Remove LOD/0 #%&0" )]
        public static void LODGroupsRemoveLOD0()
        {
            LODGroupsRemoveLOD( 0 );
        }
        [MenuItem( "GameObject/LODGroups/Remove LOD/1 #%&1" )]
        public static void LODGroupsRemoveLOD1()
        {
            LODGroupsRemoveLOD( 1 );
        }
        [MenuItem( "GameObject/LODGroups/Remove LOD/2 #%&2" )]
        public static void LODGroupsRemoveLOD2()
        {
            LODGroupsRemoveLOD( 2 );
        }

		
		
		//VALIDATE FUNCTIONS
		[MenuItem("GameObject/Order/Children/By Name Descending", true)]
		public static bool ValidateOrderChildrenByNameDescending()
		{
			return ValidateActiveObj();
		}
		[MenuItem("GameObject/Order/Children/By Name Ascending", true)]
		public static bool ValidateOrderChildrenByNameAscending()
		{
			return ValidateActiveObj();
		}
		public static bool ValidateActiveObj()
		{
			if( !Selection.activeGameObject )
				return false;
			return true;
		}
		[MenuItem("GameObject/Tris/Log Count Per Object %&t", true)]
		public static bool ValidateTrisCountPerObj()
		{
			return ValidateTrisCount();
		}		
		[MenuItem("GameObject/Tris/Log Total Count %#t", true)]
		public static bool ValidateTrisCount()
		{
			GameObject obj = Selection.activeGameObject;
			if( !obj )
			{
				return false;
			}
			else if( !obj.GetComponent<MeshFilter>() )
			{
				if( obj.GetComponent<SkinnedMeshRenderer>() )
				{
					return true;
				}
				var mfilters = obj.GetComponentsInChildren<MeshFilter>();
				if( mfilters != null )
				{
					if( mfilters.Length > 0 )
					{
						return true;
					}
				}
				var skinnedMeshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
				if( skinnedMeshes != null )
				{
					if( skinnedMeshes.Length > 0 )
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}            
		#endregion



		public static AudioClip[] GetSelectedAudioClips()
		{
			Object[] clipsObjs = Selection.GetFiltered( typeof( AudioClip ), SelectionMode.DeepAssets );
			if( clipsObjs == null )
				return null;
			List<AudioClip> clips = new List<AudioClip>( clipsObjs.Length );
			foreach( AudioClip clip in clipsObjs )
			{
				clips.Add ( clip );
			}
			return clips.ToArray();
		}
        public static void LODGroupsRemoveLOD( int lvl )
        {
            LODGroup[] lodGroups = Selection.gameObjects.GetComponents<LODGroup>();
            if( lodGroups.Length == 0 )
            {
                Debug.LogWarning( "No gameObjects selected.." );
                return;
            }
            LOD[] lods;
            LOD[] _lods;
            Undo.SetCurrentGroupName( "LODGroups_RemoveLOD" );
            int undoGroup = Undo.GetCurrentGroup();
            Undo.RegisterCompleteObjectUndo( lodGroups, "LODGroups" );
            for( int i=0; i<lodGroups.Length; i++ )
            {
                lods = lodGroups[i].GetLODs();
                _lods = new LOD[ lods.Length - 1 ];
                for( int j=0, k=0; j<lods.Length; j++ )
                {
                    if( j == lvl )
                    {
                        Undo.DestroyObjectImmediate( lodGroups[ i ].transform.GetChild( j ).gameObject );
                        continue;
                    }
                    _lods[k] = lods[j];
                    k++;
                }
                lodGroups[ i ].SetLODs( _lods );
            }
            Undo.CollapseUndoOperations( undoGroup );
            Debug.Log( "LOD levels removed for selected objects" );
        }
	}

}
