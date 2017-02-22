Shader "Nikita/Wiggly" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_VertPush("Vert Push", Range(0, 1)) = 0.1
		_Slide1("Slide 1", float) = 1.0
		_Slide2("Slide 2", float) = 1.0
	}
	
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows
#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _NoiseTex;

	struct Input {
		float2 uv_MainTex;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float _VertPush;
	float _Slide1;
	float _Slide2;

	void vert(inout appdata_full v)
	{
		v.vertex.xyz += v.normal * abs(_VertPush * (sin(sin(_Time.x*_Slide1*(v.vertex.x + _Time.x*_Slide2)))));
	}

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
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
