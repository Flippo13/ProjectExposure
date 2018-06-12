Shader "ProjectExposure/Water" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_NormalMap("Bumpmap", 2D) = "bump" {}
	_FoamMap("Foam", 2D) = "bump" {}
		_NormalMap2("Bumpmap", 2D) = "bump" {}
		_NoiseMap("Noise", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_NormalScrollSpeed("Normal Scroll Speed", Range(0,50)) = 0.0
		_WaveSpeed("Wave Speed", Range(0,50)) = 0.0
		_WaveAmplitude("Wave Amplitude", Range(0,1)) = 0.0
		_NoiseAmplitude("Noise Amplitude", Range(0,1)) = 0.0
	}
		SubShader{
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off


			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert fullforwardshadows alpha:fade
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _NormalMap;
			sampler2D _NormalMap2;
			sampler2D _NoiseMap;
			sampler2D _FoamMap;
			sampler2D _CameraDepthTexture;
			half _WaveSpeed;
			half _WaveAmplitude;
			half _NormalScrollSpeed;
			half _NoiseAmplitude;

			struct Input {
				float2 uv_NormalMap;
				float2 uv_NormalMap2;
				float2 uv_NoiseTex;
			};


			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)


			void vert(inout appdata_full v)
			{
				fixed offsetX = _NormalScrollSpeed * _Time;
				fixed offsetY = _NormalScrollSpeed * _Time;
				fixed2 offsetUV = fixed2(offsetX, offsetY);

				float noise = tex2Dlod(_NoiseMap, float4(v.texcoord.xy, 0, 0)).rgb;
				float3 worldPos = mul(v.vertex, unity_ObjectToWorld).xyz;
				float phase = _Time * _WaveSpeed;
				float offset = (v.vertex.y + v.vertex.z * 0.2) * 50;
				v.vertex.y += (sin(phase + offset) * _WaveAmplitude) * (noise * _NoiseAmplitude) * worldPos;

			}

			void surf(Input IN, inout SurfaceOutputStandard o) {

				fixed offsetX = _NormalScrollSpeed * _Time;
				fixed offsetY = _NormalScrollSpeed * _Time;
				fixed2 offsetUV = fixed2(offsetX, offsetY);

				o.Normal = normalize((UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap + offsetUV)) + UnpackNormal(tex2D(_NormalMap2, IN.uv_NormalMap2 + offsetUV))));

				//half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(o.scrPos))); // depth
				//half4 foamLine = 1 - saturate(_FoamMap * (depth - i.scrPos.w));// foam line by comparing depth and screenposition
				//col += foamLine * _Tint; // add the foam line and tint to the texture


				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = _Color.a;
				o.Albedo = _Color;
			}


			ENDCG
		}
			FallBack "Diffuse"
}
