//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using System.Collections.Generic;

namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Color class extension.
    /// </summary>
	public static class ColorExt 
    {
		#region GET
		/// <summary>
		/// Gets the saturation without afecting the specified color.
		/// <returns>A new color with the desired saturation</returns>
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="saturation">Saturation. A value between 0 and 1. NOTE: 0 = completely white</param>
		public static Color GetSaturation( this Color color, float saturation )
		{
			Color newCol = color;
			for(byte i=0; i<3; i++) newCol[i] = ( 1f - saturation ) + ( color[i] * saturation );
			return newCol;
		}
		/// <summary>
		/// Gets the brightness without afecting the specified color.
		/// <returns>A new color with the desired brightness</returns>
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="saturation">Brightness. A value between 0 and 1. NOTE: 0 = completely dark</param>
		public static Color GetBrightness( this Color color, float brightness )
		{
			Color newCol = color;
			for(byte i=0; i<3; i++) newCol[i] *= brightness;
			return newCol;
		}
		/// <summary>
		/// Returns this string (containing a color) representing a color, if no alpha is specified it will be 255 ( 1 ).
		/// This must be in the following format --> 255 /separator/ 255 /separator/ 255
		/// </summary>
		/// <param name="context"> The context object to reference when printing a wrong format warning in the console </param>
		public static Color GetFromRGBA255( this string rgba, char separator = ':', Object context = null )
		{
			string[] values = rgba.Split( separator );
			if( values != null && values.Length == 3 )
			{
				return ( rgba + ":255" ).GetFromRGBA255( separator, context );
			}
			if( values == null || values.Length != 4 )
			{
				Debug.LogWarning ( "Wrong RGBA format, it must be --> 255"+ separator +"255"+ separator +"255"+ 
				                  separator +"255 or --> 255"+ separator +"255"+ separator +"255", context );
				return Color.white;
			}
			return new Color( values[0].ConvertFrom255(), values[1].ConvertFrom255(), values[2].ConvertFrom255(), values[3].ConvertFrom255() );
		}
		#endregion

		#region SET
		/// <summary>
		/// Sets the saturation in the specified color.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="saturation">Saturation. A value between 0 and 1. NOTE: 0 = completely white</param>
		public static void SetSaturation( this Color color, float saturation )
		{
			for(byte i=0; i<3; i++) color[i] = ( 1f - saturation ) + ( color[i] * saturation );
		}		
		/// <summary>
		/// Sets the brightness in the specified color.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="saturation">Brightness. A value between 0 and 1. NOTE: 0 = completely dark</param>
		public static void SetBrightness( this Color color, float brightness )
		{
			for(byte i=0; i<3; i++) color[i] *= brightness;
		}		
		/// <summary>
		/// Sets the alpha value.
		/// </summary>
		/// <param name="alpha">Alpha from 0 to 1 (automatically clamped).</param>
		public static Color SetAlpha( this Color color, float alpha = 0f )
		{
			return color = new Color( color.r, color.g, color.b, alpha.Clamp01() );
		}
		/// <summary>
		/// Sets the alpha value.
		/// </summary>
		/// <param name="alpha">Alpha from 0 to 1 (automatically clamped).</param>
		public static Color SetAlpha( this GUIText guiText, float alpha = 0f )
		{
			return guiText.color = new Color( guiText.color.r, guiText.color.g, guiText.color.b, alpha.Clamp01() );
		}
        #endregion
        
        #region MISC
        /// <summary>
		/// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
		/// </summary>
		public static string ToHex( this Color32 color )
		{
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
		}
        /// <summary>
        /// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
        /// </summary>
        public static string ToHex( this Color color )
        {
            return ColorUtility.ToHtmlStringRGBA( color );
        }
		public static Color HexToColor( this string hex )
		{
			Color color;
			ColorUtility.TryParseHtmlString( hex, out color );
			return color;
		}
		public static Color32 ConvertTo32( this Color color )
		{
			return new Color32( (byte)( color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), (byte)(color.a * 255) );
		}		
		/// <summary>
		/// Returns this float representing a color value converted into 0-1 format.
		/// </summary>
		public static float ConvertFrom255( this float value )
		{
			return ( value.Clamp( 0f, 255f ) / 255f ).Clamp01();
		}
		/// <summary>
		/// Returns this string (containing a float) representing a color value converted into 0-1 format. If the string
		/// is in wrong format, 1f will be returned.
		/// </summary>
		public static float ConvertFrom255( this string value )
		{
			return ( value.ToFloat( 1 ).Clamp( 0f, 255f ) / 255f ).Clamp01();
		}
		/// <summary>
		/// Returns true if 'col' is close to the 'color', false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if color is close, <c>false</c> otherwise.</returns>
		/// <param name="col">First color.</param>
		/// <param name="color">Color to compare.</param>
		/// <param name="maxDifference">Max difference between colors.</param>
		public static bool CloseTo( this Color col, Color color, float maxDifference = 0.1f )
		{
			if( col.r > color.r-maxDifference && col.r < color.r+maxDifference )
				if( col.g > color.g-maxDifference && col.g < color.g+maxDifference )
					if( col.b > color.b-maxDifference && col.b < color.b+maxDifference )
						return true;
			return false;
		}	
		public static Color Clamp01(this Color color)
		{
			float r, g, b, a;
			r = Mathf.Clamp01(color.r);
			g = Mathf.Clamp01(color.g);
			b = Mathf.Clamp01(color.b);
			a = Mathf.Clamp01(color.a);
			return new Color( r, g, b, a );
		}
		/// <summary>
		/// Serialize the specified color. This function converts a color into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="color">Color.</param>
		public static Vec<float> Serialize(this Color color)
		{
			return new Vec<float>( color.r, color.g, color.b, color.a );
		}
		/// <summary>
		/// Deserialize the specified color serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized color.</returns>
		/// <param name="color">The serialized color.</param>
		/// <seealso cref="Serialize"/>
		public static Color DeserializeColor( this Vec<float> color )
		{
			return new Color( color.x, color.y, color.z, color.w );
		}
		/// <summary>
		/// Serialize the specified color. This function converts a color into a Vec object allowing it to be serialized.
		/// </summary>
		/// <param name="color">Color.</param>
		public static Vec<float>[] Serialize(this Color[] colors)
		{
			Vec<float>[] vec = new Vec<float>[colors.Length];
			for( int i=0; i<vec.Length; i++ )
			{
				vec[i] = new Vec<float>( colors[i].r, colors[i].g, colors[i].b, colors[i].a );
			}
			return vec;
		}
		/// <summary>
		/// Deserialize the specified color serialized as a Vec object.
		/// </summary>
		/// <returns>The deserialized color.</returns>
		/// <param name="color">The serialized color.</param>
		/// <seealso cref="Serialize"/>
		public static Color[] DeserializeColor( this Vec<float>[] colors )
		{
			Color[] vec = (colors != null) ? new Color[colors.Length] : new Color[0];
			for( int i=0; i<vec.Length; i++ )
			{
				vec[i] = new Color( colors[i].x, colors[i].y, colors[i].z, colors[i].w );
			}
			return vec;
		}		
		/// <summary>
		/// Changes the color towards the target.
		/// </summary>
		/// <returns>The new color.</returns>
		/// <param name="color">Color.</param>
		/// <param name="target">Target.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="avoidAlpha">If set to <c>true</c> avoid alpha value.</param>
		public static Color ChangeTowards( this Color color, Color target, float speed = 0.02f, bool avoidAlpha = false )
		{
			return new Color(
				color.r.MoveTowards( target.r, speed ),
				color.g.MoveTowards( target.g, speed ),
				color.b.MoveTowards( target.b, speed ),
				(avoidAlpha) ? color.a : color.a.MoveTowards( target.a, speed ) );
		}
        public static Color Lerp( this Color color, Color target, float t, bool avoidAlpha )
        {
            Color newColor = Color.white;
            if( avoidAlpha )
            {
                float a = color.a;
                newColor = Color.Lerp( color, target, t );
                newColor.a = a;
            }
            else { newColor = Color.Lerp( color, target, t ); }
            return newColor;
        }
		/// <summary>
		/// Changes the color towards the target.
		/// </summary>
		/// <returns> Each color value closer to the target </returns>
		public static IEnumerable<Color> ChangeTowardsCo( this Color color, Color target, float duration, bool avoidAlpha = false )
		{
			/*float _time = Time.time;
			float time = Time.time;
			float t = 0f;
			Color iniColor = color;
			while( _time + duration >= time )
			{
				time = 0.1f + _time;
				t = time - _time;
				yield return new Color(
					iniColor.r.Lerp( target.r, t / duration ),
					iniColor.g.Lerp( target.g, t / duration ),
					iniColor.b.Lerp( target.b, t / duration ),
					(avoidAlpha) ? iniColor.a : iniColor.a.Lerp( target.a, t / duration ) );
			}*/
			float time = 0f;
			Color iniColor = color;
			while( time < duration )
			{
				time += Time.deltaTime;
				float t = time / duration;
				yield return new Color(
					iniColor.r.Lerp( target.r, t ),
					iniColor.g.Lerp( target.g, t ),
					iniColor.b.Lerp( target.b, t ),
					(avoidAlpha) ? iniColor.a : iniColor.a.Lerp( target.a, t ) );
			}
		}
        /// <summary>
        /// Checks if this list contains the specified color.
        /// </summary>
        /// <param name="minDifference">Minimum difference between any of the compared colors channels.</param>
        public static bool Contains( this Color[] list, Color color, float minDifference = 0.02f )
        {
            for( int i=0; i<list.Length; i++ )
            {
                for( int j=0; j<3; j++ )
                {
                    if( list[i][j].NearlyEqual( color[j], minDifference ) )
                        return true;
                }
            }
            return false;
        }
		#endregion		
		
	}
}