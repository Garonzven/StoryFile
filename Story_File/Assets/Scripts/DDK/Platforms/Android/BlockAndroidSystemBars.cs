using UnityEngine;


namespace DDK.Platforms.Android
{
	/// <summary>
	/// Prevents Android System Bars from showing up. User can slide status bar and notifications since API 16 (Honeycomb),
	/// this prevents that future from being used.
	/// </summary>
	public class BlockAndroidSystemBars : MonoBehaviour 
    {		
		#if UNITY_ANDROID
		static AndroidJavaClass systemBarsListener;
		public static BlockAndroidSystemBars Instance;
		
		
		void Start()
		{
			#if !UNITY_EDITOR
			var playerActivity = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			var currentActivity = playerActivity.GetStatic<AndroidJavaObject>("currentActivity");
			
			systemBarsListener = new AndroidJavaClass("com.DanielSoto.systembarslistener.SystemBarsListener");
			
			systemBarsListener.CallStatic("setContext", currentActivity);
			#endif
			Instance = this;
		}
        #endif		
	}
}
