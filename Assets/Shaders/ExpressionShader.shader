Shader "ProjectExposure/Expressions" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Color("Color", Color) = (0.26,0.19,0.16,0.0)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0
		_AlphaCutOff("Alpha Cutoff", Range(0,1)) = 0.0
		[Enum(Red,1,Blue,2,Green,3)]  _Expression("Expression", Float) = 1
		_EmissionStrenght("Emission Strenght", Range(0,2.5)) = 0.0

	}
	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Expression;
		half _AlphaCutOff;
		half _EmissionStrenght;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

			fixed4 color;
			if (_Expression == 1)
				color = c.r;
			if (_Expression == 2)
				color = c.b;
			if (_Expression == 3)
				color = c.g;

			o.Albedo = color * _Color;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			if (color.a < _AlphaCutOff) discard;
			o.Alpha = 1;
			o.Emission = _Color * _EmissionStrenght;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
