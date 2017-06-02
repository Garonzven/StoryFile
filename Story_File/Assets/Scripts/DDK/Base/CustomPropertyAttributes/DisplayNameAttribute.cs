//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK 
{
	/// <summary>
	/// This doesn't work properly on objects that contain a UnityEvent
	/// </summary>
	public class DisplayNameAttribute : PropertyAttribute 
    {		
		public string name;
        public int extraIndentLevel;
        /// <summary>
        /// The extra height, only when expanded.
        /// </summary>
        public float expandedExtraHeight;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IndentAttribute"/> class.
		/// </summary>
		/// <param name="newName">The variable's new name.</param>
		public DisplayNameAttribute ( string newName ) 
        {			
			name = newName;
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentAttribute"/> class.
        /// </summary>
        /// <param name="newName">The variable's new name.</param>
        public DisplayNameAttribute ( string newName, int extraIndentation, float extraHeight = 0f ) 
        {
            name = newName;
            extraIndentLevel = extraIndentation;
            expandedExtraHeight = extraHeight;
        }
	}
	
}
