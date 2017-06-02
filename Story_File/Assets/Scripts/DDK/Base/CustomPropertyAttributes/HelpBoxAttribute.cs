//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;


namespace DDK 
{
	/// <summary>
	/// Apply to a string to make it a help box. Add a Space attribute to the next property.
	/// </summary>
	public class HelpBoxAttribute : PropertyAttribute 
    {
		//public int height;
		public MessageType messageType;
        /// <summary>
        /// The validation function, field or property. Any case must return bool.
        /// </summary>
        public string methodFieldOrProperty = "";
        /// <summary>
        /// If true, the condition's result will be reversed.
        /// </summary>
        public bool reverseCondition;


		/// <summary>
		/// (The variable must be a string) Shows a help box with its string's message in the inspector.
		/// </summary>
		public HelpBoxAttribute ( MessageType messageType = MessageType.Info ) 
		{
			this.messageType = messageType;
		}
		/// <summary>
		/// (The variable must be a string) Shows a help box with its string's message in the inspector.
		/// </summary>
		/// <param name="condition">The name of a function, field or property that must return true if this help box must be shown.</param>
        public HelpBoxAttribute ( string condition, MessageType messageType = MessageType.Info ) : this( messageType )
		{
            methodFieldOrProperty = condition;
		}
        /// <summary>
        /// (The variable must be a string) Shows a help box with its string's message in the inspector.
        /// </summary>
        /// <param name="condition">The name of a function, field or property that must return true if this help box must be shown.</param>
        public HelpBoxAttribute ( string condition, bool reverseCondition, MessageType messageType = MessageType.Info ) 
            : this( condition, messageType )
        {
            this.reverseCondition = reverseCondition;
        }
	}
	
}
