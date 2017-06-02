using UnityEngine;
using UnityEngine.UI;


namespace DDK.Misc {

	[System.Obsolete("This is not working..", true)]
	public class DisplayDrawCalls : MonoBehaviour {

		public Text displayer;
		[Tooltip("In seconds..")]
		public float updateRate = 1f;


		int drawcalls = 0;
		Renderer[] allObjects;


		void Start() {

			allObjects = GameObject.FindObjectsOfType<Renderer>();
			InvokeRepeating( "_Update", 1f, updateRate );
		}
		
		void _Update(){

			for( int i=0; i<allObjects.Length; i++ )
			{
				var ren = allObjects[i];
				if( ren.isVisible )
				{
					drawcalls++;
					/*if( ren.receiveShadows )
					{
						drawcalls++;
					}
					if( ren.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On )
					{
						drawcalls++;
					}
					else if( ren.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.TwoSided )
					{
						drawcalls += 2;
					}
					drawcalls *= ren.materials.Length;*/
				}
			}
			
			//print drawcalls
			if( displayer )
			{
				displayer.text = "DrawCalls: " + drawcalls.ToString();
			}
			else Debug.Log(drawcalls);
			
			//reset drawcalls every update
			drawcalls = 0;
			
			//do some math to find average drawcall count here
		}

	}

}
