using UnityEngine;
using DDK.Base.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


namespace DDK.Networking {

	/// <summary>
	/// Check internet. Attach to a gameobject to allow checking if an internet connection is available. Usage Eg:
	/// Another script must call StartCheck() static function, when the static waitTimeExceeded becomes true the static
	/// isConnectionAvailable will return the propper value. If noInternetPopup is set, a message can be automatically 
	/// shown after an internet connection failure.
	/// </summary>
	public class CheckInternet : MonoBehaviour
	{
		[System.Serializable]
		public struct InternetCheckedEvents
		{
			public UnityEvent onNoInternetAvailable;
			public UnityEvent onInternetAvailable;
		}

#if !UNITY_WEBGL && !UNITY_WEBPLAYER
		public bool allowCarrierDataNetwork = false;
		public string pingAddress = "8.8.8.8"; // Google Public DNS server
		/// <summary>
		/// The check timeout. The time that must pass before the check is set to wrong.
		/// </summary>
		[Tooltip("The check timeout. The time that must pass before the check is set to wrong.")]
		public int waitingTime = 30;
		public bool checkOnEnable;
		public InternetCheckedEvents events = new InternetCheckedEvents();
		

		public static CheckInternet Instance;	
		/// <summary>
		/// Returns the result of the last check.
		/// </summary>
        public static bool m_IsConnectionAvailable { get; private set; }
		public static int m_WaitTime{
			get{
				return Instance.waitingTime;
			}
		}
		public static bool m_WaitTimeExceeded{
			get{
				return Time.time - _pingStartTime > m_WaitTime;
			}
		}

		private static Ping _ping;
		private static float _pingStartTime;
				
		
		public void Awake()
		{
			Instance = this;
			if( checkOnEnable )
				return;
			enabled = false;//The component is enabled only when checking the internet, to prevent unnecessary Update() calls
		}				
		public void Update()
		{
			if ( _ping.isDone ) {
				_InternetAvailable();
				enabled = false;
			}
			else if ( m_WaitTimeExceeded ) 
			{
				InternetIsNotAvailable();
				_ping = null;
				enabled = false;
			}
		}
		void OnEnable()
		{
			if( !checkOnEnable )
				return;
			StartCheck();
		}


		/// <summary>
		/// Starts the check.
		/// </summary>
		public static IEnumerator<float> CheckAndWait()
		{
			StartCheck();
			while( !m_WaitTimeExceeded )
            {
                yield return 0f;
                if( m_IsConnectionAvailable )
                    yield break;
            }
			yield return 0f;
		}	
		/// <summary>
		/// Starts the check. You must check the values of m_WaitTimeExceeded and m_IsConnectionAvailable. You can also 
		/// call the CheckAndWait() coroutine and yield for it to finish, then just check the value of m_IsConnectionAvailable
		/// </summary>
		public static void StartCheck()
		{
            m_IsConnectionAvailable = false;
			if( !Instance )
			{
				Debug.LogError("There is no instance of CheckInternet in the scene. Calling StartCheck() won't work..");
				return;
			}

			bool internetPossiblyAvailable;
			switch (Application.internetReachability)
			{
			case NetworkReachability.ReachableViaLocalAreaNetwork:
				internetPossiblyAvailable = true;
				break;
			case NetworkReachability.ReachableViaCarrierDataNetwork:
				internetPossiblyAvailable = Instance.allowCarrierDataNetwork;
				break;
			default:
				internetPossiblyAvailable = false;
				break;
			}
			if ( !internetPossiblyAvailable )
			{
				InternetIsNotAvailable();
				return;
			}
			_ping = new Ping( Instance.pingAddress );
			_pingStartTime = Time.time;
			Instance.enabled = true;
		}	
		/// <summary>
		/// Set m_IsConnectionAvailable to false and shows a debug and popup message (if specified in the instance parameters).
		/// </summary>
		public static void InternetIsNotAvailable()
		{
            Debug.LogWarning("No Internet :(");
			m_IsConnectionAvailable = false;
			if( Instance.events.onNoInternetAvailable != null )
				Instance.events.onNoInternetAvailable.Invoke();
		}	

		private static void _InternetAvailable()
		{
			Debug.Log("Internet is available! ;)");
			m_IsConnectionAvailable = true;
			if( Instance.events.onInternetAvailable != null )
				Instance.events.onInternetAvailable.Invoke();
		}
#endif

	}

}
