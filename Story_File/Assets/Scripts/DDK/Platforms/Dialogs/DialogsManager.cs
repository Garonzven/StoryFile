//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Platforms.Android;


namespace DDK.Platforms.Dialogs 
{	
	/// <summary>
	/// This allows showing multiple native dialogs in Android and iOS. This should go on a game manager obj that is
	/// only initialized once in an aplication's life.
	/// </summary>
//#if USE_DIALOGS_MANAGER
	[RequireComponent( typeof( OsDialogs ) )]
//#endif
	public class DialogsManager : MonoBehaviour 
    {
		public DialogType type;
		public string title = "Title";
		public string msg = "Message";
		[Tooltip("Also the Ok button's text for confirmation messages")]
		public string yes = "Yes";
		public string no = "No";
		public bool showOnStart;


		public enum DialogType { Yes_No, SimpleMessage }
		public delegate void d_Yes();
		public delegate void d_No();
		public d_Yes onYes;
		public d_No onNo;

		public static d_Yes OnYes;
		public static d_No OnNo;
		/// <summary>
		/// True if a dialog is being shown.
		/// </summary>
		public static bool isActive;

				
		
		// Use this for initialization
		void Start () 
        {
			if( showOnStart )
			{
				ShowYesNoDialog();
			}
		}	

			
		#region PUBLIC
		public void ShowYesNoDialog()
		{
#pragma warning disable 0162
#if UNITY_EDITOR
			return;
#endif
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSDialog dialog = IOSDialog.Create( title, msg, yes, no );
			dialog.OnComplete += OnIosYesNoDialogClose;
			Debug.Log ( "iOS Yes/No PopUp" );
			isActive = true;
			#elif UNITY_ANDROID && USE_DIALOGS_MANAGER
			OsDialogs.ShowYesNoDialog( title, msg, yes, no, name, "OnAndroidYesNoDialogClose" );
			Debug.Log ( "Android Yes/No Dialog" );
			isActive = true;
			#endif
#pragma warning restore 0162
		}
		
		public void ShowMsgDialog()
		{
#pragma warning disable 0162
#if UNITY_EDITOR
			return;
#endif
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSNativePopUpManager.showMessage( title, msg, yes );
			Debug.Log ( "iOS Msg PopUp" );
			isActive = true;
			#elif UNITY_ANDROID && USE_DIALOGS_MANAGER
			OsDialogs.ShowMsgDialog( title, msg, yes, name, "OnAndroidYesNoDialogClose" );
			Debug.Log ( "Android Msg Dialog" );
			isActive = true;
			#endif
#pragma warning restore 0162
		}
		#endregion


		#region PRIVATE
		#if UNITY_IOS && USE_IOS_NATIVE
		private void OnIosYesNoDialogClose( IOSDialogResult result ) 
		{			
			switch( result ) 
			{			
			case IOSDialogResult.YES:
				if( onYes != null )
				{
					onYes ();
				}
				Debug.Log ("Yes button pressed");
				break;
			case IOSDialogResult.NO:
				if( onNo != null )
				{
					onNo ();
				}
				Debug.Log ("No button pressed");
				break;
			}
			isActive = false;
			
			//IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");
		}
		#endif
		
		#if UNITY_ANDROID && USE_DIALOGS_MANAGER
		private void OnAndroidYesNoDialogClose( string msg ) 
		{
			switch( msg ) 
			{			
			case "yes":
				if( onYes != null )
				{
					onYes ();
				}
				Debug.Log ("Yes button pressed");
				break;
			case "no":
				if( onNo != null )
				{
					onNo ();
				}
				Debug.Log ("No button pressed");
				break;
			}
			isActive = false;
		}
		
		private void OnAndroidMsgDialogClose( string msg ) 
		{
			if( msg.Equals( "ok" ) )
			{
				if( onYes != null )
				{
					onYes ();
				}
				Debug.Log ("Ok button pressed");
			}
			isActive = false;
		}
		#endif
		#endregion


		#region STATIC
		public static void ShowYesNoDialog( string title, string msg, string yes, string no )
		{
#pragma warning disable 0162
#if UNITY_EDITOR
			return;
#endif
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSDialog dialog = IOSDialog.Create( title, msg, yes, no );
			dialog.OnComplete += _OnIosYesNoDialogClose;
			Debug.Log ( "iOS Yes/No PopUp" );
			isActive = true;
			#elif UNITY_ANDROID && USE_DIALOGS_MANAGER
			GameObject instance = new GameObject("DialogsManager Yes_No Dialog Instance");
			instance.AddComponent<DialogsManager>();
			OsDialogs.ShowYesNoDialog( title, msg, yes, no, instance.name, "_OnAndroidYesNoDialogClose" );
			Debug.Log ( "Android Yes/No Dialog" );
			isActive = true;
			instance.DestroyImmediate();
			#endif
#pragma warning restore 0162
		}
		
		public static void ShowMsgDialog( string title, string msg, string ok )
		{
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSNativePopUpManager.showMessage( title, msg, ok );
			Debug.Log ( "iOS Msg PopUp" );
			isActive = true;
			#elif UNITY_ANDROID && USE_DIALOGS_MANAGER
			GameObject instance = new GameObject("DialogsManager Yes_No Dialog Instance");
			instance.AddComponent<DialogsManager>();
			OsDialogs.ShowMsgDialog( title, msg, ok, instance.name, "_OnAndroidYesNoDialogClose" );
			Debug.Log ( "Android Msg Dialog" );
			isActive = true;
			instance.DestroyImmediate();
			#endif
		}
		
		

		
		#if UNITY_IOS && USE_IOS_NATIVE
		private static void _OnIosYesNoDialogClose( IOSDialogResult result ) 
		{			
			switch( result ) 
			{			
			case IOSDialogResult.YES:
				if( OnYes != null )
				{
					OnYes ();
				}
				Debug.Log ("Yes button pressed");
				break;
			case IOSDialogResult.NO:
				if( OnNo != null )
				{
					OnNo ();
				}
				Debug.Log ("No button pressed");
				break;
			}
			isActive = false;
		}
		#endif
		
		#if UNITY_ANDROID && USE_DIALOGS_MANAGER
		private static void _OnAndroidYesNoDialogClose( string msg ) 
		{
			switch( msg ) 
			{			
			case "yes":
				if( OnYes != null )
				{
					OnYes ();
				}
				Debug.Log ("Yes button pressed");
				break;
			case "no":
				if( OnNo != null )
				{
					OnNo ();
				}
				Debug.Log ("No button pressed");
				break;
			}
			isActive = false;
		}
		
		private static void _OnAndroidMsgDialogClose( string msg ) 
		{
			if( msg.Equals( "ok" ) )
			{
				if( OnYes != null )
				{
					OnYes ();
				}
				Debug.Log ("Ok button pressed");
			}
			isActive = false;
		}
		#endif
		#endregion

		
	}
	
	
}