//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using System.Collections;
using DDK.Base.Extensions;
using System.Reflection;
using Core = DDK.Base.Classes;


namespace DDK 
{	
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxDrawer : PropertyDrawer 
    {		
		HelpBoxAttribute _attribute 
        {			
            get{ return attribute as HelpBoxAttribute; }
		}

        /// <summary>
        /// True if the property is being drawn
        /// </summary>
        protected bool _drawn = true;

        public override float GetPropertyHeight ( SerializedProperty property, GUIContent label ) 
        {
            object source = property.GetParent();
            MethodInfo method = source.GetType().GetMethod( _attribute.methodFieldOrProperty, BindingFlags.Instance | 
                BindingFlags.NonPublic | BindingFlags.Public );

            _drawn = true;
            if( method != null )
            {
                _drawn = (bool) method.Invoke( source, null );
                if( _attribute.reverseCondition )
                    _drawn = !_drawn;
            }
            else
            {
                FieldInfo field = source.GetType().GetField( _attribute.methodFieldOrProperty, BindingFlags.Instance | 
                    BindingFlags.NonPublic | BindingFlags.Public );
                if( field != null && field.FieldType == typeof( bool ) )
                {
                    _drawn = (bool) field.GetValue( source );
                    if( _attribute.reverseCondition )
                        _drawn = !_drawn;
                }
                else
                {
                    PropertyInfo prop = source.GetType().GetProperty( _attribute.methodFieldOrProperty, 
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
                    if( prop != null && prop.PropertyType == typeof( bool ) )
                    {
                        _drawn = (bool) prop.GetValue( source, null );
                        if( _attribute.reverseCondition )
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
		
		
		// Draw the property inside the given rect
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
        {
            if( !_drawn )
                return;

			//position.height = property.stringValue.Length * 0.3f;
			//position.height = 60f;
			//string msg = property.stringValue.AddLineBreaksInSpace( 20 );
			//EditorGUI.HelpBox( position, property.stringValue, MessageType.Info );
			MessageType msgType = MessageType.Info;
			switch( _attribute.messageType )
			{
			case Core.MessageType.Warning:
				msgType = MessageType.Warning;
				break;
			case Core.MessageType.Error:
				msgType = MessageType.Error;
				break;
			case Core.MessageType.None:
				msgType = MessageType.None;
				break;
			}
			EditorGUILayout.HelpBox( property.stringValue, msgType );
			//EditorGUI.DrawRect( new Rect( position.x + 20f, position.y, position.width, position.height ), new Color(0f,0f,0f,0f) );
		}

	}
	
}
