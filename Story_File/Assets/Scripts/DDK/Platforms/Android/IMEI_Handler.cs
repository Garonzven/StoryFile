using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Statics;


namespace DDK.Platforms.Android 
{
	public class IMEI_Handler : MonoBehaviour 
	{		
		#if UNITY_ANDROID
		public bool printInLog = true;
		public Text imeiText;		
		
		
		// Use this for initialization
		void Start () {
						
			string IMEI = GetDeviceID();
			if( printInLog ) 
			{
				Debug.Log ( IMEI );
			}
			if( imeiText )
			{
				imeiText.text = IMEI;
			}
		}


		/// <summary>
		/// This requires to set the permission in the Android Manifest. <uses-permission android:name="android.permission.READ_PHONE_STATE" />
		/// </summary>
		public static string GetDeviceID()
		{
			AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");			
			return TM.Call<string>("getDeviceId");
		}
		#endif
	}
}
