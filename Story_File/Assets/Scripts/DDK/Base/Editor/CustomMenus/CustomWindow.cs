//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEditor;
using DDK.Base.Extensions;

namespace DDK {

	/// <summary>
	/// Allows creating a Custom Editor Window.
	/// </summary>
	public class CustomWindow : EditorWindow {
		
		public CustomWindow window;
		
		
		public delegate void _OnGUI();
		protected _OnGUI onGUI;



		/// <summary>
		/// Initializes and shows a new instance of the <see cref="CustomWindow"/> class. NOTE: CustomWindow must be instantiated
		/// using the ScriptableObject.CreateInstance but this method prevents window from being resizable...
		/// </summary>
		/// <param name="onGUI">The window creation.</param>
		/// <param name="title">Title.</param>
		/// <param name="w">The width.</param>
		/// <param name="h">The height.</param>
		public CustomWindow( _OnGUI onGUI, string title, float w = 200f, float h = 100f ) 
		{
			Init( onGUI, title, w, h );
		}
		/*
		/// <summary>
		/// Creates and shows an instance of the <see cref="CustomWindow"/> class. NOTE: CustomWindow must be instantiated
		/// using the ScriptableObject.CreateInstance or CustomWindow.Create but this methods prevents window from being resizable...
		/// </summary>
		/// <param name="onGUI">The window creation.</param>
		/// <param name="title">Title.</param>
		/// <param name="w">The width.</param>
		/// <param name="h">The height.</param>
		public static CustomWindow Create( _OnGUI onGUI, string title, float w = 200f, float h = 100f ) 
		{
			window = ScriptableObject.CreateInstance<CustomWindow>();
			window.Init( onGUI, title, w, h );
			return window;
		}*/


		
		public void Init( _OnGUI onGUI, string title, float w = 200f, float h = 100f ) 
		{
			this.onGUI = onGUI;
			
			window = (CustomWindow)EditorWindow.GetWindow<CustomWindow>();
			window.titleContent = new GUIContent( title );
			window.SetSize( w, h );
			window.Center();
			window.Show();
		}	
		
		
		void OnGUI () {
			
			if( onGUI != null )
			{
				onGUI();
			}
			/*GUILayout.Label ("Settings", EditorStyles.boldLabel);
		defaultField = EditorGUILayout.TextField ( "Value", defaultField );*/
		}
		
	}

}
