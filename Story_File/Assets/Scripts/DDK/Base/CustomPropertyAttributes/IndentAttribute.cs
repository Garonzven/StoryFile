//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;



namespace DDK {

	public class IndentAttribute : PropertyAttribute {
		
		public int level = 1;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IndentAttribute"/> class.
		/// </summary>
		/// <param name="level">Indentation level.</param>
		public IndentAttribute ( int level ) {
			
			this.level = level;
		}
	}

}
