//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Enables the specified components after a delay
	/// </summary>
	public class EnableAfter : MonoBehaviour 
    {		
		public Behaviour[] behaviours;
		public Renderer[] renderers;
		public float delay = 1f;
		[Tooltip("A random value from the specified to the specified will be added to the /delay/")]
        public RandomRangeFloat randomDelay = new RandomRangeFloat( true, 0f, 0f );
						
		
		
		// Use this for initialization
		protected void Start () 
        {			
            Invoke( "DisableAll", Mathf.Abs( delay + randomDelay.GetRandom() ) );
		}		
					
		
        /// <summary>
        /// Disables all the specified behaviours and renderers.
        /// </summary>
		public void DisableAll()
		{
			behaviours.SetEnabled();
			renderers.SetEnabled();
		}	
	}	
}