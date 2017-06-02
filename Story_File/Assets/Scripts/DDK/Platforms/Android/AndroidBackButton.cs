//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Fx.Transitions;
using DDK.Base.Classes;
using DDK.Platforms.Dialogs;


namespace DDK.Platforms.Android 
{
	/// <summary>
	/// Attach to an object to set the Android's back button action. NOTE: This uses the DialogsManager plugin.
	/// </summary>
	public class AndroidBackButton : MonoBehaviour 
    {		
#if UNITY_ANDROID
		public bool exitApp;
		[ShowIfAttribute( "_ExitApp", 1 )]
		public bool showConfirmDialog;
		public ComposedEvent onClick = new ComposedEvent();


		protected bool _ExitApp()
		{
			if( showConfirmDialog )
			{
				AddDialogComp();
			}
			else RemoveDialogComp();
			return exitApp;
		}


#if USE_DIALOGS_MANAGER
		internal DialogsManager confirmDialog;
#endif
		
		
		
		// Use this for initialization
		void Start () {

			if( showConfirmDialog )
			{
#if USE_DIALOGS_MANAGER
				confirmDialog = GetComponent<DialogsManager>();
				confirmDialog.onYes += Application.Quit;
#endif
			}
		}
		
		// Update is called once per frame
		void Update () {
			
#if UNITY_ANDROID
			if( Input.GetKey( KeyCode.Escape ) )
			{
				if( exitApp )
				{
#if USE_DIALOGS_MANAGER
					if( showConfirmDialog )
					{
						if( !DialogsManager.isActive )
						{
							confirmDialog.ShowYesNoDialog();
						}
					}
					else
#endif
					{
						Application.Quit();
					}
				}
				else if( onClick != null )
					onClick.Invoke ();
			}
#endif			
		}


		public void AddDialogComp()
		{
#if USE_DIALOGS_MANAGER
			if( !GetComponent<DialogsManager>() )
			{
				confirmDialog = gameObject.AddComponent<DialogsManager>();
			}
#endif
		}
		public void RemoveDialogComp()
		{
#if USE_DIALOGS_MANAGER
			if( GetComponent<DialogsManager>() )
			{
				Destroy( confirmDialog );
			}
#endif
		}
#endif

	}

}
