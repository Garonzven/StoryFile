using System;
using System.Runtime.InteropServices;
using UnityEngine;


namespace DDK.Platforms.OsHook 
{
	public class OSHookBridge
	{		
		#if UNITY_ANDROID		
		public static string ReturnString( string company = "DanielSoto" )
        {
			AndroidJavaClass ajc = new AndroidJavaClass("com."+ company +".OsHook.Bridge");
			return ajc.CallStatic<string>("ReturnString");
			
		}
		
		public static int ReturnInt( string company = "DanielSoto" )
        {
			AndroidJavaClass ajc = new AndroidJavaClass("com."+ company +".OsHook.Bridge");
			return ajc.CallStatic<int>("ReturnInt");
		}
		
		public static string ReturnInstanceString( string company = "DanielSoto" )
        {
			AndroidJavaObject ajo = new AndroidJavaObject("com."+ company +".OsHook.Bridge");
			return ajo.Call<string>("ReturnInstanceString");
		}
		
		public static int ReturnInstanceInt( string company = "DanielSoto" )
        {
			AndroidJavaObject ajo = new AndroidJavaObject("com."+ company +".OsHook.Bridge");
			return ajo.Call<int>("ReturnInstanceInt");
		}
		
		public static void SendUnityMessage( string objectName, string methodName, string parameterText, 
            string company = "DanielSoto" )
        {
			AndroidJavaClass ajc = new AndroidJavaClass("com."+ company +".OsHook.Bridge");
			ajc.CallStatic("SendUnityMessage", objectName, methodName, parameterText);
		}
		
		public static void ShowCamera( int requestCode, string company = "DanielSoto" )
        {
			AndroidJavaClass ajc = new AndroidJavaClass("com."+ company +".OsHook.Bridge");
			ajc.CallStatic("ShowCamera",requestCode);
		}				
		#endif 		
	}
}

