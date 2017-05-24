using UnityEngine;

namespace DDK.Base.Misc
{
    /// <summary>
    /// This can be added to a gameObject on a loaded scene to unload the unused assets and release memory.
    /// </summary>
	public class UnloadUnusedAssets : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "This is done OnStart()";
        #endif

		// Use this for initialization
		void Start ()
        {			
			Resources.UnloadUnusedAssets();
		}
	}
}
