//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using DDK.GamesDevelopment;
using DDK.Base.Managers;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.Classes;
using UnityEngine.Networking;
using UnityEngine.Events;

#if USE_FB_MANAGER
using Facebook.MiniJSON;
using Facebook.Unity;
#endif


namespace DDK.Networking._FB {

	/// <summary>
	/// There should only be one instance of this class. Facebook manager (Do not modify). This shouldn't be attached to a
	/// gameobject directly (maybe a FB login button), use FacebookSpriteClick or create a class that inherites from this
	/// and attached it.
	/// To activate this class USE_FB_MANAGER symbol must be defined in player settings, Custom/ScriptingSymbols... menu 
	/// bar item can also be used to setup multiple symbols.
	/// </summary>
	/// <seealso cref="FB_Login.cs"/>
	[ExecuteInEditMode]
	public class FacebookManager : MonoBehaviour {

		public enum PictureTypes { Small, Normal, Large }
		public static string GetPictureType( PictureTypes type )
		{
			switch( type )
			{
			case PictureTypes.Small: return "small";
			case PictureTypes.Normal: return "normal";
			case PictureTypes.Large: return "large";
			}
			return "normal";
		}
		[System.Serializable]
		public class FBLoginEvents
		{
			public UnityEvent onFBLoggedIn = new UnityEvent();
			public UnityEvent onFBLoginFailed = new UnityEvent();
		}


		#region PUBLIC MEMBER FIELDS
		public bool testWithInvitableFriends;
		[Header("User Options")]
		public Image m_profilePicHolder;
		[ShowIfAttribute( "_UsingProfilePic", 1 )]
		public PictureTypes m_picSize = PictureTypes.Normal;
		public Text m_uiTextUsername;
		[Space(10f)]
		[Tooltip("This will be shown/instantiated/activated when FB is initializing or while the user is calling async functions such as Login")]
		public CanvasGroup m_loadingScreen;	

		[Header( "Login Options" )]
		public bool m_autoLoginAfterInit = true;
		[Tooltip("If false, publish permissions is considered instead")]
		public bool m_withReadPermissions = true;
		public string[] m_permissions = new string[] { "email", "user_friends" };
		[Tooltip("If not null, this will hold a message when an init or login error occurs")]
		public Text errorHolder;
		public FBLoginEvents m_fbLoginEvents = new FBLoginEvents();
		#endregion


		#if UNITY_EDITOR
		protected bool _UsingProfilePic()
		{
			return m_profilePicHolder != null;
		}
		#endif
				
		
		#region PRIVATE FIELDS
		private string get_data_username;
		private string get_data_gender;
		private string get_data_friends;
		private string get_data_invitableFriends;
		private string get_data_score;
		private string get_data_scores;
		#endregion

		#region STATIC FIELDS
		public static FacebookManager Instance;
		/// <summary>
		/// Used to avoid multiple login calls.
		/// </summary>
		public static bool _IsTryingToLogin { get; private set; }
		/// <summary>
		/// The current logged in user's profile picture.
		/// </summary>
		public static Texture2D _profilePicture;
		/// <summary>
		/// The current logged in user's username.
		/// </summary>
		public static string _username;
		/// <summary>
		/// The current logged in user's gender.
		/// </summary>
		public static string _gender;
		/// <summary>
		/// The synced friends.
		/// </summary>
		public static IDictionary[] _friends;
		/// <summary>
		/// The synced friends with their scores.
		/// </summary>
		public static IDictionary[] _friends_scores;
		/// <summary>
		/// The synced invitable friends.
		/// </summary>
		public static IDictionary[] _InvitableFriends{ get; private set; }
		public static int _totalInvitableFriends;
		public static int _totalFriends;
		public static int _totalFriends_scores;
		/// <summary>
		/// Current logged in user's last retrieved score.
		/// </summary>
		public static int _score;
		public static bool _AutoLoginWithFacebook {
			get{
				return PlayerPrefs.GetInt( AUTO_LOGIN_WITH_FB, 0 ) == 1;
			}
			set{
				PlayerPrefs.SetInt( AUTO_LOGIN_WITH_FB, value ? 1 : 0 );
				PlayerPrefs.Save();
			}
		}

		public const string AUTO_LOGIN_WITH_FB = "AUTO_LOGIN_WITH_FB";
		#endregion
		
		public delegate IEnumerator LoginAction();		
						
#pragma warning disable 414
		private string _lastResponse = "";
#pragma warning restore 414		

		
		void Awake()
		{
			if( Instance != null )
			{
				Debug.LogWarning("There is a FacebookManager instance already in the scene. Destroying the new one..", Instance );
				DestroyImmediate( gameObject );
				return;
			}
			Instance = this;
		}
        #if USE_FB_MANAGER  
		void Start() 
		{
			if( _AutoLoginWithFacebook )
			{
				m_autoLoginAfterInit = true;
				Init();
			}
		}		

		#region FB.Init()		
		public static bool _IsInit {
			get{
				return FB.IsInitialized;
			}
		}
		public static bool _isDoingInit;


		public void Init()
		{
			StartCoroutine( InitCo() );
		}
		/// <summary>
		/// If already initialised, and not logged in, Login() will be invoked (but not yielded).
		/// </summary>
		public static IEnumerator InitCo()
		{
			if( _IsInit )
			{
				if( !FB.IsLoggedIn )
					Login();
				yield break;
			}
			//CHECK INTERNET
			Instance.m_loadingScreen.SetActiveInHierarchy();
			Debug.Log ("FB trying to init...");
			yield return CheckInternet.CheckAndWait().Start();
			if( CheckInternet._isConnectionAvailable )
			{
				_isDoingInit = true;
				FB.Init( _OnInitComplete, _OnHideUnity );
			}
			else Instance.m_loadingScreen.SetActiveInHierarchy(false);
		}

		protected static void _OnInitComplete()
		{
			Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
			_isDoingInit = false;
			if( _IsInit && Instance.m_autoLoginAfterInit )
			{
				Login();
			}
			else {
				Debug.LogError( "Couldn't initialize..", Instance );
				if( Instance.errorHolder )
					Instance.errorHolder.text = "Facebook initialization failed";
			}
		}		
		protected static void _OnHideUnity(bool isGameShown)
		{
			Debug.Log("Is game showing? " + isGameShown);
		}		
		#endregion
		
		#region FB.Login()	
		public static void Login()
		{
			Instance.StartCoroutine( Instance.LoginCo() );
		}
		public IEnumerator LoginCo()
		{
			if( _IsTryingToLogin || FB.IsLoggedIn )
				yield break;
			float time = Time.time;
			WaitForSeconds _wait = new WaitForSeconds( 0.5f );
			while( !_IsInit )
			{
				if( time + 60 <= Time.time )
					break;
				yield return _wait;
			}
			if( _IsInit )
			{				
				_IsTryingToLogin = true;
				//CHECK INTERNET
				if( m_loadingScreen.alpha == 0f )
					m_loadingScreen.SetActiveInHierarchy();
				if( m_withReadPermissions )
					FB.LogInWithReadPermissions( m_permissions, _LoginCallback );
				else FB.LogInWithPublishPermissions( m_permissions, _LoginCallback );
			}
			else { //Show couldn't connect message
				Debug.Log ( "Facebook connection failed, not init" );
				if( m_fbLoginEvents.onFBLoginFailed != null )
					m_fbLoginEvents.onFBLoginFailed.Invoke();
				m_loadingScreen.SetActiveInHierarchy(false);
			}
		}
		
		protected virtual void _LoginCallback( ILoginResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
			{
				_lastResponse = "FB Error Response:\n" + result.Error;
				if( errorHolder )
					errorHolder.text = result.Error;
				if( m_fbLoginEvents.onFBLoginFailed != null )
					m_fbLoginEvents.onFBLoginFailed.Invoke ();
			}
			else if ( result.Cancelled )	_lastResponse = "FB Login cancelled by Player";
			else {
				_AutoLoginWithFacebook = true;
				_lastResponse = "FB Login was successful!";
			}
			_DoLoginCallback();
		}		
		private void _DoLoginCallback()
		{
			_IsTryingToLogin = false;
			m_loadingScreen.SetActiveInHierarchy( false );
			if( FB.IsLoggedIn )
			{				
				_SyncUsername();
				if( m_profilePicHolder != null ){
					_SyncPic().Start();
				}
				if( testWithInvitableFriends ){
					_SyncInvitableFriends();
				}
				else _SyncFriends();
				
				_LoginEvent();
			}
			else 
			{
				CheckInternet.InternetIsNotAvailable();
			}
			if( !string.IsNullOrEmpty( _lastResponse ) )
				Debug.Log ( _lastResponse );
		}
		/// <summary>
		/// The event to execute when logged in to Facebook.
		/// </summary>
		protected virtual void _LoginEvent()
		{
			Debug.Log ( "Executing FB logged in event" );
			if( m_fbLoginEvents.onFBLoggedIn != null )
				m_fbLoginEvents.onFBLoggedIn.Invoke ();
		}
		#endregion			
		
		#region SYNC FUNCTIONS		
		/// <summary>
		/// Syncs the profile picture. Assigns the profile picture to the /_profilePicture/ variable. Call LoginAction ( SyncPic() )
		/// </summary>
		private IEnumerator _SyncPic()
		{
			string url = string.Format( "https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", 
				AccessToken.CurrentAccessToken.UserId , m_picSize, AccessToken.CurrentAccessToken );
			UnityWebRequest www = UnityWebRequest.GetTexture( url );
			yield return www.Send();
			_profilePicture = DownloadHandlerTexture.GetContent( www );
			Instance.m_profilePicHolder.SetSprite( _profilePicture );
		}		
		/// <summary>
		/// Syncs the profile username. Assings the username to the username variable.
		/// </summary>
		private void _SyncUsername()
		{
			FB.API("me?fields=name", HttpMethod.GET, _SyncUsernameCallback );
		}		
		/// <summary>
		/// Syncs the profile username. Assings the username to the username variable.
		/// </summary>
		private void _SyncInvitableFriends()
		{
			FB.API("me/invitable_friends", HttpMethod.GET, _SyncInvitableFriendsCallback );
		}		
		/// <summary>
		/// Syncs the user friends. Assings the friends to the friends variable.
		/// </summary>
		private void _SyncFriends()
		{
			FB.API("me/friends", HttpMethod.GET, _SyncFriendsCallback );
		}
		/// <summary>
		/// Syncs the profile gender. Assings the gender to the gender variable.
		/// </summary>
		private void _SyncGender()
		{
			FB.API("me?fields=gender", HttpMethod.GET, _SyncGenderCallback );
		}
		
		public void SyncScores()
		{
			FB.API(FB.AppId+"/scores", HttpMethod.GET, _SyncScoresCallback );
		}	
		#endregion

		#region CALLBACKS
		protected void _SyncUsernameCallback( IGraphResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
				_lastResponse = "FB Username Error Response:\n" + result.Error;
			else 
			{
				_lastResponse = "FB Username Synced";

				if( m_uiTextUsername != null )
				{
					get_data_username = result.RawResult;
					var dict = Json.Deserialize( get_data_username ) as IDictionary;
					_username = dict["name"].ToString();
					m_uiTextUsername.text = _username;
				}
			}
			Debug.Log ( _lastResponse );
		}		
		protected void _SyncFriendsCallback( IGraphResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
				_lastResponse = "FB Friends Error Response:\n" + result.Error;
			else 
			{
				_lastResponse = "FB Friends Synced";
				
				get_data_friends = result.RawResult;
				var _friends = (Json.Deserialize(get_data_friends) as IDictionary)["data"] as IList;
				
				_totalFriends = _friends.Count;
				
				_friends = new IDictionary[ _totalFriends ];
				
				for( int i=0; i< _totalFriends; i++ )
				{
					_friends[i] = _friends[i] as IDictionary;
				}				
			}
			Debug.Log ( _lastResponse );
		}		
		protected void _SyncInvitableFriendsCallback( IGraphResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
				_lastResponse = "FB Invitable Friends Error Response:\n" + result.Error;
			else 
			{
				_lastResponse = "FB Invitable Friends Synced";
				
				get_data_invitableFriends = result.RawResult;
				IList _friends = (Json.Deserialize(get_data_invitableFriends) as IDictionary)["data"] as IList;
				
				_totalInvitableFriends = _friends.Count;
				
				_InvitableFriends = new IDictionary[ _totalInvitableFriends ];
				
				for( int i=0; i< _totalInvitableFriends; i++ )
				{
					_InvitableFriends[i] = _friends[i] as IDictionary;
				}				
			}
			Debug.Log ( _lastResponse );
		}		
		protected void _SyncScoresCallback( IGraphResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) || Application.isEditor )
			{
				_lastResponse = "Scores Error Response:\n" + result.Error;
			}
			else 
			{
				_lastResponse = "FB Scores Synced";
				
				get_data_scores = result.RawResult;
				var _scores = (Json.Deserialize(get_data_scores) as IDictionary)["data"] as IList;
				
				_totalFriends_scores = _scores.Count;
				
				_friends_scores = new IDictionary[ _totalFriends_scores ];
				for( int i=0; i< _totalFriends_scores; i++ )
				{
					_friends_scores[i] = _scores[i] as IDictionary;
				}	
			}
			Debug.Log ( _lastResponse );
		}		
		protected void _SyncGenderCallback( IGraphResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
				_lastResponse = "FB Gender Error Response:\n" + result.Error;
			else 
			{
				_lastResponse = "FB Gender Synced";

				get_data_gender = result.RawResult;
				var dict = Json.Deserialize( get_data_gender ) as IDictionary;
				_gender = dict["gender"].ToString();
			}
			Debug.Log ( _lastResponse );
		}
		protected void _SetScoreCallback( IResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
				_lastResponse = "FB Set Score Error Response:\n" + result.Error;
			else Debug.Log ( "FB score set!" );
			if( !string.IsNullOrEmpty( _lastResponse ) )
				Debug.Log ( _lastResponse );
		}		
		protected void _GetScoreCallback( IResult result )
		{
			if ( !string.IsNullOrEmpty( result.Error ) )
				_lastResponse = "FB Get Score Error Response:\n" + result.Error;
			else 
			{
				Debug.Log ( "FB score received!" );
				_score = (int)( (Json.Deserialize( result.RawResult ) as IDictionary)["data"] as IDictionary )["score"];
			}
			if( !string.IsNullOrEmpty( _lastResponse ) )
				Debug.Log ( _lastResponse );
		}
		#endregion CALLBACKS
		
		#region STATIC FUNCTIONS	
		public static void Sync_Friends()
		{
			Instance._SyncFriends();
		}		
		public static void Sync_Scores()
		{
			Instance.SyncScores();
		}
		/// <summary>
		/// Sets the picture ( into a sprite ) from an invitable friend. InvitableFriends must have been synced
		/// </summary>
		/// <returns>The picture from invitable friend.</returns>
		/// <param name="friendIndex">Friend index from the InvitableFriends array.</param>
		/// <param name="spritePictureHolder">Sprite picture holder, must be a sprite (with a SpriteRenderer).</param>
		public static IEnumerator SetUIPictureFromInvitableFriend( int friendIndex, GameObject spritePictureHolder )
		{
			string url = (string)(( (IDictionary)( (IDictionary)_InvitableFriends[friendIndex]["picture"] )["data"] )["url"]);
			UnityWebRequest www = UnityWebRequest.GetTexture( url );
			yield return www.Send();
			spritePictureHolder.SetUISpriteTexture( DownloadHandlerTexture.GetContent( www ) );
		}		
		/// <summary>
		/// Gets the name from an invitable friend. InvitableFriends must have been synced
		/// </summary>
		/// <param name="friendIndex">Friend index from the InvitableFriends array.</param>
		public static string GetNameFromInvitableFriend( int friendIndex )
		{
			return (string)_InvitableFriends[friendIndex]["name"];
		}		
			
		/// <summary>
		/// A friend's last get picture.
		/// </summary>
		public static Texture2D _FriendLastPicture{ get; private set; }
		/// <summary>
		/// Syncs the picture from a friend. Friends must have been synced. The picture is stored in _FriendLastPicture variable.
		/// </summary>
		/// <param name="friendId">Friend id.</param>
		public static void SyncPictureFromFriend( string friendId, PictureTypes type = PictureTypes.Normal )
		{
			SetUIPictureFromFriend( friendId, null, type );
		}		
		/// <summary>
		/// Sets the picture from a friend. Friends must have been synced,
		/// </summary>
		/// <param name="friendId">Friend id.</param>
		/// <param name="spritePictureHolder">The gameobject (must contain Ui Image component) that will hold the picture.</param>
		public static IEnumerator SetUIPictureFromFriend( string friendId, GameObject spritePictureHolder, PictureTypes type = PictureTypes.Small )
		{
			string url = string.Format( "https://graph.facebook.com/{0}/picture?type={1}", friendId, GetPictureType( type ) );
			UnityWebRequest www = UnityWebRequest.GetTexture(url);
			yield return www.Send();
			_FriendLastPicture = DownloadHandlerTexture.GetContent( www );
			if( spritePictureHolder != null )
				spritePictureHolder.SetUISpriteTexture( _FriendLastPicture );
		}			
		/// <summary>
		/// Syncs the picture from a friend. Friends must have been synced. The picture is stored in _FriendLastPicture variable.
		/// </summary>
		/// <param name="friendIndex">Friend index from the Friends array.</param>
		public static void SyncPictureFromFriend( int friendIndex, PictureTypes type = PictureTypes.Small )
		{
			SetPictureFromFriend( friendIndex, null, type );
		}
		/// <summary>
		/// Sets the picture ( into a sprite ) from a friend. Friends must have been synced
		/// </summary>
		/// <returns>The picture from a friend.</returns>
		/// <param name="friendIndex">Friend index from the Friends array.</param>
		/// <param name="spritePictureHolder">Sprite picture holder, must be a sprite (with a SpriteRenderer).</param>
		public static void SetPictureFromFriend( int friendIndex, GameObject spritePictureHolder, PictureTypes type = PictureTypes.Small )
		{
			string friendId = (string)_friends[friendIndex]["id"] ;
			SetUIPictureFromFriend( friendId, spritePictureHolder, type ).Start();
		}		
		/// <summary>
		/// Sets the score from a user or from the actual user if param is null.
		/// </summary>
		/// <param name="score">Score.</param>
		/// <param name="userId">User identifier.</param>
		/// <param name="saveUsername">PARSE ONLY. This must be true if saving score to parse and want to save the username.</param>
		public static void SetScore( int score, string userId = null, bool saveUsername = true )
		{
			_SetScore( score, userId, saveUsername );
		}

		private static void _SetScore( int score, string userId = null, bool saveUsername = true )
		{
			if( userId == null )
				userId = AccessToken.CurrentAccessToken.UserId;
			FB.API( string.Format( "{0}/scores?score={1}", userId, score ), HttpMethod.POST, Instance._SetScoreCallback );
		}

		/// <summary>
		/// Gets the score from a user or from the actual user if param is null.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		/// <param name="score">
		public static IEnumerable<int> GetScore( string userId = null )
		{
			if( userId == null )
				userId = AccessToken.CurrentAccessToken.UserId;
			FB.API(AccessToken.CurrentAccessToken.UserId+"/scores?score", HttpMethod.GET, Instance._GetScoreCallback );
			yield return FacebookManager._score;
		}		
		/// <summary>
		/// Gets the name from a friend. Friends must have been synced.
		/// </summary>
		/// <param name="friendIndex">Friend index from the Friends array.</param>
		public static string GetNameFromFriend( int friendIndex )
		{
			return (string)_friends[friendIndex]["name"];
		}		
		/// <summary>
		/// Gets the id from a friend. Friends_Scores must have been synced.
		/// </summary>
		/// <param name="friendIndex">Friend index from the Friends array.</param>
		public static string GetIdFromFriend( int friendIndex )
		{
			return (string)( _friends[friendIndex]["id"] );
		}		
		/// <summary>
		/// Gets the name from a friend. Friends_Scores must have been synced.
		/// </summary>
		/// <param name="friendIndex">Friend index from the Friends array.</param>
		public static string GetNameFromFriends_Scores( int friendIndex )
		{
			return (string)( (IDictionary)_friends_scores[friendIndex]["user"] )["name"];
		}		
		/// <summary>
		/// Gets the id from a friend. Friends_Scores must have been synced.
		/// </summary>
		/// <param name="friendIndex">Friend index from the Friends array.</param>
		public static string GetIdFromFriends_Scores( int friendIndex )
		{
			return (string)( (IDictionary)_friends_scores[friendIndex]["user"] )["id"];
		}		
		/// <summary>
		/// Gets the score from a friend. Friends_Scores must have been synced.
		/// </summary>
		/// <param name="friendIndex">Score index (friend index, scores are retrieved in descending order) from the Friends_Scores array.</param>
		public static int GetScoreFromFriends_Scores( int scoreIndex )
		{
			return (int)Convert.ToInt32( _friends_scores[scoreIndex]["score"] );
		}		

		public static bool _invitingFriends;

		protected static void _LogCallback(IResult response) {
			Debug.Log( response.RawResult );
			_invitingFriends = false;
		}

		public static void AddFriends( string msg = "Come play this great game!" )
		{
			_invitingFriends = true;
			FB.AppRequest(
				message: msg, 
				callback: _LogCallback
			);
		}
		#endregion
		
		#endif
		
		
		
		
	}

}
