//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;



namespace DDK.Base.Events.States {

	/// <summary>
	/// Disables the specified components for the specified amount of time
	/// </summary>
	public class DisableFor : DisableAfter {

		public float duration;



		new protected void OnEnable()
		{
			Invoke( "DisableAll", delay );
			Invoke( "EnableAll", delay + duration );
			Invoke( "DisableThisIfNecessary", delay + duration );
		}




		
		public void EnableAll()
		{
			behaviours.SetEnabled( true );
			renderers.SetEnabled( true );
		}


	}
	
}