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
	
	[CustomPropertyDrawer(typeof( ShowIfAttribute ))]
	public class ShowIfDrawer : PropertyDrawer {
		
		ShowIfAttribute _attribute {
			get{
				return attribute as ShowIfAttribute;
			}
 		}
		public override float GetPropertyHeight ( SerializedProperty property, GUIContent label ) 
		{
			object source = property.GetParent();
			MethodInfo method = source.GetType().GetMethod( _attribute._methodFieldOrProperty, BindingFlags.Instance | 
				BindingFlags.NonPublic | BindingFlags.Public );

			_drawn = true;
			if( method != null )
			{
				_drawn = (bool) method.Invoke( source, null );
				if( _attribute._reverseCondition )
					_drawn = !_drawn;
			}
			else
			{
				FieldInfo field = source.GetType().GetField( _attribute._methodFieldOrProperty, BindingFlags.Instance | 
					BindingFlags.NonPublic | BindingFlags.Public );
				if( field != null && field.FieldType == typeof( bool ) )
				{
					_drawn = (bool) field.GetValue( source );
					if( _attribute._reverseCondition )
						_drawn = !_drawn;
				}
				else
				{
					PropertyInfo prop = source.GetType().GetProperty( _attribute._methodFieldOrProperty, 
						BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
					if( prop != null && prop.PropertyType == typeof( bool ) )
					{
						_drawn = (bool) prop.GetValue( source, null );
						if( _attribute._reverseCondition )
							_drawn = !_drawn;
					}
				}
			}

			if( !_drawn )
				return 0f;
			float extraH = 0f;
			if( property.isExpanded )
			{
				extraH = property.GetChildrenPropertiesHeightSum() + 16f;
			}
			/*if( !string.IsNullOrEmpty( _attribute.header ) )
			{
				extraH += 16f;
			}*/
			return base.GetPropertyHeight ( property, label ) + extraH;
		}


		/// <summary>
		/// True if the property is being drawn
		/// </summary>
		protected bool _drawn = true;
		
		
		// Draw the property inside the given rect
		public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label ) {

			GUI.enabled = false;
			EditorGUI.indentLevel += _attribute._extraIndentLevel;
			if( _drawn )
			{
				GUI.enabled = true;
				/*if( !string.IsNullOrEmpty( _attribute.header ) )
				{
					EditorGUI.LabelField( position, _attribute.header, );
					position.position += new Vector2( 0f, 18f );
				}*/
				//DRAW PROP
				if( _attribute.IsItRange( property ) )//IT'S A RANGE ATTRIBUTE
				{
					if( _attribute.ShouldBeIntSlider )
						EditorGUI.Slider( position, property, _attribute.iMin, _attribute.iMax, label );
					else if( _attribute.ShouldBeFloatSlider )
						EditorGUI.Slider( position, property, _attribute.fMin, _attribute.fMax, label );
					else 
					{
						if( property.propertyType == SerializedPropertyType.Float )
							property.floatValue = property.floatValue.Clamp( _attribute.fMin, _attribute.fMax );
						else property.intValue = property.intValue.Clamp( _attribute.iMin, _attribute.iMax );
						EditorGUI.PropertyField( position, property, label, true );
					}
				}
				else EditorGUI.PropertyField( position, property, label, true );
			}
			EditorGUI.indentLevel -= _attribute._extraIndentLevel;
			GUI.enabled = true;
		}
		
	}
	
}
