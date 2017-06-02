//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
#if USE_THREE_D
using DDK._3D;
#endif


namespace DDK.Base.Classes 
{
	/// <summary>
	/// Class for enabling or disabling components.
	/// </summary>
	[System.Serializable]
	public class EnableDisable 
    {
		public float delay;
		public Behaviour[] behaviours = new Behaviour[0];
		public GameObject[] objs = new GameObject[0];
		public Renderer[] renderers = new Renderer[0];
		public Collider[] colliders = new Collider[0];
		
		
		
		private static bool _wasDisabled = false;
		public static bool _WasDisabled
        {
			get{ return _wasDisabled; }
		}		
		
		

		/// <summary>
		/// Enable or disable the components in the array.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enables all components; otherwise it disables them.</param>
		public void EnableAtEndOfFrame ( bool enable = true ) 
        {			
			_EnableAtEndOfFrame( enable ).Start();
		}
		/// <summary>
		/// Enable or disable the components in the array.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enables all components; otherwise it disables them.</param>
		public void Enable ( bool enable = true ) 
        {			
			_Enable( delay, enable ).Start();
		}



		/// <summary>
		/// Enable or disable the components in the array.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enables all components; otherwise it disables them.</param>
		protected void _Enable ( bool enable = true ) 
        {			
			//ENABLE/DISABLE COMPONENTS IN OTHER GAMEOBJECTS
			behaviours.SetEnabled( enable );
			//ENABLE/DISABLE GAMEOBJECTS
			objs.SetActiveInHierarchy( enable );
			//ENABLE/DISABLE RENDERERS
			renderers.SetEnabled( enable );
			//ENABLE/DISABLE COLLIDERS
			colliders.SetEnabled( enable );
			if( !enable )
				_wasDisabled = true;
		}
		/// <summary>
		/// Enable or disable the components in the array.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enables all components; otherwise it disables them.</param>
		private IEnumerator _EnableAtEndOfFrame ( bool enable = true ) 
        {			
			yield return null;
			_Enable( enable );
		}
		/// <summary>
		/// Enable or disable the components in the array.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enables all components; otherwise it disables them.</param>
		private IEnumerator _Enable ( float after, bool enable = true ) 
        {			
			yield return new WaitForSeconds( after );
			_Enable( enable );
		}		
	}
}