//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;


namespace DDK.Base.UI
{
	/// <summary>
	/// Editor Only. Sets the Text component's text value as the gameObject's name.
	/// </summary>
	[ExecuteInEditMode]
	public class TextValueAsName : MonoBehaviour 
    {		
        #if UNITY_EDITOR
		[HelpBoxAttribute]
		public string msg = "Editor Only";
		public bool stopUpdate;
		
		
		protected Text _text;
		
		
		void Start() 
        {			
			if( Application.isPlaying )
				Destroy ( this );
			_text = GetComponentInChildren<Text>();
		}
		
		// Update is called once per frame
		void Update () 
        {			
			if( stopUpdate )
				return;
			if( name != null )
			{
                if( !_text )
                    _text = GetComponentInChildren<Text>();
				_text.text = name;
			}
		}
        #endif
	}
}