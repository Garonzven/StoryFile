//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using MovementEffects;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Attach to an object to deactivate it after a delay
	/// </summary>
	public class DeactivateAfter : MonoBehaviour 
    {		
		public float delay = 1f;
		
		
		/// <summary>
        /// Deactivates this gameObject after the specified /delay/.
        /// </summary>
        protected virtual void Start () 
        {			
            Timing.RunCoroutine( gameObject.SetActiveAfter( delay, false ) );
		}	
	}
}