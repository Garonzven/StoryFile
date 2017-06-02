using UnityEngine;
using System.Collections.Generic;

using DDK.Base.Extensions;
using DDK.Base.Managers;

using UnityEngine.UI;
using DDK.Base.SoundFX;


namespace DDK.Base.Events {
	
	public class OnClickPlaySfxList : MonoBehaviour {

		[HelpBoxAttribute]
		public string msg = "If this object has a Button component, the event is automatically added to the onClick Listener";
		[Space(15f)]

		public float delay;
		public List<Sfx> sfx;

	    private AudioSource source;
		
		
		// Use this for initialization
		void Start () {
			
			var bt = GetComponent<Button>();
			if( bt )
			{
				bt.onClick.AddListener( () => Play() );
			}

		    source = SfxManager.GetSource("Effects");
		}
		
		
		
		public void Play()
		{
            sfx.PlayAfter(delay, source);
		}
		
	}
	
}

