//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// This can be used by any script to inherit from and add more functionality to Unity's MonoBehaviour, 
    /// specifically for save/load related actions.
    /// </summary>
	public class MonoBehaviourPlayerPrefs : MonoBehaviourExt 
    {		
		public SaveLoadPersistent saveLoad = new SaveLoadPersistent();
		[System.Serializable]
		public class SaveLoadPersistent{
			
			/// <summary>
			/// Allows to be stored in PlayerPrefs.
			/// </summary>
			[Tooltip("Allows information to be stored in PlayerPrefs")]
			public bool storeData = true;
			public bool savePrefsOnStart = false;
			public bool deleteAllDataOnNextRun = false;
			public bool loadAllOnStart = false;
			/// <summary>
			/// The stored data will be auto saved every -?- seconds. If Update() is called the objects saved to the Persistent
			///class objs hashtable will be saved. For other implementations, the programmer must implement the use of this variable.
			/// </summary>
			[Tooltip("The stored data will be auto saved every -?- seconds. If Update() is called the objects saved to the Persistent"+
			" class objs hashtable will be saved. For other implementations, the programmer must implement the use of this variable.")]
			public int autoSaveEvery = 30;
		}
				
		
		
		internal static string previousLevel;
		internal static GameObject previousScene;
		/// <summary>
		/// Indicates a file has been loaded. Must be manually setup and reset. Unless an instance of this class is active,
		/// if so, it will reset at the Start()'s end frame.
		/// </summary>
		internal static bool loadedFile = false;

		
		
		// Use this for initialization
		protected void Start () {
			saveLoad.autoSaveEvery *= 60;
			if( saveLoad.savePrefsOnStart )
				PlayerPrefs.Save();
			if( saveLoad.deleteAllDataOnNextRun )
				PlayerPrefs.DeleteAll();
			/*if( saveLoad.loadAllOnStart )
			Persistent.LoadAll( Game.appName, Game.playerName );*/
			
			//StartCoroutine( ResetNewFile() );
			StartCoroutine( ResetLoadedFile() );
			
		}
		
		void OnApplicationQuit()//Does work properly on some platforms
		{
			if( saveLoad.deleteAllDataOnNextRun )
				PlayerPrefs.DeleteAll();
			/*else
			Persistent.SaveAll();*/
		}		
		
		
		
		/*public static IEnumerator ResetNewFile()
	{
		if( NewFile.newFile ){

			yield return null;
			NewFile.newFile = false;
		}
	}*/
		
		public static IEnumerator ResetLoadedFile()
		{
			if( loadedFile ){
				
				yield return null;
				loadedFile= false;
			}
		}		
	}
}