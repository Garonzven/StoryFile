using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;
using DDK.Base.Managers;
using DDK.Base.SoundFX;


namespace DDK.Base.Events 
{	
	public class OnClickPlaySfx : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If this object has a Button component, the event is automatically added to the onClick Listener";
        #endif
		[Space(15f)]
		[Tooltip("If true, and the Source is playing when the specified clip is about to be played, the source will be interupted; otherwise the clip won't be played")]
		public bool interrupt = true;
        [Tooltip("Will call the actions and events in OnClipPlay")]
		public Sfx sfx;
		
		// Use this for initialization
		void Start () 
        {			
			var bt = GetComponent<Button>();
			if( bt )
			{
				bt.onClick.AddListener( () => Play() );
			}
		}
		
		public void Play()
		{
			SfxManager.PlaySfx( sfx, interrupt, gameObject );
		}		
	}	
}

