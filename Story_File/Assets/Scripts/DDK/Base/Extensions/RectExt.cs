//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Rect class extension.
    /// </summary>
	public static class RectExt 
    {				
		public static void DrawRects( this Rect[] rects, Color[] colors, float duration = float.MaxValue )
		{
			for( int i=0; i < rects.Length; i++ )
			{
				rects[i].DrawRect( colors[i], duration );
			}
		}
		
		public static void DrawRect( this Rect rect, Color color, float duration = float.MaxValue )
		{
			var cam = Camera.main;
			Vector3 start0 = cam.ScreenToWorldPoint( new Vector3( rect.x, rect.y, 0f ) );
			Vector3 end0 = cam.ScreenToWorldPoint( new Vector3( rect.x + rect.width, rect.y, 0f ) );
			Vector3 start1 = cam.ScreenToWorldPoint( new Vector3( rect.x, rect.y, 0f ) );
			Vector3 end1 = cam.ScreenToWorldPoint( new Vector3( rect.x, rect.y + rect.height, 0f ) );
			Vector3 start2 = cam.ScreenToWorldPoint( new Vector3( rect.x, rect.y + rect.height, 0f ) );
			Vector3 end2 = cam.ScreenToWorldPoint( new Vector3( rect.x + rect.width, rect.y + rect.height, 0f ) );
			Vector3 start3 = cam.ScreenToWorldPoint( new Vector3( rect.x + rect.width, rect.y, 0f ) );
			Vector3 end3 = cam.ScreenToWorldPoint( new Vector3( rect.x + rect.width, rect.y + rect.height, 0f ) );
			
			Debug.DrawLine( start0, end0, color, duration );
			Debug.DrawLine( start1, end1, color, duration );
			Debug.DrawLine( start2, end2, color, duration );
			Debug.DrawLine( start3, end3, color, duration );
		}
		
		/// <summary>
		/// Converts the screen x & y percent (%) values into screen pixels values. NOTE: Percent values go from 0 to 100.
		/// </summary>
		/// <returns>The screen pixels Rect.</returns>
		/// <param name="pos">Position.</param>
		public static Rect PercentToPixelsPos( this Rect pos )
		{
			pos.x *= Screen.width * 0.01f;
			pos.y *= Screen.height * 0.01f;
			return pos;
		}
		
		
		/// <summary>
		/// Converts the screen x & y percent (%) values into screen pixels values. NOTE: Percent values go from 0 to 100.
		/// </summary>
		/// <returns>The screen pixels Rect.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="image">If not null, the height will be automatically calculated. </param>
		public static Rect PercentToPixels( this Rect rect, Texture2D image = null )
		{
			rect = rect.PercentToPixelsPos();
			rect.width *= Screen.width * 0.01f;
			if( image != null )
				rect.height = rect.width * image.height / image.width;
			else rect.height *= Screen.height * 0.01f;
			return rect;
		}
		
		public static Rect Invert( this Rect rect, bool x = true, bool y = true )
		{
			Rect inverted = rect;
			if( x )
			{
				inverted.x = Screen.width - rect.x - rect.width;
			}
			if( y )
			{
				inverted.y = Screen.height - rect.y - rect.height;
			}
			return inverted;
		}
		
		
	}


}