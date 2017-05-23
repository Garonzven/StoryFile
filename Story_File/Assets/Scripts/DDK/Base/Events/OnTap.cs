using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Classes;
using DDK.Base.Extensions;


namespace DDK.Base.Events
{
    /// <summary>
    /// Attach to a gameObject to handle on tap events.
    /// </summary>
	public class OnTap : MonoBehaviourExt 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If this component is disabled, its events won't be invoked";
        #endif
		[Tooltip("If false, it will be destroyed OnMouseUp(). If this is a UI Button, this will always be true," +
			" and the Event is automatically added. Otherwise, this object needs a collider")]
		[ShowIfAttribute( "_OnMouseDown", true )]
		public bool onMouseUpAsButton = true;
		[Tooltip("If true, it will be destroyed OnMouseDown(). If this is a UI Button, this will always be true," +
		         " and the Event is automatically added. Otherwise, this object needs a collider")]
		public bool onMouseDown;
		[Header("Events")]
		public ComposedEvent onTap = new ComposedEvent();


		protected bool _OnMouseDown()
		{
			return onMouseDown;
		}
		
		
		
		// Use this for initialization
		void Start () 
        {
			var bt = GetComponent<Button>();
			if( bt )
			{
				onMouseUpAsButton = true;
				bt.onClick.AddListener( OnMouseUpAsButton );
			}
		}


		public void OnMouseUpAsButton()
		{
			if( !enabled || onMouseDown )
				return;
			if( onMouseUpAsButton )
			{
				onTap.Invoke ();
			}
		}		
		public void OnMouseUp()
		{
			if( !enabled || onMouseDown )
				return;
			if( !onMouseUpAsButton )
			{
				onTap.Invoke ();
			}
		}
		public void OnMouseDown()
		{
			if( !enabled )
				return;
			if( onMouseDown )
			{
				onTap.Invoke ();
			}
		}
	}
}
