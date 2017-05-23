//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.ScriptableObjects;


namespace DDK.Networking._FB {

	public class FB_AddFriends : MonoBehaviour {

		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string msg = "If this has a Button component the OnMouseUp() function will be automatically added to its events";
		#endif
		public LanguagesDictionaries message;
		[Header( "Platform Specific Message Suffix" )]
		public string androidMsg;
		public string iOsMsg;
		public string wp8Msg;
		public string fbMsg;
		
						
		// Use this for initialization
		void Start () {

			var button = GetComponent<UnityEngine.UI.Button>();
			if( button )
			{
				button.onClick.AddListener( OnMouseUp );
			}
		}
		public void OnMouseUp()
		{
			#if USE_FB_MANAGER
			string msg = message.GetCurrentLanguageDictionary().texts[0];
			#if UNITY_ANDROID
			msg += androidMsg;
			#endif
			#if UNITY_IPHONE
			msg += iOsMsg;
			#endif
			#if UNITY_WP8
			msg += wp8Msg;
			#endif
			#if UNITY_WEBPLAYER
			msg += fbMsg;
			#endif
			if( !FacebookManager._invitingFriends )
				FacebookManager.AddFriends( msg );
			#endif
		}
	}
}
