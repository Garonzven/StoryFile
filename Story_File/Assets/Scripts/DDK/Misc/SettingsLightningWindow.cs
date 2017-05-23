//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngineInternal;
using UnityEngine.Rendering;
using DDK;
using System;
using System.Reflection;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DDK.Misc {

	/// <summary>
	/// Attach to an object to set the more important fields in the Lightning Window through an object (inspector) which can be made 
	/// a prefab to maintain the same settings throughout multiple projects.
	/// </summary>
	[ExecuteInEditMode]
	public class SettingsLightningWindow : MonoBehaviour {

#if UNITY_EDITOR
		public enum ReflectionResolution { _128=128, _256=256, _512=512, _1024=1024 }


		[Tooltip("If true, the changes will be updated in the Unity Editor")]
		public bool updateChanges = false;
		[Space(10f)]
		public Material skybox;

		[Space(5f)]
		public AmbientMode ambientMode;
		[ShowIfAttribute( "_IsAmbientModeColor", 1 )]
		public Color ambientColor = Color.gray;
		[ShowIfAttribute( "_IsAmbientModeGradient", 1 )]
		public Color skyColor = Color.gray;
		[ShowIfAttribute( "_IsAmbientModeGradient", 1 )]
		public Color equatorColor = Color.gray;
		[ShowIfAttribute( "_IsAmbientModeGradient", 1 )]
		public Color groundColor = Color.gray;
		[Range( 0f, 8f )]
		public float ambientIntensity = 1f;

		[Space(10f)]
		public DefaultReflectionMode reflectionSource;
		[ShowIfAttribute( "_IsReflectionSkybox", 1 )]
		public ReflectionResolution reflectionResolution = ReflectionResolution._128;
		[ShowIfAttribute( "_IsReflectionSkybox", true, 1 )]
		public Cubemap reflectionCubemap;
		[Range( 0f, 1f )]
		public float reflectionIntensity;
		[Range( 1f, 5f )]
		public int reflectionBounces = 1;

        /*[Space(10f)]
        [Header("Precomputed Realtime GI")]
        public float realtimeResolution = 2f;*/
        //public 

        [Space(10f)]
        [Header("General GI")]
        [Range( 0f, 5f )]
        public float indirectIntensity = 1f;
        [Range( 1f, 10f )]
        public float bounceBoost = 1f;

		[Space(10f)]
		public bool fog;
        [ShowIfAttribute( "fog", 1 )]
		public Color fogColor = Color.gray;
        [ShowIfAttribute( "fog", 1 )]
		public FogMode fogMode = FogMode.ExponentialSquared;
		[ShowIfAttribute( "_IsFogNotLinear", 1 )]
		public float fogDensity = 0.01f;
		[ShowIfAttribute( "_IsFogLinear", 1 )]
		public float fogStart = 0f;
		[ShowIfAttribute( "_IsFogLinear", 1 )]
		public float fogEnd = 300f;

		[Space(10f)]
		[Header( "Other Settings" )]
		[Range( 0f, 1f )]
		public float haloStrength = 0.5f;
		[Space(5f)]
		public float flareFadeSpeed = 3f;
		[Range( 0f, 1f )]
		public float flareStrength = 1f;
		
		
		/*protected bool _IsAmbientModeSkybox()
		{
			return ambientMode == AmbientMode.Skybox;
		}*/
		protected bool _IsAmbientModeGradient()
		{
			return ambientMode == AmbientMode.Trilight;
		}
		protected bool _IsAmbientModeColor()
		{
			return ambientMode == AmbientMode.Flat;
		}
		protected bool _IsReflectionSkybox()
		{
			return reflectionSource == DefaultReflectionMode.Skybox;
		}
		protected bool _IsFogLinear()
		{
            if( !fog )
			{
				return false;
			}
			return fogMode == FogMode.Linear;
		}
		protected bool _IsFogNotLinear()
		{
            if( !fog )
			{
				return false;
			}
			return fogMode != FogMode.Linear;
		}

        //private bool _wasRealtimeGiInitialized;
        private bool _wasGeneralGiInitialized;

        /*protected bool _WasGeneralGiInitialized{
            get{
                if( !_wasRealtimeGiInitialized )
                {
                    _wasRealtimeGiInitialized = true;
                    indirectIntensity = ;
                    bounceBoost = Lightmapping.bounceBoost;
                }
                return true;
            }
        }*/
        protected bool _WasGeneralGiInitialized{
            get{
                if( !_wasGeneralGiInitialized )
                {
                    _wasGeneralGiInitialized = true;
                    indirectIntensity = Lightmapping.indirectOutputScale;
                    bounceBoost = Lightmapping.bounceBoost;
                }
                return true;
            }
        }


		// Update is called once per frame
		void Update () {
			
			if( !Application.isPlaying && updateChanges )
			{
				_Update();
			}			
		}
        void Start()
        {
            Update();
        }

		void _Update()
		{
			RenderSettings.skybox = skybox;
			
			RenderSettings.ambientMode = ambientMode;
			if( ambientMode == AmbientMode.Trilight )
			{
				RenderSettings.ambientSkyColor = skyColor;
				RenderSettings.ambientEquatorColor = equatorColor;
                RenderSettings.ambientGroundColor = groundColor;
			}
			else if( ambientMode == AmbientMode.Flat )
			{
				RenderSettings.ambientLight = ambientColor;
			}
			RenderSettings.ambientIntensity = ambientIntensity;
			
			RenderSettings.defaultReflectionMode = reflectionSource;
			if( _IsReflectionSkybox() )
				RenderSettings.defaultReflectionResolution = (int) reflectionResolution;
			else RenderSettings.customReflection = reflectionCubemap;
			RenderSettings.reflectionIntensity = reflectionIntensity;
			RenderSettings.reflectionBounces = reflectionBounces;

            if( _WasGeneralGiInitialized )
            {
                Lightmapping.indirectOutputScale = indirectIntensity;
                Lightmapping.bounceBoost = bounceBoost;
            }
			
			RenderSettings.fog = fog;
			if( fog )
			{
				RenderSettings.fogColor = fogColor;
				RenderSettings.fogMode = fogMode;
				if( _IsFogLinear() )
				{
					RenderSettings.fogStartDistance = fogStart;
					RenderSettings.fogEndDistance = fogEnd;
				}
				else RenderSettings.fogDensity = fogDensity;
			}

			RenderSettings.haloStrength = haloStrength;
			RenderSettings.flareFadeSpeed = flareFadeSpeed;
			RenderSettings.flareStrength = flareStrength;
		}



        public static void SetIndirectResolution( float val )
        {
            SetFloat("m_LightmapEditorSettings.m_Resolution", val);
        }
        public static void SetAmbientOcclusion( float val )
        {
            SetFloat("m_LightmapEditorSettings.m_CompAOExponent", val);
        }
        public static void SetBakedGiEnabled( bool enabled )
        {
            SetBool("m_GISettings.m_EnableBakedLightmaps", enabled);
        }
        public static void SetFinalGatherEnabled( bool enabled )
        {
            SetBool("m_LightmapEditorSettings.m_FinalGather", enabled);
        }
        public static void SetFinalGatherRayCount(int val)
        {
            SetInt("m_LightmapEditorSettings.m_FinalGatherRayCount", val);
        }
        public static void SetPrecomputedRealtimeGIEnabled( bool enabled )
        {
            SetBool("m_GISettings.m_EnableRealtimeLightmaps",enabled);
        }
        public static void SetFloat(string name, float val)
        {
            ChangeProperty(name, property => property.floatValue= val);
        }
        public static void SetInt(string name, int val)
        {
            ChangeProperty(name, property => property.intValue = val);
        }
        public static void SetBool(string name, bool val)
        {
            ChangeProperty(name, property => property.boolValue = val);
        }
        public static void ChangeProperty(string name, Action<SerializedProperty> changer)
        {
            var lightmapSettings = GetLighmapSettings();
            var prop = lightmapSettings.FindProperty(name);
            if (prop != null)
            {
                changer(prop);
                lightmapSettings.ApplyModifiedProperties();
            }
            else Debug.LogError("lighmap property not found: " + name);
        }
        static SerializedObject GetLighmapSettings()
        {
            var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
            var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as UnityEngine.Object;
            return new SerializedObject(lightmapSettings);
        }
#endif		
	}
}