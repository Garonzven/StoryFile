//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Allows activating/enabling objects after a delay. This stores the first execution data which allows telling 
	/// if the game/screen/window/object... is being executed/shown/called/created... for the first time.
	/// </summary>
	public class FirstExecutionActivateAfter : ActivateAfter 
    {		
		public string executionName = "FirstExecution";
		public bool resetExecution;
		

        /// <summary>
        /// Checks if this is the first App's execution. If so, activates/instantiates the specified gameObjects, enables 
        /// the specified components, and invokes the specified events. Otherwise, it just invokes the specified events.
        /// </summary>
		protected override void OnEnable () 
        {			
			if( resetExecution )
            {
                PlayerPrefs.SetInt( executionName, 1 );
                PlayerPrefs.Save();
            }
				
			bool first = PlayerPrefs.GetInt( executionName, 1 ) == 1;
			if( first )
            {
                StartCoroutine( _StartCoroutines() );
			}
			else InvokeEvents();
		}		
        /// <summary>
        /// Resets the first execution if specified. In other words, the next time the App is played, it will still be 
        /// its first execution.
        /// </summary>
        void OnDisable()
		{
            if( !resetExecution && PlayerPrefs.GetInt( executionName, 1 ) == 1 )
            {
                PlayerPrefs.SetInt( executionName, 0 );
                PlayerPrefs.Save();
            }
		}	
	}
}
