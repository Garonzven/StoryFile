//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.Base.Classes 
{
	/// <summary>
	/// This class can be used to serialize any object/struct containing a similar structure such as Vectors, Color...
	/// </summary>
	[System.Serializable]
	public class Vec<T> where T : struct 
    {		
		public T x, y, z, w;		
		
		
		public Vec() { _Defaults(); }		
		public Vec( T x, T y, T z = default(T), T w = default(T) )
		{
			_Defaults();
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Vec ( Vector3 vector )
		{
			_Defaults();
			this.x = (T)(object)vector.x;
			this.y = (T)(object)vector.y;
			this.z = (T)(object)vector.z;
		}
		public Vec ( Vector2 vector ) : this( (Vector3)vector ) {}


		public static Vec<float> Create( Vector2 vector )
		{
			return new Vec<float>( vector.x, vector.y );
		}
		public static Vec<float> Create( Vector3 vector )
		{
			return new Vec<float>( vector.x, vector.y, vector.z );
		}

		
		void _Defaults()
		{
			x = default(T);
			y = default(T);
			z = default(T);
			w = default(T);
		}


		public Vector3 GetAsVector3()
		{
			float x, y, z;
			x = (float)(object)this.x;
			y = (float)(object)this.y;
			z = (float)(object)this.z;
			return new Vector3( x, y, z );
		}
		public void Set( Vector3 vec )
		{
			x = (T)(object)vec.x;
			y = (T)(object)vec.y;
			z = (T)(object)vec.z;
		}



		/// <summary>
		/// Moves towards the specified target. This need to be called multiple times. The next position is automatically set.
		/// </summary>
		public void MoveTowards( Vector3 target, float deltaSpeed = 1f )
		{
			Set ( Vector3.MoveTowards( GetAsVector3(), target, deltaSpeed ) );
		}
		/// <summary>
		/// Moves towards the specified target.
		/// </summary>
		public IEnumerator MoveTowardsCo( Vector3 target, float deltaSpeed = 1f )
		{
			while( GetAsVector3() != target )
			{
				Set ( Vector3.MoveTowards( GetAsVector3(), target, deltaSpeed ) );
				yield return null;
				if( Vector3.Distance( GetAsVector3(), target ) < 0.001f )
				{
					break;
				}
			}
			Set ( target );
		}
		/// <summary>
		/// Moves towards the specified target until it is reached. This calls MoveTowardsCo()
		/// </summary>
		public void AnimMoveTowards( Vector3 target, float deltaSpeed = 1f )
		{
			MoveTowardsCo( target, deltaSpeed ).Start();
		}		
	}
}
