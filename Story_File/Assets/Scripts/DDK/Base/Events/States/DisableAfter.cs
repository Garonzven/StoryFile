//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;



namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Disables the specified components after a delay
	/// </summary>
	public class DisableAfter : MonoBehaviour 
    {
		public bool disableAfterDone = true;
		public Behaviour[] behaviours;
		public Renderer[] renderers;
		public float delay = 1f;		
		
		
		// Use this for initialization
		protected void OnEnable () 
        {			
			Invoke( "DisableAll", delay );
			Invoke( "DisableThisIfNecessary", delay );
		}


		public void DisableAll()
		{
			behaviours.SetEnabled( false );
			renderers.SetEnabled( false );
		}
		public void DisableThisIfNecessary()
		{
			if( disableAfterDone )
			{
				enabled = false;
			}
		}
	}	
}