//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Classes;
using UnityEngine.UI;

namespace DDK.Base.UI
{
	/// <summary>
	/// Replaces the UIText's text's specified arguments: {0}, {1}...
	/// </summary>
	public class UITextFormat : MonoBehaviour 
	{
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "Replaces the UIText's text's specified arguments: {0}, {1}...";
        #endif
		[Tooltip("If false, on Awake() instead")]
        [ShowIfAttribute( "onEnable", true )]
		public bool onStart = true;
        [ShowIfAttribute( "onStart", true )]
		public bool onEnable;
        [ShowIfAttribute( "onEnable", 1 )]
		public float onEnableDelay;
		public string[] args;
		public SearchableUIText text = new SearchableUIText();


		protected Text _text;


		protected virtual void Awake()
		{
			if( !text.m_object )
				text.m_object = GetComponent<Text>();
			_text = text.m_object;

			if( onStart )
				return;
			Format();
		}
		// Use this for initialization
		void Start () 
		{
			if( !onStart )
				return;
			Format();
		}
		void OnEnable()
		{
			if( !onEnable )
				return;
			Invoke( "Format", onEnableDelay );
		}


		public void Format()
		{
			_text.text = string.Format( _text.text, args );
		}
		public void Format( string[] args )
		{
			_text.text = string.Format( _text.text, args );
		}
		public void Format( Text text )
		{
			text.text = string.Format( text.text, args );
		}
		public void Format( Text text, string[] args )
		{
			text.text = string.Format( text.text, args );
		}
	}
}
