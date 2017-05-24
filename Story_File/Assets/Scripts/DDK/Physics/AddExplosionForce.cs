//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK._Physics {

	/// <summary>
	/// Add an explosion force to all children rigidbodies.
	/// </summary>
	public class AddExplosionForce : MonoBehaviour {
		
		public bool onStart = true;
		public float force = 1000000f;
		public float radius = 5f;
		public float upwardsModifier = 2f;
		
		
		/// <summary>
		/// All children rigidbodies
		/// </summary>
		protected Rigidbody[] _all;
		/// <summary>
		/// Modify this value from another script before this one is started, to specify the explosion's position.
		/// </summary>
		internal Vector3 m_explosionPos;
		
		
		
		// Use this for initialization
		void Start () {
			
			_all = GetComponentsInChildren<Rigidbody>();
			if( !onStart )
				return;
			AddForces();
		}


		public void AddForces()
		{
			if( _all == null )
				return;
			if( m_explosionPos == default(Vector3) )
			{
				m_explosionPos = transform.position;
			}
			for( int i=0; i<_all.Length; i++ )
			{
				_all[i].AddExplosionForce( force, m_explosionPos, radius, upwardsModifier );
			}
		}


		#region MULTI PARAM ACTIONS
		public class Explosion
		{
			public float force = 1000000f;
			public float radius = 5f;
			public float upwardsModifier = 2f;
		}
		/// <summary>
		///If no /explosionPos/ this object's transform position will be used.
		/// </summary>
		public void AddForces( Rigidbody[] rigidbodies, Vector3 explosionPos, Explosion explosion )
		{
			if( rigidbodies == null )
				return;
			if( explosionPos == default(Vector3) )
			{
				explosionPos = transform.position;
			}
			for( int i=0; i<rigidbodies.Length; i++ )
			{
				rigidbodies[i].AddExplosionForce( explosion.force, explosionPos, explosion.radius, explosion.upwardsModifier );
			}
		}
		#endregion
	}
}
