//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using System.Collections;


namespace DDK 
{	
	[CustomPropertyDrawer(typeof(DisplayNameAttribute))]
	public class DisplayNameDrawer : PropertyDrawer 
    {		
		DisplayNameAttribute displayName 
        {
			get
            {
				return attribute as DisplayNameAttribute;
			}
		}

		public override float GetPropertyHeight ( SerializedProperty property, GUIContent content ) 
		{
            float extraH = 0f;
            if( property.isExpanded )
            {
                extraH = property.GetChildrenPropertiesHeightSum() + displayName.expandedExtraHeight;
            }
            return base.GetPropertyHeight ( property, content ) + extraH;
		}
		
		
		// Draw the property inside the given rect
		public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label ) 
        {
			label.text = displayName.name;
            EditorGUI.indentLevel += displayName.extraIndentLevel;
			EditorGUI.PropertyField( position, property, label, true );
            EditorGUI.indentLevel -= displayName.extraIndentLevel;
		}
	}	
}