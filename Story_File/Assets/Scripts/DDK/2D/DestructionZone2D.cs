//By: Daniel Soto
//4dsoto@gmail.com 
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using System.Collections.Generic;


namespace DDK._2D {

	[RequireComponent( typeof( BoxCollider2D ) )]
	public class DestructionZone2D : MonoBehaviour { 
	
		[NotLessThan( 0f, "If not cero, the OnTriggerEnter() will be called every specified seconds. This allows doing something" +
		             " similar to OnTriggerStay() with better performance" )]
		public float checkOnTriggerEnterRate = 0f;
		public string[] objsWithTags = new string[]{ "Untagged" };


		protected List<Collider2D> _others;


		public delegate void OnOtherDestroyed();
		/// <summary>
		/// This is called when an object is destroyed on trigger enter
		/// </summary>
		internal OnOtherDestroyed _onOtherDestroyed;
		
		
		
		// Use this for initialization
		void Start () {
			
			GetComponent<BoxCollider2D>().isTrigger = true;
			if( checkOnTriggerEnterRate > 0 )
			{
				_others = new List<Collider2D>();
				InvokeRepeating( "_OnTriggerStay", checkOnTriggerEnterRate, checkOnTriggerEnterRate );
			}
		}
		
		
		
		public void OnTriggerEnter2D( Collider2D other )
		{
			if( objsWithTags.Contains( other.tag ) )
			{
				if( _onOtherDestroyed != null )
				{
					_onOtherDestroyed();
				}
				/*if( checkOnTriggerEnterRate > 0f && _others.Contains( other ) )
				{
					_others.Remove( other );
				}*/
				Destroy( other.gameObject );
			}
			else if( checkOnTriggerEnterRate > 0 && !_others.Contains( other ) ) 
			{
				_others.Add( other );
			}
		}



		protected void _OnTriggerStay()
		{
			if( _others == null )
			{
				return;
			}
			_others = _others.RemoveNull();
			for( int i=0; i<_others.Count; i++ )
			{
				OnTriggerEnter2D( _others[i] );
			}
		}

	}

}
