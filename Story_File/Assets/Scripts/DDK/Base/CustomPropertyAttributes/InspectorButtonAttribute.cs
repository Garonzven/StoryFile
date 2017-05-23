using System;

using UnityEngine;

/// <summary>
///     This attribute can only be applied to fields because its
///     associated PropertyDrawer only operates on fields (either
///     public or tagged with the [SerializeField] attribute) in
///     the target MonoBehaviour.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
    public static float kDefaultButtonWidth = 80;

    public readonly string MethodName;
	public readonly string enabledChecker;
    public readonly bool FitToScreenWidth = false;

    private float _buttonWidth = kDefaultButtonWidth;

    public float ButtonWidth
    {
        get { return _buttonWidth; }
        set { _buttonWidth = value; }
    }

	/// <param name="enabledStateChecker">Name of a method that should return true/false if the button should be enabled/disabled.</param>
    public InspectorButtonAttribute( string methodName, string enabledStateChecker = "" )
    {
        MethodName = methodName;
		enabledChecker = enabledStateChecker;
    }
	/// <param name="enabledStateChecker">Name of a method that should return true/false if the button should be enabled/disabled.</param>
	public InspectorButtonAttribute( string methodName, float buttonWidth, string enabledStateChecker = "" )
    {
        MethodName = methodName;
        ButtonWidth = buttonWidth;
		enabledChecker = enabledStateChecker;
    }
	/// <param name="enabledStateChecker">Name of a method that should return true/false if the button should be enabled/disabled.</param>
	public InspectorButtonAttribute( string methodName, bool fitToToScreenWidth, string enabledStateChecker = "" )
    {
        MethodName = methodName;
        FitToScreenWidth = fitToToScreenWidth;
		enabledChecker = enabledStateChecker;
    }
}