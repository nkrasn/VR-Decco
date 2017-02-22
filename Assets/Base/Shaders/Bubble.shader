// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Nikita/Bubble" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_VertPush("Push", Range(0, 1)) = 0.028
		_Speed("Speed", Range(0, 64)) = 33.8
		_Period("Period", Range(0, 128)) = 55.9
		[Toggle] _O2W("World Coordinates", float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		float _VertPush;
		float _Speed;
		float _Period;
		float _O2W;

		void vert(inout appdata_full v)
		{
			float3 norm;
			if (_O2W == 1)
				norm = mul(unity_ObjectToWorld, v.normal).xyz;
			else
				norm = v.normal.xyz;

			norm.x *= abs(sin(_Time.x * _Speed + v.vertex.x * _Period));
			norm.y *= abs(sin(_Time.x * _Speed + v.vertex.y * _Period));
			norm.z *= abs(sin(_Time.x * _Speed + v.vertex.z * _Period));
			v.vertex.xyz += norm * _VertPush;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
