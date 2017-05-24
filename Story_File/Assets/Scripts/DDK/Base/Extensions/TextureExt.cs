//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Classes;


namespace DDK.Base.Extensions 
{
    // <summary>
    /// Texture class extension.
    /// </summary>
	public static class TextureExt 
    {		
		public static Texture2D Clone( this Texture2D tex, bool setOriginalToNull = true )
		{
			Texture2D clone = null;
			
			if( tex != null )
			{
				clone = new Texture2D( tex.width, tex.height, tex.format, tex.HasMipmap() );
				if( tex.HasMipmap() ) 
				{
					clone.mipMapBias = tex.mipMapBias;
				}
				
				clone.SetPixels32( tex.GetPixels32() );
				clone.Apply();
				if( setOriginalToNull )
					tex = null;
			}
			else Debug.Log ( "Can't clone a null texture" );
			return clone;
		}
		
		public static bool HasMipmap( this Texture2D tex )
		{
			return (tex.mipmapCount > 1) ? true : false;
		}
		
		/// <summary>
		/// Determines if is equal to the specified texture. The comparison is done pixel by pixel.
		/// </summary>
		/// <returns><c>true</c> if is equal to the specified texture; otherwise, <c>false</c>.</returns>
		/// <param name="tex">Texture 1.</param>
		/// <param name="texture">Texture 2.</param>
		public static bool IsEqualTo( this Texture2D tex, Texture2D texture )
		{
			if( tex == null || texture == null )
				return false;
			Color[] pxs1 = tex.GetPixels();
			Color[] pxs2 = texture.GetPixels();
			for( int i=0; i<pxs1.Length; i++ )
			{
				if( pxs1[i] != pxs2[i] )
					return false;
			}
			return true;
		}
		
		public static void Scale( this Texture2D tex, int newW, int newH )
		{
			TextureScale.Bilinear( tex, newW, newH );
		}
		/// <summary>
		/// Scale the specified texture the specified amount in percentage (from 0 to 1). NOTE: 1 = actual scale.
		/// </summary>
		/// <param name="tex">Texture.</param>
		/// <param name="percent">Percent from 0 to 1. Higher values will scale it up. NOTE: 1 = actual scale.</param>
		public static void Scale( this Texture2D tex, float percent )
		{
			int newW = (int)(tex.width * percent), 
			newH = (int)(tex.height * percent);
			bool exit1 = false, exit2 = false;
			while(true)
			{
				if( newW % 2 == 0 && !exit1 )
					exit1 = true;
				else newW -= 1;
				if( newH % 2 == 0 && !exit2 )
					exit2 = true;
				else newH -= 1;
				if( exit1 && exit2 ) 
					break;
			}
			TextureScale.Bilinear( tex, newW, newH );
		}
		/// <summary>
		/// Scale the specified texture the specified amount in percentage (from 0 to 1) compared with the original size.
		/// Usefull when using multiple textures with different sizes. NOTE: 1 = actual scale.
		/// </summary>
		/// <returns>The scale from the original size.</returns>
		/// <param name="tex">Texture.</param>
		/// <param name="percent">Percent from 0 to 1. Higher values will scale it up. NOTE: 1 = actual scale.</param>
		/// <param name="originalSize">Original size.</param>
		public static void SetScaleFromOriginalSize( this Texture2D tex, float percent, int originalSize = 64 )
		{
			float newScl = (originalSize / tex.width) * percent;
			tex.Scale( newScl );
		}
		
		public static byte[] EncodeToPNG( this Texture tex )
		{
			return ( (Texture2D)tex ).EncodeToPNG();
		}
		
		public static void FillEmpty( this Texture2D[] emptyArray, int size = 512 )
		{
			if( emptyArray != null )
			{
				for( long i=0; i<emptyArray.Length; i++ )
				{
					emptyArray[i] = new Texture2D(size, size);
				}
			}
		}
		/// <summary>
		/// Fills the specified texture with transparent pixels.
		/// </summary>
		public static void FillWithTransparent( this Texture2D tex )
		{
			if( !tex )
				return;
			Color32[] pixels = tex.GetPixels32();
			for( int i=0; i<pixels.Length; i++ )
			{
				pixels[i].a = 0;
			}
			tex.SetPixels32( pixels );
			tex.Apply ();
		}

		/// <summary>
		/// Returns the amount of textures inside the specified textures list that contain the specified substring in its name.
		/// </summary>
		/// <param name="textures">Textures.</param>
		/// <param name="substring">Substring.</param>
		public static int Count( this IList<Texture2D> textures, string substring )
		{
            if( textures == null )
                return 0;
			int count = 0;
			for( int i=0; i<textures.Count; i++ )
			{
                if( textures[i] == null )
                    continue;
				if( textures[i].name.Contains( substring ) )
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Returns true if any texture inside the specified textures list contains the specified substring in its name.
		/// </summary>
		/// <param name="textures">Textures.</param>
		/// <param name="substring">Substring.</param>
		public static bool Contains( this IList<Texture2D> textures, string substring )
		{
			for( int i=0; i<textures.Count; i++ )
			{
				if( textures[i].name.Contains( substring ) )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a new list of the textures that contain the specified substring in their name.
		/// </summary>
		/// <param name="textures">Textures.</param>
		/// <param name="substring">Substring.</param>
		public static List<Texture2D> GetWhichContain( this IList<Texture2D> textures, string substring )
		{
			List<Texture2D> texs = new List<Texture2D>( textures.Count( substring ) );
			for( int i=0; i<textures.Count; i++ )
			{
				if( textures[i].name.Contains( substring ) )
				{
					texs.Add( textures[i] );
				}
			}
			return texs;
		}

		public static Texture2D DecodeTexture( this byte[] data )
		{
			Texture2D tex = new Texture2D(1,1);
			tex.LoadImage( data );
			return tex;
		}

		public static Texture2D Rotate( this Texture2D tex, float angle)
		{
			//Debug.Log("rotating");
			Texture2D rotImage = new Texture2D(tex.width, tex.height);
			int  x,y;
			float x1, y1, x2,y2;
			
			int w = tex.width;
			int h = tex.height;
			float x0 = R_Rot_x (angle, -w/2.0f, -h/2.0f) + w/2.0f;
			float y0 = R_Rot_y (angle, -w/2.0f, -h/2.0f) + h/2.0f;
			
			float dx_x = R_Rot_x (angle, 1.0f, 0.0f);
			float dx_y = R_Rot_y (angle, 1.0f, 0.0f);
			float dy_x = R_Rot_x (angle, 0.0f, 1.0f);
			float dy_y = R_Rot_y (angle, 0.0f, 1.0f);
			
			
			x1 = x0;
			y1 = y0;
			
			for (x = 0; x < tex.width; x++) {
				x2 = x1;
				y2 = y1;
				for ( y = 0; y < tex.height; y++) {
					//rotImage.SetPixel (x1, y1, Color.clear);          
					
					x2 += dx_x;//rot_x(angle, x1, y1);
					y2 += dx_y;//rot_y(angle, x1, y1);
					rotImage.SetPixel ( (int)Mathf.Floor(x), (int)Mathf.Floor(y), R_GetPixel(tex,x2, y2));
				}
				
				x1 += dy_x;
				y1 += dy_y;
				
			}
			
			rotImage.Apply();
			return rotImage;
		}
		
		private static Color R_GetPixel( Texture2D tex, float x, float y )
		{
			Color pix;
			int x1 = (int) Mathf.Floor(x);
			int y1 = (int) Mathf.Floor(y);
			
			if(x1 > tex.width || x1 < 0 ||
			   y1 > tex.height || y1 < 0) {
				pix = Color.clear;
			} else {
				pix = tex.GetPixel(x1,y1);
			}
			
			return pix;
		}		
		private static float R_Rot_x (float angle, float x, float y) {
			float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
			float sin = Mathf.Sin(angle/180.0f*Mathf.PI);
			return (x * cos + y * (-sin));
		}
		private static float R_Rot_y (float angle, float x, float y) {
			float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
			float sin = Mathf.Sin(angle/180.0f*Mathf.PI);
			return (x * sin + y * cos);
		} 
		
		/*public static void PrintMemoryUsage( this Texture[] tex )
	{
		for( int i=0; i<tex.Length; i++ )
		{
			Debug.Log ( tex[i].EncodeToPNG().Length + "\n" );
		}
	}*/
		
		
	}


}