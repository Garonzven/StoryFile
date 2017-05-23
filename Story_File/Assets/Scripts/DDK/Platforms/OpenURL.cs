using UnityEngine;
using DDK.Base.Extensions;


namespace DDK.Platforms
{
	public class OpenURL : MonoBehaviourExt 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If disabled, actions won't be invoked";
        #endif
		public string androidURL;
		public string iosURL;
		public string webglURL;
		public string windowsURL;
		public string macURL;
		public string tizenURL;
		public string wp8URL;
		public string wp8_1URL;
		public string linuxURL;


		// Use this for initialization
		void Start () {} //Allows to enable/disable this component


		public void Open()
		{
#if UNITY_ANDROID
			OpenURL( androidURL );
#elif UNITY_IOS
			OpenURL( iosURL );
#elif UNITY_WEBGL
			OpenURL( webglURL );
#elif UNITY_STANDALONE_WIN
			OpenURL( windowsURL );
#elif UNITY_STANDALONE_MACOSX
			OpenURL( macURL );
#elif UNITY_TIZEN
			OpenURL( tizenURL );
#elif UNITY_WP8
			OpenURL( wp8URL );
#elif UNITY_WP8_1
			OpenURL( wp8_1URL );
#elif UNITY_STANDALONE_LINUX
			OpenURL( linuxURL );
#endif
		}
    }
}
