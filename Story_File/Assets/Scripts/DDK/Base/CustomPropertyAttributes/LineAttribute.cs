using UnityEngine;
using System.Collections;
//By: Daniel Soto
//4dsoto@gmail.com



namespace DDK {

	public class LineAttribute : PropertyAttribute {
		
		public float width = 1f;
		public int height = 1;
		public float space = 10f;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LineAttribute"/> class.
		/// </summary>
		/// <param name="width">Width. In % from 0 to 1.</param>
		/// <param name="height">Height. In pixels.</param>
		/// <param name="space">Space. In pixels.</param>
		public LineAttribute ( float width = 1f, int height = 1, float space = 10f ) {
			
			width = Mathf.Clamp01(width);
			this.width = width;
			this.height = height;
			this.space = space;
		}
	}

}
