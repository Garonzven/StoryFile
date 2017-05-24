//Improved By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Classes;
using DDK.Base.Extensions;
using DDK.Base.Events;
using DDK.Base.Statics;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif


namespace DDK.Base.Fx.Transitions 
{
	/// <summary>
	/// Use this class's static methods to fade in game or while transitioning from one scene to another, as both 
    /// (Unity 5.2 and below) Application.LoadLevel.. and (Unity 5.3 and over) SceneManager.LoadScene... replacement. 
    /// DO NOT attach this directly to an object.
	/// </summary>
	public class AutoFade : MonoBehaviour//This class has a custom menu item
	{
        private static AutoFade _Instance;

        private Material _material;
        private string _sceneName;
        private int _sceneIndex;
        private bool _fading;
		
		private static AutoFade Instance
		{
			get
			{
                return _Instance == null ? _Instance = new GameObject("AutoFade").AddComponent<AutoFade>() : _Instance;
			}
		}
		public static bool _Fading
		{
			get 
            { 
                #if UNITY_EDITOR //PREVENT INSTANCES FROM BEING CREATED IN THE EDITOR
                if( !Application.isPlaying )
                    return false;
                #endif
                return Instance._fading;
            }
		}


		
		void Awake()
		{
			DontDestroyOnLoad(this);
			_Instance = this;
#if !UNITY_5
			m_Material = new Material("Shader \"Plane/No zTest\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off Fog { Mode Off } BindChannels { Bind \"Color\",color } } } }");
#else
			_material = new Material( Resources.Load<Shader>( "Shaders/AutoFade" ) );
#endif
		}
        #if UNITY_EDITOR
        void Start()
        {
            if( !Application.isPlaying )
            {
                Debug.LogError( "This shouldn't be directly attached to a gameObject. Destroying component.." );
                DestroyImmediate( this );
            }
        }
        #endif



        /// <summary>
        /// Draws a quad in the screen using GL functions.
        /// </summary>
        /// <param name="aColor">The screen quad's color.</param>
        /// <param name="aAlpha">The screen quad's alpha.</param>
		private void _DrawQuad( Color aColor, float aAlpha )
		{
			aColor.a = aAlpha;
			_material.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Begin(GL.QUADS);
			GL.Color(aColor);
			GL.Vertex3(0, 0, -1);
			GL.Vertex3(0, 1, -1);
			GL.Vertex3(1, 1, -1);
			GL.Vertex3(1, 0, -1);
			GL.End();
			GL.PopMatrix();
		}		
        /// <summary>
        /// Fades the screen to the specified /aColor/ in the specified /aFadeOutTime/, Loads the specified /_sceneName/
        ///  and fades back again.
        /// </summary>
        /// <param name="aFadeOutTime">The time it takes to fade the screen to the specified /aColor/.</param>
        /// <param name="aFadeInTime">The time it takes to fade the screen back.</param>
        /// <param name="aColor">The color in which this will fade into before loading the screen.</param>
		private IEnumerator _Fade( float aFadeOutTime, float aFadeInTime, Color aColor )
		{
            _fading = true;
			float t = 0.0f;
			while( t<1.0f )
			{
				yield return new WaitForEndOfFrame();
				t = Mathf.Clamp01( t + Time.unscaledDeltaTime / aFadeOutTime );
                if( !aColor.a.CloseTo( 0f ) )
                {
                    _DrawQuad( aColor,t );
                }
			}
			#if UNITY_5_3_OR_NEWER
			if ( !string.IsNullOrEmpty( _sceneName ) )
				SceneManager.LoadScene( _sceneName );
			else SceneManager.LoadScene( _sceneIndex );
			#else
			if ( !string.IsNullOrEmpty( m_SceneName ) )
				Application.LoadLevel( m_SceneName );
			else Application.LoadLevel( m_SceneIndex );
			#endif
			while ( t>0.0f )
			{
				yield return new WaitForEndOfFrame();
				t = Mathf.Clamp01(t - Time.unscaledDeltaTime / aFadeInTime);
                if( !aColor.a.CloseTo( 0f ) )
                {
                    _DrawQuad( aColor,t );
                }
			}
			_fading = false;
		}

		
        /// <summary>
        /// Loads the level with the specified /aLevelName/ name by fading out the screen to the specified /aColor/ in 
        /// the specified /aFadeOutTime/ and fading back in in the specified /aFadeInTime/ after loading.
        /// </summary>
		public static void LoadLevel( string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor )
		{
			if (_Fading) return;
			Instance._sceneName = aLevelName;
            Instance.StartCoroutine( Instance._Fade( aFadeOutTime, aFadeInTime, aColor ) );
		}
        /// <summary>
        /// Loads the level with the specified /aLevelIndex/ index by fading out the screen to the specified /aColor/ in 
        /// the specified /aFadeOutTime/ and fading back in in the specified /aFadeInTime/ after loading.
        /// </summary>
		public static void LoadLevel( int aLevelIndex,float aFadeOutTime, float aFadeInTime, Color aColor)
		{
			if (_Fading) return;
			Instance._sceneName = "";
			Instance._sceneIndex = aLevelIndex;
            Instance.StartCoroutine( Instance._Fade( aFadeOutTime, aFadeInTime, aColor ) );
		}


		/// <summary>
		/// Holds a reference if a level is being loaded (asynchronously)
		/// </summary>
		public static Dictionary<string, AsyncOperation> _loadingOperation;
		/// <summary>
		/// The name of the last loaded level.
		/// </summary>
		public static string _lastLoadedLevel;
		private static Dictionary<string, bool> _showAsyncLoadedLevel;
		/// <summary>
		/// If true, the asynchronously loaded level will be shown immediately after it has been loaded.
		/// </summary>
		public static Dictionary<string, bool> _ShowAsyncLoadedLevel
        {
			get
            {
                return _showAsyncLoadedLevel == null ? 
                    _showAsyncLoadedLevel = new Dictionary<string, bool>() : _showAsyncLoadedLevel;
			}
			set { _showAsyncLoadedLevel = value; }
		}

        /// <summary>
        /// Shows the async loaded level.
        /// </summary>
        /// <param name="levelName">Level name.</param>
        /// <param name="delay">Delay before showing the level.</param>
		public static IEnumerator ShowAsyncLoadedLevelCo( string levelName, float delay = 0f )
		{
			if( delay > 0f )
				yield return new WaitForSeconds( delay );
			_ShowAsyncLoadedLevel[ levelName ] = true;
		}
        /// <summary>
        /// Shows the last async loaded level.
        /// </summary>
        /// <param name="delay">Delay before showing the level.</param>
		public static IEnumerator ShowLastAsyncLoadedLevelCo( float delay = 0f )
		{
			yield return Instance.StartCoroutine( ShowAsyncLoadedLevelCo( _lastLoadedLevel, delay ) );
        }
        /// <summary>
        /// Shows the async loaded level.
        /// </summary>
        /// <param name="levelName">Level name.</param>
        /// <param name="delay">Delay before showing the level.</param>
		public static void ShowAsyncLoadedLevel( string levelName, float delay = 0f )
		{
			Instance.StartCoroutine( ShowAsyncLoadedLevelCo( levelName, delay ) );
		}
        /// <summary>
        /// Shows the last async loaded level.
        /// </summary>
        /// <param name="delay">Delay before showing the level.</param>
		public static void ShowLastAsyncLoadedLevel( float delay = 0f )
		{
			Instance.StartCoroutine( ShowLastAsyncLoadedLevelCo( delay ) );
        }


        /// <summary>
        /// Fades out the screen to the specified /aColor/ in the specified /aFadeOutTime/, loads the level with the 
        /// specified /levelName/ and waits until /_ShowAsyncLoadedLevel/'s key with the level name holds a true value, 
        /// then it shows the level and fades the screen back in in the specified /aFadeInTime/.
        /// </summary>
        private IEnumerator _FadeAsync( string levelName, float aFadeOutTime, float aFadeInTime, Color aColor )
		{
			if( string.IsNullOrEmpty( levelName ) )
			{
				yield break;
			}
			if( !this.IsLevelAvailable( levelName, true ) )
			{
				yield break;
			}
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while( _fading )
			{
                yield return waitForEndOfFrame;
			}
			if( _loadingOperation == null )
			{
				_loadingOperation = new Dictionary<string, AsyncOperation>();
			}
			else if ( _loadingOperation.ContainsKey( levelName ) )
			{
				Utilities.LogWarning ("The level \"" + levelName + "\" is already being loaded async", gameObject );
				yield break;
			}
			if( !_ShowAsyncLoadedLevel.ContainsKey( levelName ) )
			{
				_ShowAsyncLoadedLevel.Add( levelName, false );
			}
			//LOAD ASYNC
			#if UNITY_5_3_OR_NEWER
			_loadingOperation[ levelName ] = SceneManager.LoadSceneAsync( levelName );
			#else
			loadingOperation[ levelName ] = Application.LoadLevelAsync( levelName );
			#endif
			//WAIT AND SHOW
			if( _loadingOperation[ levelName ] == null )
				yield break;
            Utilities.Log ( "Loading level async: " + levelName, gameObject );
			_loadingOperation[ levelName ].allowSceneActivation = false;
			_lastLoadedLevel = levelName;
			while( !_ShowAsyncLoadedLevel[ levelName ] )
			{
				yield return null;
				if( !_ShowAsyncLoadedLevel.ContainsKey( levelName ) )//PREVENT BUG
				{
					if( _loadingOperation != null )
						_loadingOperation.Remove( levelName );
					yield break;
				}
			}
			if( _ShowAsyncLoadedLevel.ContainsKey( levelName ) )
			{
                WaitForEndOfFrame _wait = new WaitForEndOfFrame();
				//FADE OUT - SHOW - FADE IN
				_fading = true;
				float t = 0.0f;
				while( t<1.0f )
				{
                    yield return _wait;
					t = Mathf.Clamp01( t + Time.unscaledDeltaTime / aFadeOutTime );
                    if( !aColor.a.CloseTo( 0f ) )
                    {
                        _DrawQuad(aColor,t);
                    }
				}
				_ShowAsyncLoadedLevel.Remove( levelName );

				while( _loadingOperation[ levelName ] != null )
				{
                    yield return _wait;
                    if( !aColor.a.CloseTo( 0f ) )
                    {
                        _DrawQuad(aColor,t);
                    }
					if( _loadingOperation[ levelName ].progress >= 0.9f )
						break;
				}
				_loadingOperation[ levelName ].allowSceneActivation = true;
				while( _loadingOperation[ levelName ] != null )
				{
                    yield return _wait;
                    if( !aColor.a.CloseTo( 0f ) )
                    {
                        _DrawQuad(aColor,t);
                    }
					if( _loadingOperation[ levelName ].isDone || _loadingOperation[ levelName ].progress == 1f )
						break;
				}
                #if UNITY_5_3_OR_NEWER
                while( !SceneManager.GetActiveScene().name.Equals( levelName ) )
                #else
                while( !Application.loadedLevelName.Equals( levelName ) )
                #endif
				{
                    yield return _wait;
                    if( !aColor.a.CloseTo( 0f ) )
                    {
                        _DrawQuad(aColor,t);
                    }
				}
				_loadingOperation.Remove( levelName );
				_loadingOperation = null;
				
				while( t>0.0f )
				{
                    yield return _wait;
					t = Mathf.Clamp01( t - Time.unscaledDeltaTime / aFadeInTime );
                    if( !aColor.a.CloseTo( 0f ) )
                    {
                        _DrawQuad(aColor,t);
                    }
				}
				_fading = false;
			}
		}
        /// <summary>
        /// Fades out the screen to the specified /aColor/ in the specified /aFadeOutTime/, loads the level with the 
        /// specified /levelName/ and waits until /_ShowAsyncLoadedLevel/'s key with the level name holds a true value, 
        /// then it shows the level and fades the screen back in in the specified /aFadeInTime/.
        /// </summary>
		private void _StartFadeAsync( string levelName, float aFadeOutTime, float aFadeInTime, Color aColor )
		{
			StartCoroutine( _FadeAsync( levelName, aFadeOutTime, aFadeInTime, aColor ) );
		}

		/// <summary>
		/// Loads the specified /aLevelName/ async. Use the static variable /ShowAsyncLoadedLevel/ to allow or prevent 
        /// automatic level loading after load is completed, by default it is set to false. This function uses a default 
        /// FadeSettings object with a black fade color and 0.5f fade in and out duration.
		/// </summary>
		public static void LoadLevelAsync( string aLevelName )
		{
			FadeSettings transition = new FadeSettings( Color.black );
			Instance._StartFadeAsync( aLevelName, transition.fadeOutTime, transition.fadeInTime, transition.fadeColor );
		}
		/// <summary>
		/// Load the level with the specified /aLevelName/ using the specified /transition/. Use the static variable 
        /// /ShowAsyncLoadedLevel/ to allow or prevent automatic level loading after load is completed, by default it is 
        /// set to false.
		/// </summary>
		public static void LoadLevelAsync( string aLevelName, FadeSettings transition )
		{
			Instance._StartFadeAsync( aLevelName, transition.fadeOutTime, transition.fadeInTime, transition.fadeColor );
		}
		/// <summary>
		/// Loads a random level from the specified /levelsNames/ list using the specified /transition/. Use the static 
        /// variable /ShowAsyncLoadedLevel/ to allow or prevent automatic level loading after load is completed, by 
        /// default it is set to false.
		/// </summary>
		/// <returns>The randomly chosen level.</returns>
		public static string LoadLevelAsync( IList<string> levelsNames, FadeSettings transition )
		{
			if( levelsNames == null || levelsNames.Count == 0 )
				return null;
			string aLevelName = levelsNames.GetRandom();
			LoadLevelAsync( aLevelName, transition );
			return aLevelName;
		}
        /// <summary>
        /// Fades out the screen to the specified /aColor/ in the specified /aFadeOutTime/, loads the level with the 
        /// specified /aLevelName/ and waits until /_ShowAsyncLoadedLevel/'s key with the level name holds a true value, 
        /// then it shows the level and fades the screen back in in the specified /aFadeInTime/.
        /// </summary>
		public static void LoadLevelAsync( string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor )
		{
			Instance._StartFadeAsync( aLevelName, aFadeOutTime, aFadeInTime, aColor );
		}


		private bool _fadingIn = false;
		private bool _fadingOut = false;
		public static bool _FadingIn
        {
			get{ return Instance._fadingIn; }
		}
		public static bool _FadingOut
        {
			get{ return Instance._fadingOut; }
		}		
	

        /// <summary>
        /// Fades out the screen.
        /// </summary>
        /// <param name="aFadeOutTime">The fade out time.</param>
        /// <param name="aColor">The color to fade into.</param>
		private IEnumerator _FadeOutNoLoad( float aFadeOutTime, Color aColor )
		{
			float t = 0.0f;
			_fadingIn = true;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while( t<1.0f )
			{
                yield return waitForEndOfFrame;
				t = Mathf.Clamp01( t + Time.unscaledDeltaTime / aFadeOutTime );
                if( !aColor.a.CloseTo( 0f ) )
                {
                    _DrawQuad( aColor, t );
                }
			}
			_fadingIn = false;
        }
        /// <summary>
        /// Fades in the screen.
        /// </summary>
        /// <param name="aFadeInTime">The fade in time.</param>
        /// <param name="aColor">The color to fade from.</param>
		private IEnumerator _FadeInNoLoad( float aFadeInTime, Color aColor )
		{
			float t = 1f;
			_fadingOut = true;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while( t>0.0f )
			{
                yield return waitForEndOfFrame;
				t = Mathf.Clamp01( t - Time.unscaledDeltaTime / aFadeInTime );
                if( !aColor.a.CloseTo( 0f ) )
                {
                    _DrawQuad( aColor, t );
                }
            }
            _fading = false;
            _fadingOut = false;
        }
        /// <summary>
        /// Fades the screen out to the specified /aColor/ and then back in.
        /// </summary>
        /// <param name="aFadeOutTime">The fade out time.</param>
        /// <param name="aFadeInTime">The fade in time.</param>
        /// <param name="aColor">The color to fade into.</param>
		private IEnumerator _FadeNoLoad( float aFadeOutTime, float aFadeInTime, Color aColor )
		{
            _fading = true;
			yield return StartCoroutine( _FadeOutNoLoad( aFadeOutTime, aColor ) );
			yield return StartCoroutine( _FadeInNoLoad( aFadeInTime, aColor ) );
		}   
        
        

        /// <summary>
        /// Fades the out the screen to the specified /aColor/ and then back in.
        /// </summary>
        /// <param name="aFadeOutTime">The fade out time.</param>
        /// <param name="aFadeInTime">The fade in time.</param>
        /// <param name="aColor">The color to fade into</param>
		public static void FadeOutIn( float aFadeOutTime, float aFadeInTime, Color aColor )
		{
			if (_Fading) return;
            Instance.StartCoroutine( Instance._FadeNoLoad( aFadeOutTime, aFadeInTime, aColor ) );
		}
        /// <summary>
        /// Fades the out the screen to the specified color and then back in.
        /// </summary>
        /// <param name="fade">The fade settings.</param>
		public static void FadeOutIn( FadeSettings fade )
		{
			if (_Fading) return;
            Instance.StartCoroutine( Instance._FadeNoLoad( fade.fadeOutTime, fade.fadeInTime, fade.fadeColor ) );
        }
		/// <summary>
		/// Fades in the screen from the specified color.
		/// </summary>
		/// <param name="fade">The fade settings.</param>
		public static void FadeIn( FadeSettings fade )
		{
			if (_Fading) return;
            Instance.StartCoroutine( Instance._FadeInNoLoad( fade.fadeInTime, fade.fadeColor ) );
		}
		/// <summary>
        /// Fades out the screen to the specified color.
        /// </summary>
        /// <param name="fade">The fade settings.</param>
		public static void FadeOut( FadeSettings fade )
		{
			if (_Fading) return;
            Instance.StartCoroutine( Instance._FadeOutNoLoad( fade.fadeOutTime, fade.fadeColor ) );
        }
	}
}
