using UnityEngine;
using System.Collections;
using UnityEditor;

using DDK.ObjManagement;


namespace DDK 
{
	[CustomEditor(typeof(ObjAutoScale))]
	[CanEditMultipleObjects]
	public class ObjAutoScaleEditor : Editor 
	{
		ObjAutoScale script;
		SerializedProperty largestScl;
		SerializedProperty squarestScl;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			script = (ObjAutoScale) target;
			
			EditorGUILayout.HelpBox( "Largest Aspect is "+ script.original_largestAspect.ToString() +"\n"+ "Squarest Aspect is "
			                        + script.widestAspect.ToString() + "\n(Orientation doesn't matter)", MessageType.Info );
			EditorGUILayout.PropertyField( largestScl, new GUIContent( "Largest Scl: " ) );
			EditorGUILayout.PropertyField(  squarestScl, new GUIContent( "Squarest Scl: " ) );
			
			serializedObject.ApplyModifiedProperties();
		}
		
		public void OnEnable()
		{
			largestScl = serializedObject.FindProperty("originalScl");
			squarestScl = serializedObject.FindProperty("widestTargetScl");
			EditorApplication.update += CallbackFunction;
		}
		
		public void OnDisable()
		{
			EditorApplication.update -= CallbackFunction;
		}
		
		private void CallbackFunction()
		{
			//EditorUtility.SetDirty(script);
			foreach ( ObjAutoScale obj in targets )
			{
				obj.Update();
			}
		}		
	}
}