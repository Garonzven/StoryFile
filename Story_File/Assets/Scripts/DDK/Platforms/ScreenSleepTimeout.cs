//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.Platforms
{
	public class ScreenSleepTimeout : MonoBehaviour {
		
		public enum Timeout 
		{ 
			NeverSleep = SleepTimeout.NeverSleep,
			SystemSetting = SleepTimeout.SystemSetting
		}
		
		
		[Tooltip("This is set OnEnable() or when calling SetTimeout()")]
		public Timeout timeout = Timeout.NeverSleep;
		[NotLessThan( 0f, "If /timeout/ is set to NeverSleep, then this will reset it to SystemSettings after" +
			"the specified seconds pass, only if higher than 0" )]
		public float seconds = 0f;

		public bool multitouch;

		protected static bool _handlingTimeout;

		// Use this for initialization
		void Start()
		{
			Input.multiTouchEnabled = multitouch;
		}

		void OnEnable () {
			
			SetTimeout();
		}
		
		
		public void SetTimeout()
		{
			if( seconds > 0 )
			{
				SetTimeout( seconds );
			}
			Screen.sleepTimeout = (int)timeout;
		}
		public void SetToNeverSleep()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			timeout = Timeout.NeverSleep;
		}
		public void SetToSystemSetting()
		{
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
			timeout = Timeout.SystemSetting;
		}
		public void SetTimeout( float seconds )
		{
			if( seconds == 0f )
				StopHandlingTimeout();
			if( seconds <= 0f )
				return;
			if( !_handlingTimeout )
				_StartHandlingTimeout( seconds ).Start( "ScreenSleepTimeoutHandler" );
			else this.seconds = seconds;
		}
		public void StopHandlingTimeout()
		{
			_handlingTimeout = false;
		}


		protected IEnumerator _StartHandlingTimeout( float seconds )
		{
			this.seconds = seconds;
			_handlingTimeout = true;
			float time = 0f;
			while( _handlingTimeout )
			{
				if( Input.GetMouseButton(0) )
				{
					Screen.sleepTimeout = SleepTimeout.NeverSleep; //(int)timeout;
					time = 0f;
				}
				else if( time >= seconds )
				{
					Screen.sleepTimeout = SleepTimeout.SystemSetting;
				}
				if( this.seconds == 0f )
					yield break;
				yield return null;
				time += Time.unscaledDeltaTime;
			}
		}
	}
}
