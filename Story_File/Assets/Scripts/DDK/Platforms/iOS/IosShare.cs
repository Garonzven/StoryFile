//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.ScriptableObjects;
using DDK.Base.Statics;


namespace DDK.Platforms.IOS 
{
	/// <summary>
	/// Add to an object that's used as a button, this will allow to use share functionality on iOS platform.
	/// </summary>
	public class IosShare : MonoBehaviour 
	{		
		public bool shareScreenshot;

		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string appStoreLinkMessage = "Use this pattern to share your App's App Store link: http://itunes.apple.com/app/id<app_apple_id>";
		#endif
		[Tooltip("This should hold the share text and posted messages shown to the user after sharing.")]
		public LanguagesDictionaries languagesDictionaries;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the share message")]
		public int shareTextIndex = 0;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the share message's title")]
		public int postTitleIndex = 1;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the post success message")]
		public int postMsgsIndex = 2;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the post failed message")]
		public int postFailedMsgsIndex = 3;

		public GameObject customAppChoserDialog;
		[ShowIfAttribute( "_UsingCustomChoserDialog", 1 )]
		[Tooltip("The name of the objects that want to be excluded when disabling active colliders which is done when showing " +
			"the Custom App Choser Dialog")]
		public string[] excludeColliders;


		protected bool _UsingCustomChoserDialog()
		{
			return customAppChoserDialog != null;
		}
		protected bool _UsingLanguagesDictionaries()
		{
			return languagesDictionaries != null;
		}

		
		public static IosShare Instance;
		public static bool _shared;

		/// <summary>
		///The current language dictionary.
		/// </summary>
		public string[] languageDictionary 
		{
			get { return languagesDictionaries.GetCurrentLanguageDictionary().texts; }
		}
		public string ShareText 
		{
			get { return languageDictionary[ shareTextIndex ]; }
		}
		

		void Awake()
		{
			if( Instance )
			{
				Utilities.LogError( "There should only be 1 instance of this class, destroying new one..", gameObject );
				Destroy( this );
				return;
			}
			Instance = this;
		}
		// Use this for initialization
		void Start () 
		{
#if UNITY_IOS && USE_IOS_NATIVE		
			IOSSocialManager.OnFacebookPostResult += OnPostResult;
			IOSSocialManager.OnTwitterPostResult += OnPostResult;
			IOSSocialManager.OnInstagramPostResult += OnPostResult;
			IOSSocialManager.OnMailResult += OnPostResult;
#endif
		}		
		void OnMouseUpAsButton()
		{
#if UNITY_IOS && USE_IOS_NATIVE
			Texture2D screenshot = null;
			if( shareScreenshot )
			{
				Utilities.TakeScreenshot( screenshot );
			}
			if( customAppChoserDialog )
			{
				customAppChoserDialog.SetActiveInHierarchy();
				EnableColliders( false );
			}
			else IOSSocialManager.instance.ShareMedia( ShareText, screenshot );
			Utilities.Log ( "iOS share clicked!" );
#endif
		}



#if UNITY_IOS && USE_IOS_NATIVE
		void OnPostResult( ISN_Result result ) 
		{
			if( result.IsSucceeded ) 
			{
				OnPostSuccess();
			} 
			else 
			{
				OnPostFailed();
			}
		}
#endif
		void OnPostSuccess()
		{
#if UNITY_IOS && USE_IOS_NATIVE
			if( languagesDictionaries != null )
				IOSNativePopUpManager.showMessage( languageDictionary[ postTitleIndex ], languageDictionary[ postMsgsIndex ] );
			_shared = true;
			EnableColliders( true );
#endif
		}
		void OnPostFailed()
		{
#if UNITY_IOS && USE_IOS_NATIVE
			if( languagesDictionaries != null )
				IOSNativePopUpManager.showMessage( languageDictionary[ postTitleIndex ], languageDictionary[ postFailedMsgsIndex ] );
			EnableColliders( true );
#endif
		}



		public void EnableColliders( bool enable )
		{
			this.EnableColliders( enable, excludeColliders );
		}
	}
}
