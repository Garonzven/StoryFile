using System.Collections;
using UnityEditor;

using DDK.Base.UI;
using UnityEngine;
using DDK.Base.SoundFX;
using DDK.Base.Events;


namespace DDK {
	
	[CustomEditor(typeof(OnClickPlayAudio))]
	[CanEditMultipleObjects]
	public class OnClickPlayAudioEditor : Editor 
	{
		SerializedProperty msg;
		
		SerializedProperty useSfxManager;

		SerializedProperty delay;
		SerializedProperty interrupt;
		SerializedProperty source;
		SerializedProperty sourceName;
		SerializedProperty clip;
		SerializedProperty clipName;
		
		bool fold1, fold2;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.HelpBox( msg.stringValue, MessageType.Info );
			EditorGUILayout.PropertyField( useSfxManager, new GUIContent( "Use Sfx Manager" ) );
			EditorGUILayout.PropertyField( delay, new GUIContent("Delay") );
			EditorGUILayout.PropertyField( interrupt, new GUIContent("Interrupt") );
			if( useSfxManager.boolValue )
			{
				EditorGUILayout.PropertyField( sourceName, new GUIContent( "Source Name" ) );
            }
			else
			{
				EditorGUILayout.PropertyField( source, new GUIContent( "Source" ), true );
			}
			if( string.IsNullOrEmpty( clipName.stringValue ) )
			{
				EditorGUILayout.PropertyField( clip, new GUIContent( "Clip" ), true );
			}
			if( !clip.objectReferenceValue && useSfxManager.boolValue )
			{
				EditorGUILayout.PropertyField( clipName, new GUIContent( "Clip Name" ) );
			}
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
			msg = serializedObject.FindProperty("msg");
			useSfxManager = serializedObject.FindProperty("useSfxManager");
			delay = serializedObject.FindProperty("delay");
			interrupt = serializedObject.FindProperty("interrupt");
			source = serializedObject.FindProperty("source");
			sourceName = serializedObject.FindProperty("sourceName");
			clip = serializedObject.FindProperty("clip");
			clipName = serializedObject.FindProperty("clipName");
        }
        
        
        
    }
    
}
