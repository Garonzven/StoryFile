//By: Daniel Soto
//4dsoto@gmail.com

#if !UNITY_STANDALONE
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using UnityEngine.Events;


namespace DDK.Base.Classes
{
	/// <summary>
	/// Eases the touch screen keyboard handling
	/// </summary>
	[System.Serializable]
	public class Keyboard 
	{
		public TouchScreenKeyboardType type = TouchScreenKeyboardType.ASCIICapable;
		public bool autocorrect;
		public bool multiline;
		public bool secure;
		public bool alert;
		public string textPlaceholder;
		[Tooltip("This is invoked after the keyboard becomes inactive. NOTE: This is only invoked automatically if using " +
			"ShowTouchScreenKeyboard() or BeginKeyboardControl() functions.")]
		public UnityEvent onBecameInactive = new UnityEvent();


		public static TouchScreenKeyboard instance;


		/// <summary>
		/// Returns the current TouchScreenKeyboard instance, or a new one if there is no instance reference. If a new 
		/// instance is created, the keyboard is automatically shown. If you just want to get the instance, you can do 
		/// it by calling: Keyboard.instance.
		/// </summary>
		/// <param name="overrideInstance">If set to <c>true</c> the current instance will be overriden by a new one.</param>
		public TouchScreenKeyboard GetTouchScreenKeyboard( bool overrideInstance )
		{
			if( instance != null && !overrideInstance )
				return instance;
			return instance = new TouchScreenKeyboard( "", type, autocorrect, multiline, secure, alert, textPlaceholder );
		}
		/// <summary>
		/// Shows the keyboard and waits for it to be hidden.
		/// </summary>
		/// <param name="edit">If not null, the specified Text's text value will be edited.</param>
		/// <param name="realtimeEdit">If set to <c>true</c> and /edit/ is not null, its text value will be updated each frame.</param>
		public IEnumerator ShowTouchScreenKeyboard( Text edit, bool realtimeEdit = false )
		{
			if( instance != null )
			{
				instance.active = true;
			}
			else instance = new TouchScreenKeyboard( "", type, autocorrect, multiline, secure, alert, textPlaceholder );
			yield return BeginKeyboardControl( edit, realtimeEdit ).Start();
		}
		/// <summary>
		/// Handles the current keyboard's instance. This yields until the keyboard is hidden.
		/// </summary>
		/// <param name="edit">If not null, the specified Text's text value will be edited.</param>
		/// <param name="realtimeEdit">If set to <c>true</c> and /edit/ is not null, its text value will be updated each frame.</param>
		public IEnumerator BeginKeyboardControl( Text edit, bool realtimeEdit )
		{
			if( instance == null )
			{
				Debug.LogWarning( "There is no keyboard instance" );
				yield break;
			}
			while( instance.active || !instance.done )
			{
				yield return null;
				if( edit && realtimeEdit )
				{
					edit.text = instance.text;
				}
			}
			if( !instance.wasCanceled && !string.IsNullOrEmpty( instance.text ) )
			{
				if( edit )
				{
					edit.text = instance.text;
				}
			}
			onBecameInactive.Invoke();
			instance = null;//to allow done to be reset
		}
	}
}
#endif