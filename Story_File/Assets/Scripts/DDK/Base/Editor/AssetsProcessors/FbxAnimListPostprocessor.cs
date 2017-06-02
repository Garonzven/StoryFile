// FbxAnimListPostprocessor.cs : Use an external text file to import a list of 
// splitted animations for FBX 3D models.
//
// Put this script in your "Assets/Editor" directory. When Importing or 
// Reimporting a FBX file, the script will search a text file with the 
// same name and the ".txt" extension.
// File format: one line per animation clip "firstFrame-lastFrame loopFlag animationName"
// The keyworks "loop" or "noloop" are optional.
// Example:
// 0-50 loop Move forward
// 100-190 die

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class FbxAnimListPostprocessor : AssetPostprocessor
{
	static bool _importAllAnimsWasAsked;

	public void  OnPreprocessModel()
	{
		if (Path.GetExtension(assetPath).ToLower() == ".fbx" && !assetPath.Contains("@"))
		{
			try
			{
				// Remove 6 chars because dataPath and assetPath both contain "assets" directory
				string fileAnim = Application.dataPath + Path.ChangeExtension(assetPath, ".txt").Substring(6);
				StreamReader file = new StreamReader(fileAnim);
				
				string sAnimList = file.ReadToEnd();
				file.Close();

				if( !_importAllAnimsWasAsked )
				{
					if( EditorUtility.DisplayDialog("Import FBX Animations from..", "All .txt files", "Import All", "Skip All") )
						EditorPrefs.SetBool( "ImportAllFbxAnims", true );
				}
				_importAllAnimsWasAsked = true;

				if( EditorPrefs.GetBool( "ImportAllFbxAnims", false ) /*|| EditorUtility.DisplayDialog("Import FBX Animations from..", "File: " + fileAnim, "Import", "Skip")*/ )
				{
					System.Collections.ArrayList List = new ArrayList();
					ParseAnimFile(sAnimList, ref List);
					
					ModelImporter modelImporter = assetImporter as ModelImporter;
					//modelImporter.splitAnimations = true;
					modelImporter.clipAnimations = (ModelImporterClipAnimation[])
						List.ToArray(typeof(ModelImporterClipAnimation));

					if( !EditorPrefs.GetBool( "ImportAllFbxAnims" ) )
					{
						Debug.Log( "Number of imported clips (In file "+ fileAnim +"): " + 
						          modelImporter.clipAnimations.GetLength(0).ToString() );
					}
				}
			}
			catch {}
			// (Exception e) { EditorUtility.DisplayDialog("Imported animations", e.Message, "OK"); }
		}
	}

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		if( !_importAllAnimsWasAsked )
			return;
		_importAllAnimsWasAsked = false;
		if( EditorPrefs.GetBool( "ImportAllFbxAnims" ) )
			Debug.Log( "All FBX Animations have been imported" );
	}
	
	void ParseAnimFile(string sAnimList, ref System.Collections.ArrayList List)
	{
		/*Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(poseloop|loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
		                              RegexOptions.Compiled | RegexOptions.ExplicitCapture);*/
		Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(poseloop|loop|noloop| ))" +
		                              " *(?<all>((all-(bakedoriginal|baked|original))| *)) *(?<rot>((rot-(bakedoriginal|baked|original))| *))" +
		                              " *(?<posY>((posy-(bakedoriginal|baked|original))| *)) *(?<posXZ>((posxz-(bakedoriginal|baked|original))| *))" +
		                              " *(?<name>[^\r^\n]*[^\r^\n^ ])",
		                              RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		
		Match match = regexString.Match(sAnimList, 0);
		while (match.Success)
		{
			ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
			
			if (match.Groups["firstFrame"].Success)
			{
				clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
			}
			if (match.Groups["lastFrame"].Success)
			{
				clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
			}
			if (match.Groups["loop"].Success)
			{
				clip.loopTime = match.Groups["loop"].Value.Equals( "loop" ) | match.Groups["loop"].Value.Equals( "poseloop" );
				clip.loopPose = match.Groups["loop"].Value.Equals( "poseloop" );
			}
			if (match.Groups["rot"].Success)
			{
				clip.lockRootRotation = match.Groups["rot"].Value.Equals( "rot-baked" ) | match.Groups["rot"].Value.Equals( "rot-bakedoriginal" );
				clip.keepOriginalOrientation = match.Groups["rot"].Value.Equals( "rot-original" ) | match.Groups["rot"].Value.Equals( "rot-bakedoriginal" );
			}
			if (match.Groups["posY"].Success)
			{
				clip.lockRootHeightY = match.Groups["posY"].Value.Equals( "posy-baked" ) | match.Groups["posY"].Value.Equals( "posy-bakedoriginal" );
				clip.keepOriginalPositionY = match.Groups["posY"].Value.Equals( "posy-original" ) | match.Groups["posY"].Value.Equals( "posy-bakedoriginal" );
			}
			if (match.Groups["posXZ"].Success)
			{
				clip.lockRootPositionXZ = match.Groups["posXZ"].Value.Equals( "posxz-baked" ) | match.Groups["posXZ"].Value.Equals( "posxz-bakedoriginal" );
				clip.keepOriginalPositionXZ = match.Groups["posXZ"].Value.Equals( "posxz-original" ) | match.Groups["posXZ"].Value.Equals( "posxz-bakedoriginal" );
			}
			if (match.Groups["all"].Success && !string.IsNullOrEmpty( match.Groups["all"].Value ))
			{
				clip.lockRootRotation = match.Groups["all"].Value.Equals( "all-baked" ) | match.Groups["all"].Value.Equals( "all-bakedoriginal" );
				clip.keepOriginalOrientation = match.Groups["all"].Value.Equals( "all-original" ) | match.Groups["all"].Value.Equals( "all-bakedoriginal" );
				clip.lockRootHeightY = match.Groups["all"].Value.Equals( "all-baked" ) | match.Groups["all"].Value.Equals( "all-bakedoriginal" );
				clip.keepOriginalPositionY = match.Groups["all"].Value.Equals( "all-original" ) | match.Groups["all"].Value.Equals( "all-bakedoriginal" );
				clip.lockRootPositionXZ = match.Groups["all"].Value.Equals( "all-baked" ) | match.Groups["all"].Value.Equals( "all-bakedoriginal" );
				clip.keepOriginalPositionXZ = match.Groups["all"].Value.Equals( "all-original" ) | match.Groups["all"].Value.Equals( "all-bakedoriginal" );
			}
			if (match.Groups["name"].Success)
			{
				clip.name = match.Groups["name"].Value;
			}
			
			List.Add(clip);
			
			match = regexString.Match(sAnimList, match.Index + match.Length);
		}
	}
}