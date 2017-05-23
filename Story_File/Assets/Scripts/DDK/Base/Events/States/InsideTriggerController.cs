//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// This allows detecting if an object is inside a specified trigger. Access the public property /m_Inside/ to know
    /// if the gameObject is inside or not.
	/// </summary>
	public class InsideTriggerController : MonoBehaviour 
    {		
		public Collider2D trigger2D;
		public Collider trigger;		
		
		
        public bool m_Inside { get; private set; }		
		
		
		void OnTriggerEnter2D( Collider2D col )
		{
			if( col == trigger2D )
			{
				m_Inside = true;
			}
		}		
		void OnTriggerExit2D( Collider2D col )
		{
			if( col == trigger2D )
			{
				m_Inside = false;
			}
		}
		void OnTriggerEnter( Collider col )
		{
			if( col == trigger )
			{
				m_Inside = true;
			}
		}		
		void OnTriggerExit( Collider col )
		{
			if( col == trigger )
			{
				m_Inside = false;
			}
		}
	}

}
