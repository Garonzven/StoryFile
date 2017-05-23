//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Classes 
{	
    /// <summary>
    /// This allows specifying the parameters for any behaviour that executes a fade in and/or out action. 
    /// </summary>
	[System.Serializable]
	public class FadeSettings
	{
		/// <summary>
		/// The amount of time in which the screen will become /fadeColor/
		/// </summary>
		[Tooltip("The amount of time in which the screen will become /fadeColor/")]
		public float fadeOutTime = 0.5f;//To many script are using it so changing the name may affect lots of objects
		/// <summary>
		/// The amount of time in which the /fadeColor/ will disappear
		/// </summary>
		[Tooltip("The amount of time in which the /fadeColor/ will fade out (disappear)")]
		public float fadeInTime = 0.5f;//To many script are using it so changing the name may affect lots of objects
		public Color fadeColor = Color.black;


		public FadeSettings( float outTime = 0.5f, float inTime = 0.5f )
		{
			fadeOutTime = outTime;
			fadeInTime = inTime;
			fadeColor = Color.black;
		}
		public FadeSettings( Color color, float outTime = 0.5f, float inTime = 0.5f )
		{
			fadeOutTime = outTime;
			fadeInTime = inTime;
			fadeColor = color;
		}
	}	
}