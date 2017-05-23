using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Classes;
using System.Globalization;
using UnityEngine.UI;
using DDK.Base.Extensions;
using System;
using DDK.Base.Statics;


#if USE_ONE_SIGNAL
using OneSignalPush.MiniJSON;
#endif


namespace DDK.OneSignalExtensions
{
	/// <summary>
	/// One signal controller.
	/// </summary>
	/// <see cref="https://documentation.onesignal.com/docs/corona-sdk-api#EnableNotificationsWhenActive"/>
	public class OneSignalController : MonoBehaviourExt 
	{
#if USE_ONE_SIGNAL
		public enum ActiveState { Either, Active, Inactive }
		
		[System.Serializable]
		public class KeyActions
		{
			[Tooltip("If this key is present in the received notification's /additionalData/ then the /action/ is invoked. Use 'actionSelected'")]
			public string key;
			[Tooltip("If the key is found and this is not null, then the key MUST MATCH this value")]
			[ShowIfAttribute( "_KeyNotEmpty" )]
			public string mustMatch;
			[Tooltip( "The received notification's /isActive/ MUST MATCH this value. When /isActive/ is true this means the user is currently in your game." )]
			public ActiveState mustMatchState = ActiveState.Either;
			[ShowIfAttribute( "_KeyNotEmpty" )]
			public ComposedEvent action = new ComposedEvent();
			
			protected bool _KeyNotEmpty()
			{
				return !string.IsNullOrEmpty( key );
			}
		}
		
		
		public string appId = "b2f7f966-d8cc-11e4-bed1-df8f05be55ba";
		public string googleProjectNumber;
		/// <summary>
		/// Setting to control how OneSignal notifications will be shown when one is received while your app is in focus.
		/// Notification - ANDROID iOS 10+ native notification display while user has app in focus (can be distracting).
		/// InAppAlert (DEFAULT) - native alert dialog display, which can be helpful during development.
		/// None - notification is silent.
		/// If your app only sends notification and your deployment target is iOS 9 or below, your app will default to inAppAlert.
		/// </summary>
		[Tooltip("Setting to control how OneSignal notifications will be shown when one is received while your app is in focus. " +
			"Notification - ANDROID iOS 10+ native notification display while user has app in focus (can be distracting). " +
			"InAppAlert (DEFAULT) - native alert dialog display, which can be helpful during development. " +
			"None - notification is silent. " +
			"If your app only sends notification and your deployment target is iOS 9 or below, your app will default to inAppAlert.")]
		public OneSignal.OSInFocusDisplayOption inFocusDisplayOption = OneSignal.OSInFocusDisplayOption.None;
		[Tooltip("If true, the userID will be sent as a tag named 'userId'")]
		public bool sendUserIdAsTag;
		[Header("Events")]
		[Tooltip("If this is not null when a notification is received, the /text/ will be set to the notification's message")]
		public Text lastMessage;
		[Tooltip("Each key is evaluated, if found in the received notification's /additionalData/ then the specified action is invoked")]
		public KeyActions[] keysActions = new KeyActions[0];
		
		
		public static OneSignalController Instance;
		public static string LastMessage { get; protected set; }
		public static string ExtraMessage { get; protected set; }
		public static Action<OSNotification> onNotificationReceived;
		public static Action<OSNotificationOpenedResult> onNotificationOpened;
		/// <summary>
		/// The user identifier received from the available ids. This is per device.
		/// </summary>
		public static string _UserId { get; private set; }
		
		
		void Awake()
		{
			if( Instance )
			{
				Debug.LogWarning( "There's already an active instance of this class", gameObject );
				DestroyImmediate( this ); //enabled = false;
				return;
			}
			Instance = this;
		}
		void Start ()
		{
			ExtraMessage = null;
			if( googleProjectNumber.Equals( "" ) )
				googleProjectNumber = null;
			
			// Enable line below to debug issues with setuping OneSignal. (logLevel, visualLogLevel)
			//OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);
			
			// The only required method you need to call to setup OneSignal to receive push notifications.
			// Call before using any other methods on OneSignal.
			// Should only be called once when your app is loaded.
			OneSignal.StartInit( appId, googleProjectNumber );
			OneSignal.StartInit( appId, googleProjectNumber )
				.HandleNotificationReceived( _HandleNotificationReceived )
				.HandleNotificationOpened( _HandleNotificationOpened )
				.InFocusDisplaying( inFocusDisplayOption )
				.EndInit();

			OneSignal.IdsAvailable( _IdsAvailable );
		}


		private void _IdsAvailable( string userID, string pushToken ) 
		{
			//Do something, like save the Id to a server.
			_UserId = userID;
			if( sendUserIdAsTag )
				OneSignal.SendTag( "userId", userID );
		}


		/// <summary>
		/// Called when a notification is opened.
		/// The name of the method can be anything as long as the signature matches.
		/// Method must be static or this object should be marked as DontDestroyOnLoad
		/// </summary>
		private static void _HandleNotificationOpened ( OSNotificationOpenedResult result )
		{
			_HandlePayload( result.notification.payload, false );
			ExtraMessage = "Notification received with text: " + LastMessage;

			//TODO
			/*string actionID = result.action.actionID;
			if( actionID != null ) 
			{
				// actionSelected equals the id on the button the user pressed.
				// actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
				extraMessage = "Pressed ButtonId: " + actionID;
			}*/
			onNotificationOpened.Invoke( result );
		}
		/// <summary>
		/// Called when your app is in focus and a notificaiton is recieved.
		/// The name of the method can be anything as long as the signature matches.
		/// Method must be static or this object should be marked as DontDestroyOnLoad
		/// </summary>
		private static void _HandleNotificationReceived ( OSNotification result )
		{				
			_HandlePayload( result.payload, true );
			ExtraMessage = "Notification received with text: " + LastMessage;
			onNotificationReceived.Invoke( result );
		}
		private static void _HandlePayload( OSNotificationPayload payload, bool isAppActive )
		{
			LastMessage = payload.body;
			Debug.Log( "Handle Notification message: " + LastMessage );
			if( Instance.lastMessage )
			{
				Instance.lastMessage.text = LastMessage;
			}
			Dictionary<string, object> additionalData = payload.additionalData;
			if( additionalData != null )
			{
				KeyActions[] kActions = Instance.keysActions;
				for( int i=0; i<kActions.Length; i++ )
				{
					#region CONDITIONS
					if( !additionalData.ContainsKey( kActions[i].key ) )
					{
						continue;
					}
					if( !string.IsNullOrEmpty( kActions[i].mustMatch ) && !kActions[i].mustMatch.Equals( (string)additionalData[ kActions[i].key ] ) )
					{
						continue;
					}
					if( kActions[i].mustMatchState == ActiveState.Active && !isAppActive )
						continue;
					else if( kActions[i].mustMatchState == ActiveState.Inactive && isAppActive )
						continue;
					#endregion
					kActions[i].action.Invoke();
				}
			}
		}
#endif
    }
}

