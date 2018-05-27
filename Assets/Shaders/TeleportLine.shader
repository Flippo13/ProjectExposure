Shader "ProjectExposure/TeleportLine" {
	Properties {
		_BlockedColor ("Blocked Color", Color) = (1,1,1,1)
		_UnblockedColor("Unblocked Color", Color) = (1,1,1,1)
		_MainTex ("Sprite", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Speed("Speed", Range(0,50)) = 0.0
		[MaterialToggle]	_Blocked("Blocked", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _UnblockedColor;
		fixed4 _BlockedColor;
		half _Speed;
		half _Blocked;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			IN.uv_MainTex.x -= _Time.x * _Speed;
			fixed4 c;
			if(_Blocked)
				c = tex2D(_MainTex, IN.uv_MainTex) * _BlockedColor;
			else
				c = tex2D(_MainTex, IN.uv_MainTex) * _UnblockedColor;


			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			if (c.a < 0.5) discard;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
