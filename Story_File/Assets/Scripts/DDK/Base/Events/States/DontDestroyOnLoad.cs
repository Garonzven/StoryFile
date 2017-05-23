using UnityEngine;
using System.Collections.Generic;


namespace DDK.Base.Events.States 
{
    /// <summary>
    /// Attach to a gameObject to prevent it from being destroyed when changing scenes.
    /// </summary>
	public class DontDestroyOnLoad : MonoBehaviour 
    {
		public bool ensureSingleInstance;
        [HelpBoxAttribute( "ensureSingleInstance" )]
		public string msg = "Ensure your objects names are unique";


		public static List<string> Instances;

		/// <summary>
		/// Prevents Awake() from being called again. when changing scenes.
		/// </summary>
		protected bool _started;


		void Awake () 
        {
            if( _started || !enabled )
				return;
			if( Instances == null )
				Instances = new List<string>();

			if( ensureSingleInstance && Instances.Contains( name ) )
			{
				DestroyImmediate( gameObject );
				return;
			}

			Instances.Add( name );

            DontDestroyOnLoad( gameObject );
			_started = true;
		}
        void Start(){}//Allow to enable/disable this component
	}
}
