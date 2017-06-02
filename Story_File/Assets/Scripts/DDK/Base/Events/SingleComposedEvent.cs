//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.Events 
{
    /// <summary>
    /// Attach to a gameObject to invoke multiple events just like you would do with UnityEvent but withthe posibility 
    /// to add a description, delays, and destroy the component after events are invoked. You can invoke them manually 
    /// or OnEnable().
    /// </summary>
	public class SingleComposedEvent : MonoBehaviourExt 
    {
		public string m_description;
		public bool onEnable;
        [ShowIfAttribute( "onEnable", 1 )]
		public float delay;
		[Tooltip("If true, this component will be destroyed after the events are executed")]
		public bool destroyThisAfter;
		[Space(5f)]
		public ComposedEvent m_composedEvent = new ComposedEvent();


		void OnEnable()
		{
			if( onEnable )
				Invoke( delay );
		}


        /// <summary>
        /// Invoke all the specified events.
        /// </summary>
		public void Invoke()
		{
			Invoke ( 0f );
		}
        /// <summary>
        /// Invoke all the specified events.
        /// </summary>
		public void Invoke( float pDelay )
		{
			_Invoke( pDelay ).Start();
		}
        /// <summary>
        /// Invoke all the specified events.
        /// </summary>
		public IEnumerator _Invoke( float pDelay )
		{
			if( pDelay > 0f )
				yield return new WaitForSeconds( pDelay );
			if( m_composedEvent != null )
			{
				m_composedEvent.Invoke( gameObject );
                if( destroyThisAfter )
                    Destroy( this, m_composedEvent.delay );
			}
			if( destroyThisAfter )
				Destroy( this );
		}
        /// <summary>
        /// Sets the delay used when /onEnale/ is true.
        /// </summary>
        /// <param name="pDelay">OnEnable() delay.</param>
        public void SetOnEnabledDelay( float pDelay )
        {
            delay = pDelay;
        }
	}
}
