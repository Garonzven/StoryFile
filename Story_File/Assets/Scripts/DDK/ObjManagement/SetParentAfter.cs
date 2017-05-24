//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;



namespace DDK.ObjManagement {

	/// <summary>
	/// Attach to an object you want to parent after a delay.
	/// </summary>
	public class SetParentAfter : MonoBehaviourExt {

		[Tooltip("If false, SetParent() will be called on Start()")]
		public bool onAwake;
		[ShowIfAttribute( "_OnAwake", true )]
		public float delay = 0f;
		public string parent;	
		[Tooltip("World Position Stays")]
		public bool worldPosStays;
		[Header("Events")]
		[Tooltip("If true, when the object is parented, this component will be destroyed")]
		public bool destroyThisAfterDone;
		public ComposedEvent onParented = new ComposedEvent();


		protected bool _OnAwake()
		{
			return onAwake;
		}
					
		

		void Awake()
		{
			if( onAwake )
			{
				SetParent();
			}
		}
		// Use this for initialization
		void Start () {
			if( !onAwake )
			{
				Invoke( "SetParent", delay );
			}
		}



		public void SetParent()
		{
			Transform _parent = parent.Find<Transform>();
			if( transform.parent != _parent )
				transform.SetParent( _parent, worldPosStays );
			onParented.Invoke ();
			if( destroyThisAfterDone )
				Destroy( this );
		}
	}

}
