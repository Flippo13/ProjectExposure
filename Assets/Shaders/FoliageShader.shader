
Shader "ProjectExposure/FoliageShader" {

	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCutOff("Alpha Cutoff", float) = 1.0

		[Header(Global Settings)]
		[Toggle]_Tide("Tide", float) = 1.0
		_TideX("Tide X", Range(-1, 1)) = 0
		_TideZ("Tide Z", Range(-1, 1)) = 0
		_TideAmount("Tide Amount", Range(0, 1)) = 0
		_TideSpeed("Tide Speed", Range(0,5)) = 0
		_SpeedMultiplier("Speed Multiplier", Range(1,5)) = 0
		_BranchMinimum("Branch Minimum", Range(0,5)) = 0

		[Header(X Axis Settings)][Toggle]_XAxis("X Axis", float) = 1.0
		_XScale("X Amount", Range(-1,5)) = 0.5
		_XSpeed("X Speed", Range(-1,12)) = 0.5

		[Header(Z Axis Settings)][Toggle]_ZAxis("Z Axis", float) = 1.0
		_ZScale("Z Amount", Range(-1,5)) = 0.5
		_ZSpeed("Z Speed", Range(-1,12)) = 0.5

		[Header(Y Axis Settings)][Toggle]_YAxis("Y Axis", float) = 1.0

		_YScale("Y Amount", Range(-1,5)) = 0.5
		_YSpeed("Y Speed", Range(-1,12)) = 0.5

	}


	SubShader{
	Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
	LOD 100

	//ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#pragma surface surf Lambert vertex:vert addshadow fullforwardshadows 
	#pragma target 3.0
	//#include "AutoLight.cginc"

	sampler2D _MainTex;
	float4 _Color;

	//YAxis
	float _YAxis;
	float _YScale;
	float _YSpeed;
	//XAxis
	float _XAxis;
	float _XScale;
	float _XSpeed;
	//ZAxis
	float _ZAxis;
	float _ZScale;
	float _ZSpeed;

	//Global
	half _AlphaCutOff;
	half _SpeedMultiplier;
	half _TideX;
	half _TideZ;
	half _TideAmount;
	half _TideSpeed;
	half _Tide;
	half _BranchMinimum;

	struct Input
	{
		float2 uv_MainTex;
	};

	// Maybe if we want a bit more randomness?
	/*float rand(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
	}*/

	void vert(inout appdata_full v)
	{
		float3 worldPos = mul(v.vertex, unity_ObjectToWorld).xyz;

		if(v.vertex.y > _BranchMinimum){
			if(_Tide){
				v.vertex.x += _TideX * (v.vertex.y * sin(v.vertex.y + _Time.y * _TideSpeed)) * (_TideAmount / 10);
				v.vertex.z += _TideZ *(v.vertex.y * sin(v.vertex.y + _Time.y * _TideSpeed)) * (_TideAmount / 10);
			};
			if(_YAxis)
				v.vertex.y += sin(worldPos.y * (_Time.x * (_YSpeed * _SpeedMultiplier))) * _YScale * v.vertex.y;
			if (_XAxis)
				v.vertex.x += sin(worldPos.x * (_Time.x *  (_XSpeed * _SpeedMultiplier))) * _XScale * v.vertex.x;
			if (_ZAxis)
				v.vertex.z += sin(worldPos.z * (_Time.x *  (_ZSpeed * _SpeedMultiplier))) * _ZScale * v.vertex.z;
		}
	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		if (c.a < _AlphaCutOff) discard;
		o.Albedo = c.rgb;
	}

	ENDCG

		}
}