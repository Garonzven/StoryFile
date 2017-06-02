//Created by Germain Ramos, modified Daniel Soto
//Germainramosr@gmail.com
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Statics;

namespace DDK._3D.Optimization 
{
    /// <summary>
    /// Allows handling billboards. CustomMenuItems.cs has the Preferences Window, displayed in Custom/Preferences/BillboardManager...
    /// </summary>
    [ExecuteInEditMode]
    public class BillboardManager : MonoBehaviour
    { 
        /// <summary>
        /// Set to false, to stop automatically updating the billboards.
        /// </summary>
        [Tooltip("Set to false, to stop automatically updating the billboards.")]
		public bool m_autoUpdate;
        [Tooltip("The billboards update rate in seconds")]
        [ShowIfAttribute( "m_autoUpdate", 1 )]
        public float updateRate = 60f;
        [Tooltip("The billboards target camera's tag")]
        public string cameraTag = "MainCamera";
        [Tooltip("If true, the billboards will look at the camera with their Z+ axis perpendicular to it.")]
        public bool perpendicular;
        [Tooltip("The max amount of billboards that this instance can handle at any time.")]
        [SerializeField]
        private int maxBillboardCount = 500;
        [ReadOnlyAttribute]
        [SerializeField]
        private int billboardsCount;



		private static BillboardManager _instance;
		public static BillboardManager Instance 
        {
			get 
            {
				if ( _instance == null )
                {
                    _instance = new GameObject( "BillboardsManager" ).AddComponent<BillboardManager>();
                    DontDestroyOnLoad( _instance.gameObject );
				}
				return _instance;
			}
		}
            
        private CameraFacingBillboard[] _billboards;
        private Camera _camera;

        public static Camera m_Camera 
        {
            get
            {
                if( !Instance )
                {
                    Utilities.LogWarning("BillboardManager: There is no instance of this class, returning main camera");
                    return Camera.main;
                }
                if( !Instance._camera )
                {
                    GameObject camObj = GameObject.FindWithTag( Instance.cameraTag );
                    if( camObj )
                    {
                        m_Camera = camObj.GetComponent<Camera>();
                    }
                    if( !Instance._camera )
                    {
                        m_Camera = Camera.main;
                    }                        
                }
                else if( !Instance._camera.gameObject.activeSelf || !Instance._camera.enabled ) //If camera is changed
                {
                    m_Camera = Camera.main;
                    if( !Instance._camera )
                    {
                        m_Camera = Camera.current;
                    }
                }
                return Instance._camera;
            }
            set{ Instance._camera = value; }
        }
        public static Transform m_CameraTransform
        {
            get
            {
                if( !m_Camera )
                    return null;
                return m_Camera.transform;
            }
        }


        /// <summary>
        /// Initial validations.
        /// </summary>
        void Awake()
        {
            #if UNITY_EDITOR
            if( !Application.isPlaying )
            {
                Utilities.LogError( "This shouldn't be added directly, it will be automatically instantiated at runtime" );
                DestroyImmediate( this );
            }
            #endif
            if( _instance != null )
            {
                Utilities.LogError("There must be only one instance of this class, destroying this one", gameObject );
                DestroyImmediate( this );
                return;
            }
            _instance = this;
            _billboards = new CameraFacingBillboard[ maxBillboardCount ];
        }
        /// <summary>
        /// If m_autoUpdate is true, this instance will handle the billboards update (force them to look at the camera) 
        /// depending on the update rate.
        /// </summary>
		void Update() 
        {
			if ( !m_autoUpdate )
            {
				return;
			} 
            if( Time.frameCount % updateRate != 0 )
			{
				return;
			}
			_UpdateBillboards();
		}


        /// <summary>
        /// Adds the specified /billboard/ to the end of the /_billboards/ array and increases the /billboardsCount/.
        /// </summary>
        private void _AddBillboard( CameraFacingBillboard billboard )
        {
            if( billboardsCount == maxBillboardCount )
                return;
            _billboards[ billboardsCount ] = billboard;
            billboardsCount++;
		}
        /// <summary>
        /// Removes the last billboard from the /_billboards/ array and decreases the /billboardsCount/.
        /// </summary>
        private void _RemoveLastBillboard()
        {
            billboardsCount--;
            if( _billboards.Length <= billboardsCount || billboardsCount < 0 )
            {
                billboardsCount++;
                return;
            }
            _billboards[ billboardsCount ] = null;
		}
        /// <summary>
        /// Invokes the Look() function on each CameraFacingBillboard, forcing them to look at the camera.
        /// </summary>
        private void _UpdateBillboards()
        {
            for (int i = 0; i < billboardsCount; i++) 
            {
                if( _billboards[i] == null || !_billboards[i]._Visible )
                    return;
				_billboards[i].Look();
			}
		}


        /// <summary>
        /// Adds the specified /billboard/ to the /_billboards/ array and increases the /billboardsCount/.
        /// </summary>
        public static void AddBillboard( CameraFacingBillboard billboard )
        {
            if( !Instance )
            {
                Utilities.LogWarning("There is no instance of BillboardManager, can't add the specified billboard..", billboard);
                return;
            }
            if( !billboard )
            {
                Utilities.LogWarning("The specified billboard is null, it can't be added", Instance );
                return;
            }
            Instance._AddBillboard( billboard );
            billboard.Look();
        }
        /// <summary>
        /// Removes the last billboard from the /_billboards/ array and decreases the /billboardsCount/.
        /// </summary>
        public static void RemoveLastBillboard()
        {
            if( !Instance )
            {
                Utilities.LogWarning("There is no instance of BillboardManager, can't remove last billboard..");
                return;
            }
            Instance._RemoveLastBillboard();
        }
        /// <summary>
        /// Invokes the Look() function on each CameraFacingBillboard, forcing them to look at the camera.
        /// </summary>
        public static void UpdateBillboards()
        {
            if( !Instance )
            {
                Utilities.LogWarning("There is no instance of BillboardManager, can't update billboards..");
                return;
            }
            Instance._UpdateBillboards();
        }
        /// <summary>
        /// Sets the rate (in seconds) in which the billboards are updated (forced to look at the camera).
        /// </summary>
        public static void SetCurrentUpdateRate( float rate )
        {
            if( !Instance )
            {
                Utilities.LogWarning("There is no instance of BillboardManager, can't set the update rate..");
                return;
            }
            Instance.updateRate = rate;
        }
	}
}
