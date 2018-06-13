Shader "ProjectExposure/DeformationShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_Outline("Outline Size", Range(0,1)) = 0.0
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_MainTex("Albedo map", 2D) = "white" {}
		_MetallicSmoothness("Metallic/Smoothness", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0,1)) = 0.0
		_Deform("Deform", Range(0,1)) = 0.0
		[MaterialToggle]_InRange("In Range", Range(0,1)) = 0.0

	}
		SubShader{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 200
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard  vertex:vert fullforwardshadows addshadow

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
			#pragma shader_feature _METALLICGLOSSMAP

			sampler2D _MainTex;
			sampler2D _NoiseTexture;
			sampler2D _MetallicSmoothness;


			struct Input {
				float2 uv_MainTex;
			};

			half _Deform;
			half _Metallic;
			fixed4 _OutlineColor;
			fixed4 _Color;
			half _Outline;
			fixed _Smoothness;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void vert(inout appdata_full v)
			{
				float noise = tex2Dlod(_NoiseTexture, float4(v.texcoord.xy, 0, 0)).rgb;
				v.vertex.xyz -= v.vertex.xyz * noise * _Deform;
			}

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 _MetallicSmoothnesssColor = tex2D(_MetallicSmoothness, IN.uv_MainTex);
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _MetallicSmoothnesssColor.rgb;
				o.Smoothness = _Smoothness * _MetallicSmoothnesssColor.a;
				o.Alpha = c.a;
			}
			ENDCG

				Pass
			{
				Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha:fade
			#include "UnityCG.cginc"
			#pragma target 3.0

				struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;

			};

			sampler2D _NoiseTexture;
			half _Deform;
			uniform float4 _OutlineColor;
			half _Outline;
			half _InRange;

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;
				float4 objPos = mul(unity_ObjectToWorld, float4(0,0,0,1));

				float dist = distance(_WorldSpaceCameraPos, objPos.xyz) / _ScreenParams.g;
				float noise = tex2Dlod(_NoiseTexture, float4(v.texcoord.xy, 0, 0)).rgb;
				v.vertex.xyz -= v.vertex.xyz * noise * _Deform;
				float expand = dist * _Outline * 5;
				float4 pos = float4(v.vertex.xyz + v.normal * expand, 1);
				o.pos = UnityObjectToClipPos(pos);
				return o;
			}

			float4 frag(VertexOutput i) : COLOR
			{
				fixed4 returnColor;
				if (_InRange)
					returnColor = fixed4(_OutlineColor.rgb, 1);
				else
					returnColor = fixed4(0, 0, 0, 0);

				return returnColor;
			}
				ENDCG
			}

		}
			FallBack "Diffuse"
}