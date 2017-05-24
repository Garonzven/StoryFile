//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Clamps the text in a UI Text without slicing a word.
	/// </summary>
	[RequireComponent( typeof( Text ) )]
	public class UITextClamper : MonoBehaviour 
	{		
		public int maxLength = 20;
				
		
		// Use this for initialization
		void Start () 
		{			
			gameObject.GetComponent<Text>().text = gameObject.GetComponent<Text>().text.ClampLastSpace( maxLength );
		}		
	}	
}