//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections.Generic;


namespace DDK.Base.Statics 
{
	/// <summary>
	/// Contains multiple functions for percentage related expressions (Useful when working with probabilities).
	/// </summary>	
	public static class Percent 
    {		
		/// <summary>
		/// Could return the number. If not, returns 0.
		/// </summary>
		/// <returns>
		/// The possible number, 0 otherwise.
		/// </returns>
		/// <param name='number'>
		/// Number.
		/// </param>
		/// <param name='percentage'>
		/// Percentage of possible return.
		/// </param>
		public static float GetPossibleNumber( float number, float percentage )
		{
			return Random.Range( 0f, 100f ) < percentage ? number : 0f;
		}		
		/// <summary>
		/// Could return the number.
		/// </summary>
		/// <returns>
		/// The possible number, 0 otherwise.
		/// </returns>
		/// <param name='number'>
		/// Number.
		/// </param>
		/// <param name='percentage'>
		/// Percentage of possible return.
		/// </param>
		public static float GetPossibleNumber( float number, short percentage )
		{
			return Random.Range( 0, 100 ) < percentage ? number : 0f;
		}		
		/// <summary>
		/// Could return the number (50% of chances).
		/// </summary>
		/// <returns>
		/// The possible number, 0 otherwise.
		/// </returns>
		/// <param name='number'>
		/// Number.
		/// </param>
		public static float GetPossibleNumber50( float number )
		{
			return Random.Range( 0, 2 ) == 1 ? number : 0.0f;
		}		
		/// <summary>
		/// Could change the sign of the specified number.
		/// </summary>
		/// <returns>
		/// The number.
		/// </returns>
		/// <param name='number'>
		/// Number.
		/// </param>
		/// <param name='percentage'>
		/// Percentage.
		/// </param>
		public static float ChangeSign( float number, short percentage )
		{
			return Random.Range( 0, 100 ) < percentage ? -number : number;
		}		
		/// <summary>
		/// Could change the sign of the specified number (50% of chances).
		/// </summary>
		/// <returns>
		/// The number.
		/// </returns>
		/// <param name='number'>
		/// Number.
		/// </param>
		public static float ChangeSign50( float number )
		{
			return Random.Range( 0, 2 ) == 1 ? -number : number;
		}		
		/// <summary>
		/// Gets the relative values for objects screen positioning or scaling in %.
		/// </summary>
		/// <returns>The relative values.</returns>
		/// <param name="rows">Rows.</param>
		/// <param name="columns">Columns.</param>
		/// <param name="w">The width that will be used to calculate the relative X values.</param>
		/// <param name="h">The height that will be used to calculate the relative Y value.</param>
		public static Vector2[,] GetRelativeValues( int rows, int columns, int w, int h )
		{
			Vector2[,] relativeVals = new Vector2[ rows, columns ];
			
			for( int i=0; i<rows; i++ )
			{
				for( int j=0; j<columns; j++ )
				{
					relativeVals[i, j].x = ( w / (columns * 2) ) + ( (w / columns) * j );
					relativeVals[i, j].y = ( h / (rows * 2) ) + ( (h / rows) * i );
				}
			}
			return relativeVals;
		}		
		/// <summary>
		/// Gets the mouse position in screen %. y=0, x=0 --> top, left
		/// </summary>
		/// <returns>The mouse position.</returns>
		public static Vector2 GetMousePosition()
		{
			Vector2 mpos = new Vector2( Mathf.Clamp( Input.mousePosition.x, 0, Screen.width ), 
                Mathf.Clamp( Input.mousePosition.y, 0, Screen.height ) );
			return new Vector2( mpos.x * 100 / Screen.width, 100-( mpos.y * 100 / Screen.height ) );
		}		
		/// <summary>
		/// Establishes a 0 to 1 relationship between a max and min value. Example: If a perspective camera is zooming in,
		/// its field of view will decrease; its field of view minimum limit can be taken in account as 100% zoom while its
		/// maximum as 0%. This function automatically asumes that max is higher than min, otherwise maxIsLess should be
		/// set as true.
		/// </summary>
		/// <returns>The values 0 to 1 relationship depending on the actual value (1 = 100%).</returns>
		/// <param name="value">The actual value. Example: If max = 100, min = 0 and value = 50, this function will 
		/// return 0.5f .</param>
		/// <param name="max">Max.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="maxIsLess">If set to <c>true</c> max is less than min.</param>
		public static float GetValues01( float value, float max, float min, bool maxIsLess = false )
		{
			if( !maxIsLess ) 
            {
				float temp = max;
				max = min;
				min = temp;
			}
			float dif = max - min;
			return ( value - min ) / dif;
		}		
	}
}