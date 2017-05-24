//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DDK.Base.Statics;
using System.Linq;
using DDK.Base.Classes;
using UnityEngine.UI;


namespace DDK.Base.Extensions 
{
	/// <summary>
	/// Extension class for arrays, lists and vectors.
	/// </summary>
	public static class ArrayExt 
    {		
		#region CONTAINS
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		public static bool Contains(this float[] array, float item)
		{
			for( int i=0; i<array.Length; i++ ) 
				if(item == array[i])
					return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		/// <param name="endIndex">The last index that will be checked before returning false. first index = 0.</param>
		public static bool Contains(this float[] array, float item, int endIndex)
		{
			int counter=0;
			while(counter++ < endIndex) if(array[counter] == item) return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		public static bool Contains(this int[] array, int item)
		{
            if( array == null )
                return false;
			for( int i=0; i<array.Length; i++ ) 
				if(item == array[i]) 
					return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		/// <param name="endIndex">The last index that will be checked before returning false. first index = 0.</param>
		public static bool Contains(this int[] array, int item, int endIndex)
		{
			int counter=0;
			while(counter++ < endIndex) if(array[counter] == item) return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		public static bool Contains(this byte[] array, byte item)
		{
			for( int i=0; i<array.Length; i++ ) 
				if(item == array[i]) 
					return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		/// <param name="endIndex">The last index that will be checked before returning false. first index = 0.</param>
		public static bool Contains(this byte[] array, byte item, int endIndex)
		{
			byte counter=0;
			while(counter++ < endIndex) if(array[counter] == item) return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		public static bool Contains(this bool[] array, bool item)
		{
			for( int i=0; i<array.Length; i++ ) 
				if(item == array[i]) 
					return true;
			return false;
		}		
		/// <summary>
		/// Verifies if an array contains the specified item.
		/// </summary>
		/// <param name="endIndex">The last index that will be checked before returning false. first index = 0.</param>
		public static bool Contains(this bool[] array, bool item, int endIndex)
		{
			byte counter=0;
			while(counter++ < endIndex) if(array[counter] == item) return true;
			return false;
		}
		public static bool Contains<T>( this T[] array, T value )
		{
			if( array == null )
				return false;
			for( int i=0; i<array.Length; i++ )
			{
				if( Equals( array[i], value ) )
					return true;
			}
			return false;
		}		
        public static bool ContainsNamed<T>( this T[] array, string name ) where T : UnityEngine.Object
        {
            if( array == null )
                return false;
            for( int i=0; i<array.Length; i++ )
            {
                if( array[i].name.Equals( name ) )
                    return true;
            }
            return false;
        }   
		public static bool ContainsAny<T>( this T[] array, params T[] values )
		{
			if( array == null )
				return false;
			for( int i=0; i<array.Length; i++ )
			{
				for( int j=0; j<values.Length; j++ )
				{
					if( Equals( array[i], values[j] ) )
						return true;
				}
			}
			return false;
		}
		public static bool ContainsAll<T>( this IList<T> list, IList<T> items )
		{
			for( int i=0; i<items.Count; i++ )
			{
				if( !list.Contains( items[i] ) )
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Returns the amount of items that are contained inside the specified list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="items">Items.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static int Contains<T>( this List<T> list, IList<T> items )
		{
			int c = 0;
			for( int i=0; i<items.Count; i++ )
			{
				if( list.Contains( items[i] ) )
				{
					c++;
				}
			}
			return c;
		}
		public static bool ContainsIgnoreCase( this string[] array, string value )
		{
			for( int i=0; i<array.Length; i++ )
			{
				if( array[i].Equals( value, StringComparison.CurrentCultureIgnoreCase ) )
					return true;
			}
			return false;
		}
		#endregion
		
		#region FILL
		/// <summary>
		/// Fill the specified array with a unique value, to avoid having 0 when a value hasn't been set.
		/// </summary>
		/// <param name="item">The item that will fill the array.</param>
		public static void FillUnique(this float[] array, float item)
		{
			for(int i=0; i<array.Length; i++) array[i] = item;
		}				
		/// <summary>
		/// Fills the colors array with specified color.
		/// </summary>
		/// <param name="colors">Colors to be filled.</param>
		/// <param name="color">The Color to be filled with.</param>
		/// <param name="minAlpha">Minimum alpha. Colors with alpha higher than the specified value will be filled</param>
		/// <param name="color2">The Color to fill the unfilled pixels.</param>
		public static void FillWith(this Color[] colors, Color color, float minAlpha = 0.5f, Color color2 = default(Color) )
		{
			for( int i=0; i<colors.Length; i++ ) {
				if( colors[i].a > minAlpha ) colors[i] = color;
				else colors[i] = color2;
			}
		}
		public static void FillMultiDimArray( this object[,] multiDimArray, object[] array )
		{
			int k=0;
			for( int i=0; i<multiDimArray.GetLength(0); i++ )
			{
				for( int j=0; j<multiDimArray.GetLength(1); j++ )
				{
					multiDimArray[i,j] = array[ k++ ];
				}
			}
		}
		#endregion
		
		#region SWITCH
		public static Vector3 SwitchXY( this Vector3 vec )
		{
			return new Vector3( vec.y, vec.x, vec.z );
		}		
		public static Vector3 SwitchYZ( this Vector3 vec )
		{
			return new Vector3( vec.x, vec.z, vec.y );
		}
		public static Vector2 SwitchXY( this Vector2 vec )
		{
			return new Vector2( vec.y, vec.x );
		}		
		public static Vector3 SwitchXY3( this Vector3 vec )
		{
			return new Vector3( vec.y, vec.x, vec.z );
		}
		#endregion
		
		#region INVERT
		public static Vector3 InvertX( this Vector3 vec )
		{
			return new Vector3( -vec.x, vec.y, vec.z );
		}		
		public static Vector3 InvertXY( this Vector3 vec )
		{
			return new Vector3( -vec.x, -vec.y, vec.z );
		}		
		public static Vector3 InvertY( this Vector3 vec )
		{
			return new Vector3( vec.x, -vec.y, vec.z );
		}		
		public static Vector3 InvertZ( this Vector3 vec )
		{
			return new Vector3( vec.x, vec.y, -vec.z );
		}		
		public static Vector2 InvertX( this Vector2 vec )
		{
			return new Vector2( -vec.x, vec.y );
		}		
		public static Vector2 InvertXY( this Vector2 vec )
		{
			return new Vector2( -vec.x, -vec.y );
		}		
		public static Vector2 InvertY( this Vector2 vec )
		{
			return new Vector2( vec.x, -vec.y );
		}
		#endregion
						
		#region CLAMP
		public static Vector3 Clamp( this Vector3 values, Vector3 max, Vector3 min = default(Vector3) )
		{
			for( byte i=0; i<3; i++ )
			{
				values[i] = Mathf.Clamp( values[i], min[i], max[i] );
			}
			return values;
		}
        /// <summary>
        /// Clamps the values to the opposite. Ex: If value is higher than -max- then the min will be returned.
        /// </summary>
        public static Vector3 ClampOpposite( this Vector3 values, float max, float min = 0f )
        {
            for( byte i=0; i<3; i++ )
            {
                values[i] = values[i].ClampOpposite( min, max );
            }
            return values;
        }
		public static Vector3 Clamp( this Vector3 values, float max, float min = 0.1f )
		{
			for( byte i=0; i<3; i++ )
			{
				values[i] = Mathf.Clamp( values[i], min, max );
			}
			return values;
		}
		public static Vector2 Clamp2( this Vector2 values, float max, float min = 0.1f )
		{
			for( byte i=0; i<2; i++ )
			{
				values[i] = Mathf.Clamp( values[i], min, max );
			}
			return values;
		}
		/// <summary>
		/// Clamps multiple values (positive, negative). Ex: max = 5, min = 1, _max = -1, _min = -5 : values between 
		/// -1 and 1 will never be returned.
		/// </summary>
		/// <returns>The multiple.</returns>
		/// <param name="values">Values.</param>
		/// <param name="max">Max.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="_max">Negative Maximum.</param>
		/// <param name="_min">Negative Minimum.</param>
		public static Vector3 ClampMultiple( this Vector3 values, Vector3 max, Vector3 min, Vector3 _max, Vector3 _min )
		{
			for( byte i=0; i<3; i++ )
			{
				if( values[i] > 0f )
				{
					values[i] = Mathf.Clamp( values[i], min[i], max[i] );
				}
				else
				{
					values[i] = Mathf.Clamp( values[i], _min[i], _max[i] );
				}
			}
			return values;
		}		
		public static Vector2 Clamp2( this Vector2 values, Vector2 max, Vector2 min = default(Vector2) )
		{
			for( byte i=0; i<2; i++ )
			{
				values[i] = Mathf.Clamp( values[i], min[i], max[i] );
			}
			return values;
		}
		#endregion
		
		#region ZERO
		/// <summary>
		/// This will zero (0) the specified axis and return a new vector.
		/// </summary>
		public static Vector3 Zero( this Vector3 vec, bool x, bool y, bool z )
		{
			return vec.Zero ( new Vector3( x.ToFloat(), y.ToFloat(), z.ToFloat() ) );
		}	
		/// <summary>
		/// This will zero (0) the specified axis and return a new vector.
		/// </summary>
		public static Vector3 Zero( this Vector3 vec, Vector3 axis )
        {
			if( axis.x > 0 )
			{
				vec.x = 0f;
			}
			if( axis.y > 0 )
			{
				vec.y = 0f;
			}
			if( axis.z > 0 )
			{
				vec.z = 0f;
			}
			return vec;
		}
		/// <summary>
		/// This will zero (0) the specified axis and return a new vector.
		/// </summary>
		public static Vector3 Keep( this Vector3 vec, Vector3 axis )
		{
			if( axis.x > 0 )
			{
				vec.x = 0f;
			}
			if( axis.y > 0 )
			{
				vec.y = 0f;
			}
			if( axis.z > 0 )
			{
				vec.z = 0f;
			}
			return vec;
		}
		/// <summary>
		/// This will zero (0) the specified axis and return a new vector.
		/// </summary>
		public static Vector3 ZeroX( this Vector3 vec )
		{
			vec.x = 0f;
			return vec;
		}		
		/// <summary>
		/// This will zero (0) the specified axis and return a new vector.
		/// </summary>
		public static Vector3 ZeroY( this Vector3 vec )
		{
			vec.y = 0f;
			return vec;
		}		
		/// <summary>
		/// This will zero (0) the specified axis and return a new vector.
		/// </summary>
		public static Vector3 ZeroZ( this Vector3 vec )
		{
			vec.z = 0f;
			return vec;
		}
		#endregion
		
		#region MISC
		/// <summary>
		/// Returns true if the any value inside the vector is higher than the specified max or lower than the specified
		/// min.
		/// </summary>
		/// <returns><c>true</c>, if out of range, <c>false</c> otherwise.</returns>
		/// <param name="values">Values.</param>
		/// <param name="max">Max.</param>
		/// <param name="min">Minimum.</param>
		public static bool OutOfRange( this Vector3 values, Vector3 max, Vector3 min = default(Vector3) )
		{
			for( byte i=0; i<3; i++ )
			{
				float oVal = values[i];
				values[i] = Mathf.Clamp( values[i], min[i], max[i] );
				if( oVal != values[i] )
					return true;
			}
			return false;
		}
		public static bool Inside( this Vector2 touch, Vector2 from, Vector2 to )
		{
			if( touch.x >= from.x && touch.x <= to.x  )
				if( touch.y >= from.y && touch.y <= to.y )
					return true;
			return false;
		}		
		public static void Reverse(this Array array)
		{
			System.Array.Reverse( array );
		}		
		public static float Distance( this Vector3 a, Vector3 b )
		{
			return Vector3.Distance( a, b );
		}
		/// <summary>
		/// Distance between two vector with the option to use square root distance
		/// which for performance reasons.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dst">The DST.</param>
		/// <param name="useSqr">if set to <c>true</c> [use SQR].</param>
		/// <returns></returns>
		public static float Distance(this Vector3 src, Vector3 dst, bool useSqr)
		{
			return useSqr ? (src - dst).sqrMagnitude : Vector3.Distance(src, dst);
		}
		public static int IndexOf<T>( this T[] array, T value )
		{
			for( int i=0; i<array.Length; i++ )
			{
				if( Equals( array[i], value ) )
					return i;
			}
			return -1;
		}		
        /// <summary>
        /// Add the specified amount to each axis.
        /// </summary>
        public static Vector3 Add( this Vector3 vec, float amount )
        {
            return new Vector3( vec.x + amount, vec.y + amount, vec.z + amount );
        }
		public static Vector3 AddToAxis( this Vector3 vec, float amount, byte axis = 0 )
		{
			switch( axis )
			{
			case 0:
				return new Vector3( vec.x + amount, vec.y, vec.z );
			case 1:
				return new Vector3( vec.x, vec.y + amount, vec.z );
			case 2:
				return new Vector3( vec.x, vec.y, vec.z + amount );
			}
			return vec;
		}		
		public static Vector3[] AddToAxisMulti( this Vector3[] vecs, float amount, byte axis = 0 )
		{
			for( int i=0; i<vecs.Length; i++ )
			{
				switch( axis )
				{
				case 0:
					vecs[i] = new Vector3( vecs[i].x + amount, vecs[i].y, vecs[i].z );
					break;
				case 1:
					vecs[i] = new Vector3( vecs[i].x, vecs[i].y + amount, vecs[i].z );
					break;
				case 2:
					vecs[i] = new Vector3( vecs[i].x, vecs[i].y, vecs[i].z + amount );
					break;
				}
			}
			return vecs;
		}		
		/// <summary>
		/// Find the specified gameobject in the components array. Returns null if it not found.
		/// </summary>
		/// <param name="comps">Componentss.</param>
		/// <param name="obj">Object.</param>
		public static GameObject Find( this Component[] comps, GameObject obj )
		{
			for( int i=0; i<comps.Length; i++ )
			{
				if( comps[i].name.Equals( obj.name ) )
					return comps[i].gameObject;
			}
			return null;
		}		
		public static Vector3 Abs( this Vector3 vec )
		{
			return new Vector3( Mathf.Abs( vec.x ), Mathf.Abs( vec.y ), Mathf.Abs( vec.z ) );
		}		
		public static Vector3 Abs( this Vector2 vec )
		{
			return new Vector2( Mathf.Abs( vec.x ), Mathf.Abs( vec.y ) );
		}
		/// <summary>
		/// If forwardAxis is default Vector3, the Vector3.forward will be used.
		/// </summary>
		public static float Angle360To( this Vector3 a, Vector3 b, Vector3 forwardAxis )
		{
			if( forwardAxis == default( Vector3 ) )
			{
				forwardAxis = Vector3.forward;
			}
			Vector3 c = a - b;
			if( b.y > a.y )
				c = c.InvertX();
			return Vector3.Angle( forwardAxis, c ) + (( b.y < a.y ) ? 180 : 0);
		}		
		/// <summary>
		/// Forwards axis MUST be Vector3.right.
		/// </summary>
		public static float Angle360To( this Vector2 a, Vector2 b )
		{
			Vector2 c = a - b;
			if( b.y > a.y )
				c = c.InvertX();
			return Vector2.Angle( Vector2.right, c ) + (( b.y < a.y ) ? 180 : 0);
		}
		/// <summary>
		/// Returns a new vector with the specified axis values replaced by the new vector's values.
		/// </summary>
		/// <param name="vec">The original vector.</param>
		/// <param name="newVec">New vector.</param>
		/// <param name="axis">The axis to replace. Ex: (0,1,0) would be the Y axis only.</param>
		public static Vector3 Replace( this Vector3 vec, Vector3 newVec, Vector3 axis )
		{
			if( axis.x > 0 )
			{
				vec.x = newVec.x;
			}
			else if( axis.y > 0 )
			{
				vec.y = newVec.y;
			}
			else if( axis.z > 0 )
			{
				vec.z = newVec.z;
			}
			return vec;
		}
		/// <summary>
		/// Shuffle this list.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void Shuffle<T>( this List<T> list )
		{
            int count = list.Count;
			for( int i = 0; i < count; i++ ) 
            {
				T temp = list[i];
				int randomIndex = UnityEngine.Random.Range(i, count);
				list[i] = list[randomIndex];
				list[randomIndex] = temp;
			}
		}
		public static List<T> ToList<T>( this T[] array )
		{
			return new List<T>( array );
		}
		/// <summary>
		/// Returns the amount of items that are not contained inside the specified list.
		/// </summary>
		public static int CountRemaining<T>( this List<T> list, IList<T> items )
		{
			int c = 0;
			for( int i=0; i<items.Count; i++ )
			{
				if( !list.Contains( items[i] ) )
				{
					c++;
				}
			}
			return c;
		}
		/// <summary>
		/// Returns the length of the list / array without taking in account the null elements
		/// </summary>
		/// <returns>The length of the list without taking into account the null elements.</returns>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static int LengthSkipNulls<T>( this IList<T> list ) where T : UnityEngine.Object
		{
			int count = 0;
			for( int i=0; i<list.Count; i++ )
			{
				if( list[i] != null )
				{
					count++;
				}
			}
			return count;
		}
		/// <summary>
		/// Negates the vector's values
		/// </summary>
		public static Vector3 NegateValues( this Vector3 vec )
		{
			for( int i=0; i<3; i++ )
			{
				if( vec[i] > 0f )
				{
					vec[i] *= -1f;
				}
			}
			return vec;
		}
		/// <summary>
		/// Negates the vector's values
		/// </summary>
		public static Vector2 NegateValues( this Vector2 vec )
		{
			for( int i=0; i<2; i++ )
			{
				if( vec[i] > 0f )
				{
					vec[i] *= -1f;
				}
			}
			return vec;
		}
		/// <summary>
		/// Determines if the vector has an infinity or NaN (Not a Number) value.
		/// </summary>
		public static bool HasInfinityOrNaN( this Vector3 vec )
		{
			for( int i=0; i<3; i++ )
			{
				if( float.IsNaN( vec[i] ) || float.IsInfinity( vec[i] ) )
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Returns a new list of the specified type (T2).
		/// </summary>
		public static List<T2> Convert<T1, T2>( this List<T1> list ) where T2 : T1 where T1 : class
		{
			List<T2> targetList = new List<T2>( list.Count );
			for( int i=0; i<list.Count; i++ )
			{
				targetList.Add( (T2)list[i] );
			}
			return targetList;
			
		}
		/// <summary>
		/// Returns a new Quaternion with the specified axis's (the one with values different from 0) locked, meaning the 
		/// -newQ- values will not replace those axis.
		/// </summary>
		/// <param name="original">Original.</param>
		/// <param name="newQ">Quaternion that will replace the original's values where -axis- equals 0.</param>
		/// <param name="axis">If a value is different from 0 that axis will remain the same.</param>
		public static Quaternion Lock( this Quaternion original, Quaternion newRot, Vector3 axis )
		{
			Vector3 euler = original.eulerAngles;
			for( int i=0; i<3; i++ )
			{
				if( axis[i] == 0 )
				{
					euler[i] = newRot.eulerAngles[i];
				}
			}
			return Quaternion.Euler( euler );
		}
		/// <summary>
		/// Returns a new Quaternion with the specified axis's (the one with values different from 0) locked, meaning the 
		/// -newQ- values will not replace those axis.
		/// </summary>
		/// <param name="original">Original.</param>
		/// <param name="newQ">Quaternion that will replace the original's values where -axis- equals 0.</param>
		/// <param name="axis">If a value is different from 0 that axis will remain the same.</param>
		public static Quaternion Lock( this Quaternion original, Vector3 euler, Vector3 axis )
		{
			return original.Lock( Quaternion.Euler( euler ), axis );
		}
		/// <summary>
		/// Prints each element inside the array separated by a space.
		/// </summary>
		public static void Print<T>( this T[] array )
		{
			string print = "";
			for( int i=0; i<array.Length; i++ )
			{
				T item = array[i];
				print += item+" ";
			}
			Debug.Log( print );
		}
		/// <summary>
		/// Divides every component of this vector by the same component of /divisor/.
		/// </summary>
		public static Vector3 Divide( this Vector3 current, Vector3 divisor )
		{
			return new Vector3( current.x / divisor.x, current.y / divisor.y, current.z / divisor.z );
		}
		/// <summary>
		/// Returns true if this list has a null value; false otherwise. If the list itself is null, it will return false.
		/// </summary>
		public static bool HasNull<T>( this IList<T> list )
		{
			if( list == null )
				return false;
			for( int i=0; i<list.Count; i++ )
			{
				if( list[i] == null )
					return true;
			}
			return false;
		}
		public static void InvokeAll(this DelayedAction[] actions, bool withDelay = true)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				actions[i].Invoke( withDelay );
			}
		}
		/// <summary>
		/// Adds the specified /element/ to this list without exceeding the /countLimit/. If the limit is exceeded the
		/// first added elements will be removed (like a queue).
		/// </summary>
		public static void Add<T>( this List<T> list, T element, int countLimit )
		{			
			if( element == null || list == null )
				return;
			countLimit = Mathf.Clamp( countLimit, 0, int.MaxValue );
			if( list.Count >= countLimit )
			{
				list.RemoveFrom( 0, list.Count + 1 - countLimit, true );
			}
			list.Add( element );
		}
        public static T[] Add<T>( this T[] array, T element )
        {           
            if( element == null || array == null )
                return array;
            T[] newArray = new T[ array.Length + 1 ];
            Array.Copy( array, newArray, array.Length );
            newArray[ newArray.Length - 1 ] = element;
            return newArray;
        }
        /// <summary>
        /// Adds the specified /element/ to this list without exceeding the /countLimit/. If the limit is exceeded the
        /// first added elements will be removed (like a queue).
        /// </summary>
        public static T[] Add<T>( this T[] array, T element, int countLimit )
        {           
            if( element == null || array == null )
                return array;
            countLimit = Mathf.Clamp( countLimit, 0, int.MaxValue );
            T[] newArray = new T[ array.Length + 1 ];
            array.CopyTo( newArray, 0 );
            if( array.Length >= countLimit )
            {
                newArray = newArray.RemoveFrom( 0, array.Length + 1 - countLimit );
            }
            return newArray = array.Add( element );
        }
        public static List<T> ToList<T>( this T[,] matrix ) where T : class
        {
            List<T> list = new List<T>( matrix.Length );
            for( int i=0; i<matrix.GetLength( 0 ); i++ )
            {
                for( int j=0; j<matrix.GetLength( 1 ); j++ )
                {
                    list.Add( matrix[ i, j ] );
                }
            }
            return list;
        }
        public static T[] ToArray<T>( this T[,] matrix ) where T : class
        {
            T[] array = new T[ matrix.Length ];
            int count = 0;
            for( int i=0; i<matrix.GetLength( 0 ); i++ )
            {
                for( int j=0; j<matrix.GetLength( 1 ); j++ )
                {
                    array[ count++ ] = matrix[ i, j ];
                }
            }
            return array;
        }
        public static int Count<T>( this T[] array, bool skipNull = false )
        {
            int count = 0;
            for( int i=0; i<array.Length; i++ )
            {
                if( skipNull && array[i] == null )
                    continue;
                count++;
            }
            return count;
        }
        /// <summary>
        /// Returns a new array without the null elements.
        /// </summary>
        public static int CountNull<T>( this T[] array ) where T : class
        {
            if( array == null )
                return 0;
            int count = 0;
            for( int i=0; i<array.Length; i++ )
            {
                if( array[i] == null )
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// Returns the index of the specified sprite (where the name matches). If / contains/ is true, then instead of 
        /// comparing the sprite's name, the first sprite whose name contains the specified /name/ will be returned. If 
        /// no index matches, then -1 is returned.
        /// </summary>
        public static int IndexOf( this Sprite[] sprites, string name, bool contains = false )
        {
            for( int i=0; i<sprites.Length; i++ )
            {
                if( contains && sprites[i].name.Contains( name ) )
                {
                    return i;
                }
                else if( sprites[i].name.Equals( name ) )
                {
                    return i;
                }
            }
            return -1;
        }
		#endregion

		#region GET
		public static Vector3 GetWorldPoint( this Vector3 screenPos, Camera cam = null )
		{
			if( cam == null )
				cam = Camera.main;
			return cam.ScreenToWorldPoint( screenPos );
		}
		
		public static Vector3 GetViewportPoint( this Vector3 screenPos, Camera cam = null )
		{
			if( cam == null )
				cam = Camera.main;
			return cam.ScreenToViewportPoint( screenPos );
		}
		/// <summary>
		/// Gets the specified aspect ratio, a value always higher than 1. NOTE: x must be lower than y. Eg: 9, 16
		/// </summary>
		/// <returns>The aspect ratio.</returns>
		/// <param name="aspectRatio">Aspect ratio.</param>
		public static float GetAspectRatio( this Vector2 aspectRatio )
		{
			float aspect = 0f;
			if( Device.isVertical )
            {				
				aspect = aspectRatio.y / aspectRatio.x;
			}
			else aspect = aspectRatio.x / aspectRatio.y;
			return aspect;
		}
		/// <summary>
		/// Gets the texture from a .png.
		/// </summary>
		/// <returns>The from texture.</returns>
		/// <param name="tex">Texture as png.</param>
		public static Texture2D GetTextureFrom( this byte[] tex )
		{
			var size = (int)Mathf.Sqrt( tex.Length );
			var t = new Texture2D( size, size );
			t.LoadImage(tex);
			return t;
		}
		/// <summary>
		/// Determines if the vec is inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> 0 1 2 3 in counter clockwise directions (0, bottom Y bound exceeded; 1, right X bound exceeded...), 4 if no bounds were exceeded. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="xOffset">Bounds offset.</param>
		public static byte GetExceededScreenBound( this Vector3 vec, float boundsOffset )
		{
			return vec.GetExceededScreenBound( new Vector2( boundsOffset, boundsOffset ), new Vector2( -boundsOffset, -boundsOffset ) );
		}
		
		/// <summary>
		/// Determines if the vec is inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> 0 1 2 3 in counter clockwise directions (0, bottom Y bound exceeded; 1, right X bound exceeded...), 4 if no bounds were exceeded. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="xOffset">X right & left offset.</param>
		/// <param name="yOffset">Y top & bottom offset.</param>
		public static byte GetExceededScreenBound( this Vector3 vec, float xOffset, float yOffset )
		{
			return vec.GetExceededScreenBound( new Vector2( xOffset, yOffset ), new Vector2( -xOffset, -yOffset ) );
		}
		
		/// <summary>
		/// Determines if the vec is inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> 0 1 2 3 in counter clockwise directions (0, bottom Y bound exceeded; 1, right X bound exceeded...), 4 if no bounds were exceeded. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="positiveBounds">X, Y offsets, values higher than 0 are to the outside of the screen.</param>
		/// <param name="negativeBounds">-X, -Y offsets, values higher than 0 are to the inside of the screen.</param>
		public static byte GetExceededScreenBound( this Vector3 vec, Vector2 positiveOffsets = default( Vector2 ), Vector2 negativeOffsets = default( Vector2 ) )
		{
			var xBounds = Device.xBound;
			var yBounds = Device.yBound;
			var _xBounds = Device._xBound;
			var _yBounds = Device._yBound;
			
			if( vec.x > xBounds + positiveOffsets.x )//right
			{
				return 1;
			}
			else if( vec.x < _xBounds + negativeOffsets.x )//left
			{
				return 3;
			}
			if( vec.y > yBounds + positiveOffsets.x )//top
			{
				return 2;
			}
			else if( vec.y < _yBounds + negativeOffsets.x )//bottom
			{
				return 0;
			}
			
			return 4;
		}		
		/// <summary>
		/// Gets a Vector with a correct value inside screen, if the object is not inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> The proper vector. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="boundsOffset">X offset.</param>
		public static Vector3 GetInsideScreenBounds( this Vector3 vec, float boundsOffset = 0f )
		{			
			return vec.GetInsideScreenBounds( boundsOffset, boundsOffset );
		}		
		/// <summary>
		/// Gets a Vector with a correct value inside screen, if the object is not inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> The proper vector. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="xOffset">X offset.</param>
		/// <param name="yOffset">Y offset.</param>
		public static Vector3 GetInsideScreenBounds( this Vector3 vec, float xOffset = 0f, float yOffset = 0f )
		{			
			return vec.GetInsideScreenBounds( new Vector2( xOffset, yOffset ), new Vector2( -xOffset, -yOffset ) );
		}		
		/// <summary>
		/// Gets a Vector with a correct value inside screen, if the object is not inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> The proper vector. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="positiveBounds">X, Y offsets, values higher than 0 are to the outside of the screen.</param>
		/// <param name="negativeBounds">-X, -Y offsets, values higher than 0 are to the inside of the screen.</param>
		public static Vector3 GetInsideScreenBounds( this Vector3 vec, Vector2 positiveOffsets = default(Vector2), Vector2 negativeOffsets = default(Vector2) )
		{
			var xBounds = Device.xBound;
			var yBounds = Device.yBound;
			var _xBounds = Device._xBound;
			var _yBounds = Device._yBound;
			
			if( vec.x > xBounds + positiveOffsets.x )//right
			{
				vec = new Vector3( xBounds + positiveOffsets.x, vec.y, vec.z );
			}
			else if( vec.x < _xBounds + negativeOffsets.x )//left
			{
				vec = new Vector3( _xBounds + negativeOffsets.x, vec.y, vec.z );
			}
			if( vec.y > yBounds + positiveOffsets.x )//top
			{
				vec = new Vector3( vec.x, yBounds + positiveOffsets.y, vec.z );
			}
			else if( vec.y < _yBounds + negativeOffsets.x )//bottom
			{
				vec = new Vector3( vec.x, _yBounds + negativeOffsets.y, vec.z );
			}
			
			return vec;
		}
        /// <summary>
        /// Returns the gameObjects of this buttons.
        /// </summary>
        public static GameObject[] GetGameObjects( this Component[] comps )
        {
            if( comps == null )
                return null;
            List<GameObject> objs = new List<GameObject>( comps.Length );
            for( int i=0; i<comps.Length; i++ )
            {
                objs.Add( comps[i].gameObject );
            }
            return objs.ToArray();
        }
		/// <summary>
		/// Returns a new list with objects of the 2nd specified type.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="startIndex">Start index.</param>
		/// <typeparam name="T1">The 1st type parameter.</typeparam>
		/// <typeparam name="T2">The 2nd type parameter.</typeparam>
		public static List<T2> GetElements<T1, T2>( this IList<T1> list, int startIndex ) where T2 : T1 where T1 : class
		{
			List<T2> newList = new List<T2>();
			if( list != null )
			{
                startIndex = startIndex.Clamp( 0, list.Count );
                for( int i=startIndex; i<list.Count; i++ )
				{
					if( list[i].GetType() == typeof(T2) )
					{
						newList.Add( (T2)list[i] );
					}
				}
				return newList;
			}
			return null;
		}
		/// <summary>
		/// Returns a new list with objects of the 2nd specified type.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="startIndex">Start index.</param>
		/// <param name="lastIndex">Last index.</param>
		/// <typeparam name="T1">The 1st type parameter.</typeparam>
		/// <typeparam name="T2">The 2nd type parameter.</typeparam>
		public static List<T2> GetElements<T1, T2>( this IList<T1> list, int startIndex, int lastIndex ) where T2 : T1 where T1 : class
		{
			List<T2> newList = new List<T2>();
			if( list != null )
			{
				startIndex = startIndex.Clamp( 0, list.Count );
				lastIndex = lastIndex.Clamp(0, list.Count);
				for( int i=startIndex; i<lastIndex; i++ )
				{
					if( list[i].GetType() == typeof(T2) )
					{
						newList.Add( (T2)list[i] );
					}
				}
				return newList;
			}
			return null;
		}

		/// <summary>
		/// This validates if the list is null or empty.
		/// </summary>
		public static T GetFirstItem<T>( this IList<T> list )
		{
			if( list == null || list.Count == 0 )			
            	return default(T);
			return list[0];
        }
		public static T GetLastItem<T>( this IList<T> list, bool removeFromList = false )
		{
			T item = default(T);
			if( list == null )
			{
				return item;
			}
			if( list.Count > 0 )
			{
				item = list[ list.Count - 1 ];
				if( removeFromList )
					list.RemoveAt( list.Count - 1 );
			}
			return item;
		}
		/// <summary>
		/// Returns the first found object named as the specified, or nul if non matches.
		/// </summary>
		public static UnityEngine.Object GetNamed( this IList<UnityEngine.Object> list, string name, StringComparison culture = StringComparison.CurrentCultureIgnoreCase )
		{
			if( list == null )
				return null;
			for( int i=0; i<list.Count; i++ )
			{
				if( list[i].name.Equals( name, culture ) )
				{
					return list[i];
				}
			}
			return null;
		}
		#endregion

		#region SET
		/// <summary>
		/// Sets the limits. If vec2's values are higher or lower (depending on the maxDif) vec's values will be clamped to
		/// vec += maxdif or vec -= maxDif.
		/// </summary>
		/// <param name="vec">Vec.</param>
		/// <param name="vec2">Vec2.</param>
		/// <param name="maxDif">Max difference.</param>
		public static Vector3 SetLimits(this Vector3 vec, Vector3 vec2, float maxDif )
		{
			for( byte i=0; i<3; i++ )
			{
				float val = vec[i]-vec2[i];
				if( maxDif > Mathf.Abs(val) )
				{
					if( val > 0 )
						vec[i] -= maxDif;
					else vec[i] += maxDif;
				}
			}
			return vec;
		}		
		/// <summary>
		/// Sets the limits. If vec2's values are higher or lower (depending on the maxDif) vec's values will be clamped to
		/// vec += maxdif or vec -= maxDif.
		/// </summary>
		/// <param name="vec">Vec.</param>
		/// <param name="vec2">Vec2.</param>
		/// <param name="maxDif">Max difference.</param>
		public static Vector2 SetLimits(this Vector2 vec, Vector2 vec2, float maxDif )
		{
			for( byte i=0; i<2; i++ )
			{
				float val = vec[i]-vec2[i];
				//float val = Vector2.Distance( vec, vec2 );
				if( maxDif < Mathf.Abs(val) )
				{
					if( val > 0 )
						vec[i] -= maxDif;
					else vec[i] += maxDif;
				}
			}
			return vec;
		}
		#endregion
		
		#region SERIALIZATIONS
		/// <summary>
		/// Serialize the specified array. This function converts an array into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="array">Array.</param>
		public static Vec<float> Serialize(this Vector2 array)
		{
			return new Vec<float>( array.x, array.y );
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <param name="array">The serialized array.</param>
		/// <seealso cref="Serialize"/>
		public static Vector2 Deserialize2( this Vec<float> array )
		{
			return new Vector2( array.x, array.y );
		}
		/// <summary>
		/// Serialize the specified array. This function converts an array into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="array">Array.</param>
		public static Vec<float> Serialize(this Vector3 array)
		{
			return new Vec<float>( array.x, array.y, array.z );
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <param name="array">The serialized array.</param>
		/// <seealso cref="Serialize"/>
		public static Vector3 Deserialize3( this Vec<float> array )
		{
			return new Vector3( array.x, array.y, array.z );
		}
		/// <summary>
		/// Serialize the specified array. This function converts an array into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="array">Array.</param>
		public static Vec<float>[] Serialize(this Vector2[] array)
		{
			Vec<float>[] vec = new Vec<float>[array.Length];
			for( int i=0; i<array.Length; i++ )
			{
				vec[i] = new Vec<float>( array[i].x, array[i].y );
			}
			return vec;
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <param name="array">The serialized array.</param>
		/// <seealso cref="Serialize"/>
		public static Vector2[] Deserialize2( this Vec<float>[] array )
		{
			Vector2[] vec = new Vector2[0]; 
			if( array != null )
			{
				vec = new Vector2[array.Length];
				for( int i=0; i<array.Length; i++ )
				{
					vec[i] = new Vector2( array[i].x, array[i].y );
				}
			}
			return vec;
		}
		/// <summary>
		/// Serialize the specified array. This function converts an array into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="array">Array.</param>
		public static Vec<float>[] Serialize(this Vector3[] array)
		{
			Vec<float>[] vec = new Vec<float>[array.Length];
			for( int i=0; i<array.Length; i++ )
			{
				vec[i] = new Vec<float>( array[i].x, array[i].y, array[i].z );
			}
			return vec;
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <param name="array">The serialized array.</param>
		/// <seealso cref="Serialize"/>
		public static Vector3[] Deserialize3( this Vec<float>[] array )
		{
			Vector3[] vec = new Vector3[0];
			if( array != null )
			{
				vec = new Vector3[array.Length];
				for( int i=0; i<array.Length; i++ )
				{
					vec[i] = new Vector3( array[i].x, array[i].y, array[i].z );
				}
			}
			return vec;
		}
		/// <summary>
		/// Serialize the specified array. This function converts an array into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="color">Array.</param>
		public static Vec<float>[,] Serialize(this Vector2[,] array)
		{
			Vec<float>[,] vec = new Vec<float>[array.GetLength(0), array.GetLength(1)];
			for(int i=0; i<array.GetLength(0); i++)
				for( int j=0; j<array.GetLength(1); j++ )
			{
				vec[i,j] = new Vec<float>( array[i,j].x, array[i,j].y );
			}
			return vec;
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <seealso cref="Serialize"/>
		public static Vector2[,] Deserialize2( this Vec<float>[,] array )
		{
			Vector2[,] vec = new Vector2[0,0];
			if( array != null )
			{
				vec = new Vector2[array.GetLength(0), array.GetLength(1)];
				for( int i=0; i<array.GetLength(0); i++ )
					for( int j=0; j<array.GetLength(1); j++ )
				{
					vec[i,j] = new Vector2( array[i,j].x, array[i,j].y );
				}
			}
			return vec;
		}
		/// <summary>
		/// Serialize the specified array. This function converts an array into a Vec object allowing it to be serialized.
		/// </summary>
		public static Vec<float>[,] Serialize(this Vector3[,] array)
		{
			Vec<float>[,] vec = new Vec<float>[array.GetLength(0), array.GetLength(1)];
			for(int i=0; i<array.GetLength(0); i++)
				for( int j=0; j<array.GetLength(1); j++ )
			{
				vec[i,j] = new Vec<float>( array[i,j].x, array[i,j].y, array[i,j].z );
			}
			return vec;
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <seealso cref="Serialize"/>
		public static Vector3[,] Deserialize3( this Vec<float>[,] array )
		{
			Vector3[,] vec = new Vector3[0,0];
			if( array != null ) 
			{
				vec = new Vector3[array.GetLength(0), array.GetLength(1)];
				for( int i=0; i<array.GetLength(0); i++ )
					for( int j=0; j<array.GetLength(1); j++ )
				{
					vec[i,j] = new Vector3( array[i,j].x, array[i,j].y, array[i,j].z );
				}
			}
			return vec;
		}		
		/// <summary>
		/// Serialize the specified transform. This function converts a transform into a Vec object allowing it to be serialized.
		/// </summary>
		public static Vec<float>[] Serialize(this Transform t)
		{
			Vec<float>[] vec = new Vec<float>[3];
			vec[0] = new Vec<float>( t.position.x, t.position.y, t.position.z );
			vec[1] = new Vec<float>( t.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z );
			vec[2] = new Vec<float>( t.localScale.x, t.localScale.y, t.localScale.z );
			return vec;
		}
		/// <summary>
		/// Deserialize the specified array serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized array.</returns>
		/// <param name="color">The serialized array.</param>
		/// <seealso cref="Serialize"/>
		public static Transform DeserializeT( this Vec<float>[] array )
		{
			var temp = new GameObject();
			Transform t = temp.transform;
			t.position = new Vector3( array[0].x, array[0].y, array[0].z );
			t.eulerAngles = new Vector3( array[1].x, array[1].y, array[1].z );
			t.localScale = new Vector3( array[2].x, array[2].y, array[2].z );
			GameObject.Destroy( temp );
			return t;
		}
		#endregion

		#region PERCENTAGES
		/// <summary>
		/// Converts the screen (%) vector into a screen (pixels) vector. NOTE: Percent values go from 0 to 100.
		/// </summary>
		/// <returns>The screen pixels vector.</returns>
		/// <param name="position">Position.</param>
		/// <param name="switchXY">If set to <c>true</c> switch X and Y axis.</param>
		public static Vector3 PercentToPixels3( this Vector3 position, bool switchXY = false )
		{
			var pos = ( switchXY ) ? position.SwitchXY() : position;
			pos.x *= Screen.width * 0.01f;
			pos.y *= Screen.height * 0.01f;
			return pos;
		}		
		/// <summary>
		/// Converts the screen (%) vector into a screen (pixels) vector. NOTE: Percent values go from 0 to 100.
		/// </summary>
		/// <returns>The screen pixels vector.</returns>
		/// <param name="position">Position.</param>
		/// <param name="switchXY">If set to <c>true</c> switch X and Y axis.</param>
		public static Vector2 PercentToPixels( this Vector2 position, bool switchXY = false )
		{
			var pos = ( switchXY ) ? position.SwitchXY() : position;
			pos.x *= Screen.width * 0.01f;
			pos.y *= Screen.height * 0.01f;
			return pos;
		}		
		/// <summary>
		/// Converts the screen (%) vector into a screen (pixels) vector. NOTE: Percent values go from 0 to 1.
		/// </summary>
		/// <returns>The screen pixels vector.</returns>
		/// <param name="position">Position.</param>
		/// <param name="switchXY">If set to <c>true</c> switch X and Y axis.</param>
		public static Vector3 PercentToScreen3( this Vector3 position, bool switchXY = false )
		{
			var pos = ( switchXY ) ? position.SwitchXY() : position;
			pos.x *= Screen.width;
			pos.y *= Screen.height;
			return pos;
		}		
		/// <summary>
		/// Converts the screen (%) vector into a screen (pixels) vector. NOTE: Percent values go from 0 to 1.
		/// </summary>
		/// <returns>The screen pixels vector.</returns>
		/// <param name="position">Position.</param>
		/// <param name="switchXY">If set to <c>true</c> switch X and Y axis.</param>
		public static Vector2 PercentToScreen( this Vector2 position, bool switchXY = false )
		{
			var pos = ( switchXY ) ? position.SwitchXY() : position;
			pos.x *= Screen.width;
			pos.y *= Screen.height;
			return pos;
		}		
		/// <summary>
		/// Converts the screen (%) vector into a world vector. NOTE: Percent values go from 0 to 1.
		/// </summary>
		/// <returns>The screen world vector.</returns>
		/// <param name="position">Position.</param>
		/// <param name="cam">Camera used for screen to world convertion.</param>
		/// <param name="switchXY">If set to <c>true</c> switch X and Y axis.</param>
		public static Vector3 ScreenPercentToWorld3( this Vector3 position, Camera cam = null, bool switchXY = false )
		{
			if( cam == null )
				cam = Camera.main;
			var pos = position.PercentToScreen3( switchXY );
			return  cam.ScreenToWorldPoint( pos );
		}		
		/// <summary>
		/// Converts the screen (%) vector into a world vector. NOTE: Percent values go from 0 to 1.
		/// </summary>
		/// <returns>The screen world vector.</returns>
		/// <param name="position">Position.</param>
		/// <param name="cam">Camera used for screen to world convertion.</param>
		/// <param name="switchXY">If set to <c>true</c> switch X and Y axis.</param>
		public static Vector2 ScreenPercentToWorld( this Vector2 position, Camera cam = null, bool switchXY = false )
		{
			if( cam == null )
				cam = Camera.main;
			var pos = position.PercentToScreen( switchXY );
			return  cam.ScreenToWorldPoint( pos );
		}
		#endregion

		#region IS
		/// <summary>
		/// Determines if the vec is inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> 0 1 2 3 in counter clockwise directions (0, bottom Y bound exceeded; 1, right X bound exceeded...), 4 if no bounds were exceeded. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="xOffset">Bounds offset.</param>
		public static bool IsInsideScreenBounds( this Vector3 vec, float boundsOffset )
		{
			return vec.IsInsideScreenBounds( new Vector2( boundsOffset, boundsOffset ), new Vector2( -boundsOffset, -boundsOffset ) );
		}		
		/// <summary>
		/// Determines if the vec is inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> 0 1 2 3 in counter clockwise directions (0, bottom Y bound exceeded; 1, right X bound exceeded...), 4 if no bounds were exceeded. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="xOffset">X right & left offset.</param>
		/// <param name="yOffset">Y top & bottom offset.</param>
		public static bool IsInsideScreenBounds( this Vector3 vec, float xOffset, float yOffset )
		{
			return vec.IsInsideScreenBounds( new Vector2( xOffset, yOffset ), new Vector2( -xOffset, -yOffset ) );
		}		
		/// <summary>
		/// Determines if the vec is inside screen bounds with the specified offsets.
		/// </summary>
		/// <returns> 0 1 2 3 in counter clockwise directions (0, bottom Y bound exceeded; 1, right X bound exceeded...), 4 if no bounds were exceeded. </returns>
		/// <param name="vec">Vector.</param>
		/// <param name="positiveBounds">X, Y offsets, values higher than 0 are to the outside of the screen.</param>
		/// <param name="negativeBounds">-X, -Y offsets, values higher than 0 are to the inside of the screen.</param>
		public static bool IsInsideScreenBounds( this Vector3 vec, Vector2 positiveOffsets = default( Vector2 ), Vector2 negativeOffsets = default( Vector2 ) )
		{
			return (vec.GetExceededScreenBound() < 4) ? false : true;
		}
		public static bool IsGoingUp( this Vector3 dir )
		{
			dir.Normalize();
			if( dir.y > 0.5f )
				return true;
			return false;
		}		
		public static bool IsGoingDown( this Vector3 dir )
		{
			dir.Normalize();
			if( dir.y < -0.5f )
				return true;
			return false;
		}		
		public static bool IsGoingRight( this Vector3 dir )
		{
			dir.Normalize();
			if( dir.x > 0.5f )
				return true;
			return false;
		}		
		public static bool IsGoingLeft( this Vector3 dir )
		{
			dir.Normalize();
			if( dir.y < -0.5f )
				return true;
			return false;
		}
		public static bool IsEmpty<T>( this IList<T> list )
		{
			if( list == null )
				return true;
			if( list.Count == 0 )
				return true;
			return list.HasNull();
		}
		#endregion

		#region RANDOM
		/// <summary>
		/// Adds random values from 0 to -random- to the specified vector
		/// </summary>
		/// <param name="vec">Vector.</param>
		/// <param name="random">Random values higher than 0.</param>
		public static Vector3 RandomAdd( this Vector3 vec, Vector3 random )
		{
			return new Vector3( vec.x + UnityEngine.Random.Range( 0f, random.x ), vec.y + UnityEngine.Random.Range( 0f, random.y ), vec.z + UnityEngine.Random.Range( 0f, random.z ) );
		}		
		/// <summary>
		/// Substracts random values from 0 to -random- to the specified vector
		/// </summary>
		/// <param name="vec">Vector.</param>
		/// <param name="random">Random values higher or lower than 0, they will always be substracted.</param>
		public static Vector3 RandomSubstract( this Vector3 vec, Vector3 random )
		{
			random = random.Abs();
			return new Vector3( vec.x - UnityEngine.Random.Range( 0f, random.x ), vec.y - UnityEngine.Random.Range( 0f, random.y ), vec.z - UnityEngine.Random.Range( 0f, random.z ) );
		}		
		/// <summary>
		/// Adds random values from 0 to -random- to the specified vector
		/// </summary>
		/// <param name="vec">Vector.</param>
		/// <param name="random">Random values higher than 0.</param>
		public static Vector2 RandomAdd2( this Vector2 vec, Vector2 random )
		{
			return new Vector2( vec.x + UnityEngine.Random.Range( 0f, random.x ), vec.y + UnityEngine.Random.Range( 0f, random.y ) );
		}		
		/// <summary>
		/// Substracts random values from 0 to -random- to the specified vector
		/// </summary>
		/// <param name="vec">Vector.</param>
		/// <param name="random">Random values higher or lower than 0, they will always be substracted.</param>
		public static Vector2 RandomSubstract2( this Vector2 vec, Vector2 random )
		{
			random = random.Abs();
			return new Vector2( vec.x - UnityEngine.Random.Range( 0f, random.x ), vec.y - UnityEngine.Random.Range( 0f, random.y ) );
		}		
		public static Vector3 RandomAdd( this Vector3 vec, Vector3 from, Vector3 to )
		{
			return vec.RandomAdd( new Vector3( to.x, to.y, to.z ) ).RandomSubstract( new Vector3( from.x, from.y, from.z ) );
		}		
		public static Vector2 RandomAdd2( this Vector2 vec, Vector2 from, Vector2 to )
		{
			return vec.RandomAdd2( new Vector2( to.x, to.y ) ).RandomSubstract2( new Vector2( from.x, from.y ) );
		}		
		/// <summary>
		/// Returns the same array, but its values might be negated.
		/// </summary>
		/// <param name="vec">Vec.</param>
		/// <param name="random">If false, the same vector will be returned.</param>
		public static Vector3 RandomSign( this Vector3 vec, bool random = true )
		{
			if( !random )
				return vec;
			return vec * ( UnityEngine.Random.value <= 0.5f ? 1f : -1f );
		}		
		/// <summary>
		/// Returns the same array, but its values might be negated.
		/// </summary>
		/// <param name="vec">Vec.</param>
		/// <param name="random">If false, the same vector will be returned.</param>
		public static Vector2 RandomSign2( this Vector2 vec, bool random = true )
		{
			if( !random )
				return vec;
			return vec * ( UnityEngine.Random.value <= 0.5f ? 1f : -1f );
		}
		/// <summary>
		/// Gets a vector3 with random values between the spcified min and max.
		/// </summary>
		/// <param name="vec">Vector.</param>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Max value.</param>
		/// <param name="excludeAxis">Exclude axis, if any axis value higher than 0 that axis will be excluded.</param>
		public static Vector3 GetRandom( this Vector3 vec, float minVal, float maxVal, Vector3 excludeAxis = default( Vector3 ) )
		{
			for( int i=0; i<3; i++ )
			{
				if( excludeAxis[i] > 0f )
				{
					excludeAxis[i] = 0f;
				}
				else excludeAxis[i] = 1f;
			}
			Vector3 from = Vector3.Scale( new Vector3( minVal, minVal, minVal ), excludeAxis );
			Vector3 to = Vector3.Scale( new Vector3( maxVal, maxVal, maxVal ), excludeAxis );
			return new Vector3( to.x, to.y, to.z ).RandomSubstract( new Vector3( from.x, from.y, from.z ) );
			/*return Vector3.zero.RandomAdd( Vector3.Scale( new Vector3( minVal, minVal, minVal ), excludeAxis ), 
			                               Vector3.Scale( new Vector3( maxVal, maxVal, maxVal ), excludeAxis ) );*/
		}
		/// <summary>
		/// Gets a vector3 with random values between the spcified min and max.
		/// </summary>
		/// <param name="vec">Vector.</param>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Max value.</param>
		/// <param name="excludeAxis">Exclude axis, if any axis value higher than 0 that axis will be excluded.</param>
		public static Vector3 GetRandom( this Vector3 vec, Vector3 minVal, Vector3 maxVal )
		{
			float x = UnityEngine.Random.Range( minVal.x, maxVal.x );
			float y = UnityEngine.Random.Range( minVal.y, maxVal.y );
			float z = UnityEngine.Random.Range( minVal.z, maxVal.z );
			return new Vector3( x, y, z );
		}
		/// <summary>
		/// Gets an index from a random chances array. The random chances array's sum must be equal to 1. 
		/// This is useful for probabilities.
		/// </summary>
		/// <returns>The random chances picked index.</returns>
		/// <param name="pickChances">Pick chances.</param>
		public static int GetIndexFromRandomChances( this float[] pickChances )
		{
			float picked = UnityEngine.Random.value;
			float acum = 0f;
			for( int i=0; i<pickChances.Length; i++ )
			{
				acum += pickChances[i];
				if( picked <= acum )
					return i;
			}
			return 0;
		}
        public static T GetRandom<T>( this T[,] matrix ) where T : class
        {
            int row = UnityEngine.Random.Range( 0, matrix.GetLength( 0 ) );
            int column = UnityEngine.Random.Range( 0, matrix.GetLength( 1 ) );
            return matrix[ row, column ];
        }
		#endregion

		#region REMOVE
		/// <summary>
		/// Returns a new list with no duplicates.
		/// </summary>
		/// <returns>The new list with no duplicates.</returns>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> RemoveDuplicates<T>( this IList<T> list ) where T : UnityEngine.Object
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			List<T> newList = new List<T>();
			newList.AddRange( list.Distinct() );
			return newList;
		}
		/// <summary>
		/// Returns a new list without the -objs- list contained elements.
		/// </summary>
		/// <returns>The new list without the elements from the -objsToRemove- list that are in the speified list.</returns>
		/// <param name="list">List.</param>
		/// <param name="objsToRemove">The name of the elements that must be removed.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> RemoveNamed<T>( this IList<T> list, IList<string> objsToRemove, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase ) where T : UnityEngine.Object
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			List<T> newList = new List<T>(list);
			for( int i=0; i<objsToRemove.Count; i++ )
			{
				for( int j=0; j<newList.Count; j++ )
				{
					if( newList[j].name.Equals( objsToRemove[i], comparison ) )
					{
						newList.RemoveAt( j );
					}
				}
			}
			return newList;
		}
        /// <summary>
        /// Returns a new array without the /objToRemove/ element.
        /// </summary>
        /// <returns>The new list without the elements from the -objs- list that are in the speified list.</returns>
        /// <param name="list">List.</param>
        /// <param name="objToRemove">The element to remove.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Remove<T>( this T[] array, T objToRemove )
        {
            if( array == null || array.Length == 0 )
                return array;
            T[] newArray = new T[ array.Length - 1 ];
            bool elementRemoved = false;
            for( int i=0; i<newArray.Length; i++ )
            {
                if( elementRemoved || array[i].Equals( objToRemove ) )
                {
                    newArray[i] = array[ i+1 ];
                    elementRemoved = true;
                }
                else newArray[i] = array[i];
            }
            return newArray;
        }
		/// <summary>
		/// Returns a new list without the /objsToRemove/ list contained elements.
		/// </summary>
		/// <returns>The new list without the elements from the -objs- list that are in the speified list.</returns>
		/// <param name="list">List.</param>
		/// <param name="objsToRemove">The elements to check.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> RemoveContained<T>( this List<T> list, IList<T> objsToRemove )
		{
			if( list == null || list.Count == 0 )
				return list;
			List<T> newList = new List<T>(list);
			for( int i=0; i<objsToRemove.Count; i++ )
			{
				if( newList.Contains( objsToRemove[i] ) )
				{
					newList.Remove(objsToRemove[i]);
				}
			}
			return newList;
		}
        /// <summary>
        /// Returns a new list without the /objsToRemove/ list contained elements.
        /// </summary>
        /// <returns>The new list without the elements from the -objs- list that are in the speified list.</returns>
        /// <param name="list">List.</param>
        /// <param name="objsToRemove">The elements to check.</param>
        /// <param name="defaultInstead">If true, the objects to remove from the array will be set to their default (type) instead.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] RemoveContained<T>( this T[] array, T[] objsToRemove, bool defaultInstead = false )
        {
            int length = array.Length;
            if( array == null || length == 0 )
                return array;
            T[] newArray = new T[ length ];
            for( int i=0; i<length; i++ )
            {
                for( int k=0; k<objsToRemove.Length; k++ )
                {
                    if( array[ i ].Equals( objsToRemove[k] ) )
                    {
                        array[i] = default( T );
                        break;
                    }
                }
                newArray[i] = array[i];
            }
            if( defaultInstead )
            {
                return newArray;
            }
            else return newArray.RemoveNull();
        }
        /// <summary>
        /// Returns a new list without the /objsToRemove/ list contained elements.
        /// </summary>
        /// <returns>The new list without the elements from the -objs- list that are in the speified list.</returns>
        /// <param name="list">List.</param>
        /// <param name="objsToRemove">The elements to check.</param>
        /// <param name="defaultInstead">If true, the objects to remove from the array will be set to their default (type) instead.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] RemoveContained<T>( this T[] array, List<T> objsToRemove, bool defaultInstead = false )
        {
            int length = array.Length;
            if( array == null || length == 0 )
                return array;
            T[] newArray = new T[ length ];
            for( int i=0; i<length; i++ )
            {
                for( int k=0; k<objsToRemove.Count; k++ )
                {
                    if( array[ i ].Equals( objsToRemove[k] ) )
                    {
                        array[i] = default( T );
                        break;
                    }
                }
                newArray[i] = array[i];
            }
            if( defaultInstead )
            {
                return newArray;
            }
            else return newArray.RemoveNull();
        }
        /// <summary>
        /// Returns a new array without the null elements.
        /// </summary>
        public static T[] RemoveNull<T>( this T[] array )
        {
            if( array == null )
                return array;
            T[] newArray = new T[ array.Count( true ) ];
            for( int i=0, j=0; i<array.Length; i++ )
            {
                if( array[i] != null )
                {
                    newArray[j] = array[i];
                    j++;
                }
            }
            return newArray;
        }
		/// <summary>
		/// Removes the specified starting subtring from each string and returns a new list.
		/// </summary>
		public static List<string> RemoveSubstringsThatStartWith( this IList<string> list, string startsWith )
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			List<string> newList = new List<string>( list );
			for( int i=0; i<newList.Count; i++ )
			{
				if( newList[i] == null )
					continue;
				if( newList[i].StartsWith( startsWith ) )
				{
					newList[i] = newList[i].Remove( 0, startsWith.Length );
				}
			}
			return newList;
		}
		/// <summary>
		/// Replaces the specified /oldValue/ from each string and returns a new list.
		/// </summary>
		public static List<string> Replace( this IList<string> list, string oldValue, string newValue )
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			List<string> newList = new List<string>( list );
			for( int i=0; i<newList.Count; i++ )
			{
				if( newList[i] != null )
				{
					newList[i] = newList[i].Replace( oldValue, newValue );
				}
			}
			return newList;
		}
		/// <summary>
		/// Returns a new list without the -indexesToRemove- list contained indexes.
		/// </summary>
		/// <returns>The new list without the indexes from the -indexesToRemove- list that are in the speified list.</returns>
		/// <param name="list">List.</param>
		/// <param name="indexesToRemove">The indexes to check.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> RemoveContainedIndexes<T>( this IList<T> list, IList<int> indexesToRemove ) where T : UnityEngine.Object
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			List<T> newList = new List<T>( list );
			for( int i=0; i<indexesToRemove.Count; i++ )
			{
				newList.RemoveAt( indexesToRemove[i] - i );
			}
			return newList;
		}		
		/// <summary>
		/// Returns a new list without the NULL elements.
		/// </summary>
		public static List<T> RemoveNull<T>( this IList<T> list ) where T : UnityEngine.Object
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			List<T> newList = new List<T>( list );
			for( int i=0; i<newList.Count; i++ )
			{
				if( newList[i] == null )
				{
					newList.RemoveAt( i-- );
				}
			}
			return newList;
		}
		/// <summary>
		/// Returns a new list without the elements from the specified /startIndex/ to the end.
		/// </summary>
		/// <param name="affectThisList"> If true, this list will be affected, hence, a new one won't be created </param>
		public static List<T> RemoveFrom<T>( this List<T> list, int startIndex, bool affectThisList = false )
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			return list.RemoveFrom( startIndex, list.Count, affectThisList );
		}
		/// <summary>
		/// Returns a new list without the elements from the specified /startIndex/ to the /endIndex/.
		/// </summary>
		/// <param name="affectThisList"> If true, this list will be affected, hence, a new one won't be created </param>
		public static List<T> RemoveFrom<T>( this List<T> list, int startIndex, int endIndex, bool affectThisList = false  )
		{
			if( list == null || list.Count == 0 )
				return list.ToList();
			if( startIndex >= endIndex )
			{
				Debug.LogWarning ("The startIndex equals or exceed the endIndex");
				return list.ToList();
			}
			IList<T> newList = null;
			if( affectThisList )
				newList = list;
			else newList = new List<T>( list );
			endIndex = Mathf.Clamp( endIndex, startIndex + 1, list.Count );
			for( int i=startIndex; i<endIndex; i++ )
			{
				newList.RemoveAt( i );
			}
			return newList.ToList();
		}
        /// <summary>
        /// Returns a new array without the elements from the specified /startIndex/ to the /endIndex/.
        /// </summary>
        /// <param name="affectThisList"> If true, this array will be affected, hence, a new one won't be created </param>
        public static T[] RemoveFrom<T>( this T[] array, int startIndex, int endIndex  )
        {
            if( array == null || array.Length == 0 )
                return array;
            if( startIndex >= endIndex )
            {
                Debug.LogWarning ("The startIndex equals or exceed the endIndex");
                return array;
            }
            T[] newArray = null;
            endIndex = Mathf.Clamp( endIndex, startIndex + 1, array.Length );
            newArray = new T[ array.Length - ( endIndex - startIndex ) ];
            for( int i=startIndex; i<endIndex; i++ )
            {
                newArray = newArray.RemoveAt( i );
            }
            return newArray;
        }
        /// <summary>
        /// Returns a new array without the element from the specified /index/.
        /// </summary>
        public static T[] RemoveAt<T>( this T[] array, int index )
        {
            T[] dest = new T[array.Length - 1];
            if( index > 0 )
                Array.Copy(array, 0, dest, 0, index);

            if( index < array.Length - 1 )
                Array.Copy(array, index + 1, dest, index, array.Length - index - 1);

            return dest;
        }
		#endregion
				
		#region ORDER
		public static int[] OrderByDescending( this IList<int> array )
		{
			List<int> _array = new List<int>( array );
			int temp = 0;
			
			for( int i=0; i<_array.Count; i++ ) 
			{
				for ( int j=0; j<_array.Count - 1; j++ ) 
				{
					if( _array[ j ] < _array[ j+1 ] ) 
					{
						temp = _array[ j+1 ];
						_array[ j+1 ] = _array[ j ];
						_array[ j ] = temp;
					}
				}
			}
			return _array.ToArray();
		}
		public static int[] OrderByAscending( this IList<int> array )
		{
			List<int> _array = new List<int>( array );
			int temp = 0;
			
			for( int i=0; i<_array.Count; i++ ) 
			{
				for ( int j=0; j<_array.Count - 1; j++ ) 
				{
					if( _array[ j ] > _array[ j+1 ] ) 
					{
						temp = _array[ j+1 ];
						_array[ j+1 ] = _array[ j ];
						_array[ j ] = temp;
					}
				}
			}
			return _array.ToArray();
		}
		public static string[] OrderByDescending( this IList<string> array )
		{
			List<string> _array = new List<string>( array );
			string temp = "";
			
			for( int i=0; i<_array.Count; i++ ) 
			{
				for ( int j=0; j<_array.Count - 1; j++ ) 
				{
					if( _array[ j ].CompareTo( _array[ j+1 ] ) < 0 ) 
					{
						temp = _array[ j+1 ];
						_array[ j+1 ] = _array[ j ];
						_array[ j ] = temp;
					}
				}
			}
			return _array.ToArray();
		}
		public static string[] OrderByAscending( this IList<string> array )
		{
			List<string> _array = new List<string>( array );
			string temp = "";
			
			for( int i=0; i<_array.Count; i++ ) 
			{
				for ( int j=0; j<_array.Count - 1; j++ ) 
				{
					if( _array[ j ].CompareTo( _array[ j+1 ] ) > 0 ) 
					{
						temp = _array[ j+1 ];
						_array[ j+1 ] = _array[ j ];
						_array[ j ] = temp;
					}
				}
			}
			return _array.ToArray();
		}
		#endregion
				
	}
}