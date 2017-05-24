using UnityEngine;
using UnityEditor;

using DDK.Base.UI;
using DDK.Base.Misc;
using DDK.ObjManagement;


namespace DDK 
{
	[CustomEditor(typeof(Tag))]
	[CanEditMultipleObjects]
	public class TagEditor : Editor 
	{
		SerializedProperty _tag;
        SerializedProperty m_createTag;
		
		public override void OnInspectorGUI()
		{

			serializedObject.Update();

			EditorGUILayout.PropertyField( _tag, new GUIContent( "Tag" ) );
			EditorGUILayout.PropertyField( m_createTag, new GUIContent( "Create Tag" ) );

			if( m_createTag.boolValue )
			{
				CheckCreateTag();
				m_createTag.boolValue = false;
				Debug.Log ( "Tag \" "+ _tag.stringValue +" \" Created!" );
			}

			serializedObject.ApplyModifiedProperties();

		}
		
		public void OnEnable()
		{
			_tag = serializedObject.FindProperty("_tag");
			m_createTag = serializedObject.FindProperty("createTag");
		}

		public void CheckCreateTag()
		{
			// Open tag manager
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty tagsProp = tagManager.FindProperty("tags");
			
			// Adding a Tag
			string s = _tag.stringValue;
			
			// First check if it is not already present
			bool found = false;
			for (int i = 0; i < tagsProp.arraySize; i++)
			{
				SerializedProperty t = tagsProp.GetArrayElementAtIndex( i );
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
			}
			
			// and to save the changes
			tagManager.ApplyModifiedProperties();
		}
		
		
		
	}

}