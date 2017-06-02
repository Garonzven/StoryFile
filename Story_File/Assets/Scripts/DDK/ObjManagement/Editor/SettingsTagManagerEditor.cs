using UnityEngine;
using System.Collections;
using UnityEditor;

using DDK.Base.UI;
using DDK.Base.Misc;
using DDK.ObjManagement;


namespace DDK {
	
	[CustomEditor(typeof(SettingsTagManager))]
	[CanEditMultipleObjects]
	public class SettingsTagManagerEditor : Editor 
	{
		SerializedProperty _tags;
		SerializedProperty createTags;
		
		public override void OnInspectorGUI()
		{
			
			serializedObject.Update();
			
			EditorGUILayout.PropertyField( _tags, new GUIContent( "Tags" ), true );
			EditorGUILayout.PropertyField( createTags, new GUIContent( "Create Tags" ) );
			
			if( createTags.boolValue )
			{
				CheckCreateTag();
				createTags.boolValue = false;
			}
			
			serializedObject.ApplyModifiedProperties();
			
		}
		
		public void OnEnable()
		{
			_tags = serializedObject.FindProperty("_tags");
			createTags = serializedObject.FindProperty("createTags");
		}



		public void CheckCreateTag()
		{
			// Open tag manager
			SerializedObject tagManager = new SerializedObject( AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0] );
			SerializedProperty tagsProp = tagManager.FindProperty("tags");
			
			for( int i=0; i<_tags.arraySize; i++ )//for each new tag
			{
				string s = _tags.GetArrayElementAtIndex( i ).stringValue;//the tag to add

				// Check if it is not already present
				bool found = false;
				for (int j = 0; j < tagsProp.arraySize; j++)
				{					
					SerializedProperty t = tagsProp.GetArrayElementAtIndex( j );
					if ( t.stringValue.Equals( s ) ) 
					{ 
						found = true; 
						break;
					}
				}

				// if not found, add it
				if (!found)
				{
					tagsProp.InsertArrayElementAtIndex(0);
					SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
					n.stringValue = s;
					Debug.Log ( "Tag \" "+ s +" \" Created!" );
				}
				else Debug.LogWarning ( "Tag \" "+ s +" \" was already created..." );
			}

			tagManager.ApplyModifiedProperties();
		}
		
		
		
	}
	
}