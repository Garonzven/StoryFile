//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.AssetBundles {

	/// <summary>
	/// This allows extra control over objects that form part of an asset bundle. For Example, you can attach this to a
	/// prefab that has been included in an asset bundle to prevent it from being instantiated when using the AssetBundlesLoader.cs
	/// </summary>
	public class ABController : MonoBehaviour {

		[Tooltip("If true, this Game Object (prefab) won't be instantiated from the Asset Bundles Loader")]
		public bool m_doNotInstantiateFromLoader = true;

	}

}
