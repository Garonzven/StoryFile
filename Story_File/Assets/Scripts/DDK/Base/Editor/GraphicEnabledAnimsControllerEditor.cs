using UnityEngine;
using System.Collections;
using UnityEditor;

using DDK.Base.UI;
using DDK.Base.Misc;


namespace DDK {

	[CustomEditor(typeof(GraphicEnabledAnimsController))]
	[CanEditMultipleObjects]
	public class GraphicEnabledAnimsControllerEditor : Editor 
	{
		SerializedProperty enabledColor;
		SerializedProperty avoidAlpha;
		SerializedProperty disabledColorIsSameAsOriginal;
		SerializedProperty disabledColor;
		SerializedProperty applyToSiblingsOnEnableAnim;
		SerializedProperty siblingsColor;
		SerializedProperty includeSiblingsSubChildren;
		SerializedProperty includeInactive;
		SerializedProperty enabledScl;
		SerializedProperty animDuration;

		SerializedProperty disableAfterAnimDone;
		SerializedProperty destroyAfterAnimDone;
		SerializedProperty enableOnNextSiblingAfterDone;
		SerializedProperty onAnimationEnd;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.PropertyField( enabledColor, new GUIContent( "Enabled Color" ) );
			EditorGUILayout.PropertyField( avoidAlpha, new GUIContent( "Avoid Alpha Channel" ) );
			if( !disabledColorIsSameAsOriginal.boolValue )
			{
				GUILineStyle.Draw();
			}
			EditorGUILayout.PropertyField(  disabledColorIsSameAsOriginal, new GUIContent( "Disabled Color is Same as Original" ) );
			if( !disabledColorIsSameAsOriginal.boolValue )
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( disabledColor, new GUIContent( "Disabled Color" ) );
				EditorGUI.indentLevel = 0;
				GUILineStyle.Draw();
			}
			
			if( applyToSiblingsOnEnableAnim.boolValue )
			{
				GUILineStyle.Draw();
			}
			EditorGUILayout.PropertyField( applyToSiblingsOnEnableAnim, new GUIContent( "Apply to Siblings " +
			                                                                           "On Enable Anim" ) );
			if( applyToSiblingsOnEnableAnim.boolValue )
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( siblingsColor, new GUIContent( "Siblings Color" ) );
				EditorGUILayout.PropertyField( includeSiblingsSubChildren, new GUIContent( "Include Siblings Sub Children" ) );
				EditorGUILayout.PropertyField( includeInactive, new GUIContent( "Include Inactive" ) );
				EditorGUI.indentLevel = 0;
			}
			GUILineStyle.Draw();
			EditorGUILayout.PropertyField( enabledScl, new GUIContent( "Enabled Scale" ) );
			EditorGUILayout.PropertyField( animDuration, new GUIContent( "Anim Duration" ) );
			EditorGUILayout.PropertyField( disableAfterAnimDone, new GUIContent( "Disable After Anim Done" ) );
			EditorGUILayout.PropertyField( destroyAfterAnimDone, new GUIContent( "Destroy After Anim Done" ) );
			EditorGUILayout.PropertyField( enableOnNextSiblingAfterDone, new GUIContent( "Enable On Next Sibling After Done" ) );
			EditorGUILayout.PropertyField( onAnimationEnd, new GUIContent( "On Animation End" ), true );
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
			enabledColor = serializedObject.FindProperty("enabledColor");
			avoidAlpha = serializedObject.FindProperty("avoidAlpha");
			disabledColorIsSameAsOriginal = serializedObject.FindProperty("disabledColorIsSameAsOriginal");
			disabledColor = serializedObject.FindProperty("disabledColor");
			enabledScl = serializedObject.FindProperty("enabledScl");
			animDuration = serializedObject.FindProperty("animDuration");
			applyToSiblingsOnEnableAnim = serializedObject.FindProperty("applyToSiblingsOnEnableAnim");
			siblingsColor = serializedObject.FindProperty("siblingsColor");
			includeSiblingsSubChildren = serializedObject.FindProperty("includeSiblingsSubChildren");
			includeInactive = serializedObject.FindProperty("includeInactive");

			disableAfterAnimDone = serializedObject.FindProperty("disableAfterAnimDone");
			destroyAfterAnimDone = serializedObject.FindProperty("destroyAfterAnimDone");
			enableOnNextSiblingAfterDone = serializedObject.FindProperty("enableOnNextSiblingAfterDone");
			onAnimationEnd = serializedObject.FindProperty("_onAnimationEnd");
		}
		
	}

}
