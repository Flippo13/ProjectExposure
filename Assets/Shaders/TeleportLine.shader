Shader "ProjectExposure/TeleportLine" {
	Properties{
		_BlockedColor("Blocked Color", Color) = (1,1,1,1)
		_UnblockedColor("Unblocked Color", Color) = (1,1,1,1)
		_Sprite("Sprite", 2D) = "white" {}
	_Speed("Speed", Range(0,100)) = 50
		[MaterialToggle]	_Blocked("Blocked", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard noambient 

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _Sprite;

	struct Input {
		float2 uv_Sprite;
	};

	fixed4 _UnblockedColor;
	fixed4 _BlockedColor;
	half _Speed;
	half _Blocked;

	UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {

		// Scroll UV
		IN.uv_Sprite.x += _Time.x * -_Speed;

		fixed4 c = tex2D(_Sprite, IN.uv_Sprite);

		if (_Blocked)
			o.Emission = _BlockedColor;
		else
			o.Emission = _UnblockedColor;
		if (c.a < 0.5) discard;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
