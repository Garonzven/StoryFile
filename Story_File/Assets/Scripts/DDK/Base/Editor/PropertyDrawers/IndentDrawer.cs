//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using System.Collections;



namespace DDK {

	[CustomPropertyDrawer(typeof(IndentAttribute))]
	public class IndentDrawer : PropertyDrawer {
		
		IndentAttribute indent {
			get{
				return attribute as IndentAttribute;
			}
		}

		public override float GetPropertyHeight ( SerializedProperty property, GUIContent content ) 
		{
			if( property.hasVisibleChildren )
			{
				return base.GetPropertyHeight ( property, content ) * property.CountInProperty() + 10f;
			}
			return base.GetPropertyHeight ( property, content );
		}
		
		
		// Draw the property inside the given rect
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			
			EditorGUI.indentLevel += indent.level;
			EditorGUI.PropertyField( position, property, label, true );
			EditorGUI.indentLevel -= indent.level;
		}
	}

}
