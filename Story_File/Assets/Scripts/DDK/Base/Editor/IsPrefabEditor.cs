using UnityEngine;
using System.Collections;
using UnityEditor;
using DDK.Base.Components;


namespace DDK 
{
	[CustomEditor(typeof(IsPrefab))]
	public class IsPrefabEditor : Editor 
	{
        IsPrefab _script;
		

		public override void OnInspectorGUI()
		{
			_script = (IsPrefab) target;
			
			EditorGUILayout.HelpBox("This helps identify prefab objects by calling IsPrefab() extension methods, you can also " +
			                        "call GetPrefabInstance() GameObject extension method. NOTE: This shouldn't be added manually! The first time " +
			                        "IsPrefab() is called this will be permanently added to prefabs so future executions can work properly, but " +
			                        "MAKE SURE already instantiated prefabs have <(Clone)> in their names, at least the top parent.", MessageType.Info);
			if( PrefabUtility.GetPrefabType(_script.gameObject) == PrefabType.None )
			{
				EditorGUILayout.LabelField( "Instance Id: "+_script.id );
			}
			else EditorGUILayout.LabelField( "Id: "+_script.id );
		}

		
		public void OnEnable()
		{
			EditorApplication.update += _CallbackFunction;
		}		
		public void OnDisable()
		{
			EditorApplication.update -= _CallbackFunction;
		}
		

		private void _CallbackFunction()
		{
			foreach ( IsPrefab obj in targets )
			{
				if( obj )
				{
					if( PrefabUtility.GetPrefabType(obj.gameObject) == PrefabType.None )//PREVENT EXECUTION ON PREFABS IN EDITOR MODE
					{
						obj.CheckPrefab();
					}
				}
			}
		}		
	}
}
