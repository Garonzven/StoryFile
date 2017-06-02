//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Platforms.Dialogs 
{
	/// <summary>
	/// Rate us manager. This should go on a game manager obj that is only initialized once in an aplication's life.
	/// </summary>
	[RequireComponent( typeof( OsDialogs ) )]
	public class RateUsDialogManager : Singleton<MonoBehaviour> 
    {		
		public float delay = 1.5f;
		public string title = "Like this game?";
		public string msg = "Please rate to support developers!";
		/// <summary>
		/// The amount of times the app has to be launched to activate the rate us popup again.
		/// </summary>
		[Tooltip("The amount of times the app has to be launched to activate the rate us popup again.")]
		public int remindEvery = 4;
		public int remindEveryDays = 2;
		public string objMustBeActive;		
		
		
        private int _launchCount;
        private int _rated;		
		
		
		// Use this for initialization
		void Start () 
        {			
			_launchCount = PlayerPrefs.GetInt( "RateUsChecker", 0 );
			_rated = PlayerPrefs.GetInt( "Rated", 0 );
			
			if( _rated == 0 )
			{
				_launchCount++;
				PlayerPrefs.SetInt( "RateUsChecker", _launchCount );
				PlayerPrefs.Save();
			}
			StartCoroutine( ShowRateUs() );
		}			

			
			
		public IEnumerator ShowRateUs()
		{
#pragma warning disable 0162
#if UNITY_EDITOR
			yield break;
#endif
			#if UNITY_IOS && USE_IOS_NATIVE
			if( ShouldRateUsBeShown() )
			{
				yield return new WaitForSeconds( delay );
				if( objMustBeActive.ObjectContainingSubstringExists() )
				{
					//SHOW
					IOSRateUsPopUp rate = IOSRateUsPopUp.Create("Like this game?", "Please rate to support developers!");
					Debug.Log ( "iOS Rate PopUp" );
					rate.OnComplete += OnRatePopUpClose;
				}
			}
			#elif UNITY_ANDROID && USE_DIALOGS_MANAGER
			yield return new WaitForSeconds( delay );
			if( objMustBeActive.ObjectContainingSubstringExists() )
			{
				OsDialogs.ShowRatePopup();
				Debug.Log ( "Android Rate PopUp" );
			}
			#else
			yield return null;
			#endif
#pragma warning restore 0162
		}		
		public bool ShouldRateUsBeShown()
		{
			if( _rated == 0 && _launchCount == remindEvery )
			{
				_launchCount = 0;
				PlayerPrefs.SetInt( "RateUsChecker", _launchCount );
				PlayerPrefs.Save();
				return true;
			}
			return false;
		}
		
		#if UNITY_IOS && USE_IOS_NATIVE
		private void OnRatePopUpClose( IOSDialogResult result ) 
		{
			switch(result) {
				
			case IOSDialogResult.RATED:
				IOSNativeUtility.RedirectToAppStoreRatingPage();
				Rated();
				Debug.Log ("Rate button pressed");
				break;
			case IOSDialogResult.REMIND:
				IOSNativePopUpManager.dismissCurrentAlert ();
				Debug.Log ("Remind button pressed");
				break;
			case IOSDialogResult.DECLINED:
				IOSNativePopUpManager.dismissCurrentAlert ();
				Rated();
				Debug.Log ("Decline button pressed");
				break;
				
			}
			
			//IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");
		}
		#endif
		
		public void Rated()
		{
			_rated = 1;
			PlayerPrefs.SetInt( "Rated", _rated );
			PlayerPrefs.Save();
		}
		
	}


}