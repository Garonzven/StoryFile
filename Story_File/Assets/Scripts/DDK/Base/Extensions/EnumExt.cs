//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Enum class extension.
    /// </summary>
	public static class EnumExt 
    {		
		/*public static bool IsFlagged( this Enum flags, Enum flagType )
	{
		if( ((int)flags & (int)flagType) == (int)flagType )
			return true;
		return false;
	}*/
		/// <summary>
		/// Check to see if a flags enumeration has a specific flag set.
		/// </summary>
		/// <param name="variable">Flags enumeration to check</param>
		/// <param name="value">Flag to check for</param>
		/// <returns></returns>
		public static bool IsFlagged( this Enum variable, Enum value )
		{
			if( variable == null )
				return false;
			
			if( value == null )
				throw new ArgumentNullException("value");
			
			// Not as good as the .NET 4 version of this function, but should be good enough
			if (!Enum.IsDefined(variable.GetType(), value))
			{
				throw new ArgumentException(string.Format(
					"Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
					value.GetType(), variable.GetType()));
			}
			
			ulong num = Convert.ToUInt64(value);
			return ((Convert.ToUInt64(variable) & num) == num);        
		}
		/// <summary>
		/// Check to see if a flags enumeration has a specific flag set.
		/// </summary>
		/// <param name="variable">Flags enumeration to check</param>
		/// <param name="value">Flag to check for</param>
		/// <returns></returns>
		public static bool IsFlagged( this Enum variable, int value )
		{
			if( variable == null )
				return false;

			ulong num = Convert.ToUInt32(value);
			return ((Convert.ToUInt32(variable) & num) == num);        
		}
		/// <summary>
		/// Check to see if a flags enumeration has a specific flag set.
		/// </summary>
		/// <param name="variable">Flags enumeration to check</param>
		/// <param name="value">Flag to check for</param>
		/// <returns></returns>
		public static bool IsFlagged( this Enum variable, long value )
		{
			if( variable == null )
				return false;

			ulong num = Convert.ToUInt64(value);
			return ((Convert.ToUInt64(variable) & num) == num);        
		}
		
		/// <summary>
		/// Sets the flag to the specified /flagged/ value.
		/// </summary>
		/// <param name="variable">The flags enum.</param>
		/// <param name="value">The flag to set.</param>
		/// <param name="flagged">If set to <c>true</c> flagged.</param>
		public static Enum SetFlag( this Enum flags, Enum flag, bool flagged = true )
		{
			if( flags == null )
				return flags;
			
			if (flag == null)
				return flags;

			/*if( flagged )
				return flags |= flag;
			else return flags ^= ( flags & flag );*/

			// Not as good as the .NET 4 version of this function, but should be good enough
			if (!Enum.IsDefined(flags.GetType(), flag))
			{
				return flags;
			}

			ulong _flags = flags.ToInt64();
			ulong _flag = flag.ToInt64();
			if( flagged )       
				flags = (Enum) Enum.ToObject( flags.GetType(), ( _flags | _flag ) ); //( variable.ToInt64() | num ).ToHexString();
			else if( flags.IsFlagged( flag ) )
			{
				flags = (Enum) Enum.ToObject( flags.GetType(), ( _flags ^ ( _flags & _flag ) ) );//variable = (Enum) ( variable.ToInt64() ^ num ); 
			}
			return flags;
		}
		/// <summary>
		/// Sets the flag to the specified /flagged/ value.
		/// </summary>
		/// <param name="variable">The flags enum.</param>
		/// <param name="value">The flag to set.</param>
		/// <param name="flagged">If set to <c>true</c> flagged.</param>
		public static Enum SetFlag( this Enum flags, Int64 flag, bool flagged = true )
		{
			if (flags == null)
				return flags;

			ulong _flags = flags.ToInt64();
			ulong _flag = (ulong) flag;
			if( flagged )       
				flags = (Enum) Enum.ToObject( flags.GetType(), ( _flags | _flag ) );
			else if( flags.IsFlagged( flag ) )
			{
				flags = (Enum) Enum.ToObject( flags.GetType(), ( _flags ^ ( _flags & _flag ) ) );
			}
			return flags;
		}
		/// <summary>
		/// Sets the flag to the specified /flagged/ value.
		/// </summary>
		/// <param name="variable">The flags enum.</param>
		/// <param name="value">The flag to set.</param>
		/// <param name="flagged">If set to <c>true</c> flagged.</param>
		public static Enum SetFlag( this Enum variable, string value, bool flagged = true )
		{
			if (variable == null)
				return variable;
			
			if ( string.IsNullOrEmpty( value ) )
				return variable;

			// Not as good as the .NET 4 version of this function, but should be good enough
			if (!Enum.IsDefined(variable.GetType(), value))
			{
				return variable;
			}
			Enum _value = Enum.Parse( variable.GetType(), value ) as Enum;

			return variable.SetFlag( _value, flagged );
		}
		/// <summary>
		/// Sets the flag to the specified /flagged/ values.
		/// </summary>
		/// <param name="variable">The flags enum.</param>
		/// <param name="value">The flags to set.</param>
		/// <param name="flagged">If set to <c>true</c> flagged.</param>
		public static Enum SetFlags( this Enum variable, IList<string> values, bool flagged = true )
		{
			if( values == null )
				return variable;
			for( int i=0; i<values.Count; i++ )
			{
				variable = variable.SetFlag ( values[i], flagged );
			}
			return variable;
		}
		public static ulong ToInt64( this Enum variable )
		{
			if (variable == null)
				return 0;
			
			return Convert.ToUInt64( variable );		
		}
		public static uint ToInt32( this Enum variable )
		{
			if (variable == null)
				return 0;
			
			return Convert.ToUInt32( variable );		
		}
		public static int ToInt( this Enum variable )
		{
			if (variable == null)
				return 0;
			
			return Convert.ToInt32( variable );		
		}
		public static Array GetValuesAsArray<T>() where T : struct, IConvertible, IFormattable, IComparable
		{
			if( !typeof(T).IsEnum ) 
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			return Enum.GetValues( typeof(T) );
		}
		public static T[] GetFlaggedValuesAsArray<T>( this Enum flags ) where T : struct, IConvertible, IFormattable, IComparable
		{
			if( !typeof(T).IsEnum ) 
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			List<T> list = new List<T>();
			foreach( T e in Enum.GetValues( typeof( T ) )  )
			{
				Enum flag = e as Enum;
				if( flags.IsFlagged( flag ) )
				{
					list.Add( e );
				}
			}
			return list.ToArray();
		}
		public static IEnumerable<T> GetFlaggedValues<T>( this Enum flags ) where T : struct, IConvertible, IFormattable, IComparable
		{
			if( !typeof(T).IsEnum ) 
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			foreach( T e in Enum.GetValues( typeof( T ) ) )
			{
				Enum flag = e as Enum;
				if( flags.IsFlagged( flag ) )
					yield return e;
			}
        }
	}

}