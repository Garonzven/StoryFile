//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DDK.Base.Statics;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Extension class for floating point classes.
    /// </summary>
	public static class FloatExt 
    {		
		#region GET
		/// <summary>
		/// Gets the decimals from a number.
		/// </summary>
		/// <returns>
		/// The decimals.
		/// </returns>
		/// <param name='number'>
		/// Number.
		/// </param>
		/// <param name='numDecimals'>
		/// Number of decimals to receive.
		/// </param>
		public static int GetDecimals(this float number, int numDecimals)
		{
			float x = Mathf.Pow(10.0f, numDecimals);
			string Number = ""+(int)(x*number);
			return int.Parse(Number.Substring(Number.Length-numDecimals));
		}
		/// <summary>
		/// Returns the sum of every element inside the specified array.
		/// </summary>
		/// <returns>The sum.</returns>
		/// <param name="array">Array.</param>
		public static float GetSum( this IList<float> array )
		{
			float sum = 0f;
			for( int i=0; i<array.Count; i++ )
			{
				sum += array[i];
			}
			return sum;
		}
		/// <summary>
		/// Gets a vector3 with random values between the spcified min and max.
		/// </summary>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Max value.</param>
		/// <param name="excludeAxis">Exclude axis, if any axis value higher than 0 that axis will be excluded.</param>
		public static Vector3 GetRandomVec3( this float minVal, float maxVal, Vector3 excludeAxis = default( Vector3 ) )
		{
			for( int i=0; i<3; i++ )
			{
				if( excludeAxis[i] > 0f )
				{
					excludeAxis[i] = 0f;
				}
				else excludeAxis[i] = 1f;
			}
			return new Vector3().RandomAdd( Vector3.Scale( new Vector3( minVal, minVal, minVal ), excludeAxis ), 
			                               Vector3.Scale( new Vector3( maxVal, maxVal, maxVal ), excludeAxis ) );
		}
		/// <summary>
		/// Gets a vector3 with random values between the spcified min and max.
		/// </summary>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Max value.</param>
		/// <param name="excludeAxis">Exclude axis, if any axis value higher than 0 that axis will be excluded.</param>
		public static Vector2 GetRandomVec2( this float minVal, float maxVal, Vector3 excludeAxis = default( Vector3 ) )
		{
			for( int i=0; i<2; i++ )
			{
				if( excludeAxis[i] > 0f )
				{
					excludeAxis[i] = 0f;
				}
				else excludeAxis[i] = 1f;
			}
			return new Vector2().RandomAdd2( Vector2.Scale( new Vector2( minVal, minVal ), excludeAxis ), 
			                                Vector2.Scale( new Vector2( maxVal, maxVal ), excludeAxis ) );
		}
		/// <summary>
		/// Returns a Vector3 with this value in all its axis.
		/// </summary>
		public static Vector3 GetVector3( this float value )
		{
			return new Vector3( value, value, value );
		}
        /// <summary>
        /// Returns a Vector3 with this value in all its axis.
        /// </summary>
        public static Vector3 GetVector3( this float value, float overrideZ  )
        {
            return new Vector3( value, value, overrideZ );
        }
		/// <summary>
		/// Returns a Vector2 with this value in all its axis.
		/// </summary>
		public static Vector2 GetVector2( this float value )
		{
			return new Vector2( value, value );
		}
		#endregion
		
		#region SET
		/// <summary>
		/// Sets the number decimals.
		/// </summary>
		/// <returns> The number with the specified decimals. </returns>
		/// <param name='number'> The number to be rounded. </param>
		/// <param name='numDecimals'> Number of decimals that the number must have. </param>
		public static float SetNumDecimals(this float number, int numDecimals)
		{
			float x = Mathf.Pow(10.0f, numDecimals);
			return ( (float)( (int)(x*number) ) ) / x;
		}
		#endregion
		
		#region MISC
		/// <summary>
		/// Compares two floats.
		/// </summary>
		/// <returns><c>true</c>, if the specified floats are close, <c>false</c> otherwise.</returns>
		/// <param name="a">The first float.</param>
		/// <param name="b">The second float.</param>
		/// <param name="minDifference">Minimum difference.</param>
		public static bool CloseTo( this float a, float b, float minDifference = 0.002f )
		{
			//Debug.Log( Mathf.Abs(a - b) );
			if( Mathf.Abs(a - b) <= minDifference ) return true;
			else return false;
		}
        /// <summary>
        /// Use CloseTo( float a, float b, float minDifference ) for better performance.
        /// </summary>
        public static bool NearlyEqual( this float a, float b, float epsilon )
        {
            float absA = Mathf.Abs(a);
            float absB = Mathf.Abs(b);
            float diff = Mathf.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            } 
            else if (a == 0 || b == 0 || diff < float.Epsilon) 
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
        public static bool NearlyEqual( this double a, double b, double epsilon )
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            } 
            else if (a == 0 || b == 0 || diff < double.Epsilon) 
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
		public static float MoveTowards( this float current, float target, float speed = 0.05f )
		{
			return Mathf.MoveTowards( current, target, speed );
		}
		/*public static IEnumerable MoveTowardsCo( this float from, float target, float duration )
		{
			float time = 0f;
			float initialValue = from;
			while( time < duration )
			{
				time += Time.deltaTime;
				yield return Mathf.Lerp( initialValue, target, time / duration );
			}
		}*/
		public static float Lerp( this float current, float target, float t = 0.05f )
		{
			return Mathf.Lerp( current, target, t );
		}
		/// <summary>
		/// Each returned value is a float value closer to the target. NOTE: This returns a huge amount of values, break 
		/// from the foreach loop by checking the current returned value and rounding the desired one to the target.
		/// </summary>
		public static IEnumerable MoveFromTo( this float current, float target, float duration )
		{
			float time = Time.deltaTime;
			while( time <= duration )
			{
				current = Mathf.Lerp( current, target, time/duration );
				time += Time.deltaTime;
				yield return current;
			}
			yield return current = target;
		}
		public static int CeilToInt( this float current )
		{
			return Mathf.CeilToInt( current );
		}
		public static float Clamp01( this float value )
		{
			return Mathf.Clamp01( value );
		}
		public static float Clamp( this float value, float min = 0f, float max = 1f )
		{
			return Mathf.Clamp( value, min, max );
		}
        /// <summary>
        /// Clamps the value to the opposite. Ex: If value is higher than -max- then the min will be returned.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static float ClampOpposite( this float value, float min, float max )
        {
			if( min >= max )
			{
				Utilities.LogError( "min can't be higher or equal than the max, returning same value" );
				return value;
			}
            if( value > max )   return min;
            if( value < min )   return max;
            return value;
        }
		/// <summary>
		/// Returns true if this /value/ is higher than /max/ or lower than /min/
		/// </summary>
		public static bool IsOutOfBounds( this float value, float min, float max )
		{
			if( min >= max )
			{
				Utilities.LogError( "min can't be higher or equal than the max, returning false" );
				return false;
			}
			return value > max || value < min;
		}
		public static float Abs( this float value )
		{
			return Mathf.Abs( value );
		}
		/// <summary>
		/// Returns the result of: 1/value.
		/// </summary>
		/// <param name="val">Value.</param>
		public static float Invert( this float val )
		{
			return 1 / val;
		}
		/// <summary>
		/// Returns the same value, but its sign might be reversed.
		/// </summary>
		/// <param name="random">If false, the same value will be returned.</param>
		public static float RandomSign( this float value, bool random = true )
		{
			if( !random )
				return value;
			return value * ( ( UnityEngine.Random.value <= 0.5f ) ? 1f : -1f );
		}
		#endregion
		
	}
}