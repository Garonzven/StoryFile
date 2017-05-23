using UnityEngine;


namespace DDK._3D.Optimization 
{
    /// <summary>
    /// Allows invoking BillboardManager functions. Since the BillboardManager has only 1 active instance at a time and 
    /// it can be created automatically, this allows setting/overwriting some of its parameters without doing it directly.
    /// </summary>
    public class BillboardsController : MonoBehaviour 
    {
        public bool setRateOnStart;
        public float rate = 60f;


    	// Use this for initialization
    	void Start () 
        {    	
            if( setRateOnStart )
            {
                SetCurrentUpdateRate( rate );
            }
    	}


        /// <summary>
        /// Sets the rate in which the billboards are updated (forced to look at the camera)
        /// </summary>
        /// <param name="rate">Rate.</param>
        public void SetCurrentUpdateRate( float rate )
        {
            BillboardManager.SetCurrentUpdateRate( rate );
        }
    }
}
