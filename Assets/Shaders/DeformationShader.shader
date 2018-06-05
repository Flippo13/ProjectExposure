Shader "ProjectExposure/DeformationShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_MainTex ("Albedo map", 2D) = "white" {}
		_MetallicSmoothness ("Metallic/Smoothness", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0,1)) = 0.0
		_Deform("Deform", Range(0,1)) = 0.0
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		Cull Off
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
		fixed4 _Color;
		fixed _Smoothness;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v)
		{
			float noise = tex2Dlod(_NoiseTexture, float4(v.texcoord.xy, 0, 0)).rgb;
			v.vertex.xyz -= v.vertex.xyz * noise * _Deform; //my "fix"
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 _MetallicSmoothnesssColor = tex2D(_MetallicSmoothness, IN.uv_MainTex);
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _MetallicSmoothnesssColor.rgb;
			o.Smoothness = _Smoothness * _MetallicSmoothnesssColor.a;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
