Shader "ProjectExposure/Expressions" {
	Properties{
		_ExpressionMap1("Expression map 1", 2D) = "white" {}
		_ExpressionMap2("Expression map 2", 2D) = "white" {}
		_AlphaCutOff("Alpha Cutoff", Range(0,1)) = 0.0
		_Color("Color", Color) = (0.26,0.19,0.16,0.0)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0
		[Enum(Expressions)] _Expression("Expression", Float) = 1
		_EmissionStrenght("Emission Strenght", Range(0,1)) = 0.0

	}
		SubShader{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma surface surf Standard
			#pragma target 3.0

			sampler2D _ExpressionMap1;
			sampler2D _ExpressionMap2;


			struct Input {
				float2 uv_ExpressionMap1;
				float2 uv_ExpressionMap2;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			half _Expression;
			half _AlphaCutOff;
			half _EmissionStrenght;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			fixed4 getExpression(fixed4 c)
			{
				fixed4 _ExpressionColor = 0;
				// If enum is more than 3, keep it within the 0-3 range due to RGB
				if (_Expression > 2)
					_Expression = _Expression - 3;

				switch (_Expression)
				{
				case 0: _ExpressionColor = c.r;
					break; 
				case 1: _ExpressionColor = c.b;
					break;
				case 2: _ExpressionColor = c.g;
					break;
				}

				return _ExpressionColor;
			}

			void surf(Input IN, inout SurfaceOutputStandard o) {

				fixed4 _ExpressionColor;
				// If enum is more than 3, use the other texture
				if(_Expression > 2)
					_ExpressionColor = getExpression(tex2D(_ExpressionMap2, IN.uv_ExpressionMap2));
				else
					_ExpressionColor = getExpression(tex2D(_ExpressionMap1, IN.uv_ExpressionMap1));

				o.Albedo = _ExpressionColor * _Color;

				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;

				if (_ExpressionColor.a < _AlphaCutOff) discard;
				o.Alpha = 1;
				o.Emission = _Color * _EmissionStrenght;
			}
			ENDCG
	}
		FallBack "Diffuse"
}
