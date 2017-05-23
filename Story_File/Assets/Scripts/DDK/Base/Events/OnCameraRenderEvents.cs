//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;


namespace DDK.Base.Events
{
    /// <summary>
    /// Attach to a Camera to handle On Camera Render events from the inspector.
    /// </summary>
	[RequireComponent( typeof( Camera ) )]
	public class OnCameraRenderEvents : MonoBehaviour 
    {
		public ComposedEvent onPreRender = new ComposedEvent();
		public ComposedEvent onPreCull = new ComposedEvent();
		public ComposedEvent onPostRender = new ComposedEvent();


		public bool IsPreRendered { get; protected set; }
		public bool IsPreCulled { get; protected set; }
		public bool IsPostRendered { get; protected set; }


		// Use this for initialization
		void Start () {} //Allows enbling/disabling this component


		void OnPreRender()
		{
			onPreRender.Invoke();
			IsPreRendered = true;
		}
		void OnPreCull()
		{
			onPreCull.Invoke();
			IsPreCulled = true;
		}
		void OnPostRender()
		{
			onPostRender.Invoke();
			IsPostRendered = true;
		}
	}

}