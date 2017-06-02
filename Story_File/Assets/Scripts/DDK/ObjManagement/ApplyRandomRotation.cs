//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.ObjManagement {

	/// <summary>
	/// Apply random rotation (at runtime) to an object, or its children.
	/// </summary>
	public class ApplyRandomRotation : MonoBehaviour {

		[Tooltip("If false, to this object instead.")]
		public bool toChildren = true;
		[Tooltip("If an axis's value is higher than 0 the random rotation will be applied to that axis.")]
		public Vector3 axis = new Vector3( 0f, 0f, 1f );
		public bool includeSubChildren;
		public bool includeInactive;
		public bool onAwake;


		void Awake()
		{
			if( onAwake )
			{
				Apply();
			}
		}

		// Use this for initialization
		void Start () {

			if( !onAwake )
			{
				Apply();
			}
		}


		protected void Apply()
		{
			if( toChildren )
			{
				CalculateApplyToChildren();
			}
			else CalculateApplyToThis();
		}
		protected void CalculateApplyToThis()
		{
			gameObject.ApplyRandomRot( axis );
		}
		protected void CalculateApplyToChildren()
		{
			var children = gameObject.GetChildren( includeSubChildren, includeInactive );
			for( int i=0; i<children.Length; i++ )
			{
				children[i].ApplyRandomRot( axis );
			}
		}
	}

}
