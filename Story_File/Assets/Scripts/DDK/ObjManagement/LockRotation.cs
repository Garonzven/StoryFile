//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.ObjManagement {

	public class LockRotation : MonoBehaviour {

		public bool x = true;
		public bool y = true;
		public bool z = true;
		public bool local;


		Vector3 _iniRot;		
		
		
		// Use this for initialization
		void Awake () {
			
			_iniRot = (local) ? transform.localEulerAngles : transform.eulerAngles;
			if(  !x && !y && !z ) enabled = false;
		}
		
		// Update is called once per frame
		void LateUpdate () {

			LockRot();
		}

		public void LockRot()
		{
			Vector3 rot = (local) ? transform.localEulerAngles : transform.eulerAngles;
			if( x )
			{
				rot.x = _iniRot.x;
			}
			if( y )
			{
				rot.y = _iniRot.y;
			}
			if( z )
			{
				rot.z = _iniRot.z;
			}
			if( local )
			{
				transform.localEulerAngles = rot;
			}
			else transform.eulerAngles = rot;
		}


	}

}
