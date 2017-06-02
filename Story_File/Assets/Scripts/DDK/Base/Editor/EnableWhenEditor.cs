//By: Daniel Soto
//4dsoto@gmail.com
using UnityEditor;
using UnityEngine;
using DDK.Base.Misc;
using DDK.Base.Events.States;


namespace DDK 
{
	[CustomEditor(typeof(EnableWhen))]
	[CanEditMultipleObjects]
	public class EnableWhenEditor : Editor 
	{
		SerializedProperty m_frameRate;
		SerializedProperty m_willBeEnabled;
		SerializedProperty m_willBeEnabledR;
		SerializedProperty m_whenAnySoundIsPlaying;
		SerializedProperty m_whenMouseDown;
		SerializedProperty m_reDisableAfter;
        SerializedProperty m_whenNotInScenes;

        bool _fold, _fold2;
		

		public override void OnInspectorGUI()
		{
			serializedObject.Update();		

			_fold = EditorGUILayout.Foldout( _fold, "Will Be Enabled" );
			if( _fold )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( m_willBeEnabled, new GUIContent( "Behaviours" ), true );
				EditorGUILayout.PropertyField( m_willBeEnabledR, new GUIContent( "Renderers" ), true );
				EditorGUI.indentLevel--;
			}
			if( m_willBeEnabled.arraySize > 0 || m_willBeEnabledR.arraySize > 0 )
			{
                GUILineStyle.Draw();
                _fold2 = EditorGUILayout.Foldout( _fold2, "Conditions" );
                if( _fold2 )
                {
                    EditorGUILayout.PropertyField( m_frameRate, new GUIContent( "Frame Rate" ) );
                    EditorGUI.indentLevel++;
                    m_whenAnySoundIsPlaying.boolValue = ( m_whenAnySoundIsPlaying.boolValue & !m_whenMouseDown.boolValue );
                    m_whenMouseDown.boolValue = ( !m_whenAnySoundIsPlaying.boolValue & m_whenMouseDown.boolValue );
                    EditorGUILayout.PropertyField( m_whenAnySoundIsPlaying, new GUIContent( "When Any Sound Is Playing" ) );
                    EditorGUILayout.PropertyField( m_whenMouseDown, new GUIContent( "When Mouse Down" ) );
                    if( m_whenAnySoundIsPlaying.boolValue || m_whenMouseDown.boolValue )
                    {
                        EditorGUILayout.PropertyField( m_reDisableAfter, new GUIContent( "Re Disable After" ) );
                    }
                    EditorGUILayout.PropertyField( m_whenNotInScenes, new GUIContent( "When Not In Scenes" ), true );
                    EditorGUI.indentLevel--;
                }				
			}
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
            m_frameRate = serializedObject.FindProperty("frameRate");
            m_willBeEnabled = serializedObject.FindProperty("willBeEnabled");
            m_willBeEnabledR = serializedObject.FindProperty("willBeEnabledR");
            m_whenAnySoundIsPlaying = serializedObject.FindProperty("whenAnySoundIsPlaying");
            m_whenMouseDown = serializedObject.FindProperty("whenMouseDown");
            m_reDisableAfter = serializedObject.FindProperty("reDisableAfter");
            m_whenNotInScenes = serializedObject.FindProperty("whenNotInScenes");
		}		
	}
}

