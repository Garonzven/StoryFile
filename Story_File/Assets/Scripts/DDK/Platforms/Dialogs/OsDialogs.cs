using UnityEngine;
using System.Collections;


namespace DDK.Platforms.Dialogs 
{
	/// <summary>
	/// OS native dialogs (Android by the moment).
	/// </summary>
	public class OsDialogs : MonoBehaviour 
    {
		public string gameName;
		public string bundleId = "com.Company.Product";
		
        #if UNITY_ANDROID && USE_DIALOGS_MANAGER
		static AndroidJavaClass dialogsManager;
		static AndroidJavaObject currentActivity;
		#endif
		public static OsDialogs Instance;
		

		void Awake()
		{
			Instance = this;
		}

		void Start()
		{
#if !UNITY_EDITOR && UNITY_ANDROID && USE_DIALOGS_MANAGER
			var playerActivity = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			currentActivity = playerActivity.GetStatic<AndroidJavaObject>("currentActivity");
			
			dialogsManager = new AndroidJavaClass("com.DanielSoto.DialogsManager.DialogsManager");
			
			dialogsManager.CallStatic("setContext", currentActivity);
			
#region PROPERTIES SETUP
			var rateManager = GetComponent<RateUsDialogManager>();
			if( rateManager )
			{
				dialogsManager.CallStatic( "setLaunchRateProperties", rateManager.remindEveryDays, rateManager.remindEvery );
				dialogsManager.CallStatic( "setRateProperties", gameName, bundleId, rateManager.title, rateManager.msg );
			}
#endregion
#endif
		}



		/// <summary>
		/// Shows the rate pop up if conditions are fulfilled.
		/// </summary>
		public static void ShowRatePopup()
		{
#if !UNITY_EDITOR && UNITY_ANDROID && USE_DIALOGS_MANAGER
			Instance.StartCoroutine( _ShowRatePopup() );
#endif
		}

		public static void ShowYesNoDialog( string title, string msg, string yes, string no, string unityMsgObjName, 
            string unityMsgMethod, string unityYesMsg = "yes", string unityNoMsg = "no" )
		{
#if !UNITY_EDITOR && UNITY_ANDROID && USE_DIALOGS_MANAGER
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable( () => {
				dialogsManager.CallStatic( "showYesNoDialog", title, msg, yes, no, unityMsgObjName, unityMsgMethod, unityYesMsg, unityNoMsg );
			}));
#endif
		}

		public static void ShowMsgDialog( string title, string msg, string ok, string unityMsgObjName, 
            string unityMsgMethod, string unityOkMsg = "ok" )
		{
#if !UNITY_EDITOR && UNITY_ANDROID && USE_DIALOGS_MANAGER
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable( () => {
				dialogsManager.CallStatic( "showMsgDialog", title, msg, ok, unityMsgObjName, unityMsgMethod, unityOkMsg );
			}));
#endif
		}



#if !UNITY_EDITOR && UNITY_ANDROID && USE_DIALOGS_MANAGER
		private static IEnumerator _ShowRatePopup()
		{
			while( dialogsManager == null )
            {
				yield return null;
			}
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable( () => {
				dialogsManager.CallStatic( "showRatePopup" );
			}));
		}
#endif		
	}
}