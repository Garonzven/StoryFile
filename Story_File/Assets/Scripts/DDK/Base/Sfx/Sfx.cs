//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.Managers;
using UnityEngine.Events;


namespace DDK.Base.SoundFX 
{
    /// <summary>
    /// The purpose of this class is to work with the SfxManager for better AudioSource and AudioClip handling. Instead 
    /// of referencing an AudioSource directly from a script, use this class and setup its parameters.
    /// </summary>
	[System.Serializable]
	public class Sfx 
    {
		[System.Serializable]
		public class ActiveStates 
        {
			public UnityEvent events = new UnityEvent();
			[Tooltip("This will be activated when clip is played")]
			public List<GameObject> activate = new List<GameObject>();
			[Tooltip("This will be deactivated when clip is played")]
			public List<GameObject> deactivate = new List<GameObject>();
			[Tooltip("This will be enabled when clip is played")]
			public List<Behaviour> enable = new List<Behaviour>();
			[Tooltip("This will be disabled when clip is played")]
			public List<Behaviour> disable = new List<Behaviour>();
		}



		[Tooltip("The clip's name inside the SfxManager.")]
		[ShowIfAttribute( "_IsClipRefNull" )]
		public string clipName = "";
		/// <summary>
		/// It's better to call /clip/ when getting the value.
		/// </summary>
		[SerializeField]
		protected AudioClip _clip;
		/// <summary>
		/// This might be empty/null. Use /source/ or /Source/ for get.
		/// </summary>
		[Tooltip("The SfxManager's source name that must be used to play this Sfx. If empty, Effects will be used")]
		public string _source;
		[Space(5f)]
		[Range( 0f, 1f )]
		public float volume = 1f;
		[Range( -3f, 3f )]
		public float pitch = 1f;
		public bool playOnAwake;
		public float delay;

		[Space(10f)]
        [DisplayNameAttribute("On Play", 0, 16f)]
		public ActiveStates onClipPlay = new ActiveStates();
		[Tooltip("This can be used to delay something after clip ends playing")]
		public float nextDelay;


		protected bool _IsClipRefNull()
		{
			if( _clip && !clipName.Equals( _clip.name ) )
			{
				clipName = _clip.name;
			}
			return _clip == null;
		}



		/// <summary>
		/// If true, the pitches will be applied to the source when calling Play()
		/// </summary>
		public static bool m_applyPitches = true;
		/// <summary>
		/// If true, the volumes will be applied to the source when calling Play()
		/// </summary>
		public static bool m_applyVolumes = true;
		/// <summary>
		/// This is used by the SfxManager's warning messages for debugging purposed.
		/// </summary>
		public static Object m_context;

		/// <summary>
		/// If no clip was set, the /clipName/ will be searched in the Sfx Manager.
		/// </summary>
		/// <value>The clip.</value>
		internal AudioClip clip 
        {
			get
            {
				if( !_clip )
				{
					_clip = SfxManager.GetClip( clipName, m_context );
				}
				return _clip;
			}
			set
            {
				if( value )
				{
					_clip = value;
				}
			}
		}
		/// <summary>
		/// The SfxManager's source name that must be used to play this Sfx. If empty, Effects will be used.
		/// </summary>
		internal string source
        {
			get
            {
				if( string.IsNullOrEmpty( _source ) )
				{
					_source = "Effects";
				}
				return _source;
			}
			set
            {
				if( !string.IsNullOrEmpty( value ) )
					_source = value;
			}
		}
		/// <summary>
		/// The SfxManager's specified source. If non was specified the Effects audio source is returned.
		/// </summary>
		internal AudioSource Source
        {
			get
            {
				return SfxManager.GetSource( source );
			}
		}


		public Sfx( string source, float volume, float pitch = 1f ) : this( volume, pitch )
		{
			this.source = source;
		}
		public Sfx( float volume = 1f, float pitch = 1f )
		{
			this.volume = volume;
			this.pitch = pitch;
		}
		public Sfx( AudioClip clip )
		{
			this.clip = clip;
		}
		public Sfx( string clip )
		{
			clipName = clip;
		}


		/// <summary>
		/// Executes the On Clip Play events.
		/// </summary>
		public void OnClipPlay()
		{
			onClipPlay.events.Invoke();
			onClipPlay.activate.SetActiveInHierarchy();//ACTIVATE..
			onClipPlay.deactivate.SetActiveInHierarchy( false );//DEACTIVATE..
			onClipPlay.enable.SetEnabled();//ENABLE..
			onClipPlay.disable.SetEnabled( false );//DISABLE..
		}
		public void PlayAtPoint( Vector3 point )
		{
			if( clip )
				AudioSource.PlayClipAtPoint( clip, point, volume );
		}


		/// <summary>
		/// Stops all playing sounds.
		/// </summary>
		/// <param name="omit"> The name of the objects holding audio sources that must be omitted from this. </param>
		public static void StopAll( IList<string> omit )
		{
			AudioSourceExt.StopAll( omit );
		}
		/// <summary>
		/// Creates a new list of Sfx with the specified clips and assigns it to the referenced Sfx list.
		/// </summary>
		/// <param name="sfx">Sfx.</param>
		/// <param name="clips">Clips.</param>
		public static void Create( ref List<Sfx> sfx, IList<AudioClip> clips )
		{
			sfx = sfx.Create( clips );
		}
		/// <summary>
		/// Returns a new list of Sfx with the specified clips.
		/// </summary>
		/// <param name="clips">Clips.</param>
		public static List<Sfx> Create( IList<AudioClip> clips )
		{
            return new List<Sfx>().Create( clips );
        }
		/// <summary>
		/// Creates a new list of Sfx with the specified clips and assigns it to the referenced Sfx list.
		/// </summary>
		/// <param name="sfx">Sfx.</param>
		/// <param name="clips">Clips.</param>
		public static void Create( ref List<Sfx> sfx, IList<Object> clips )
		{
			sfx = sfx.Create( clips );
		}
		/// <summary>
		/// Returns a new list of Sfx with the specified clips.
		/// </summary>
		/// <param name="clips">Clips.</param>
		public static List<Sfx> Create( IList<Object> clips )
		{
			return new List<Sfx>().Create( clips );
		}
	}
}

