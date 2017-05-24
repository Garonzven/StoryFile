//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using DDK.Base.UI;
using DDK.Base.Misc;


namespace DDK 
{
    /// <summary>
    /// Custom editor class for DraggableImage.
    /// </summary>
    /// <see cref="DDK.Base.UI.DraggableImage"/>
	[CustomEditor(typeof(DraggableImage))]
	[CanEditMultipleObjects]
	public class DraggableImageEditor : Editor 
	{
		SerializedProperty iconPath;
		SerializedProperty dragOnSurfaces;
		SerializedProperty setNativeSize;
		SerializedProperty size;
		SerializedProperty beenDraggedSourceOpacity;
		SerializedProperty mTarget;
		SerializedProperty playWhenDroppedOnTarget;
		SerializedProperty enableWDOT;
		SerializedProperty copyToIcon;
		SerializedProperty parentIconToT;
		SerializedProperty iconTSI;
		SerializedProperty destroyIconDelay;
		SerializedProperty destroyIfDroppedOnTarget;
		SerializedProperty destroyDelay;
		SerializedProperty incorrectTargets;
		SerializedProperty enableWhenDroppedOnIncorrectTarget;
		SerializedProperty playWhenDroppedOnIncorrectTargets;
		
        bool _fold1, _fold2;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField( iconPath, new GUIContent("Icon Path"), true );
			EditorGUILayout.PropertyField( dragOnSurfaces, new GUIContent("Drag On Surfaces") );
			EditorGUILayout.PropertyField( setNativeSize, new GUIContent("Set Native Icon Size") );
			if( !setNativeSize.boolValue )
			{
				EditorGUILayout.PropertyField( size, new GUIContent("Icon Size"), true );
				EditorGUILayout.Space();
			}
			EditorGUILayout.PropertyField( copyToIcon, new GUIContent("Copy to Icon"), true );

            EditorGUILayout.Slider( beenDraggedSourceOpacity, 0f, 1f, new GUIContent( "Been Dragged Source Opacity", 
                "The opacity (alpha value) of the source image when dragging" ) );
			
			_fold1 = EditorGUILayout.Foldout( _fold1, "Target" );
			if( _fold1 )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( mTarget, new GUIContent("Drop Target"), true );
				if( mTarget.objectReferenceValue )
				{
					//------------------------------
					if( parentIconToT.boolValue )
					{
						GUILineStyle.Draw();
					}
					EditorGUILayout.PropertyField( parentIconToT, new GUIContent( "Parent Icon To Target" ), true );
					if( parentIconToT.boolValue )
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField( iconTSI, new GUIContent( "Icon Target Sibling Index" ), true );
						EditorGUI.indentLevel--;
						GUILineStyle.Draw();
					}
					//------------------------------
					EditorGUILayout.PropertyField( enableWDOT, new GUIContent( "Enable When Dropped On Target" ), true );
					//------------------------------
					if( destroyIfDroppedOnTarget.boolValue )
					{
						GUILineStyle.Draw();
					}
                    if( !parentIconToT.boolValue )
                    {
                        EditorGUILayout.PropertyField( destroyIconDelay, new GUIContent("Destroy Icon Delay", "The destroy icon delay ONLY" +
                            " when dropped on target"), true );
                    }
					EditorGUILayout.PropertyField( destroyIfDroppedOnTarget, new GUIContent("Destroy Obj If Dropped On Target"), true );
					if( destroyIfDroppedOnTarget.boolValue )
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField( destroyDelay, new GUIContent("Destroy Delay"), true );
						EditorGUI.indentLevel--;
						GUILineStyle.Draw();
					}
					//------------------------------
					EditorGUILayout.PropertyField( playWhenDroppedOnTarget, new GUIContent("Play When Dropped On Target"), true );
					//------------------------------
					GUILineStyle.Draw();
				}
				EditorGUI.indentLevel--;
			}
			_fold2 = EditorGUILayout.Foldout( _fold2, "Incorrect Targets" );
			if( _fold2 )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( incorrectTargets, new GUIContent("Incorrect Targets"), true );
				EditorGUILayout.PropertyField( enableWhenDroppedOnIncorrectTarget, new GUIContent( "Enable When Dropped On Incorrect Target" ), true );
				EditorGUILayout.PropertyField( playWhenDroppedOnIncorrectTargets, new GUIContent("Play When Dropped On Incorrect Targets"), true );
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
			iconPath = serializedObject.FindProperty("iconPath");
			dragOnSurfaces = serializedObject.FindProperty("dragOnSurfaces");
			setNativeSize = serializedObject.FindProperty("setNativeSize");
			size = serializedObject.FindProperty("size");
			beenDraggedSourceOpacity = serializedObject.FindProperty("beenDraggedSourceOpacity");
			mTarget = serializedObject.FindProperty("target");
			playWhenDroppedOnTarget = serializedObject.FindProperty("playWhenDroppedOnTarget");
			enableWDOT = serializedObject.FindProperty("enableWhenDroppedOnTarget");
			copyToIcon = serializedObject.FindProperty("copyToIcon");
			destroyIconDelay = serializedObject.FindProperty("destroyIconDelay");
			parentIconToT = serializedObject.FindProperty("parentIconToTarget");
			iconTSI = serializedObject.FindProperty("iconTargetSiblingIndex");
			destroyDelay = serializedObject.FindProperty("destroyDelay");
			destroyIfDroppedOnTarget = serializedObject.FindProperty("destroyObjIfDroppedOnTarget");
			incorrectTargets = serializedObject.FindProperty("incorrectTargets");
			enableWhenDroppedOnIncorrectTarget = serializedObject.FindProperty("enableWhenDroppedOnIncorrectTarget");
			playWhenDroppedOnIncorrectTargets = serializedObject.FindProperty("playWhenDroppedOnIncorrectTargets");
		}		
	}
}