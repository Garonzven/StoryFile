//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEditor;
using DDK.Base.Misc;

namespace DDK {

	[CustomEditor( typeof( SceneDescriptor ) )]
	[CanEditMultipleObjects]
	public class SceneDescriptorEditor : Editor 
	{
		SerializedProperty customIcon;
		SerializedProperty description;
		SerializedProperty msgType;
		SerializedProperty objs;
		SerializedProperty Lock;
		SerializedProperty Ref;
		SerializedProperty CustomIcon;
		
		public override void OnInspectorGUI()
		{			
			serializedObject.Update();

			//EditorGUILayout.PropertyField( Lock, new GUIContent( "Lock" ) );
			if( !Lock.boolValue )
			{
				if( CustomIcon.boolValue )
				{
					EditorGUILayout.PropertyField( customIcon, new GUIContent( "Custom Icon" ) );
				}
				EditorGUILayout.PropertyField( msgType, new GUIContent( "Message Type" ) );
				description.stringValue = EditorGUILayout.TextArea( description.stringValue );
				description.stringValue = description.stringValue.Replace( "\\n", "\n" );
			}
			EditorGUILayout.HelpBox( description.stringValue, msgType.Value<MessageType>() );
			if( Ref.boolValue )
			{
				objs = serializedObject.FindProperty("objs");//rewind
				bool expanded = EditorGUILayout.PropertyField( objs, new GUIContent( "References" ) );
				if( expanded )
				{
					objs.Next(true);
					objs.Next(true);
					EditorGUI.indentLevel++;
					do
					{
						if( !objs.name.Equals( Lock.name ) && !objs.name.Equals( Ref.name ) )
						{
							EditorGUILayout.PropertyField( objs, new GUIContent( objs.displayName ) );
						}
					} while( objs.Next(false) );
					EditorGUI.indentLevel--;
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		public void OnEnable()
		{
			customIcon = serializedObject.FindProperty("customIcon");
			description = serializedObject.FindProperty("description");
			msgType = serializedObject.FindProperty("messageType");
			Lock = serializedObject.FindProperty("Lock");
			Ref = serializedObject.FindProperty("Ref");
			CustomIcon = serializedObject.FindProperty("CustomIcon");
		}
	}
	
}
