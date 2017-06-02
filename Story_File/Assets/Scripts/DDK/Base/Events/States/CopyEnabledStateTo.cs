//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;


namespace DDK.Base.Events.States 
{	
	/// <summary>
	/// Copies the enabled state from this component to another.
	/// </summary>
	public class CopyEnabledStateTo : MonoBehaviour 
    {
		[Tooltip("If true, the active state will be copied as the opposite state")]
		public bool reversed;
		public EnableDisable to;
				
		
		
		public void OnEnable()
		{
			to.Enable( !reversed );
		}
		public void OnDisable()
		{
			to.Enable( false | reversed );
		}
		
	}
}