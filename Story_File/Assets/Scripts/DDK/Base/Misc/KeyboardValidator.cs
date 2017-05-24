//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Text;


namespace DDK.Base.Misc 
{
	/// <summary>
	/// This can help validate keyboard input. Attach to an object, then reference an instance of this class from another 
    /// script. Then you just assign the /m_keyboard/ variable and validation will be done automatically.
	/// </summary>
	public class KeyboardValidator : MonoBehaviour 
    {
		//NUMBER
		public bool validateNumber;
        [ShowIfAttribute( "validateNumber", 1)]
		public float from, to;


		internal TouchScreenKeyboard m_keyboard;


		// Use this for initialization
		void Start () {} //Allows enabling/disabling this component
		
		// Update is called once per frame
		void Update () 
        {
			if( m_keyboard == null )
			{
				return;
			}
			if( m_keyboard.active )
			{
				if( string.IsNullOrEmpty( m_keyboard.text ) )
					return;
				_ValidateNumber();
			}
		}


		protected void _ValidateNumber()
		{
			if( !validateNumber )
				return;
			float input = float.Parse( m_keyboard.text );
			if( input < from ) 
            {
				m_keyboard.text = from.ToString();
			}
			else if( input > to ) 
            {
				m_keyboard.text = to.ToString();
			}
		}
	}
}
