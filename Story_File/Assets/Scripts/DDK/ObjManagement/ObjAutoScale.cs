//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.ObjManagement 
{
	/// <summary>
	/// Automatically scales an object depending on the aspect ratio's value
	/// </summary>
	[ExecuteInEditMode]
    public class ObjAutoScale : MonoBehaviour //THIS HAS AN EDITOR CLASS
    {
		
		public Vector2 original_largestAspect = new Vector2( 9f, 17f );
		public Vector2 widestAspect = new Vector2( 4f, 5f );
		/// <summary>
		/// The largest aspect scale.
		/// </summary>
		public Vector3 originalScl = new Vector3( 1f, 1f, 1f );
		/// <summary>
		/// The most square aspect scale.
		/// </summary>
		public Vector3 widestTargetScl = new Vector3( 1f, 1f, 1f );


        public Camera _Cam
        {
            get
            {
                if ( Camera.main == null )
                {
                    Debug.LogWarning("There is no camera tagged as MainCamera, returning the first found Camera object", gameObject);
                    return GameObject.FindObjectOfType<Camera>();
                }
                return Camera.main;
            }
        }
						
		
		
		// Use this for initialization
		void Start () 
        {
			if( enabled )
				CalculateApply();
		}
		
		#if UNITY_EDITOR
		// Update is called once per frame
		public void Update () 
        {			
            if( this && !Application.isPlaying && enabled )//If not checking for *this* the script throws a MissingReferenceException
			{
				CalculateApply();
			}
		}
		#endif				
		
		
		public void CalculateApply()
		{
			if( originalScl.sqrMagnitude == 0f || widestTargetScl.sqrMagnitude == 0f )
				return;

			float olAR = original_largestAspect.GetAspectRatio();
			float wAR = widestAspect.GetAspectRatio();
			Vector3 oScl = originalScl;
			Vector3 tScl = widestTargetScl;
			
			float maxAR = olAR - wAR;//100%
            float actAR = olAR - ( 1 / _Cam.aspect );//actual aspect %
			float AR = actAR / maxAR;//actual aspect ( 0 - 1 )%
			
			Vector3 maxScl = tScl - oScl;//100%
			float x = oScl.x + ( AR * maxScl.x );
			float y = oScl.y + ( AR * maxScl.y );
			float z = oScl.z + ( AR * maxScl.z );
			transform.localScale = new Vector3( x, y, z );
		}			
	}
}
