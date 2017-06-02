//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using DDK.Base.Extensions;
using System.Reflection;
using System;
using System.Linq;
using System.Collections;


namespace DDK {
	
	[CustomPropertyDrawer(typeof( DisableIfAttribute ))]
	public class DisableIfDrawer : PropertyDrawer {
		
		DisableIfAttribute _attribute {
			get{
				return attribute as DisableIfAttribute;
			}
		}
		public override float GetPropertyHeight ( SerializedProperty property, GUIContent label ) 
		{
			object source = property.GetParent();
			MethodInfo method = source.GetType().GetMethod( _attribute._methodFieldOrProperty, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			
			_disabled = true;
			if( method != null )
			{
				_disabled = (bool) method.Invoke( source, null );
				if( _attribute._reverseCondition )
					_disabled = !_disabled;
			}
			else
			{
				FieldInfo field = source.GetType().GetField( _attribute._methodFieldOrProperty, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
				if( field != null && field.FieldType == typeof( bool ) )
				{
					_disabled = (bool) field.GetValue( source );
					if( _attribute._reverseCondition )
						_disabled = !_disabled;
				}
				else
				{
					PropertyInfo prop = source.GetType().GetProperty( _attribute._methodFieldOrProperty, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
					if( prop != null && prop.PropertyType == typeof( bool ) )
					{
						_disabled = (bool) prop.GetValue( source, null );
						if( _attribute._reverseCondition )
							_disabled = !_disabled;
					}
				}
			}

			float extraH = 0f;
			if( property.isExpanded )
			{
				extraH = property.GetChildrenPropertiesHeightSum();
			}
			return base.GetPropertyHeight ( property, label ) + extraH;
		}
		
		
		/// <summary>
		/// True if the property is being drawn
		/// </summary>
		protected bool _disabled = true;
		
		
		// Draw the property inside the given rect
		public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label ) {

			EditorGUI.indentLevel += _attribute._extraIndentLevel;

			if( _disabled )
				GUI.enabled = false;

			//DRAW PROP
			if( _attribute.ShouldBeIntSlider )
				EditorGUI.Slider( position, property, _attribute.iMin, _attribute.iMax, label );
			else if( _attribute.ShouldBeFloatSlider )
				EditorGUI.Slider( position, property, _attribute.fMin, _attribute.fMax, label );
			else if( _attribute.IsItRange( property ) )
			{
				if( property.propertyType == SerializedPropertyType.Float )
					property.floatValue = property.floatValue.Clamp( _attribute.fMin, _attribute.fMax );
				else property.intValue = property.intValue.Clamp( _attribute.iMin, _attribute.iMax );
                EditorGUI.PropertyField( position, property, label, true );
            }
			else EditorGUI.PropertyField( position, property, label, true );

            EditorGUI.indentLevel -= _attribute._extraIndentLevel;
            GUI.enabled = true;
        }
        
    }
    
}