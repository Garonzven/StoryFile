Shader "Custom/Ghost" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Fresnel("FresnelIntensity", Range(0, 1)) = .2
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alpha

	sampler2D _MainTex;
	fixed4 _Color;
	float _Fresnel;

	struct Input {
		float2 uv_MainTex;
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = 1 - clamp(abs(dot(normalize(o.Normal), normalize(IN.viewDir.xyz)))*_Fresnel, 0, 1);
		//o.Emission = c.rgb * o.Alpha;
	}
	ENDCG
	}
	Fallback "Transparent/VertexLit"
}
