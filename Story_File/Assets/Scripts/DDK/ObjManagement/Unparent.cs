//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Statics;
using DDK.Base.Extensions;


namespace DDK.ObjManagement 
{
	/// <summary>
	/// Attach to an object to unparent it after a delay
	/// </summary>
	public class Unparent : MonoBehaviour 
    {		
		public float after = 0f;
		[Tooltip("If true, this component will be destroyed after the object is unparented.")]
		public bool destroyThisAfterDone = true;
				
		
		
		// Use this for initialization
		IEnumerator Start () 
        {
			yield return new WaitForSeconds( after );
			transform.SetParent( null );
			if( destroyThisAfterDone )
				Destroy ( this );
		}
	}
}
