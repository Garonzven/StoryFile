using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReadOnlyAttribute : PropertyAttribute
{
    public int indentLevel;

    public ReadOnlyAttribute() {}
    public ReadOnlyAttribute( int indentationLevel )
    {
        indentLevel = indentationLevel;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    ReadOnlyAttribute readOnly 
    {
        get{ return attribute as ReadOnlyAttribute; }
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.indentLevel += readOnly.indentLevel;
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.indentLevel -= readOnly.indentLevel;
        GUI.enabled = true;
    }
}
#endif
