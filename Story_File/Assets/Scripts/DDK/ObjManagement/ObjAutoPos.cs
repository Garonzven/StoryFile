//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.ObjManagement 
{
	/// <summary>
	/// Automatically positions an object depending on the aspect ratio's value.
	/// </summary>
	[ExecuteInEditMode]
	public class ObjAutoPos : MonoBehaviour 
    {		
		[System.Serializable]
		public class Overrider
        {
			public Vector2 aspect = new Vector2( 3f, 5f );
			public Vector3 pos;
			public float minDifference = 0.05f;
		}		
		

        [HelpBoxAttribute]
        public string msg = "Largest Aspect Ratio is 17:9 and Squarest AR is 5:4";
		public Vector3 posInLargest = new Vector3( 0f, 0f, 0f );
		public Vector3 posInSquarest = new Vector3( 0f, 0f, 0f );
		public Overrider[] posOverriders = new Overrider[0];//THIS IMPROVES POSITION		
						
		
		// Use this for initialization
		void Start ()
        {			
			CalculateApply();
		}
		
		#if UNITY_EDITOR
		// Update is called once per frame
		void Update () 
        {			
			if( !Application.isPlaying && enabled )
			{
				CalculateApply();
			}
		}
		#endif			
		
		
		public void CalculateApply()
		{
            float largestAR = new Vector2( 9f, 17f ).GetAspectRatio();
            float squarestAR = new Vector2( 4f, 5f ).GetAspectRatio();
            Vector3 largestPos = posInLargest;
            Vector3 squarestPos = posInSquarest;
			
			//Overrider?
			for( int i=0; i<posOverriders.Length; i++ )
			{
				if( posOverriders[i].aspect.GetAspectRatio().CloseTo( ( 1 / Camera.main.aspect ), posOverriders[i].minDifference ) )
				{
					squarestAR = posOverriders[i].aspect.GetAspectRatio();
					squarestPos = posOverriders[i].pos;
				}
			}
			
			float maxAR = largestAR - squarestAR;//100%
			float actAR = largestAR - ( 1 / Camera.main.aspect );//actual aspect %
			float AR = actAR / maxAR;//actual aspect ( 0 - 1 )%
			
			Vector3 maxPos = squarestPos - largestPos;//100%
			
			float x = largestPos.x + ( AR * maxPos.x );
			float y = largestPos.y + ( AR * maxPos.y );
			float z = largestPos.z + ( AR * maxPos.z );
            RectTransform rTransform = GetComponent<RectTransform>();
            if( rTransform )
            {
                rTransform.anchoredPosition = new Vector3( x, y );
            }
			else transform.position = new Vector3( x, y, z );
		}
		public void Invert( bool x = true, bool y = false )
		{
			if( x )
			{
				posInLargest = new Vector2( -posInLargest.x, posInLargest.y );
				posInSquarest = new Vector3( -posInSquarest.x, posInSquarest.y );
			}
			if( y )
			{
				posInLargest = new Vector2( posInLargest.x, -posInLargest.y );
				posInSquarest = new Vector3( posInSquarest.x, -posInSquarest.y );
			}
		}		
	}
}
