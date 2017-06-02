using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DDK.Misc
{
    [ExecuteInEditMode]
    public class SettingsQuality : MonoBehaviour {

        public int defaultQualityLevel = 0;
        [ShowIfAttribute( "_LimitLevel" )]
        public int qualityLevel = -1;
        [ReadOnlyAttribute]
        #if UNITY_EDITOR
        public string[] levels;


        public bool _LimitLevel()
        {
            if( levels != null && levels.Length > 0 )
            {
                qualityLevel = Mathf.Clamp( qualityLevel, -1, levels.Length - 1 );
                defaultQualityLevel = Mathf.Clamp( defaultQualityLevel, 0, levels.Length - 1 );
            }
            return true;
        }
        #endif


        /*[SerializeField]
        [HideInInspector]
        private int _initialLevel;*/
        private const string DEFAULT_QUALITY_LEVEL = "DEFAULT_QUALITY_LEVEL";


        // Use this for initialization
        void Start () {
            #if UNITY_EDITOR
            defaultQualityLevel = EditorPrefs.GetInt( DEFAULT_QUALITY_LEVEL, QualitySettings.GetQualityLevel() );
            #endif
            if( qualityLevel == -1 )
            {
                qualityLevel = defaultQualityLevel;
                _SetDefaultQualityLevel( qualityLevel );
            }
            #if UNITY_EDITOR
            levels = QualitySettings.names;
            #endif
        }
        #if UNITY_EDITOR
        void Update() {
            if( Application.isPlaying || qualityLevel == -1 || qualityLevel == QualitySettings.GetQualityLevel() )
                return;
            QualitySettings.SetQualityLevel( qualityLevel );
            if( defaultQualityLevel != EditorPrefs.GetInt( DEFAULT_QUALITY_LEVEL ) )
            {
                _SetDefaultQualityLevel( defaultQualityLevel );
            }
        }
        #endif

        void OnDisable()
        {
            if( enabled )//Fix this function being invoked when entering play mode and using ExecuteInEditMode
                return;
            QualitySettings.SetQualityLevel( defaultQualityLevel );
            _SetDefaultQualityLevel( defaultQualityLevel );
        }
        void _SetDefaultQualityLevel( int level )
        {
            #if UNITY_EDITOR
            EditorPrefs.SetInt( DEFAULT_QUALITY_LEVEL, level );
            #endif
        }
    }

}