using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using DDK.Base.Extensions;
using UnityEngine.SceneManagement;


namespace DDK.Base.Events
{
	/// <summary>
	/// This can be added to an object that won't be destroyed to execute events after loading a level.
	/// </summary>
	public class OnLevelLoaded : MonoBehaviourExt
	{		
		public ComposedEvent onLevelWasLoaded = new ComposedEvent();
		[Header("Conditions")]
		[Tooltip("If an index matches the loaded level's index, the /onLevelWasLoaded/ event won't be invoked")]
		public int[] notAcceptedLevelsIndexes = new int[0];
		[Tooltip("If a name matches the loaded level's name, the /onLevelWasLoaded/ event won't be invoked")]
		public string[] notAcceptedLevelsNames = new string[0];


        #if UNITY_5_4_OR_NEWER
        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded( Scene scene, LoadSceneMode mode ) 
        {
            OnSceneLoaded( scene.buildIndex );
        }
        #elif UNITY_5_2 || UNITY_5_3
        void OnLevelWasLoaded( int lvl ) 
        {
            OnSceneLoaded( lvl );
        }
        #endif


        void OnSceneLoaded( int index ) 
        {
            if( notAcceptedLevelsIndexes.Contains( index ) )
                return;
            if( notAcceptedLevelsNames.Contains( SceneManager.GetActiveScene().name ) )
                return;
            onLevelWasLoaded.Invoke ();
        }
	}
}
