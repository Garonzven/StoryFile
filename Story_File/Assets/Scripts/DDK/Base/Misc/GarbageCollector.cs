using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;


namespace DDK.Base.Misc
{
	public class GarbageCollector : MonoBehaviour 
    {
		public bool onEnable;
        [NotLessThan( 0f, "In seconds. If equal to cero, this won't be taken into account." )]
        public float rate = 0f;


        void Start() {} //Allows enabling/disabling this component
		void OnEnable () 
        {
			if( onEnable )
				Collect ();
            if( rate > 0f )
                Timing.RunCoroutine( _Collect(), "GarbageCollector" );
		}
		
		
		public void Collect()
		{
			System.GC.Collect();
		}
        protected IEnumerator<float> _Collect()
        {
            yield return Timing.WaitForSeconds( rate );
            while( this )
            {
                Collect();
                yield return Timing.WaitForSeconds( rate );
            }
        }
	}
}
