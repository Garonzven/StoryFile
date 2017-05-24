// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Ghost Doble Sided" 
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_Fresnel("FresnelIntensity", Range(0, 1)) = .2
	}
	Category
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		SubShader
		{
			/* Outside Faces */
			Pass
			{
				Cull Back

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _Color;
				float _Fresnel;
	

				struct Input {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : TEXCOORD1;
					float3 viewDir : TEXCOORD2;
				};

				v2f vert(Input v) {
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.normal = v.normal;
					o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
					return o;
				}

				fixed4 frag(v2f i) : SV_TARGET{
					fixed4 c = tex2D(_MainTex, i.uv) * _Color;
					c.w = 1 - clamp(abs(dot(i.viewDir, i.normal))*_Fresnel, 0, 1);
					return c;
				}
				ENDCG
			}

			/* Inside Faces */
			
			Pass
			{
				Cull Front

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _Color;
				float _Fresnel;


				struct Input {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : TEXCOORD1;
					float3 viewDir : TEXCOORD2;
				};

				v2f vert(Input v) {
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.normal = v.normal;
					o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
					return o;
				}

				fixed4 frag(v2f i) : SV_TARGET{
					fixed4 c = tex2D(_MainTex, i.uv) * _Color;
				c.w = 1 - clamp(abs(dot(i.viewDir, i.normal))*_Fresnel, 0, 1);
				return c;
				}
				ENDCG
			}
		}
		Fallback "Transparent/VertexLit"
	}	
}
