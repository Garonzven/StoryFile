//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.Base.Animations {

	/// <summary>
	/// This is used by the Align(), MoveTowards(), RotateTo() and other similar functions in the Extension classes to prevent multiple calls on an object that is being /flagType/d
	/// </summary>
	public class ValidationFlag : MonoBehaviour {

		[System.Flags]
		public enum Flags { 
			Align = 1 << 0, 
			Move = 1 << 1, 
			Rotate = 1 << 2, 
			RotateAround = 1 << 3,
            Scale = 1 << 4,
            Alpha = 1 << 5
            #if USE_SVG_IMPORTER
            ,Frame = 1 << 6
            #endif
		}


		[HelpBoxAttribute()]
		public string info = "This is used by the Align(), MoveTowards(), RotateTo() and other similar functions in the Extension classes to prevent multiple calls on an object that is being /flagType/d";
		/// <summary>
		/// This is used by the Align(), MoveTowards(), RotateTo() and other similar functions in the Extension classes to prevent multiple calls on an object that is being /flagType/d
		/// </summary>
		[Tooltip("This is used by the Align(), MoveTowards(), RotateTo() and other similar functions in the Extension classes to prevent multiple calls on an object that is being /flagType/d")]
		[EnumFlagsAttribute]
		[Space(50f)]
		public Flags flags;



		/*// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}*/
	}
}
