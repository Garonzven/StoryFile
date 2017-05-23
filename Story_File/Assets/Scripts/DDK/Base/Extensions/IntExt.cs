//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DDK.Base.Statics;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Integer class extension.
    /// </summary>
	public static class IntExt 
    {
		public static int Abs( this int value )
		{
			return Mathf.Abs( value );
		}
		public static void Call( this int phoneNumber )
		{
			Application.OpenURL("tel://"+phoneNumber);
		}		
		public static int Clamp( this int value, int min, int max )
		{
			return Mathf.Clamp( value, min, max );
		}
		/// <summary>
		/// Clamps the value to the opposite. Ex: If value is higher than /max/ then the /min/ will be returned.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public static int ClampOpposite( this int value, int min, int max )
		{
			if( min >= max )
			{
				Utilities.LogError( "min can't be higher or equal than the max, returning same value" );
				return value;
			}
			if( value > max )	return min;
			if( value < min )	return max;
			return value;
		}
		/// <summary>
		/// Clamps the value to the opposite and adds the remaining. In other words: If value is higher than /max/ then 
		/// the /min/ plus the remaining will be returned. Eg: if /value/ is 8 where /max/ is 6 and /min/ 0, then 2 will 
		/// be returned. If the difference is bigger than the difference between /max/ - /min/ then an error is printed 
		/// to the console and the same /value/ is returned, this can be avoided by setting /forceExtend/ to true.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		/// <param name="forceExtend">If true, this /value/ can be bigger than the difference between /min/ and /max/ 
		/// meaning it will still be clamped. Eg: if /value/ is 15 where /max/ is 6 and /min/ 0, then 3 will be returned.</param>
		public static int ClampOppositeExtended( this int value, int min, int max, bool forceExtend = false )
		{
			if( min >= max )
			{
				Utilities.LogError( "min can't be higher or equal than the max, returning same value" );
				return value;
			}
			int val = value;
			if( value > max )	val = min + ( value - max );
			if( value < min )	val = max - ( min - value );
			if( val.IsOutOfBounds( min, max ) )
			{
				if( forceExtend )
					return val.ClampOppositeExtended( min, max );
				Utilities.LogError( "The specified value is bigger than the difference between min and max, returning " +
					"the same value. If this was intended, then set parameter /forceExtend/ to true" );
				return value;
			}
			return val;
		}
		/// <summary>
		/// Clamps the value to the opposite. Ex: If value is higher than -max- the the min will be returned.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public static byte ClampOpposite( this byte value, byte min, byte max )
		{
			if( min >= max )
			{
				Utilities.LogError( "min can't be higher or equal than the max, returning same value" );
				return value;
			}
			if( value > max )	return min;
			if( value < min )	return max;
			return value;
		}
		/// <summary>
		/// Returns true if this /value/ is higher than /max/ or lower than /min/
		/// </summary>
		public static bool IsOutOfBounds( this int value, int min, int max )
		{
			if( min >= max )
			{
				Utilities.LogError( "min can't be higher or equal than the max, returning false" );
                return false;
			}
			return value > max || value < min;
		}
		/// <summary>
		/// Check to see if this flags have a specific flag set.
		/// </summary>
		/// <param name="variable">Flags to check</param>
		/// <param name="value">Flag to check for</param>
		/// <returns> True if the specified flag is set</returns>
		public static bool HasFlag( this Int64 variable, Int64 value )
		{			
			return ( variable & value ) == value;        
		}
		/// <summary>
		/// Check to see if this flags have a specific flag set.
		/// </summary>
		/// <param name="variable">Flags to check</param>
		/// <param name="value">Flag to check for</param>
		/// <returns> True if the specified flag is set</returns>
		public static bool HasFlag( this Int32 variable, Int32 value )
		{
			return ( variable & value ) == value;        
		}
		/// <summary>
		/// Sets the flag to the specified /flagged/ value.
		/// </summary>
		/// <param name="variable">The flags.</param>
		/// <param name="value">The flag to set.</param>
		/// <param name="flagged">If set to <c>true</c> flagged.</param>
		public static Int64 SetFlag( this Int64 flags, Int64 flag, bool flagged = true )
		{
			if( flagged )       
				flags |= flag; 
			else if( flags.HasFlag( flag ) ) flags ^= flag; 
			return flags;
		}
		/// <summary>
		/// Sets the flag to the specified /flagged/ value.
		/// </summary>
		/// <param name="variable">The flags.</param>
		/// <param name="value">The flag to set.</param>
		/// <param name="flagged">If set to <c>true</c> flagged.</param>
		public static Int32 SetFlag( this Int32 flags, Int32 flag, bool flagged = true )
		{
			if( flagged )       
				flags |= flag; 
			else if( flags.HasFlag( flag ) ) flags ^= flag; 
			return flags;
		}
		public static string ToHexString( this UInt64 variable )
		{
			byte[] _bytes = BitConverter.GetBytes( variable );
			_bytes.Reverse();
			string temp = BitConverter.ToString( _bytes ).Replace ("-", "" );
			
			return string.Format("0x{0:X}", temp );
		}
		public static string ToHexString( this UInt32 variable )
		{
			byte[] _bytes = BitConverter.GetBytes( variable );
			_bytes.Reverse();
			string temp = BitConverter.ToString( _bytes ).Replace ("-", "" );
			
            return string.Format("0x{0:X}", temp );
        }
        public static int CountDivisions( this int value, int divisor )
        {
            int count = 0;
            for( int i = 1; i<=value; i += divisor )
            {
                count++;
            }
            return count;
        }
        /// <summary>
        /// Sums all the elements inside this array.
        /// </summary>
        public static int SumAll( this IList<int> array )
        {
            int total = 0;
            for( int i=0; i<array.Count; i++ )
            {
                total += array[ i ];
            }
            return total;
        }
		public static IEnumerable<int> MoveTowardsCo( this int from, int target, float duration )
		{
			float time = 0f;
			while( time < duration )
			{
				time += Time.deltaTime;
				yield return Mathf.RoundToInt( Mathf.Lerp( from, target, time / duration ) );
			}
		}
		
	}

}
