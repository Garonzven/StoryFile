//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.SoundFX
{
    /// <summary>
    /// This allows referencing an AudioClip from any script/component and setting some extra parameters for better 
    /// manipulation/control over the clip.
    /// </summary>
	[System.Serializable]
	public class DelayedAudioClip //This has an extension class
	{		
		public float delay;
		public AudioClip clip;
		[Tooltip("The SfxManager's source in which the clip should be played")]
		public string sfxManagerSource = "Voices";
#if USE_NODE_CANVAS
		public float talkingRate = 0.1f;
#endif
    }	
}
