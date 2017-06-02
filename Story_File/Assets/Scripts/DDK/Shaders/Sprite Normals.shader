Shader "Custom/Sprite Normals" {
	Properties {
		[MaterialToggle] LightVer ("Use provided lightDir", Float) = 0
		[MaterialToggle] InvertNorm ("Invert Normals", Float) = 0
		[PerRendererData] _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_NormTex ("Normals", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (0, 0, 0, 1)
		_SpecPower ("Specular Power", Range(0.01, 1)) = 0.078125
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUserSpriteAtlas" = "True"
		}
		
		Cull Off
		Lighting On
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf SpriteNormal alpha vertex:vert
		#pragma multi_compile LIGHTVER_OFF LIGHTVER_ON
		#pragma multi_compile INVERTNORM_OFF INVERTNORM_ON

		sampler2D _MainTex;
		sampler2D _NormTex;
		uniform float _SpecPower;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormTex;
			float3 lightDir;
		};

		struct SurfaceOutputCustom {
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half3 Specular;
			half Gloss;
			half Alpha;
			float3 lightDir;
		};
		
		void vert (inout appdata_full v, out Input o) {
			v.normal = float3(0, 0, 1);
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.lightDir = ObjSpaceLightDir(v.vertex);
		}
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			half4 n = tex2D (_NormTex, IN.uv_NormTex);
			#if defined(INVERTNORM_ON)
			n.g = 1.0 - n.g;
			#endif
			
			o.Normal = UnpackNormal(n);
			o.lightDir = IN.lightDir;
			
			o.Gloss = 1;
			o.Specular = _SpecPower;
		}
		
		half4 LightingSpriteNormal (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
			#if defined(LIGHTVER_ON)
			half3 ld = lightDir;
			#else
			float3 ld = s.lightDir;
			#endif
		
			half3 h = normalize(ld + viewDir);
			fixed diff = max(0, dot(s.Normal, ld));
			
			float nh = max(0, dot(s.Normal, h));
			float spec = pow(nh, s.Specular * 128.0) * s.Gloss;
			
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
			c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
			return c;
		}
		
		ENDCG
	} 

}
