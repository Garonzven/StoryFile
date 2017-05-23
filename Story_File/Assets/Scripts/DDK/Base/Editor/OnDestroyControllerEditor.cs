using UnityEngine;
using System.Collections;
using UnityEditor;

using DDK.Base.Misc;
using DDK.Base.Events;



namespace DDK {

	[CustomEditor(typeof(OnDestroyController))]
	[CanEditMultipleObjects]
	public class OnDestroyControllerEditor : Editor 
	{
		SerializedProperty destroyAfter;
        SerializedProperty showBeforeDestruction;
		SerializedProperty substitutes;
		SerializedProperty chancesOSLS;
		SerializedProperty lastObjRPC;
		SerializedProperty toNextSibling;
		SerializedProperty mustBeActive;
		SerializedProperty destroyOnMouseDown;
		SerializedProperty uiEventSystemMustBeActive;
		
		bool fold, fold2;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
            EditorGUILayout.HelpBox( "This class allows doing something when this object is destroyed", MessageType.Info, true );
			EditorGUILayout.PropertyField( destroyAfter, new GUIContent( "Destroy After" ), true );
            if( destroyAfter.floatValue > 0f && substitutes.arraySize > 0 )
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField( showBeforeDestruction, new GUIContent( "Show Before Destruction" ) );
                EditorGUI.indentLevel--;
            }

			GUILineStyle.Draw();
			fold = EditorGUILayout.Foldout( fold, new GUIContent("Substitions", "This controls other objects spawning when this one" +
			                                                     " is destroyed") );
			if( fold )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( substitutes, new GUIContent( "Substitues" ), true );
				EditorGUILayout.PropertyField( chancesOSLS, new GUIContent( "Chances of Showing Last Substitute" ), true );
				EditorGUILayout.PropertyField( lastObjRPC, new GUIContent( "Last Obj Random Pick chances" ), true );
				EditorGUI.indentLevel--;
			}
			GUILineStyle.Draw();
			
			fold2 = EditorGUILayout.Foldout( fold2, new GUIContent("Transfer Tag") );
			if( fold2 )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( toNextSibling, new GUIContent( "To Next Sibling" ), true );
				if( toNextSibling.boolValue )
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( mustBeActive, new GUIContent( "Must Be Active" ), true );
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			GUILineStyle.Draw();
			
			EditorGUILayout.PropertyField( destroyOnMouseDown, new GUIContent( "Destroy On Mouse Down Anywhere"), true );
			if( destroyOnMouseDown.boolValue )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( uiEventSystemMustBeActive, new GUIContent( "UI Event System Must Be Active" ), true );
				EditorGUI.indentLevel--;
			}
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
			destroyAfter = serializedObject.FindProperty("destroyAfter");
            showBeforeDestruction = serializedObject.FindProperty("showBeforeDestruction");
			substitutes = serializedObject.FindProperty("substitutes");
			chancesOSLS = serializedObject.FindProperty("chancesOfShowingLastSubstitute");
			lastObjRPC = serializedObject.FindProperty("lastObjRandomPickChances");
			toNextSibling = serializedObject.FindProperty("toNextSibling");
			mustBeActive = serializedObject.FindProperty("mustBeActive");
			destroyOnMouseDown = serializedObject.FindProperty("destroyOnMouseDown");
			uiEventSystemMustBeActive = serializedObject.FindProperty("uiEventSystemMustBeActive");
		}
		
		
		
	}


}