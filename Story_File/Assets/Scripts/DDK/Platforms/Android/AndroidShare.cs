//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.IO;
using DDK.Base.Extensions;
using DDK.Platforms.OsHook;
using DDK.Platforms.Dialogs;
using DDK.Base.Fx.Transitions;
using DDK.Base.ScriptableObjects;
using UnityEngine.UI;


namespace DDK.Platforms.Android 
{
	/// <summary>
	/// This can be used as a button.
	/// </summary>
	public class AndroidShare : MonoBehaviour 
	{		
		#if UNITY_ANDROID
		// PUBLIC
		[Header("Screenshot")]
		public AudioClip flashSound;
		public string album;
		public string filename;
		[Tooltip("A loading screen to show while the screenshot is being saved")]
		public GameObject loading;
		/// <summary>
		/// If true, the screenshot will not be shared.
		/// </summary>
		[Tooltip( "If true, the screenshot will not be shared" )]
		public bool justSaveToGallery;
		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string storeLinkMessage = "Use this pattern to share your App's App Store link: https://play.google.com/store/apps/details?id=com.yourpackagename";
		#endif
		//public string shareText;
		[Tooltip("This should hold the share text and posted messages shown to the user after sharing.")]
		public LanguagesDictionaries languagesDictionaries;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the share message")]
		public int shareTextIndex = 0;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the shared message's title")]
		public int sharedTitleIndex = 1;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the shared success message")]
		public int sharedMsgIndex = 2;
		[ShowIfAttribute( "_UsingLanguagesDictionaries", 1 )]
		[Tooltip("The index of the dictionary's string that contains the shared message's ok button's text")]
		public int sharedOkIndex = 3;

		public bool usingLivesManager;
		[ShowIfAttribute( "_UsingLanguagesDictionariesAndLivesManager", 1 )]
		[Tooltip("The index of the dictionary's string that contains the wrong share message's title")]
		public int wrongShareTitleIndex = 1;
		[ShowIfAttribute( "_UsingLanguagesDictionariesAndLivesManager", 1 )]
		[Tooltip("The index of the dictionary's string that contains the wrong share message")]
		public int wrongShareMsgIndex = 2;
		[ShowIfAttribute( "_UsingLanguagesDictionariesAndLivesManager", 1 )]
		[Tooltip("The index of the dictionary's string that contains the wrong share message's ok button's text")]
		public int wrongShareOkIndex = 3;


		#if USE_ANDROID_SHARE && USE_DIALOGS_MANAGER


		#if UNITY_EDITOR
		protected bool _UsingLanguagesDictionaries()
		{
			return languagesDictionaries != null;
		}
		protected bool _UsingLanguagesDictionariesAndLivesManager()
		{
			return _UsingLanguagesDictionaries() && usingLivesManager;
		}
		#endif



		private string _selectedApp;
		private string _ShareText{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ shareTextIndex ];
			}
		}
		private string _SharedTitle{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ sharedTitleIndex ];
			}
		}
		private string _SharedMsg{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ sharedMsgIndex ];
			}
		}
		private string _SharedOk{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ sharedOkIndex ];
			}
		}
		private string _WrongShareTitle{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ wrongShareTitleIndex ];
			}
		}
		private string _WrongShareMsg{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ wrongShareMsgIndex ];
			}
		}
		private string wrongShareOk{
			get{
				return languagesDictionaries.GetCurrentLanguageDictionary()[ wrongShareOkIndex ];
			}
		}


		// STATIC
		/// <summary>
		/// True if the screenshot is being processed.
		/// </summary>
		static bool m_IsProcessing{
			get{
				return _isProcessing;
			}
		}
		private static bool _isProcessing = false;
		private static bool _isTakingScreenshot;
#pragma warning disable 0649
		static AndroidJavaClass androidshare;
		static AndroidJavaObject currentActivity;
#pragma warning restore 0649
		static AndroidShare _Instance { get; set; }
		private static bool _isSharing;
		private static float _clickTime;

		/// <summary>
		/// The share text.
		/// </summary>
		public static string m_shareTxt;
		public static bool m_shared;


		//INTERNAL
		/// <summary>
		/// The data from the screenshot (bytes from the texture).
		/// </summary>
		internal byte[] m_data;
		
		
		void Start()
		{
			#if !UNITY_EDITOR && UNITY_ANDROID
			var playerActivity = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			currentActivity = playerActivity.GetStatic<AndroidJavaObject>("currentActivity");
			
			androidshare = new AndroidJavaClass("com.DanielSoto.AndroidShare.AndroidShare");
			
			androidshare.CallStatic("setContext", currentActivity);
			androidshare.CallStatic("setLimitShareTextApps", usingLivesManager);
			androidshare.CallStatic("setUnityOkMsgProperties", name, "OnShared", "1");
			#endif

			_Instance = this;

			Button shareBt = GetComponent<Button>();
			if( shareBt )
			{
				shareBt.onClick.AddListener( OnMouseUpAsButton );
			}
		}		
		void OnMouseUpAsButton ()
		{
			m_shared = false;
			Debug.Log ( "Share was pressed" );
			if( !string.IsNullOrEmpty( _ShareText ) )
			{
				ShareText( _ShareText );
			}
			else if( !string.IsNullOrEmpty( m_shareTxt ) )
			{
				ShareText( m_shareTxt );
			}
			else if( justSaveToGallery )
			{
				StartCoroutine( SaveScreenshot() );
			}
			else StartCoroutine( SaveShareScreenshot() );
		}
		void OnApplicationPause( bool paused )
		{
			if( !paused && _isSharing )
			{
				if( usingLivesManager )
				{
					float waitTime = 1.4f;//improves: get life cheating prevention
					if( _selectedApp.Contains( "facebook" ) )
					{
						waitTime = 12f;
					}
					else if( _selectedApp.Contains( "whatsapp" ) )
					{
						waitTime = 4f;
					}
					if( Time.realtimeSinceStartup >= _clickTime + waitTime )
					{
						m_shared = true;
					}
					else DialogsManager.ShowMsgDialog( _WrongShareTitle, _WrongShareMsg, wrongShareOk );
				}
				else m_shared = true;
				_isSharing = false;
			}
		}
			
		public void OnShared( string msg )//callback
		{
			if( msg.Equals( "1" ) )
			{
				_isSharing = true;
				DialogsManager.ShowMsgDialog( _SharedTitle, _SharedMsg, _SharedOk );
			}
			else
			{
				Debug.Log ( msg );
				_selectedApp = msg;
				_clickTime = Time.realtimeSinceStartup;
			}
		}

		
		
		protected void _TakeScreenshot()
		{
			StartCoroutine( _TakeScreenshotCo() );
		}		
		protected IEnumerator _TakeScreenshotCo()
		{
			_isTakingScreenshot = true;
			yield return null;// wait for graphics to render
			#region SCREENSHOT
			Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
			
			screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
			
			screenTexture.Apply();
			#endregion
			
			m_data = screenTexture.EncodeToPNG();
			_isTakingScreenshot = false;
			
			//FLASH AND LOADING
			if( flashSound )
			{
				AudioSource.PlayClipAtPoint( flashSound, Vector3.zero );
			}
			AutoFade.FadeOutIn( 0.3f, 0.3f, Color.white );
			while( AutoFade._Fading )
				yield return null;
			if( !androidshare.CallStatic<bool>( "WasSaved" ) )
			{
				StartCoroutine( SetActiveAfter( loading, 0.1f ) );
				while( !androidshare.CallStatic<bool>( "WasSaved" ) )
				{
					yield return null;
				}
				StartCoroutine( SetActiveAfter( loading, 0.1f, false ) );
			}
		}						
		
		
		#region STATIC FUNCTIONS
		/// <summary>
		/// Saves the screenshot to the gallery and pops up a share dialog.
		/// </summary>
		public static IEnumerator SaveShareScreenshot()
		{
			if( !_isProcessing )
			{
				_isProcessing = true;
				
				_Instance._TakeScreenshot();
				while( _isTakingScreenshot )
					yield return null;
				
				#if UNITY_ANDROID
				_Instance.StartCoroutine( SaveShare( _Instance.m_data, _Instance.album, _Instance.filename ) );
				#endif
				_isProcessing = false;
			}
		}		
		/// <summary>
		/// Saves the screenshot to the gallery.
		/// </summary>
		public static IEnumerator SaveScreenshot()
		{
			if( !_isProcessing )
			{
				_isProcessing = true;
				
				_Instance._TakeScreenshot();
				yield return null;
				
				#if UNITY_ANDROID
				SaveToGallery( _Instance.m_data, _Instance.album, _Instance.filename );
				#endif
				_isProcessing = false;
			}
		}	
		public static void SaveToGallery( byte[] data, string album, string filename )
		{
			androidshare.CallStatic("SaveToGallery", data, album, filename );
		}
		public static IEnumerator Share( string path )
		{
			while( !androidshare.CallStatic<bool>( "WasSaved" ) )
			{
				yield return null;
			}
			androidshare.CallStatic("Share", path );
		}
		public static void ShareText( string txt )
		{
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				androidshare.CallStatic("ShareText", txt );
			}));
		}
		public static IEnumerator SaveShare( byte[] data, string album, string filename )
		{
			SaveToGallery ( data, album, filename );
			while( !androidshare.CallStatic<bool>( "WasSaved" ) )
			{
				yield return null;
			}
			string path = androidshare.CallStatic<string>( "GetPath" );
			_Instance.StartCoroutine( Share (path) );
		}		
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static GameObject SetActiveInHierarchy( GameObject obj, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			if( obj )
			{
				obj.SetActive(active);
				if( !obj.activeInHierarchy && active )
				{
					return GameObject.Instantiate( obj, obj.Position(), obj.transform.rotation ) as GameObject;
				}
				else if( obj.name.Contains( "(Clone)" ) && destroyIfClone )
				{
					if( immediate ) obj.DestroyImmediate();
					else obj.Destroy(after);
				}
				else
				{
					string name = obj.name;
					var clone = GameObject.Find( name+"(Clone)" );
					if( clone )
					{
						if( immediate ) clone.DestroyImmediate();
						else clone.Destroy(after);
					}
				}
			}
			return null;
		}		
		public static IEnumerator SetActiveAfter( GameObject obj, float time, bool active = true )
		{
			yield return new WaitForSeconds(time);
			if( obj )
				obj.SetActive(active);
		}
		#endregion
		#else
		#if UNITY_EDITOR
		[HelpBoxAttribute(DDK.Base.Classes.MessageType.Warning)]
		public string noPluginMsg = "This script uses AndroidShare and DialogsManager Android plugins " +
			"(by Daniel Soto).\n\nImport the plugins and enable the respective scripting symbols Custom/Scripting " +
			"Symbols.. In order to use this component";
		#endif
		#endif
		#endif
				
	}

}
