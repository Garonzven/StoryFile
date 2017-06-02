using UnityEngine;
using System;


namespace DDK.Platforms.OsHook
{
	public class JavaCallbackScript : MonoBehaviour 
    {
		public string company = "DanielSoto";
		public bool printResultData;


		public static string[] lastData;



		// Use this for initialization
		void Start () 
        {
#if UNITY_ANDROID
			AndroidJavaClass ajc = new AndroidJavaClass("com."+ company +".OsHook.Bridge");
			ajc.CallStatic( "SetJavaCallbackObjName", name );
#endif
		}
		
		
		void onActivityResult(string resultData)
		{
			if( enabled )			
			{
				if( printResultData )
				{
					Debug.Log("onActivityResult = " + resultData);
				}
				
				lastData = resultData.Split(new string[] {":"}, StringSplitOptions.None);
				
				if( lastData.Length > 1 && printResultData )
				{
					Debug.Log("requestCode = " + lastData[0]);
					Debug.Log ("resultCode = " + lastData[1]);				
					Debug.Log ("data = " + lastData[2]);	
				}
			}		
		}		
	}
}
