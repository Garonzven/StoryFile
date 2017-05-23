//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEditor;
using DDK.GamesDevelopment;
using DDK.Base.Misc;


namespace DDK {

	[CustomEditor(typeof(DraggableLineRenderer))]
	[CanEditMultipleObjects]
	public class DraggableLineRendererEditor : Editor 
	{
		SerializedProperty autoDropOnTargetEnter;
		SerializedProperty beginTargetDragOnEnter;
		SerializedProperty makeTargetInteractable;
		SerializedProperty makeOthersInteractable;
		SerializedProperty makeOthersNotInteractable;
		
		SerializedProperty dropOnLastTouchedTarget;
		SerializedProperty minDistanceToLastTouchedTarget;
		
		SerializedProperty lineRendererPosZ;
		SerializedProperty snapToDropTargets;
		SerializedProperty snapOnlyToDropTargets;
		
		SerializedProperty dropTarget;
		SerializedProperty dropTargetIsChild;
		SerializedProperty disableCompWhenDropOnTarget;
		SerializedProperty disableInteractableWhenDropOnTarget;
			SerializedProperty disableImageAlso;
		SerializedProperty allDropTargetsHaveMultipleParents;
		SerializedProperty allDropTargetsParent;
		SerializedProperty allDropTargetsParentsParent;
		SerializedProperty onDropOnTarget;
		SerializedProperty onDropOnOtherTarget;
		SerializedProperty srcName;



		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//autoDropOnTargetEnter.boolValue = !( !autoDropOnTargetEnter.boolValue | dropOnLastTouchedTarget.boolValue );
			if( !dropOnLastTouchedTarget.boolValue )
			{
				EditorGUILayout.PropertyField( autoDropOnTargetEnter, new GUIContent("Auto Drop on Target Enter") );
			}
			//dropOnLastTouchedTarget.boolValue = !( autoDropOnTargetEnter.boolValue | !dropOnLastTouchedTarget.boolValue );
			if( !autoDropOnTargetEnter.boolValue )
			{
				EditorGUILayout.PropertyField( dropOnLastTouchedTarget, new GUIContent("Drop on Last Touched Target") );
			}

			EditorGUI.indentLevel = 1;
			if( dropOnLastTouchedTarget.boolValue )
			{
				EditorGUILayout.PropertyField( minDistanceToLastTouchedTarget, new GUIContent("Min Distance to Last Touched Target"), true );
			}
			else if( autoDropOnTargetEnter.boolValue )
			{
				EditorGUILayout.PropertyField( beginTargetDragOnEnter, new GUIContent("Begin Target Drag On Enter") );
				if( beginTargetDragOnEnter.boolValue )
				{
					EditorGUI.indentLevel = 2;
					EditorGUILayout.PropertyField( makeTargetInteractable, new GUIContent("Make Target Interactable") );
					EditorGUILayout.PropertyField( makeOthersInteractable, new GUIContent("Make Others Interactable"), true );
					EditorGUILayout.PropertyField( makeOthersNotInteractable, new GUIContent("Make Others Not Interactable"), true );
					EditorGUI.indentLevel = 1;
				}
			}
			EditorGUI.indentLevel = 0;
			GUILineStyle.Draw();

			EditorGUILayout.PropertyField( lineRendererPosZ, new GUIContent("Line Renderer Z Position"), true );
			EditorGUILayout.PropertyField( snapToDropTargets, new GUIContent("Snap to Drop Targets") );
			if( snapToDropTargets.boolValue )
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( snapOnlyToDropTargets, new GUIContent("Snap Only to Drop Targets") );
				EditorGUI.indentLevel = 0;
			}

			EditorGUILayout.PropertyField( dropTarget, new GUIContent("Drop Target"), true );
			
			if( dropTarget.objectReferenceValue )
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( dropTargetIsChild, new GUIContent("Drop Target is Child") );
				EditorGUILayout.PropertyField( disableCompWhenDropOnTarget, new GUIContent("Disable Comp when Drop On Target") );
				EditorGUILayout.PropertyField( disableInteractableWhenDropOnTarget, new GUIContent("Disable Interactable when Drop On Target") );
				if( disableInteractableWhenDropOnTarget.boolValue )
				{
					EditorGUI.indentLevel = 2;
					EditorGUILayout.PropertyField( disableImageAlso, new GUIContent("Disable Image Also") );
					EditorGUI.indentLevel = 1;
				}

				EditorGUILayout.PropertyField( allDropTargetsHaveMultipleParents, new GUIContent("All Drop Targets Have Multiple Parents") );
				if( allDropTargetsHaveMultipleParents.boolValue )
				{
					EditorGUI.indentLevel = 2;
					EditorGUILayout.PropertyField( allDropTargetsParentsParent, new GUIContent( "All Drop Targets Parents Parent" ), true );
					EditorGUI.indentLevel = 1;
				}
				else EditorGUILayout.PropertyField( allDropTargetsParent, new GUIContent("All Drop Targets Parent"), true );

				EditorGUILayout.PropertyField( onDropOnTarget, new GUIContent( "On Drop on Target" ), true );
				EditorGUILayout.PropertyField( onDropOnOtherTarget, new GUIContent( "On Drop on Other Target" ), true );
				EditorGUILayout.PropertyField( srcName, new GUIContent("Source Name"), true );
				EditorGUI.indentLevel = 0;
			}
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
			autoDropOnTargetEnter = serializedObject.FindProperty( "autoDropOnTargetEnter" );
			beginTargetDragOnEnter = serializedObject.FindProperty( "beginTargetDragOnEnter" );
			makeTargetInteractable = serializedObject.FindProperty( "makeTargetInteractable" );
			makeOthersInteractable = serializedObject.FindProperty( "makeOthersInteractable" );
			makeOthersNotInteractable = serializedObject.FindProperty( "makeOthersNotInteractable" );

			dropOnLastTouchedTarget = serializedObject.FindProperty( "dropOnLastTouchedTarget" );
			minDistanceToLastTouchedTarget = serializedObject.FindProperty( "minDistanceToLastTouchedTarget" );

			lineRendererPosZ = serializedObject.FindProperty( "lineRendererPosZ" );
			snapToDropTargets = serializedObject.FindProperty( "snapToDropTargets" );
			snapOnlyToDropTargets = serializedObject.FindProperty( "snapOnlyToDropTargets" );

			dropTarget = serializedObject.FindProperty( "dropTarget" );
			dropTargetIsChild = serializedObject.FindProperty( "dropTargetIsChild" );
			disableCompWhenDropOnTarget = serializedObject.FindProperty( "disableCompWhenDropOnTarget" );
			disableInteractableWhenDropOnTarget = serializedObject.FindProperty( "disableInteractableWhenDropOnTarget" );
			disableImageAlso = serializedObject.FindProperty( "disableImageAlso" );
			allDropTargetsHaveMultipleParents = serializedObject.FindProperty( "allDropTargetsHaveMultipleParents" );
			allDropTargetsParent = serializedObject.FindProperty( "allDropTargetsParent" );
			allDropTargetsParentsParent = serializedObject.FindProperty( "allDropTargetsParentsParent" );
			onDropOnTarget = serializedObject.FindProperty( "onDropOnTarget" );
			onDropOnOtherTarget = serializedObject.FindProperty( "onDropOnOtherTarget" );
			srcName = serializedObject.FindProperty( "srcName" );

		}
		
	}

}


