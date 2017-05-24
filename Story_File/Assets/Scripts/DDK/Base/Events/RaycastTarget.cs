//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;

namespace DDK.Base.Events 
{
	/// <summary>
	/// Attach to an object you want to use as raycast target.
	/// </summary>
	[RequireComponent( typeof( Collider ) )]
	public class RaycastTarget : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute()]
        public string msg = "If disabled, the events won't be invoked";
        #endif
        /// <summary>
        /// The mouse button's index that must be clicked.
        /// </summary>
        [Tooltip("The mouse button's index that must be clicked")]
		public int btIndex = 0;
		[Header("Events")]
		public ComposedEvent onMouseDown = new ComposedEvent();
		public ComposedEvent onMouseUp = new ComposedEvent();
		public ComposedEvent onMouseUpAsButton = new ComposedEvent();
        public ComposedEvent onMouseEnter = new ComposedEvent();
        public ComposedEvent onMouseExit = new ComposedEvent();
        public ComposedEvent onMouseDrag = new ComposedEvent();


		// Use this for initialization
		void Start () {} //Allows enabling/disabling this component

		void OnMouseDown()
		{
			if( !enabled )
				return;
			onMouseDown.Invoke();
		}
		void OnMouseUp()
		{
			if( !enabled )
				return;
			onMouseUp.Invoke();
		}
		void OnMouseUpAsButton()
		{
			if( !enabled )
				return;
			onMouseUpAsButton.Invoke();
		}
        void OnMouseEnter()
        {
            if( !enabled )
                return;
            onMouseUpAsButton.Invoke();
        }
        void OnMouseExit()
        {
            if( !enabled )
                return;
            onMouseExit.Invoke();
        }
        void OnMouseDrag()
        {
            if( !enabled )
                return;
            onMouseDrag.Invoke();
        }

	}
}
