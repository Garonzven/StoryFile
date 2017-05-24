//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.Events;


namespace DDK.Base.Events 
{
    /// <summary>
    /// Attach to a gameObject to handle On Became Visible or Invisible events from the inspector.
    /// </summary>
	public class OnBecameVisibleInvisible : MonoBehaviour 
    {
        [Tooltip("The events must be invoked when the gameObject becomes..")]
		public bool visible;
		public UnityEvent actions;
		[Space(5)]
		[Tooltip("For UI elements only, in seconds..")]
		public float checkRate = 0.1f;


		protected bool _isVisible;



		// Use this for initialization
		void Start () 
        {			
			if( GetComponent<RectTransform>() )
			{
				InvokeRepeating( "_CheckVisibility", 0f, checkRate );
			}
		}
		void OnBecameVisible()
		{
			if( visible )
			{
                actions.Invoke();
			}
		}
		void OnBecameInvisible()
		{
			if( !visible )
			{
                actions.Invoke();
			}
		}


		/// <summary>
		/// Checks the visibility for this UI element.
		/// </summary>
		protected void _CheckVisibility()
		{
			if( gameObject.IsInsideItsCanvas() )
			{
				if( _isVisible )
					return;
				OnBecameVisible();
				_isVisible = true;
			}
			else 
            {
				if( !_isVisible )
					return;
				OnBecameInvisible();
				_isVisible = false;
			}
		}
	}
}
