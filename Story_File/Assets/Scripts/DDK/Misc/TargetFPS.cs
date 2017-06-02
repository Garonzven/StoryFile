using UnityEngine;


namespace DDK.Misc 
{
	/// <summary>
	/// Attach to an object to set the application's target frame rate
	/// </summary>
	public class TargetFPS : MonoBehaviour
    {
		public int targetFrameRate;

		
		// Use this for initialization
		void Start () 
        {			
			Application.targetFrameRate = targetFrameRate;
		}
	}

}
