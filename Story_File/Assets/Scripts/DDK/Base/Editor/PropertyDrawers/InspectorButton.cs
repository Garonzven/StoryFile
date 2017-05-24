using System.Reflection;

using UnityEditor;

using UnityEngine;
using DDK;

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer
{
    private MethodInfo _eventMethodInfo = null;
	private MethodInfo enabledChecker = null;

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
		var eventOwnerType = prop.serializedObject.targetObject.GetType();
        var inspectorButtonAttribute = (InspectorButtonAttribute)attribute;

		if( enabledChecker != null )
		{
			object source = prop.GetParent();
			GUI.enabled = (bool) enabledChecker.Invoke( source, null );
		}
		else enabledChecker = eventOwnerType.GetMethod( inspectorButtonAttribute.enabledChecker, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );

        if (inspectorButtonAttribute.FitToScreenWidth)
        {
            inspectorButtonAttribute.ButtonWidth = Screen.width - 30;
        }

        var buttonRect = new Rect(
            position.x + (position.width - inspectorButtonAttribute.ButtonWidth)
            * 0.5f, position.y, inspectorButtonAttribute.ButtonWidth,
            position.height);

        if ( !GUI.Button(buttonRect, label.text) )
        {
			GUI.enabled = true;
            return;
        }

        var eventName = inspectorButtonAttribute.MethodName;

        if ( _eventMethodInfo == null )
        {
            _eventMethodInfo = eventOwnerType.GetMethod(eventName,
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.NonPublic);
        }

        if ( _eventMethodInfo != null )
        {
            _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
        }
        else
        {
            Debug.LogWarning(
                string.Format("InspectorButton: Unable to find method {0} in {1}",
                    eventName, eventOwnerType));
        }

		GUI.enabled = true;
    }
}