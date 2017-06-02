//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.ObjManagement {

	public class LockPos : MonoBehaviour {
		
		/// <summary>
		/// If true, the initial position will be stored after the current frame ends.
		/// </summary>
		[Tooltip("If true, the initial position will be stored after the current frame ends.")]
		public bool waitForEndOfFrame = true;
		public bool onlyX;
		public bool onlyY;
		public bool onlyZ = true;
		public bool local;		
		
		
		Vector3 iniPos;
		bool initDone;		
		
		
		
		// Use this for initialization
		void Awake () 
        {			
			if( waitForEndOfFrame )
				StartCoroutine( StoreIniPos() );
			else
			{
				_StoreIniPos();
				initDone = true;
			}
		}
		
		// Update is called once per frame
		void Update () 
        {			
			if( initDone )
			{
				Lock ();
			}
		}
		
		public IEnumerator StoreIniPos()
		{
			yield return null;
			_StoreIniPos();
			initDone = true;
		}

		protected void _StoreIniPos()
		{
			if( local )
				iniPos = transform.localPosition;
			else iniPos = transform.position;
		}
		
		public void Lock()
		{
			Vector3 newPos = transform.position;
			if( local )
				newPos = transform.localPosition;
			if( onlyX )
			{
				newPos.x = iniPos.x;
			}
			if( onlyY )
			{
				newPos.y = iniPos.y;
			}
			if( onlyZ )
			{
				newPos.z = iniPos.z;
			}
			if( local )
				transform.localPosition = newPos;
			else transform.position = newPos;
		}
		
		
	}

}
