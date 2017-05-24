//By: Daniel Soto
//4dsoto@gmail.com
using System;
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using DDK.Base.Fx.Transitions;
using DDK.Base.Classes;
using DDK.Base.Statics;


#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif


namespace DDK.AssetBundles 
{
	/// <summary>
	/// Attach to an object to configure the assets you want to load. If you want more control over an asset bundle, like for 
	/// example, prevent it from being instantiated if it's a Game Object, see the ABController class.
	/// </summary>
	public class AssetBundlesLoader : MonoBehaviour 
    {
        [Tooltip("If true WWW will be used instead of UnityWebRequest")]
        public bool useWwwIntead;
		public bool useStreamingAssets = false;
        #if UNITY_EDITOR
        [InspectorButtonAttribute("SetURLFromLocalPath", true)]
        public bool setUrlFromLocalPath;
        #endif
		[Tooltip("This is the main root. This will be added before each bundle's URL")]
		public string mainUrl;
		[Tooltip("The bundles to load. Order matters..")]
		public Bundle[] bundles;
		/// <summary>
		/// This will be active while loading the bundles. If this has a Slider on any active/inactive children/subchildren it will be considered as a progress bar.
		/// </summary>
		[Space(10f)]
		[Tooltip("This will be active while loading the bundles. If this has a Slider on any active/inactive children/subchildren it will be considered as a progress bar")]
		public GameObject loadingIndicator;
		/// <summary>
		/// If true, the loading indicator will be automatically destroyed after all bundles are loaded.
		/// </summary>
		[Tooltip("If true, the loading indicator will be automatically destroyed after all bundles are loaded")]
		[ShowIfAttribute( "_UsingLoadingIndicator", 1 )]
		public bool autoDestroyLoadingIndicator = true;
        [Tooltip("If true, all gameObjects in bundles marked as instantiate = true will be instantiated just after downloaded.")]
        public bool autoInstantiateAll;
		public ComposedEvent onAllLoaded = new ComposedEvent();
		/// <summary>
		/// If true, the isLoaded static variable will be reset, after loading is done, to allow next Loader call to progress.
		/// </summary>
		[Tooltip("If true, the isLoaded static variable will be reset, after loading is done, to allow next Loader call to progress")]
		public bool resetIsLoaded;
		#if UNITY_EDITOR
		[Space(10f)]
		[Tooltip("If true, the download progress will be logged into the console. Just in Unity Editor")]
		public bool logProgress;


        protected void SetURLFromLocalPath()
        {
            #if UNITY_IOS
            mainUrl = "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/iOS/";
            #elif UNITY_ANDROID
            mainUrl = "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/Android/";
            #elif UNITY_STANDALONE_WIN
            mainUrl = "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/Windows/";
            #elif UNITY_STANDALONE_OSX
            mainUrl = "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/OSX/";
            #endif
        }
		#endif


		//ADDED TO TEST INSTANTIATE AT THE END
		protected List<UnityEngine.Object> _objsToInstantiate =  new List<UnityEngine.Object>();

		protected bool _UsingLoadingIndicator()
		{
			return loadingIndicator != null;
		}


		/// <summary>
		/// If the /loadingIndicator/ has a slider it will be considerer a progress bar. In such case, the progress bar
		/// will be updated automatically when loading the bundles.
		/// </summary>
		protected Slider _progressBar;
		protected List<AssetBundle> _bundles;

        /// <summary>
        /// This holds the last request made.
        /// </summary>
        private static UnityWebRequest _request;
        /// <summary>
        /// This holds the last request made.
        /// </summary>
        private static WWW _www;

        public static AssetBundlesLoader Instance { get; private set; }
		/// <summary>
		/// Is true, when an asset bundle loader is being initialized, and it hasn't unloaded any assets yet.
		/// </summary>
		public static bool _IsTryingToLoad { get; private set; }
		/// <summary>
		/// Becomes true, when all asset bundles have been loaded. This must be manually reset to allow next Loader calls
		/// to progress, or you can also set the /resetIsLoaded/ public variable to true.
		/// </summary>
		public static bool _IsLoaded { get; set; }
		/// <summary>
		/// Is true, if this class is loading asset bundles.
		/// </summary>
		public static bool _IsLoading { get; private set; }
        public static bool _CancelingCurrentDownloads { get; private set; }
        /// <summary>
        /// becomes true if the last bundle being downloaded had an error.
        /// </summary>
        public static bool _DownloadHadAnError { get; private set; }
		#region OVERRIDERS
		private static string _mainUrlOverrider;
		private static Bundle[] _bundlesOverrider;
		private static ComposedEvent _onAllLoadedOverrider;
		public static string _MainUrlOverrider {
			get {
				return _mainUrlOverrider;
			}
			set {
				if( !string.IsNullOrEmpty( value ) )
				{
					_mainUrlOverrider = value;
				}
			}
		}
		public static Bundle[] _BundlesOverwriter {
			get {
				return _bundlesOverrider;
			}
			set {
				if( value != null )
				{
					_bundlesOverrider = value;
				}
			}
		}
		public static ComposedEvent _OnAllLoadedOverrider {
			get {
				return _onAllLoadedOverrider;
			}
			set {
				if( value != null )
				{
					_onAllLoadedOverrider = value;
				}
			}
		}
		#endregion
        public static Action onDownloadHadAnError;

		static bool _downloadedAssetPacks = false;

        void Awake()
        {
            if( Instance )
            {
                Utilities.Log( "There is an active instance of AssetBundlesLoader already in the scene. Destroying this one" );
                DestroyImmediate( gameObject );
                return;
            }
            Instance = this;
        }
		void OnEnable() 
		{
			DownloadAndCacheAll().Start();
		}



		void _SetProgressBar( GameObject loadingIndicator )
		{
			if( loadingIndicator )
			{
				_progressBar = loadingIndicator.GetComponentIncludeChildren<Slider>();
			}
		}


        public static string GetLocalAssetBundlesPath()
        {
#if UNITY_IOS
            return "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/iOS/";
#elif UNITY_ANDROID
            return "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/Android/";
#elif UNITY_STANDALONE_WIN
            return "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/Windows/";
#elif UNITY_STANDALONE_OSX
            return "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/OSX/";
#else
            return "file://" + Application.dataPath.RemoveLastEndPoint(false) + "AssetBundles/";
#endif
        }
        /// <summary>
        /// Cancels the current downloads after the bundle that's currently being downloaded finished downloading.
        /// </summary>
        /// <param name="forceCancel">If set to <c>true</c> the bundle that's currently being downloaded will be also canceled.</param>
        public static void CancelAllCurrentDownloads( bool forceCancel = false )
        {
            ClearAllCurrentDownloads();
            _CancelingCurrentDownloads = true;
			Instance._UnloadAllBundles(true);
            if( forceCancel && !Instance.useWwwIntead )
            {
				_request.Abort ();
            }
        }
        public static void ClearAllCurrentDownloads()
        {
            Instance._objsToInstantiate.Clear();
            _IsLoading = false;
            _IsLoaded = false;
            _IsTryingToLoad = false;
        }
        public static bool AreBundlesCached( Bundle[] bundles )
        {
            bool cached = true;
            for( int i=0; i<bundles.Length; i++ )
            {
                if( bundles[i] == null || !Caching.IsVersionCached( bundles[i].url, bundles[i].version ) )
                {
                    cached = false;
                    break;
                }
            }
            return cached;
        }


        public IEnumerator DownloadAndCacheAll ()
		{
			//PROCEED OR WAIT
			if( _IsLoaded || _IsTryingToLoad )
			{
                Utilities.Log ( "Waiting for /isLoaded/ to be reset. Make sure it is being reset..." );
				while( _IsLoaded || _IsTryingToLoad )
					yield return null;
			}
			_IsTryingToLoad = true;

			_CheckOverwrites();

			// Wait for the Caching system to be ready
			while ( !Caching.ready )
				yield return null;

			GameObject _loadingIndicator = null;
			if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.Equals ("INTRO")) {
				_loadingIndicator = loadingIndicator.SetActiveInHierarchy ();//SHOW LOADING SCREEN
				_SetProgressBar (_loadingIndicator);
			}
			if( bundles.Length > 0 )
			{
				_IsLoading = true;
			}
			//START DOWNLOADING ONE BY ONE
			_bundles = new List<AssetBundle>( bundles.Length );
			for( int i=0; i<bundles.Length; i++ )
			{
				yield return StartCoroutine( _DownloadAndCache( i ) );
                if( _DownloadHadAnError || _CancelingCurrentDownloads )
                {
                    ClearAllCurrentDownloads();
                    _CancelingCurrentDownloads = false;
                    yield break;
                }
			}

			_downloadedAssetPacks = true;

            if( autoInstantiateAll )
            {
                InstantiateList();
            }
			//FINISHED LOADING
			_IsLoading = false;
			_IsLoaded = true;
            Utilities.Log("Finished Downloading ABs", gameObject);
			if( autoDestroyLoadingIndicator )
			{
                _loadingIndicator.SetActiveInHierarchy( false, true, 0f, true );//HIDE/DESTROY LOADING SCREEN
			}
			onAllLoaded.Invoke ();

			//UNLOAD
            _UnloadAllBundles( true );
			_IsTryingToLoad = false;
			if( resetIsLoaded )
			{
				_IsLoaded = false;
			}
		}
        /// <summary>
        /// Instantiates all objects in list /_objsToInstantiate/
        /// </summary>
        public void InstantiateList( bool clearList = true )
        {
            for (int i = 0; i < _objsToInstantiate.Count; i++) 
            {
                Utilities.Log("Instantiating : " + _objsToInstantiate[i]);
                GameObject go = (GameObject) Instantiate( _objsToInstantiate[i] );
                go.name = _objsToInstantiate[i].name; //To fix gameobjects find (instantiated has (Clone) in name which makes it fail)
            }
            if( clearList )
            {
                _objsToInstantiate.Clear();
            }
        }


		/// <summary>
		/// Updates the progress bar (if any), and prints the progress into the console (if /logProgress/ is true).
		/// </summary>
        /// <param name="request">The current download.</param>
		/// <param name="downloadIndex">The index of the current download. The first is cero (0). </param>
        protected IEnumerator _UpdateProgress( UnityWebRequest request, int downloadIndex )
		{
            do {
                yield return null;
                if( request == null )
                    yield break;
                try{//TRY CATCH INSIDE A LOOP IS NOT GOOD FOR PERFORMANCE BUT HAVEN'T FOUND A BETTER SOLUTION
                    #if UNITY_EDITOR
                    if( logProgress )
                        Utilities.Log ( request.downloadProgress, gameObject );
                    #endif
                    if( _progressBar )
                    {
                        _progressBar.value = ( request.downloadProgress + downloadIndex ) / bundles.Length;
                    }
                }catch( System.NullReferenceException ) { yield break; }//Request disposed..
            } while( !request.isDone );            
		}
        /// <summary>
        /// Updates the progress bar (if any), and prints the progress into the console (if /logProgress/ is true).
        /// </summary>
        /// <param name="request">The current download.</param>
        /// <param name="downloadIndex">The index of the current download. The first is cero (0). </param>
        protected IEnumerator _UpdateProgress( WWW request, int downloadIndex )
        {
            do {
                yield return null;
                if( request == null )
                    yield break;
                //try{//TRY CATCH INSIDE A LOOP IS NOT GOOD FOR PERFORMANCE BUT HAVEN'T FOUND A BETTER SOLUTION
                #if UNITY_EDITOR
                if( logProgress )
                    Utilities.Log ( request.progress, gameObject );
                #endif
                if( _progressBar )
                {
                    _progressBar.value = ( request.progress + downloadIndex ) / bundles.Length;
                }
                //}catch( System.NullReferenceException ) { yield break; }//Request disposed..
            } while( !request.isDone );            
        }
		protected IEnumerator _DownloadAndCache( int i )
		{
            _DownloadHadAnError = false;
			Bundle bundle = bundles[i];

			//Dont ReDownload assetpacks, should only be downloaded once and never unloaded
			if (!bundle.loadAll && !bundle.instantiate && _downloadedAssetPacks) {
				yield break;
			}

			if ( useStreamingAssets ){
				#if UNITY_ANDROID && !UNITY_EDITOR
					bundle.url = mainUrl + bundle.url;
				#else
					bundle.url = "file://" + Application.streamingAssetsPath + mainUrl + bundle.url;
				#endif
			}
			else{
                bundle.url = mainUrl + bundle.url;
			}
			#region DOWNLOAD BUNDLE
            Utilities.Log("Trying to download AB from " + bundle.url);
			// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            if( useWwwIntead )
            {
                _www = WWW.LoadFromCacheOrDownload( bundle.url, bundle.version );
            }
            else _request = UnityWebRequest.GetAssetBundle( bundle.url, (uint)bundle.version, 0 );
            Utilities.Log ( "Loading Bundle: "+Path.GetFileName( bundle.url ) );
            if( useWwwIntead )
            {
                StartCoroutine( _UpdateProgress( _www, i ) );
                yield return _www;
                while( !_www.isDone )
                    yield return null;
            }
            else 
            {
                StartCoroutine( _UpdateProgress( _request, i ) );
                yield return _request.Send();
                while( !_request.isDone )
                    yield return null;
            }
            Utilities.Log ( "Done Loading Bundle: "+Path.GetFileName( bundle.url ) );
            if( _CancelingCurrentDownloads )
            {
                if( useWwwIntead )
                    _www.Dispose();
				else _request.Dispose();
                yield break;
            }
			#endregion DOWNLOAD BUNDLE

            if( (!useWwwIntead && !string.IsNullOrEmpty( _request.error )) || useWwwIntead && !string.IsNullOrEmpty( _www.error ) )
			{
                if( useWwwIntead )
                    Utilities.LogError( "Download had an error:" + _www.error );
                else Utilities.LogError( "Download had an error:" + _request.error );
                _DownloadHadAnError = true;
                if( onDownloadHadAnError != null )
                {
                    onDownloadHadAnError();
                }
			}
			else
			{
                AssetBundle ab = null;
                if(  useWwwIntead)
                    ab = _www.assetBundle;
                else ab = DownloadHandlerAssetBundle.GetContent( _request );

				if( bundle.loadAll || ( bundle.assets != null && bundle.assets.Length == 0 ) )//Load all
				{
					if( !bundle.loadAll && bundle.assets.Length == 0 )
					{
                        Utilities.LogWarning("The ["+i+"] bundle's assets array is empty, loading all assets...");
					}
					if( !string.IsNullOrEmpty( bundle.sceneName ) && bundle.autoLoadAsync )//IS IT A SCENE?
					{
						AutoFade.LoadLevelAsync( bundle.sceneName, bundle.transition );
					}
					else
					{
						if( bundle.instantiate )
						{
							_AddToInstantiationList( ab );
						}
						if( bundle.loadToSfxManager )
						{
							bundle.LoadToSfxManager( ab );
						}
					}
				}
				else if( bundle.assets != null && bundle.assets.Length > 0 )//Load one by one
				{
					List<UnityEngine.Object> objs = new List<UnityEngine.Object>( bundle.assets.Length );
					for( int j=0; j<bundle.assets.Length; j++ )
					{
						if ( string.IsNullOrEmpty( bundle.assets[j] ) )
						{
                            Utilities.LogWarning( j+" - Bundle's asset name at index ["+j+"] is empty.." );
						}
						else
						{
							if( bundle.instantiate )
							{
								_AddToInstantiationList( ab.LoadAsset( bundle.assets[j] ), ab );
							}
							if( bundle.loadToSfxManager )
							{
								objs.Add( ab.LoadAsset( bundle.assets[j] ) );
							}
						}
					}
					if( bundle.loadToSfxManager )
					{
						bundle.LoadToSfxManager( objs );
					}
				}

				bundle.InvokeOnBundleLoaded();

				_bundles.Add( ab );
			}
		}
		protected void _UnloadAllBundles (bool forceUnload = false)   
		{
            if( _bundles == null )
                return;

            Bundle bundle = null;//caching
            int bundlesCount = _bundles.Count;
			for( int i=0; i<bundles.Length; i++ )
			{
				bundle = bundles[i];

				//Dont unload asset packs
				if (!bundle.loadAll && !bundle.instantiate) {
					continue;
				}

				// Unload the AssetBundles compressed contents to conserve memory
				if( bundle.instantiate || !forceUnload )                           
				{
                    continue;
				}
                if( !string.IsNullOrEmpty( bundle.sceneName ) && bundle.autoLoadAsync )//IS IT A SCENE?
                {
                    AutoFade._ShowAsyncLoadedLevel[ bundle.sceneName ] = true;
                }
                else if( bundle.unload || forceUnload )
                { 
                    if ( i < bundlesCount && _bundles [i] != null) 
                    {
                        _bundles [i].Unload (false);// memory is freed from the web stream (www.Dispose() gets called implicitly)
                    }
                } 
			}
			_bundles.Clear();
			_bundles = null;
		}
        protected void _CheckOverwrites()
		{
			if( !string.IsNullOrEmpty( _MainUrlOverrider ) )
			{
				mainUrl = _MainUrlOverrider;
			}
			if( _BundlesOverwriter != null )
			{
				bundles = _BundlesOverwriter;
			}
			if( _OnAllLoadedOverrider != null )
			{
				onAllLoaded = _OnAllLoadedOverrider;
			}
		}
		/// <summary>
		/// Instantiate the specified obj after all bundles are loaded (totalLoaded = 1).
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="ab">The Asset Bundle to unload after instantiating.</param>
        protected void _AddToInstantiationList( UnityEngine.Object obj, AssetBundle ab )
		{
			if( obj is GameObject )
			{
				//CHECK CONTROLLER
				bool instantiate = true;
                var go = (GameObject)obj;
				if( go.GetComponent<ABController>() )
				{
					var controller = go.GetComponent<ABController>();
					instantiate = !controller.m_doNotInstantiateFromLoader;
				}
				if( instantiate )
				{
					_objsToInstantiate.Add(obj);
				}			
			}
			if( ab )
			{
				ab.Unload( false );//If set to true, the loaded objects are destroyed, which is not desired.
			}

		}
		/// <summary>
        /// Adds each object to the /_objsToInstantiate/ list.
		/// </summary>
		/// <param name="objs">Objects.</param>
		/// <param name="ab">The Asset Bundle to unload after instantiating.</param>
		protected void _AddToInstantiationList( AssetBundle ab )
		{
            UnityEngine.Object[] objs = ab.LoadAllAssets();
			for( int i=0; i<objs.Length; i++ )
			{
				_AddToInstantiationList( objs[i], ab );
			}
		}      
	}
}