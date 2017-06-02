//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using MovementEffects;
using DDK.Base.Extensions;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Allows deactivating/disabling objects after a delay. This stores the first execution data which allows telling 
	/// if the game/screen/window/object... is being executed/shown/called/created... for the first time.
	/// </summary>
	public class FirstExecutionDeactivateAfter : DeactivateAfter 
    {		
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "This gameObject will be deactivated if this isn't the first App's execution";
        #endif
		public string executionName = "FirstExecution";
		public bool resetExecution;
		
		
        /// <summary>
        /// Deactivates this gameObject after the specified delay if this isn't the first App's execution. The first 
        /// execution flag might also be reset here if specified.
        /// </summary>
        protected override void Start () 
        {				
			if( resetExecution )
            {
                PlayerPrefs.SetInt( executionName, 1 );
                PlayerPrefs.Save();
            }
			
			bool first = PlayerPrefs.GetInt( executionName, 1 ) == 1;
			if( !first )
            {
                Timing.RunCoroutine( gameObject.SetActiveAfter( delay, false ) );
			}
		}		
        /// <summary>
        /// Marks the first execution flag as false when this component gets disabled, if /resetExecution/ is false and 
        /// it's the first App's execution.
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
