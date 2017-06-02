using UnityEngine;
using UnityEditor;
using System.Collections;
//By: Daniel Soto
//4dsoto@gmail.com



namespace DDK {

	[CustomPropertyDrawer(typeof(LineAttribute))]
	public class LineDrawer : PropertyDrawer {
		
		LineAttribute line {
			get{
				return attribute as LineAttribute;
			}
		}
		
		public override float GetPropertyHeight ( SerializedProperty property, GUIContent content ) 
		{
			return base.GetPropertyHeight ( property, content ) + line.space + line.height;
		}
		
		/*public override float GetHeight () 
	{
		return base.GetHeight () + line.space + line.height;
	}*/
		
		
		
		// Draw the property inside the given rect
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			
			float lineWidth = position.width * line.width;
			float lineHeight = line.height;
			float lineX = ( (position.x + position.width) - lineWidth - ( (position.width - lineWidth) * 0.5f ) );
			float space = (line.space * 0.5f);
			float lineY = position.y + space;
			
			// Draw the actual line
			EditorGUI.DrawRect (new Rect (lineX, lineY, lineWidth, lineHeight), Color.gray);
			//Draw the field
			Rect fieldRect = new Rect( position.x, lineY + lineHeight + space, position.width, position.height );
			EditorGUI.PropertyField( fieldRect, property, label, true );
			
		}
	}

}
