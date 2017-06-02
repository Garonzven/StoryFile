//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Extensions;
using System.Linq;
using DDK.Base.SoundFX;
using UnityEngine.SceneManagement;


namespace DDK.Base.Managers 
{
	/// <summary>
	/// Creates multiple objects (children) each one with a specified name representing an audio source that should 
	/// control the clips that correspond to the mentioned name. Example: All character voices could be played by the 
	/// Voices game object's audio source while the Ambient game object's audio source is playing the ambient sound. 
    /// This allows reducing the amount of audio sources in the scene by reusing the ones specified here, also in a 
    /// logic and centrilized manner, since they are all children of the gameObject holding this script/component. In the 
    /// other hand, this class provides lots of functions to handle audio clips in the most common ways.
	/// </summary>
	[ExecuteInEditMode]
	public class SfxManager : MonoBehaviour 
    {
		[Tooltip("If true, the SfxManager won't be destroyed when loading another scene")]
		public bool dontDestroyOnLoad;
		[Space(5f)]
		[Tooltip("If PlayQueue( source, clip ) is called, the specified clips will be added to that source's play queue which will be" +
			" checked every /queueRate/ seconds")]
		public float queueRate = 0.5f;
		public string[] sources = new string[] {
			"Voices",
			"Ambient",
			"Music",
			"Effects",
			"Surroundings"
		};
        [Tooltip("The sources in this array will be paused, the rest ignored")]
        public string[] sourcesToPause = new string[] {
            "Voices"
        };
		[Tooltip("Path for all the clips inside Resources folder")]
		public string allClipsPath;
		[Tooltip("True if part of the Always Active Prefab")]
		public bool alwaysActive = false;

		/// <summary>
		/// Holds the game objects that represent each source
		/// </summary>
		private Dictionary<string, GameObject> _holders = new Dictionary<string, GameObject>();
		/// <summary>
		/// Contains the audio sources that are attached to each holder
		/// </summary>
		private Dictionary<string, AudioSource> _srcs = new Dictionary<string, AudioSource>();
		/// <summary>
		/// Use _currentSource for get and set.
		/// </summary>
		private string _currentSrc;
		/// <summary>
		/// The name of the source that will be played when calling PlayCurrentSource( clip ) 
		/// </summary>
		private string _CurrentSource 
        {
			get
            {
				if( string.IsNullOrEmpty( _currentSrc ) )
				{
					if( sources.Length > 0 )
						_currentSrc = sources[0];
					else _currentSrc = "Effects";
				}
				return _currentSrc;
			}
			set{ _currentSrc = value; }
		}
		/// <summary>
		/// The clips that where loaded from Resources. Each key represents a path.
		/// </summary>
		private Dictionary<string, AudioClip> _clips;
		private Dictionary<string, Queue<AudioClip>> _queue;
		private Dictionary<string, Queue<Sfx>> _queuex;



		#region STATIC VARS/PROPS
        private static SfxManager _instance;
		/// <summary>
		/// If there is no instance, one will be created.
		/// </summary>
		public static SfxManager Instance 
        {
			get 
            {
				if( !_instance )
				{
					//CHECK IF THE OBJECT DOESN'T EXIST
					var instance = FindObjectOfType<SfxManager>();
					if( !instance )
					{
						Debug.Log ("No instance of SfxManager was found, creating one...");
						instance = new GameObject( "SfxManager", typeof(SfxManager) ).GetComponent<SfxManager>();
						_instance = instance.GetComponent<SfxManager>();
					}
					else _instance = instance.GetComponent<SfxManager>();
				}
				return _instance;
			}
		}
		public static Dictionary<string, GameObject> srcHolders 
        {
			get{ return Instance._holders; }
		}
		/// <summary>
		/// The clips that were loaded from Resources. Each key represents a path.
		/// </summary>
		public static Dictionary<string, AudioClip> _Clips 
        {
			get
            {
				if( Instance._clips == null )
				{
					_instance._clips = new Dictionary<string, AudioClip>();
				}
				return Instance._clips;
			}
			set
            {
				if( value != null )
				{
					if( value.Count > 0 )
					{
						Instance._clips = value;
					}
				}
			}
		}
		/// <summary>
		/// This is used to queue multiple audio clips and play them in order (FIFO). Each key represents a source. If 
		/// you try to GET this, the queue will be created (if null).
		/// </summary>
		/// <value>The queue.</value>
		private static Dictionary<string, Queue<AudioClip>> QUEUE
        {
			get
            {
				Instance._CheckCreateDic( ref Instance._queue );
				return Instance._queue;
			}
		}
		/// <summary>
		/// This is used to queue multiple Sound Effects and play them in order (FIFO). Each key represents a source. If 
		/// you try to GET this, the queue will be created (if null).
		/// </summary>
		/// <value>The queue.</value>
		private static Dictionary<string, Queue<Sfx>> QUEUEX
        {
			get
            {
				Instance._CheckCreateDic( ref Instance._queuex );
				return Instance._queuex;
			}
		}
		#endregion STATIC VARS/PROPS



		#region MAIN
		void Awake()
		{
			_instance = this;
			
			if( Application.isPlaying )
			{
				if( dontDestroyOnLoad )
				{
					DontDestroyOnLoad(gameObject);
				}
				
				LoadClipsFrom( allClipsPath );
			}
			
			_ValidateSources();
			SceneManager.sceneUnloaded += OnSceneFinishedUnload;
		}

		void OnSceneFinishedUnload(Scene scene)
		{
			StopAll ();
		}

		// Use this for initialization
		void Start () 
        {
			InvokeRepeating( "UpdateQueue", 0f, queueRate );
            //Ignore sources from pause
            for( int i=0; i<sources.Length; i++ )
            {
                if( sourcesToPause.Contains( sources[i] ) )
                    continue;
                GetSource( sources[i] ).ignoreListenerPause = true;
            }
		}
		void UpdateQueue()
		{
			_CheckQueue( _queue );
			_CheckQueue( _queuex );
		}		
		// Update is called once per frame
		void Update () 
        {			
			_Edit ();
		}		
		void OnGUI () 
        {			
			_Edit ();
		}

		private void _Edit()
		{
			#if UNITY_EDITOR
			if( !Application.isPlaying )
			{
				name = "SfxManager";

				if( sources.Length == 0 )
				{
					Debug.Log( "The sources array is empty.." );
					return;
				}
				_ValidateSources();
			}
			#endif
		}
		#endregion MAIN




		#region PRIVATE FUNCTIONS
        /// <summary>
        /// Ensures the audio sources are properly updated if a source gets removed or a new one added.
        /// </summary>
		private void _ValidateSources()
		{
			var children = gameObject.GetChildren();
			_RemoveInvalidChildren( children );
			children = gameObject.GetChildren();
			
			for( int i=0; i<sources.Length; i++ )
			{
				if( !_holders.ContainsKey( sources[i] ))
				{
					if( children.Length > _srcs.Count )
					{
						for( int j=0; j<children.Length; j++ )
						{
							_holders[ children[j].name ] = children[j];
							_holders[ children[j].name ].transform.SetSiblingIndex( j );
							_srcs[ children[j].name ] = children[j].GetComponent<AudioSource>();
						}
					}
					else//CREATE NEW SOURCE
					{
						_CreateSource( sources[i], i );
					}
				}
			}
		}
        /// <summary>
        /// Creates an audio source source.
        /// </summary>
        /// <param name="holderName">Name of the audio source's holder gameObject.</param>
        /// <param name="siblingIndex">Sibling index inside the Sources gameObject.</param>
        private void _CreateSource( string holderName, int siblingIndex = 0 )
		{
			GameObject holder = new GameObject( holderName );
			holder.transform.SetParent( transform );
			holder.transform.SetSiblingIndex( siblingIndex );
			_holders[ holderName ] = holder;
			
            AudioSource src = _holders[ holderName ].GetAddAudioSource();
			_srcs[ holderName ] = src;

			if( holderName.Equals("Ambient") || holderName.Equals("Music") )
			{
				src.loop = true;
			}
		}		
		/// <summary>
		/// Remove children that were removed from sources.
		/// </summary>
		private void _RemoveInvalidChildren( IList<GameObject> children )
		{
			for( int i=0; i<children.Count; i++ )
			{
				bool remove = false;
				if( !sources.Contains( children[i].name ) || i >= sources.Length )
				{
					remove = true;
				}
				if( remove )
				{
					_holders.Remove( children[i].name );
					_srcs.Remove( children[i].name );
					DestroyImmediate( children[i] );
				}
			}
		}
		private void _CheckCreateDic<T>( ref Dictionary<string, Queue<T>> dic )
		{
			if( dic == null )
			{
				dic = new Dictionary<string, Queue<T>>();
				//ADD SOURCES
				for( int i=0; i<sources.Length; i++ )
				{
					dic.Add( sources[i], new Queue<T>() );
				}
			}
		}
		#endregion FUNCTIONS




		#region STATIC FUNCTIONS
		/// <summary>
		/// Loads the clips from the specified path. If a clip with the specified path already exists, it won't be added.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void LoadClipsFrom( string path )
		{
			if( !string.IsNullOrEmpty( path ) )
			{
				Instance.allClipsPath = path;
				_Clips.AddRange( path.CheckSeparator(), Resources.LoadAll<AudioClip>( path ) );
			}
			//else Debug.LogWarning ("The specified path is null or empty..");
		}
		/// <summary>
		/// Removes all clips that were loaded to _Clips.
		/// </summary>
		public static void RemoveAllClips()
		{
			_Clips.Clear();
		}
		/// <summary>
		/// Removes the clips that match the specified path.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void RemoveClips( string path )
		{
			_Clips.Remove<AudioClip>( path );
		}
		/// <summary>
		/// Stops the specified source
		/// </summary>
		public static void Stop( string source )
		{
			GetSource( source ).Stop();
		}	
        /// <summary>
        /// Stops all sources
        /// </summary>
        public static void StopAll()
        {
            for( int i=0; i<Instance.sources.Length; i++ )
            {
                GetSource( Instance.sources[i] ).Stop();
            }
        }   
		/// <summary>
		/// Returns true if the specified Audio Source is playing, if the SfxManager doesn't contain it, then it is created.
		/// </summary>
		public static bool IsPlaying( string source )
		{
			return GetSource( source ).isPlaying;
		}	
		/// <summary>
		/// Returns true if the specified Audio Source is playing, if the SfxManager doesn't contain it, then it is created.
		/// </summary>
		public static bool IsPlaying( Sfx sfx )
		{
			if( sfx == null )
				return false;
			return GetSource( sfx.source ).isPlaying;
		}
		/// <summary>
		/// Returns true if any of the specified Audio Sources is playing, if the SfxManager doesn't contain them, then they are created.
		/// </summary>
		public static bool IsAnyPlaying( IList<Sfx> sfx )
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				if( IsPlaying( sfx[i] ) )
					return true;
			}
			return false;
		}
        /// <summary>
        /// Returns true if any of the SfxManager's Audio Sources is playing.
        /// </summary>
        public static bool IsAnySourcePlaying()
        {
            for( int i=0; i<Instance.sources.Length; i++ )
            {
                if( IsPlaying( Instance.sources[i] ) )
                    return true;
            }
            return false;
        }
		/// <summary>
		/// Returns true if the specified Audio Source exists.
		/// </summary>
		public static bool HasSource( string name )
		{
			return Instance._srcs.ContainsKey( name );
		}
		public static void Enable( string source, bool enable = true )
		{
			GetSource( source ).enabled = enable;
		}
		public static void FadeOut( string source, float duration )
		{
			AnimateVolume( source, 0f, duration );
		}
		public static void FadeIn( string source, float duration )
		{
			AnimateVolume( source, 1f, duration );
		}
		public static void AnimateVolume( string source, float targetVol, float duration )
		{
			AudioSource src = GetSource( source );
			if( !src )
				return;
			src.Fade( targetVol, duration );
		}
		#endregion STATIC FUNCTIONS
		#region STATIC FUNCTIONS: GET
		/// <summary>
		/// Returns the Audio Source that's playing the specified clip.
		/// </summary>
		public static AudioSource GetPlayingSource( string clipName )
		{
			AudioSource[] sources = new List<AudioSource>( Instance._srcs.Values ).ToArray();
			for( int i=0; i<sources.Length; i++ )
			{
				if( sources[i] && sources[i].clip && sources[i].clip.name == clipName )
				{
					return sources[i];
				}
			}
			return null;
		}
		/// <summary>
		/// Returns the current Audio Source, as specified with SetCurrentSource().
		/// </summary>
		public static AudioSource GetCurrentSource()
		{
			return GetSource( Instance._CurrentSource );
		}
		/// <summary>
		/// Returns the specified Audio Source, if the SfxManager doesn't contain it, then it is created. If empty/null, the Effects Audio Source will be returned.
		/// </summary>
		public static AudioSource GetSource( string name )
		{
			#region VALIDATE
			if( string.IsNullOrEmpty( name ) )
			{
				if( !Instance._srcs.ContainsKey( "Effects" ) )
				{
					Instance._CreateSource( "Effects" );
				}
				return Instance._srcs["Effects"];
			}
			if( !Instance._srcs.ContainsKey( name ) )
			{
				Instance._CreateSource( name );
				Debug.Log( "The specified source /"+ name +"/ didn't exist, it has being created.." );
			}
			#endregion
			return Instance._srcs[name];
		}
		/// <summary>
		/// Gets the specified clip from the _Clips array. This can return null if the array is empty or null.
		/// </summary>
        /// <param name="name">The clip's name.</param>
		/// <param name="context">The Context object for warning messages.</param>
		public static AudioClip GetClip( string name, Object context = null )
		{
			if( _AreClipsValid( context ) )
			{
				return _Clips.Get( Instance.allClipsPath +"/"+ name );
			}
			return null;
		}
		/// <summary>
        /// Returns all audio clips with the specified names. This can return an empty list if the _Clips array is empty or null.
        /// </summary>
        /// <returns>The clips.</returns>
        /// <param name="names">Names.</param>
		public static List<AudioClip> GetClips( IList<string> names )
		{
			return _Clips.Get( names.AddGetPrefix( Instance.allClipsPath +"/" ) );
		}
		
		/// <summary>
        /// Returns the clip's length or 0 if clip is not found. This will return 0 if the _Clips array is empty or null.
		/// </summary>
        /// <param name="name">The clip's name.</param>
		/// <param name="context">The Context object for warning messages.</param>
		public static float GetClipLength( string name, Object context = null )
		{
			var clip = GetClip( name, context );
			if( clip )
			{
				return clip.length;
			}
			else return 0f;
		}
		/// <summary>
        /// Returns the specified source's clip's length or 0 if no clip is found (no valid source or source has no clip).
		/// </summary>
		public static float GetClipLengthInSource( string sourceName )
		{
			var clip = GetSource( sourceName ).clip;
			if( clip )
			{
				return clip.length;
			}
			else return 0f;
		}
		#endregion STATIC FUNCTIONS: GET
		#region STATIC FUNCTIONS: SET
		/// <summary>
		/// Set the source that will be used when calling PlayCurrentSource( clip ). THIS CAN BE USED FOR BUTTON EVENTS.
		/// </summary>
		public static void SetCurrentSource( string sourceName )
		{
			SetCurrentSource( sourceName, Sfx.m_context );
		}
		/// <summary>
		/// Set the source that will be used when calling PlayCurrentSource( clip ).
		/// </summary>
        /// <param name="source">The source's name.</param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void SetCurrentSource( string source, Object context )
		{
			if( string.IsNullOrEmpty( source ) )
				source = "Effects";
			if( !Instance._srcs.ContainsKey( source ) )
			{
				Instance._CreateSource( source );
                Debug.LogWarning( string.Format( "The specified source \"{0}\" didn't exist, it has being created..", source ), context );
			}
			if( _AreClipsValid( context ) )
			{
				Instance._CurrentSource = source;
			}
		}
		/// <summary>
		/// Sets the time for the specified Audio Source, if the SfxManager doesn't contain it, then it is created.
		/// </summary>
		public static void SetTime( string source, float time )
		{
			GetSource( source ).time = time;
		}
		/// <summary>
		/// Sets the specified clip in the specified source
		/// </summary>
        /// <param name="source">The source's name.</param>
        /// <param name="clip">The clip's name.</param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void SetClip( string source, string clip, Object context = null )
		{
			AudioSource src = GetSource( source );
			src.clip = GetClip( clip, context );
		}
		/// <summary>
		/// Sets the specified clip in the specified source
		/// </summary>
		public static void SetClip( string source, AudioClip clip )
		{
			AudioSource src = GetSource( source );
			src.clip = clip;
		}
		/// <summary>
		/// Sets the specified sfx in the specified source
		/// </summary>
		public static void SetSfx( string source, Sfx sfx )
		{
			AudioSource src = GetSource( source );
			src.clip = sfx.clip;
			if( Sfx.m_applyVolumes )
			{
				src.volume = sfx.volume;
			}
			if( Sfx.m_applyPitches )
			{
				src.pitch = sfx.pitch;
			}
			src.playOnAwake = sfx.playOnAwake;
		}

		public static void SetSourceVolume( string source, float volume )
		{
			AudioSource src = GetSource( source );
			src.volume = volume;
		}
		#endregion STATIC FUNCTIONS: SET

        #region PUBLIC INSTANCE FUNCTIONS
        /// <summary>
        /// Plays the specified clip on the current source as specified in SetCurrentSource( source ). THIS CAN BE USED 
        /// FOR BUTTON EVENTS.
        /// </summary>
        public void PlayCurrentSource( string clip )
        {
            PlayCurrentSource( clip, null );
        }
        /// <summary>
        /// Plays the specified clip on the current source as specified in SetCurrentSource( source ).
        /// </summary>
        /// <param name="clip">The clip's name.</param>
        /// <param name="context">The Context object for warning messages.</param>
        public void PlayCurrentSource( string clip, Object context )
        {
            if( _IsClipValid( clip, context ) )
            {
                GetSource( Instance._CurrentSource ).Play( GetClip( clip ) );
            }
        }
        /// <summary>
        /// Plays the specified clip on the current source as specified in SetCurrentSource( source ). THIS CAN BE USED 
        /// FOR BUTTON EVENTS.
        /// </summary>
        public void PlayCurrentSource( AudioClip clip )
        {
            if( clip != null )
            {
                GetSource( Instance._CurrentSource ).Play( clip );
            }
        }
        /// <summary>
        /// Plays the current source after the specified delay. THIS CAN BE USED FOR BUTTON EVENTS.
        /// </summary>
        public void PlayCurrentSource( float delay )
        {
            AudioSource src = GetSource( Instance._CurrentSource );
            if( !src )
                return;
            src.PlaySoundAfter( delay ).Run();
        }
        #endregion
		
		#region STATIC FUNCTIONS: PLAY
		/// <summary>
		/// Plays the specified source.
		/// </summary>
        /// <param name="source">The source's name.</param>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		public static void Play( string source, bool interrupt = true )
		{
			var src = GetSource( source );
			if( src.isPlaying && !interrupt )
				return;
			if( interrupt )
			{
				_ClearQueue( ref Instance._queue, source );
			}
			SetCurrentSource( source );
			src.Play();
		}
		/// <summary>
		/// Plays the specified Sound Effect on the specified source.
		/// </summary>
        /// <param name="sfx">The sound effect.</param>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlaySfx( Sfx sfx, bool interrupt = true, Object context = null )
		{
			Instance.StartCoroutine( _Play( sfx, interrupt, context ) );
		}
		/// <summary>
        /// Plays the specified clip on the specified source. If source is null/empty, then 
        /// the Effects audio source will be used; if it hasn't been created, it will be.
		/// </summary>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayClip( string source, AudioClip clip, bool interrupt = true, Object context = null )
		{
			if( clip == null )
			{
				Debug.LogWarning( "The specified clip is null", context );
				return;
			}
			if( interrupt )
			{
				_ClearQueue( ref Instance._queue, source, context );
			}
			SetCurrentSource( source, context );
			GetSource( source ).Play( clip, interrupt );
		}
		/// <summary>
		/// Plays the specified clip on the specified source.
		/// </summary>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayClip( string source, string clip, bool interrupt = true, Object context = null )
		{
			PlayClip( source, GetClip( clip, context ), interrupt, context );
		}
        /// <summary>
        /// Plays the specified clip on the specified source after the specified delay. If source is null/empty, then 
        /// the Effects audio source will be used; if it hasn't been created, it will be.
        /// </summary>
        /// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
        /// will also clear any queued sfxs </param>
        /// <param name="context">The Context object for warning messages.</param>
        public static void PlayClip( string source, AudioClip clip, float delay, bool interrupt = true, Object context = null )
        {
            Instance.StartCoroutine( _Play( source, clip, delay, interrupt, context ) );
        }
		/// <summary>
		/// Plays the specified clip on the specified source after the specified delay.
		/// </summary>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayClip( string source, string clip, float delay, bool interrupt = true, Object context = null )
		{
			Instance.StartCoroutine( _Play( source, clip, delay, interrupt, context ) );
		}
		/// <summary>
		/// Plays a random clip on the specified source.
		/// </summary>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayRandomClip( string source, IList<AudioClip> clips, bool interrupt = true, Object context = null )
		{
			var clip = clips.GetRandom();
			if( clip == null )
			{
				Debug.LogWarning( "The specified clip is null", context );
				return;
			}
			if( interrupt )
			{
				_ClearQueue( ref Instance._queue, source, context );
			}
			SetCurrentSource( source, context );
			GetSource( source ).Play( clip, interrupt );
		}
		/// <summary>
		/// Plays a random clip on the specified source.
		/// </summary>
        /// <param name="clips"> A list of the clips names from which to choose the random one to be played </param>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayRandomClip( string source, IList<string> clips, bool interrupt = true, Object context = null )
		{
			var clip = clips.GetRandom();
			PlayClip( source, GetClip( clip, context ), interrupt, context );
		}
		/// <summary>
		/// Plays a random clip on the specified source after the specified delay.
		/// </summary>
        /// <param name="clips"> A list of the clips from which to choose the random one to be played </param>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		public static void PlayRandomClip( string source, IList<AudioClip> clips, float delay, bool interrupt = true )
		{
			var clip = clips.GetRandom();
			Instance.StartCoroutine( _Play( source, clip, delay, interrupt ) );
		}
		/// <summary>
		/// Plays a random clip on the specified source after the specified delay.
		/// </summary>
        /// <param name="clips"> A list of the clips names from which to choose the random one to be played </param>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued clips </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayRandomClip( string source, IList<string> clips, float delay, bool interrupt = true, Object context = null )
		{
			var clip = clips.GetRandom();
			Instance.StartCoroutine( _Play( source, clip, delay, interrupt, context ) );
		}
        /// <summary>
        /// Plays a random clip on the specified source.
        /// </summary>
        /// <param name="sfx"> A list of the sound effects from which to choose the random one to be played </param>
        /// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
        /// will also clear any queued sfxs </param>
        /// <param name="context">The Context object for warning messages.</param>
        /// <returns> The randomly selected Sfx </returns>
        public static Sfx PlayRandomSfx( string source, IList<Sfx> sfx, bool interrupt = true, Object context = null )
        {
            var soundFx = sfx.GetRandom();
            if( !string.IsNullOrEmpty( source ) )
                soundFx.source = source;
            PlaySfx( soundFx, interrupt, context );
            return soundFx;
        }
		/// <summary>
		/// Adds the specified Sound Effect to the specified source's play queue if a sfx is already playing in the source, if not,
		/// it is played immediately.
		/// </summary>
        /// <param name="sfx"> A list of the sound effects that will be played one after the other in order. </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayQueue( IList<Sfx> sfx, Object context = null )
		{
			_PlayQueue( sfx, context );
		}
		/// <summary>
		/// Adds the specified clip to the specified source's play queue if a clip is already playing in the source, if not,
		/// it is played immediately.
		/// </summary>
        /// <param name="clips"> A list of the clips that will be played one after the other, in order. </param>
		/// <param name="interrupt">If true, before playing the queued sfxs, their specified source will be stopped, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayQueue( string source, IList<AudioClip> clips, bool interrupt = false, Object context = null )
		{
			if( interrupt )
			{
				Stop ( source );
			}
			for( int i=0; i<clips.Count; i++ )
			{
				PlayQueue( source, clips[i], context );
			}
		}
		/// <summary>
		/// Adds the specified clip to the specified source's play queue if a clip is already playing in the source, if not,
		/// it is played immediately.
		/// </summary>
        /// <param name="clips"> A list of the clips that will be played one after the other, in order. </param>
		/// <param name="interrupt">If true, before playing the queued sfxs, their specified source will be stopped, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayQueue( string source, IList<string> clips, bool interrupt = false, Object context = null )
		{
			PlayQueue( source, GetClips( clips ), interrupt, context );
		}
		/// <summary>
		/// Adds the specified Sound Effect to the specified source's play queue if a sfx is already playing in the source, if not,
		/// it is played immediately.
		/// </summary>
        /// <param name="sfx"> The sound effect that will be played or added to the play queue if a sfx is already playing. </param>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayQueue( Sfx sfx, Object context = null )
		{
			if( sfx == null )
				return;
			//CHECK IF SOURCE IS PLAYING
			if( IsPlaying( sfx.source ) )//YES: Add to Queue
			{
				QUEUEX[ sfx.source ].Enqueue( sfx );
			}
			else //NO: Play
			{
				PlaySfx( sfx, true, context );
			}
		}
		/// <summary>
		/// Adds the specified clip to the specified source's play queue if a clip is already playing in the source, if not,
		/// it is played immediately.
		/// </summary>
		/// <param name="context">The Context object for warning messages.</param>
		public static void PlayQueue( string source, AudioClip clip, Object context = null )
		{
			if( !clip )
				return;
			//CHECK IF SOURCE IS PLAYING
			if( GetSource( source ).isPlaying )//YES: Add to Queue
			{
				QUEUE[ source ].Enqueue( clip );
			}
			else //NO: Play
			{
				PlayClip( source, clip, true, context );
			}
		}
		/// <summary>
		/// Adds the specified clip to the specified source's play queue if a clip is already playing in the source, if not,
		/// it is played immediately.
		/// </summary>
		public static void PlayQueue( string source, string clip, Object context = null )
		{
			PlayQueue( source, GetClip( clip ), context );
		}
		#endregion STATIC FUNCTIONS: PLAY



		//PRIVATE
		#region STATIC PRIVATE FUNCTIONS: PLAY
		/// <summary>
		/// Plays the specified sfx.
		/// </summary>
        /// <param name="sfx"> The sfx to play </param>
		/// <param name="interrupt"> If true, the source can be interrupted if playing; otherwise, it won't play, this
		/// will also clear any queued sfxs </param>
		/// <param name="context">The Context object for warning messages.</param>
		private static IEnumerator _Play( Sfx sfx, bool interrupt = true, Object context = null )
		{
            if( sfx.delay > 0f )
            {
                yield return new WaitForSeconds( sfx.delay );
            }
            if( sfx.clip != null ) 
			{
				var src = GetSource( sfx.source );
				if ( Sfx.m_applyPitches )
				{
					src.pitch = sfx.pitch;
				}
				if( Sfx.m_applyVolumes )
				{
					src.volume = sfx.volume;
				}
                src.playOnAwake = sfx.playOnAwake;
                // play sfx
				PlayClip( src.name, sfx.clip, interrupt, context );
			}
			// Execute events
			sfx.OnClipPlay();
            if( sfx.nextDelay > 0f )
            {
                yield return new WaitForSeconds( sfx.nextDelay );
            }
		}
		/// <summary>
        /// Plays the specified clip in the specified source after the specified delay. If source is null/empty, then 
        /// the Effects audio source will be used; if it hasn't been created, it will be.
		/// </summary>
        /// <param name="interrupt">If true, the specified source will be interrupted to start playing the specified clip.</param>
		/// <param name="context">The Context object for warning messages.</param>
		private static IEnumerator _Play( string source, AudioClip clip, float delay, bool interrupt = true, Object context = null )
		{
			yield return new WaitForSeconds( delay );
			PlayClip( source, clip, interrupt, context );
		}
		/// <summary>
		/// Plays the specified clip in the specified source. If clip is null/empty, then the specified source's clip 
        /// will be used unless it is also null. If source is null/empty, then the Effects audio source will be used; 
        /// if it hasn't been created, it will be.
		/// </summary>
        /// <param name="interrupt">If true, the specified source will be interrupted to start playing the specified clip.</param>
		/// <param name="context">The Context object for warning messages.</param>
		private static IEnumerator _Play( string source, string clip, float delay, bool interrupt = true, Object context = null )
		{
            AudioClip audioClip = GetClip( clip );
            if( !audioClip )
                audioClip = GetSource( source ).clip;
            yield return Instance.StartCoroutine( _Play( source, audioClip, delay, interrupt, context ) );
		}
        /// <summary>
        /// Adds the specified sfx to the play queue, to play them one after the other, in order.
        /// </summary>
        /// <param name="sfx">A list of sound effects that will be added to the play queue.</param>
		/// <param name="context">The Context object for warning messages.</param>
        private static void _PlayQueue( IList<Sfx> sfx, Object context = null )
		{
			for( int i=0; i<sfx.Count; i++ )
			{
				PlayQueue( sfx[i], context );
			}
		}
		#endregion STATIC PRIVATE FUNCTIONS: PLAY
		#region STATIC PRIVATE FUNCTIONS: VALIDATIONS
		/// <summary>
		/// True if the clips are valid.
		/// </summary>
		/// <param name="context">The Context object for warning messages.</param>
		private static bool _AreClipsValid ( Object context = null )
		{
			if( Instance._clips == null )
			{
				return false;
			}
			if( Instance._clips.Count == 0 )
			{
				Debug.LogWarning( "No clips have been loaded", context );
				return false;
			}
			return true;
		}
		/// <summary>
		/// Checks if the clip is inside the loaded clips dictionary.
		/// </summary>
		/// <param name="context">The Context object for warning messages.</param>
		private static bool _IsClipValid( string clip, Object context = null )
		{
			bool valid = _AreClipsValid( context );
			if( !_Clips.ContainsKey( clip ) )
			{
				Debug.LogWarning( "The specified clip is not present in the clips dictionary. Make sure the clip's name is correct, maybe you're missing the /mainPath/ prefix", context );
				valid = false;
			}
			return valid;
		}
		#endregion STATIC PRIVATE FUNCTIONS: VALIDATIONS
		#region STATIC PRIVATE FUNCTIONS
		/// <summary>
		/// This is called every /queueRate/ frames to check if there is any clip in a source's queue, if so, the source is
		/// checked to see if the next clip in the queue needs to be played.
		/// </summary>
        /// <param name="queue">A dictionary of audio sources (names) with their respective queues of the specified type.</param>
		/// <param name="context">The Context object for warning messages.</param>
        private static void _CheckQueue<T>( Dictionary<string, Queue<T>> queue, Object context = null ) where T : class
		{
			if( queue == null )
				return;
			for( int i=0; i<queue.Count; i++ )//EACH ELEMENT IN THE QUEUE (Dictionary) REPRESENTS A SOURCE
			{
				var src = queue.ElementAt(i);//Queue Source Dic
				AudioSource source = GetSource( src.Key );//Queue's Audio Source
				
				if( source.isPlaying )//IF PLAYING, DON'T PLAY NEXT CLIP
				{
					continue;
				}
				if( src.Value.Count == 0 )
					continue;
				//DEQUEUE AND PLAY
				var nextClip = src.Value.Dequeue();
                if( nextClip is AudioClip )
				{
					PlayClip ( source.name, nextClip as AudioClip, context );
				}
				else {
					PlaySfx ( nextClip as Sfx, true, context );
				}
			}
		}
		private static void _ClearQueue<T>( ref Dictionary<string, Queue<T>> dic, string source, Object context = null )
		{
			if( dic != null )
			{
				Debug.Log ( "Interrupting audio source \"" + GetSource( source ).name + 
				           "\" and Clearing the current Sfxs Queue", context );
				//CLEAR SOURCE'S QUEUE
				dic[source].Clear();
			}
		}
		#endregion STATIC PRIVATE FUNCTIONS

	}

}
