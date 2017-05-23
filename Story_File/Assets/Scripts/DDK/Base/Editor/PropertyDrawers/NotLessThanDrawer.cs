using UnityEditor;
using UnityEngine;
using System;
using DDK;
using System.Reflection;


/// <summary>
/// Presents a normal edit box, but bounds it.
/// </summary>
[CustomPropertyDrawer(typeof(NotLessThan))]
public class NotLessThanDrawer : PropertyDrawer
{
    public override void OnGUI ( Rect position, SerializedProperty prop, GUIContent label ) 
    {
        var bound = attribute as NotLessThan;

		#region DANIEL
		//DANIEL. Added to prevent Unity bug that doesn't allow using multiple attributes
		label.tooltip = bound.tooltip;
        if( !string.IsNullOrEmpty( bound._method ) )
        {
            object source = prop.GetParent();
            var method = source.GetType().GetMethod( bound._method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );

            bool _bounded = true;
            if( method != null )
            {
                _bounded = (bool) method.Invoke( source, null );
                if( bound._reverseCondition )
                    _bounded = !_bounded;
            }
            if( !_bounded )
            {
                try
                {
                    if (prop.propertyType == SerializedPropertyType.Integer)
                    {
                        prop.intValue = EditorGUI.IntField( position, label, prop.intValue );
                    }
                    else if (prop.propertyType == SerializedPropertyType.Float)
                    {
                        prop.floatValue = EditorGUI.FloatField( position, label, prop.floatValue );
                    }
                    else
                    {
                        throw new UnityException(
                            "must be int or float property to use with NotMoreThan");
                    }
                }
                catch (UnityException e)
                {
                    throw new UnityException("error on NotMoreThan attribute of property " 
                        + prop.name + "\n" + e.ToString());
                }
                return;
            }
        }
		#endregion
 
        try
        {
            if (prop.propertyType == SerializedPropertyType.Integer)
            {
                prop.intValue = Mathf.Max(EditorGUI.IntField(position, label, prop.intValue), bound.IntBound);
            }
            else if (prop.propertyType == SerializedPropertyType.Float)
            {
                prop.floatValue = Mathf.Max(EditorGUI.FloatField(position, label, prop.floatValue), bound.FloatBound);
            }
            else
            {
                throw new UnityException("must be int or float to use with NotLessThan");
            }
        }
        catch (UnityException e)
        {
            throw new UnityException("error on NotLessThan attribute of property " 
                                     + prop.name + "\n" + e.ToString());
        }
    }
}