//Created by Daniel Soto
//4dsoto@gmail.com
//Modified by Germain Ramos
//Germainramosr@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK._3D.Optimization {

	public class CameraFacingBillboard : MonoBehaviour
	{		
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string message = "All billboards (object's holding this script) will look at the Camera tagged as " +
        "specified in the Preferences at menu bar Custom/Preferences/Billboard Manager.. " +
        "The Z axis will be the one pointing towards the camera";
        #endif


        private Renderer _renderer;
        /// <summary>
        /// This is used if the billboards are perpendicular
        /// </summary>
        private GameObject _container;	
        private Vector3 _rotOffset;


        public bool _Visible{ get; private set; }


        public static Camera m_Camera
        {
            get{ return BillboardManager.m_Camera; }
        }
        public static Transform m_CameraTransform
        {
            get{ return BillboardManager.m_CameraTransform; }
        }
        public static bool m_Perpendicular
        {
            get{ return BillboardManager.Instance.perpendicular; }
        }


        private const string CONTAINER = "BB_PERPENDICULAR_CONTAINER_";

		
		void Awake() 
        {						
            if( m_Perpendicular )
			{
				_container = new GameObject();
                _container.name = CONTAINER + transform.gameObject.name;
				_container.transform.position = transform.position;
				transform.parent = _container.transform;
			}

			_renderer = GetComponent<Renderer>();
            _rotOffset = transform.localEulerAngles;
		}
		void OnBecameVisible()
        {           
            _Visible = true;
			Look();

			if ( BillboardManager.Instance != null )
            {
				BillboardManager.AddBillboard( this );
			}
		}
		void OnBecameInvisible()
        {
            _Visible = false;
			//Avoid instantiating BillboardManager when stopping play in editor
			#if UNITY_EDITOR
			if ( Camera.main == null )
				return;
			#endif			
		}
		
		
		/// <summary>
        /// Makes the object (billboard) look at the camera.
        /// </summary>
		public void Look()
		{
			if ( _renderer == null || !_renderer.isVisible )
			{
				return;
			}

			if( m_Perpendicular )
			{
				_container.transform.LookAt( _container.transform.position + m_Camera.transform.rotation * Vector3.back, 
                    m_Camera.transform.rotation * Vector3.up );
			}
            else if( m_CameraTransform )
            {
                transform.LookAt( Vector3.up , m_CameraTransform );
                transform.localEulerAngles += _rotOffset; //FIX (DO NOT TOUCH). Add default rotation                
			}
		}
	}

}
