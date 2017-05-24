//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using DDK.Base.Extensions;
using DDK.Base.Statics;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Attach to a canvas to automatically assign its camera
	/// </summary>
	[RequireComponent( typeof( Canvas ) )]
	[ExecuteInEditMode]
	public class CanvasCameraAssigner : MonoBehaviour 
    {
		[Tooltip("If empty, the main camera will be used.")]
		public SearchableCamera canvasCam = new SearchableCamera( "Main Camera" );
        [DisableIfAttribute( "cameraInUiLayerHasPriority" )]
		public bool mainCameraHasPriority = true;
        [DisableIfAttribute( "mainCameraHasPriority" )]
		[DisplayNameAttribute("Camera In UI Layer Has Priority")]
		public bool cameraInUiLayerHasPriority;
        [Tooltip("If the Canvas camera's reference is still null, and this is true, then the search will keep going " +
            "(in Update function) until a Camera is found. NOTE: This is bad for performance")]
        public bool keepSearching;
        #if UNITY_EDITOR
        [HelpBoxAttribute( "keepSearching", MessageType.Warning )]
        public string message = "Important: /keepSearching/ is bad for performance";
        #endif


        private Camera _cam;

		
		// Use this for initialization
        IEnumerator Start ()
		{
			while( Camera.main == null )
				yield return null;//wait for any camera that might be instantiated
			if( !enabled )
                yield break;
            _Assign();
		}

        /// <summary>
        /// Assigns the Canvas's Camera.
        /// </summary>
        void _Assign()
        {
            var canvas = GetComponent<Canvas>();
            _cam = canvasCam.m_object;
            if( !_cam )
            {
                _cam = Camera.main;
            }
            if( !_cam && mainCameraHasPriority && Camera.main != _cam && Camera.main != null )
            {
                _cam = Camera.main;
                canvasCam.m_object = _cam;
                Utilities.Log ("The Main Camera has priority, reassigning the Main Camera to the Canvas", gameObject );
            }
            if( ( !_cam || _cam.gameObject.layer != LayerMask.NameToLayer( "UI" ) ) && cameraInUiLayerHasPriority && 
                ( canvas.worldCamera = Camera.allCameras.GetFirstFromLayer( "UI" )) )
            {
                _cam = canvas.worldCamera;
                canvasCam.m_object = _cam;
                Utilities.Log ("The Camera in UI layer has priority, reassigning the Camera in the UI layer to the Canvas", gameObject);
            }
            canvas.worldCamera = _cam;
        }            
		void Update()
		{
            if( _cam != null || !keepSearching )
				return;
            _Assign ();
		}
	}
}
