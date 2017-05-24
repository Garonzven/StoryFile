using UnityEngine;
using System.Collections;
using UnityEditor;

using DDK.Base.Misc;
using DDK.ObjManagement;


namespace DDK {

	[CustomEditor(typeof(MoveTowards))]
	[CanEditMultipleObjects]
	public class MoveTowardsEditor : Editor 
	{
		SerializedProperty delay;
		SerializedProperty useRectTransform;
		SerializedProperty fromNameInstead;
		SerializedProperty fromName;
		SerializedProperty fromTagInstead;
		SerializedProperty fromTag;
		SerializedProperty from;
		SerializedProperty when;
		SerializedProperty frameRate;
		SerializedProperty rotateTowardsInstead;
		SerializedProperty targetRot;
		SerializedProperty speed;
		SerializedProperty targetNamesInstead;
		SerializedProperty overrideSpeeds;
		SerializedProperty _targetNames;
		SerializedProperty _targets;
		SerializedProperty mSpeeds;
		SerializedProperty activateBNI;
		SerializedProperty aatr;
		SerializedProperty eatr;
		SerializedProperty aatrN;
		SerializedProperty enableInThisObj;
		SerializedProperty aaltr;
		SerializedProperty ealtr;
		SerializedProperty aaltrN;
		SerializedProperty sfx;
		SerializedProperty stopAllSBPSfx;
		SerializedProperty executeOnTargetReachedOnDestroy;
		SerializedProperty destroyWhenTargetReached;
		SerializedProperty repeatMovementWhenLastTargetReached;
		SerializedProperty playSfxEvenIfRepeat;
		SerializedProperty keepMovingTowardsLast;

		bool fromFold;
		bool eventsFold;

		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.PropertyField( delay, new GUIContent( "Delay" ) );
			EditorGUILayout.PropertyField( speed, new GUIContent( "Speed" ) );
			EditorGUILayout.PropertyField( useRectTransform, new GUIContent( "Use RectTransform" ) );
			//---------------------------------
			fromFold = EditorGUILayout.Foldout( fromFold, "From" );
			if( fromFold )
			{
				if( !fromTagInstead.boolValue )
					EditorGUILayout.PropertyField(  fromNameInstead, new GUIContent( "Search by Name" ), true );
				if( !fromNameInstead.boolValue )
					EditorGUILayout.PropertyField(  fromTagInstead, new GUIContent( "Search by Tag" ), true );
				if( fromNameInstead.boolValue )
				{
					EditorGUILayout.PropertyField(  fromName, new GUIContent( "Obj Name" ), true );
				}
				else if( fromTagInstead.boolValue )
				{
					EditorGUILayout.PropertyField(  fromTag, new GUIContent( "Obj Tag" ), true );
				}
				else EditorGUILayout.PropertyField(  from, new GUIContent( "Object" ), true );
			}
			//---------------------------------
			EditorGUILayout.PropertyField(  when, new GUIContent( "When", ".. The specified happens" ), true );
			EditorGUILayout.PropertyField( frameRate, new GUIContent( "Frame Rate" ) );
			EditorGUILayout.PropertyField( rotateTowardsInstead, new GUIContent( "Rotate Towards Instead" ) );
			if( rotateTowardsInstead.boolValue )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( targetRot, new GUIContent( "Target Rotation" ), true );
				EditorGUILayout.PropertyField( speed, new GUIContent( "Rotation Speed" ) );
				EditorGUI.indentLevel--;
				EditorGUILayout.PropertyField( aatr, new GUIContent("Activate After Target Reached"), true );
				DoEnableInThisObj();
				DoSfxLayout();
				EditorGUILayout.PropertyField( destroyWhenTargetReached, new GUIContent("Destroy When Last Target Reached") );
			}
			else
			{
				EditorGUILayout.PropertyField( targetNamesInstead, new GUIContent("Target Names Instead"), true );
				EditorGUI.indentLevel++;
				if( targetNamesInstead.boolValue )
				{
					EditorGUILayout.PropertyField( _targetNames, new GUIContent("Target Names"), true );
					EditorGUI.indentLevel--;
					if( _targetNames.arraySize > 0 )
					{
						if( !string.IsNullOrEmpty( _targetNames.GetArrayElementAtIndex(0).stringValue ) )
						{
							DoTargetsProps();
						}
					}
				}
				else 
				{
					EditorGUILayout.PropertyField( _targets, new GUIContent("Targets"), true );
					EditorGUI.indentLevel--;
					if( _targets.arraySize > 0 )
					{
						if( _targets.GetArrayElementAtIndex(0).objectReferenceValue != null )
						{
							DoTargetsProps();
						}
					}
				}
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		private void DoTargetsProps()
		{
			EditorGUILayout.PropertyField( overrideSpeeds, new GUIContent("Override Movement Speeds") );
			EditorGUI.indentLevel++;
			if( overrideSpeeds.boolValue )
			{
				EditorGUILayout.PropertyField( speed, new GUIContent("Movement Speed") );
			}
			else EditorGUILayout.PropertyField( mSpeeds, new GUIContent("Movement Speeds"), true );
			EditorGUI.indentLevel--;

			eventsFold = EditorGUILayout.Foldout( eventsFold, "Events" );
			if( eventsFold )
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField( activateBNI, new GUIContent("Activate By Names Instead"), true );
				if( activateBNI.boolValue )
				{
					EditorGUILayout.PropertyField( aatrN, new GUIContent("Activate After Target Reached"), true );
					EditorGUILayout.PropertyField( aaltrN, new GUIContent("Activate After Last Target Reached"), true );
				}
				else
				{
					EditorGUILayout.PropertyField( aatr, new GUIContent("Activate After Target Reached"), true );
					EditorGUILayout.PropertyField( aaltr, new GUIContent("Activate After Last Target Reached"), true );
				}
				
				DoEnableInThisObj();
				
				EditorGUILayout.PropertyField( ealtr, new GUIContent("Enable After Last Target Reached"), true );
				DoSfxLayout();
				EditorGUILayout.PropertyField( executeOnTargetReachedOnDestroy, new GUIContent("Execute On Target Reached On Destroy") );
				EditorGUILayout.PropertyField( destroyWhenTargetReached, new GUIContent("Destroy When Last Target Reached") );
				EditorGUILayout.PropertyField( repeatMovementWhenLastTargetReached, new GUIContent("Repeat Movement when Last Target Reached") );
				if( repeatMovementWhenLastTargetReached.boolValue )
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( playSfxEvenIfRepeat, new GUIContent("Play Sfx Even if Repeat") );
					EditorGUI.indentLevel--;
				}
				if( rotateTowardsInstead.boolValue )
				{
					EditorGUILayout.PropertyField( keepMovingTowardsLast, new GUIContent("Keep Rotating Towards Last") );
				}
				else EditorGUILayout.PropertyField( keepMovingTowardsLast, new GUIContent("Keep Moving Towards Last") );

				EditorGUI.indentLevel--;
			}
		}
		
		private void DoSfxLayout()
		{
			int sfxCount = 0;
			#pragma warning disable 168
			foreach( var s in sfx )
			{
				sfxCount++;
			}
			#pragma warning restore 168
			if( sfxCount > 0 )
			{
				GUILineStyle.Draw();
				EditorGUILayout.PropertyField( sfx, new GUIContent("Sfx"), true );
				EditorGUILayout.PropertyField( stopAllSBPSfx, new GUIContent("Stop All Sounds Before Playing Sfx"), true );
				GUILineStyle.Draw();
			}
			else EditorGUILayout.PropertyField( sfx, new GUIContent("Sfx"), true );
		}
		
		private void DoEnableInThisObj()
		{
			int count=0;
			#pragma warning disable 168
			foreach( var comp in eatr )
			{
				count++;
			}
			#pragma warning restore 168
			if( count > 0 )
			{
				GUILineStyle.Draw();
				EditorGUILayout.PropertyField( eatr, new GUIContent("Enable After Target Reached"), true );
				EditorGUILayout.PropertyField( enableInThisObj, new GUIContent("Enable In This Obj"), true );
				GUILineStyle.Draw();
			}
			else EditorGUILayout.PropertyField( eatr, new GUIContent("Enable After Target Reached"), true );
		}
		
		public void OnEnable()
		{
			delay = serializedObject.FindProperty("delay");
			useRectTransform = serializedObject.FindProperty("useRectTransform");
			fromNameInstead = serializedObject.FindProperty("fromNameInstead");
			fromName = serializedObject.FindProperty("fromName");
			fromTagInstead = serializedObject.FindProperty("fromTagInstead");
			fromTag = serializedObject.FindProperty("fromTag");
			from = serializedObject.FindProperty("from");
			when = serializedObject.FindProperty("when");
			frameRate = serializedObject.FindProperty("frameRate");
			rotateTowardsInstead = serializedObject.FindProperty("rotateTowardsInstead");
			targetRot = serializedObject.FindProperty("targetRot");
			speed = serializedObject.FindProperty("speed");
			targetNamesInstead = serializedObject.FindProperty("targetNamesInstead");
			overrideSpeeds = serializedObject.FindProperty("overrideSpeeds");
			_targetNames = serializedObject.FindProperty("targetNames");
			_targets = serializedObject.FindProperty("targets");
			mSpeeds = serializedObject.FindProperty("movementSpeeds");
			activateBNI = serializedObject.FindProperty("activateByNamesInstead");
			aatr = serializedObject.FindProperty("activateAfterTargetReached");
			eatr = serializedObject.FindProperty("enableAfterTargetReached");
			aatrN = serializedObject.FindProperty("activateAfterTargetReachedNames");
			enableInThisObj = serializedObject.FindProperty("enableInThisObj");
			aaltr = serializedObject.FindProperty("activateAfterLastTargetReached");
			ealtr = serializedObject.FindProperty("enableAfterLastTargetReached");
			aaltrN = serializedObject.FindProperty("activateAfterLastTargetReachedNames");
			sfx = serializedObject.FindProperty("sfx");
			stopAllSBPSfx = serializedObject.FindProperty("stopAllSoundsBeforePlayingSfx");
			executeOnTargetReachedOnDestroy = serializedObject.FindProperty( "executeOnTargetReachedOnDestroy" );
			destroyWhenTargetReached = serializedObject.FindProperty("destroyWhenTargetReached");
			repeatMovementWhenLastTargetReached = serializedObject.FindProperty("repeatMovementWhenLastTargetReached");
			playSfxEvenIfRepeat = serializedObject.FindProperty("playSfxEvenIfRepeat");
			keepMovingTowardsLast = serializedObject.FindProperty("keepMovingTowardsLast");
		}
		
	}

}
