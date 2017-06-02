//By: Daniel Soto
//4dsoto@gmail.com
using UnityEditor;
using System.IO;

//TODO: Improve this class by creating an editor window with FBX Import Presets.
class FbxPreImporter : AssetPostprocessor 
{
	/*static bool _importAllWasAsked;
	
	void OnPreprocessModel () 
	{
		if( Path.GetExtension( assetPath ).ToLower() != ".fbx" || assetPath.Contains("@") )
			return;

		if( !_importAllWasAsked )
		{
			if( EditorPrefs.GetBool( "FbxImportMaterials", true ) )
				EditorPrefs.SetBool( "FbxImportMaterials", EditorUtility.DisplayDialog("Import Materials For..", "All FBX Objects", "Import All", "Skip All") );
			if( EditorPrefs.GetBool( "FbxScaleOverride", false ) )
				EditorPrefs.SetBool( "FbxScaleOverride", EditorUtility.DisplayDialog("Override Scale Factor For..", "All FBX Objects", "Override All", "Don't Override") );
		}
		_importAllWasAsked = true;

		ModelImporter model = (ModelImporter) assetImporter;

		if( EditorPrefs.GetBool( "FbxImportMaterials" ) )
		{
			model.importMaterials = true;
		}
		else model.importMaterials = EditorUtility.DisplayDialog("Import Materials For..", "FBX Object: " + model.name, "Import", "Skip");

		if( EditorPrefs.GetBool( "FbxScaleOverride" ) )
		{
			model.globalScale = EditorPrefs.GetInt( "FbxScale", 1 );
		}
    }

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		if( !_importAllWasAsked )
			return;
		_importAllWasAsked = false;
		if( EditorPrefs.GetBool( "FbxScaleOverride" ) )
			EditorUtility.DisplayDialog("Success Message", "All FBX Scale Factors have been set to: " + EditorPrefs.GetInt( "FbxScale" ) +
			                            "\n\n" + "To set a different Scale Factor navigate to Custom/FBX in the Menu Bar", "OK" );
	}*/

}