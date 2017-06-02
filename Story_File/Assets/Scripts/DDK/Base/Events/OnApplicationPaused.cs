using UnityEngine;
using DDK.Base.Classes;


namespace DDK.Base.Events
{
    /// <summary>
    /// Handles On Application Paused events from the inspector.
    /// </summary>
	public class OnApplicationPaused : MonoBehaviour 
    {
		[Tooltip("If true, the Player.Prefs will be saved on pause")]
		public bool savePlayerPrefs;
		public ComposedEvent onPause = new ComposedEvent();
		public ComposedEvent onResume = new ComposedEvent();


		void OnApplicationPause( bool paused )
		{
			if( !enabled )
				return;
			if( paused )
			{
				onPause.Invoke ();
				if( savePlayerPrefs )
					PlayerPrefs.Save ();
			}
			else
			{
				onResume.Invoke ();
			}
		}
	}
}
