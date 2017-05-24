//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.ScriptableObjects;


namespace DDK.Platforms.IOS 
{
	/// <summary>
	/// Add to a specific share button such as twitter, facebook, or e-mail. And add the specific event on the button's onClick delegate.
	/// </summary>
	[RequireComponent( typeof( Button ) )]
	public class IosShareBt : MonoBehaviour 
	{		
		public LanguagesDictionaries languagesDictionaries;
		public int mailSubjectIndex;
				
		
		public void ShareWithTwitter()
		{
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSSocialManager.instance.TwitterPost( IosShare.Instance.ShareText );
			#endif
		}		
		public void ShareWithFacebook()
		{
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSSocialManager.instance.FacebookPost( IosShare.Instance.ShareText );
			#endif
		}		
		public void ShareWithMail()
		{
			#if UNITY_IOS && USE_IOS_NATIVE
			IOSSocialManager.instance.SendMail( languagesDictionaries.GetCurrentLanguageDictionary().GetTextAt( mailSubjectIndex ),
				IosShare.Instance.ShareText, "friend@gmail.com" );
			#endif
		}							
	}
}
