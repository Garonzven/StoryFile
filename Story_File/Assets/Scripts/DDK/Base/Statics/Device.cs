//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


#pragma warning disable 162
namespace DDK.Base.Statics 
{
	/// <summary>
	/// Allows getting some device related data
	/// </summary>
	public static class Device 
    {		
		public static bool isTouchable 
        {			
            get
            {
				if( SystemInfo.deviceType == DeviceType.Desktop && Cursor.visible ) return false;
				else return true;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the screen is vertical oriented.
		/// </summary>
		/// <value><c>true</c> if vertical; otherwise, <c>false</c>.</value>
		public static bool isVertical
        {
			get{ return ( Screen.height > Screen.width ) ? true : false; }
		}
		
		public static bool isAndroid
        {
			get
            {
				#if UNITY_ANDROID
				return true;
				#endif
				return false;
			}
		}
		
		public static bool isIOS
        {
			get
            {
				#if UNITY_IOS
				return true;
				#endif
				return false;
			}
		}
		public static bool isWP8
        {
			get
            {
				#if UNITY_WP8
				return true;
				#endif
				return false;
			}
		}
		public static bool isBB10
        {
			get
            {
				#if UNITY_BLACKBERRY
				return true;
				#endif
				return false;
			}
		}
		public static bool isWindows
        {
			get
            {
				#if UNITY_STANDALONE_WIN
				return true;
				#endif
				return false;
			}
		}
		
		public static bool isWeb
        {
			get
            {
				#if UNITY_WEBPLAYER
				return true;
				#endif
				return false;
			}
		}
		
		public static float deltaTime
        {
			get{ return Time.deltaTime * 100; }
		}

		/// <summary>
		/// For 2D games, gets the x positive bound. If Camera is at 0 this value can be used negative also.
		/// </summary>
		/// <value>
		/// The +X bound.
		/// </value>
		public static float xBound
		{
			get{ return Camera.main.ScreenToWorldPoint( new Vector2( Screen.width, 0f ) ).x; }
		}
		/// <summary>
		/// For 2D games, gets the y positive bound. If Camera is at 0 this value can be used negative also.
		/// </summary>
		/// <value>
		/// The +Y bounds.
		/// </value>
		public static float yBound
		{
			get{ return Camera.main.ScreenToWorldPoint( new Vector2( 0f, Screen.height ) ).y; }
		}

		/// <summary>
		/// For 2D games, gets the x negative bound.
		/// </summary>
		/// <value>
		/// The -x bound.
		/// </value>
		public static float _xBound
		{
			get{ return Camera.main.ScreenToWorldPoint( Vector2.zero ).x; }
		}
		/// <summary>
		/// For 2D games, gets the y negative bound.
		/// </summary>
		/// <value>
		/// The -Y bounds.
		/// </value>
		public static float _yBound
		{
			get{ return Camera.main.ScreenToWorldPoint( Vector2.zero ).y; }
		}
		/// <summary>
		/// Returns a random screen point as world point.
		/// </summary>
		/// <value>A random screen point.</value>
		public static Vector2 getRandomScreenPoint 
        {
			get{ return new Vector2( Random.Range( _xBound, xBound ), Random.Range( _yBound, yBound ) ); }
		}

		/// <summary>
		/// Gets the real aspect ratio. It is always higher than 1.
		/// </summary>
		/// <value>The real_ A.</value>
		public static float aspectRatio
        {
			get
            {
				float w = Screen.width;
				float h = Screen.height;
				if( Screen.width > Screen.height )
					return w / h;
				else return h / w;
			}
		}		
	}
}