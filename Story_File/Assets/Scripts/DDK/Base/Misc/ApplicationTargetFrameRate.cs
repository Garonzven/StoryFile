using UnityEngine;


namespace DDK.Base.Misc
{
    /// <summary>
    /// Sets the application's target frame rate. It also allows to adjust Vsync count, which must be 0 (disabled) so 
    /// the target frame rate can be set.
    /// </summary>
    public class ApplicationTargetFrameRate : MonoBehaviour 
    {
        public bool editorOnly;
        public bool enableVsyncAtRuntime;
        [ShowIfAttribute( "enableVsyncAtRuntime", 1 )]
        public int vsyncCount = 1;
        [ShowIfAttribute( "enableVsyncAtRuntime", true )]
        public int targetFrameRate = 60;


        void Awake () 
        {
            if( ( !Application.isEditor && editorOnly ) || !enabled )
                return;
            if( enableVsyncAtRuntime )
            {
                QualitySettings.vSyncCount = vsyncCount;
            }
            else
            {
                QualitySettings.vSyncCount = 0;  // VSync must be disabled
                Application.targetFrameRate = targetFrameRate;
            }
        }
        void Start () //Allows enabling/disabling this component
        {

        }
    }
}
