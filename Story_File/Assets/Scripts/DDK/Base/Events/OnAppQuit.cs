using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using UnityEngine.Events;


namespace DDK.Base.Events
{
    /// <summary>
    /// Handles On Application Quit events from the inspector.
    /// </summary>
	public class OnAppQuit : MonoBehaviour 
    {
		[Tooltip("If true, the Player.Prefs will be saved on quit")]
		public bool savePlayerPrefs;
		public UnityEvent onQuit = new UnityEvent();


		public static bool _Quitting { get; private set; }


		void Start() {} //Allow to enable/disable this component
		void OnApplicationQuit()
		{
			if( !enabled )
				return;
			_Quitting = true;
			onQuit.Invoke ();
			if( savePlayerPrefs )
				PlayerPrefs.Save ();
		}
	}
}
