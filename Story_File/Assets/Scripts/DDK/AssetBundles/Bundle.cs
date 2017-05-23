//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Managers;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DDK.Base.Classes;


namespace DDK.AssetBundles 
{
	/// <summary>
	/// This is used to ease the asset bundles loading process
	/// </summary>
	/// <seealso cref="AssetBundlesLoader.cs"/>
	[System.Serializable]
	public class Bundle 
    {
		public string url;
		public int version;
		[Tooltip("If true, all assets inside the bundle will be loaded regardless of the -Assets- values")]
		public bool loadAll = true;
		[Tooltip("All the assets names")]
		[DisableIfAttribute( "loadAll", 1 )]
		public string[] assets;
		[Tooltip("If true, the assets will be instantiated")]
		public bool instantiate;
		[Tooltip("If true, the bundle MUST contain audio clips, this will be loaded to the SfxManager's /_Clips/.")]
		public bool loadToSfxManager;
		[Tooltip("If not empty, the bundle will be treated as a scene, hence it will be loaded after download is complete")]
		public string sceneName;
		[Tooltip("If true, the scene will be automatically loaded asyncronously")]
		[ShowIfAttribute( "_IsScene", 1)]
		public bool autoLoadAsync;
		[ShowIfAttribute( "_IsScene", 1)]
		public FadeSettings transition;
		public bool unload = true;
		[Tooltip("The name of a GameObject that will be activated after the bundle has been loaded")]
		public ComposedEvent onBundleLoaded;


		protected bool _IsScene()
		{
			return !string.IsNullOrEmpty( sceneName );
		}


		public Bundle() {}
		public Bundle( string url, int version, bool loadAll, bool loadToSfxManager, bool instantiate ) 
			: this( url, version, loadToSfxManager, instantiate )
		{
			this.loadAll = loadAll;
		}
		public Bundle( string url, int version, bool loadToSfxManager, bool instantiate ) 
			: this( url, version )
		{
			this.url = url;
			this.version = version;
			this.loadToSfxManager = loadToSfxManager;
			this.instantiate = instantiate;
		}
		public Bundle( string url, int version, string sceneName ) : this( url, version )
		{			
			this.sceneName = sceneName;
		}
		public Bundle( string url, int version ) 
		{			
			this.url = url;
			this.version = version;
		}



		/// <summary>
		/// Activates/Instantiates the "activateAfterBundleLoaded" GameObject.
		/// </summary>
		public void InvokeOnBundleLoaded()
		{
			if( onBundleLoaded == null )
				return;
			onBundleLoaded.Invoke();
		}
		/// <summary>
		/// If /loadToSfxManager/ is true, the /clips/ will be loaded to the SfxManager's /_Clips/.
		/// </summary>
		/// <param name="addInstead"> If true, the clips are included into the SfxManager's clips instead of replacing them </param>
		public void LoadToSfxManager( IList<AudioClip> clips, bool addInstead = false )
		{
			if( !addInstead )
			{
				SfxManager._Clips.Clear();
			}
			SfxManager._Clips.AddRange<AudioClip>( clips );
		}
		/// <summary>
		/// If "loadToSfxManager" is true, the bundle/clips will be loaded to the SfxManager's CLIPS.
		/// </summary>
		/// <param name="addInstead"> If true, the clips are included into the SfxManager's clips instead of replacing them </param>
		public void LoadToSfxManager( AssetBundle bundle, bool addInstead = false )
		{
			LoadToSfxManager( bundle.LoadAllAssets<AudioClip>(), addInstead );
		}
		/// <summary>
		/// If "loadToSfxManager" is true, the clips will be loaded to the SfxManager's CLIPS.
		/// </summary>
		/// <param name="addInstead"> If true, the clips are included into the SfxManager's clips instead of replacing them </param>
		public void LoadToSfxManager( List<Object> objs, bool addInstead = false )
		{
			LoadToSfxManager( objs.Convert<Object, AudioClip>().ToArray(), addInstead );
		}
	}
}
