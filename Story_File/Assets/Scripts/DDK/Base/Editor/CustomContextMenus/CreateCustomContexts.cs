//By: Daniel Soto
//4dsoto@gmail.com
using UnityEditor;
using System.IO;
using UnityEngine;
using DDK.Base.Extensions;


namespace DDK {

	public static class CreateCustomContexts
	{
		[MenuItem ( "Assets/Create/Text File", false, 10 )]
		static void CreateTextFile ()
		{
			var path = "";
			var obj = Selection.activeObject;
			if ( obj == null ) path = "Assets";
			else path = AssetDatabase.GetAssetPath( obj.GetInstanceID() );

			if ( path.Length > 0 )
			{
				if( !Directory.Exists( path ) )
				{
					path = path.RemoveLastEndPoint();
				}
				File.WriteAllText( path + "/NewTextFile.txt", "New Text File" );
				AssetDatabase.Refresh();
			}
			else
			{
				Debug.Log("Path not in assets folder..");
			}
		}
		[MenuItem( "Assets/Create/Combine Selected Audio Clips", false, 220 )]
		public static void CombineAudioClips()
		{
			CustomMenuItems.CombineAudioClips();
		}
		[MenuItem( "Assets/Create/Combine Selected Audio Clips Reversed", false, 220 )]
		public static void CombineAudioClipsReversed()
		{
			CustomMenuItems.CombineAudioClipsReversed();
		}
	}
}