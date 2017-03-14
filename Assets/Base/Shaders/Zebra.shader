Shader "Nikita/Zebra"
{
	Properties
	{
		_MainTex("tex", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float2 screenPos : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 vertex : SV_POSITION;
				float2 screenPos : TEXCOORD0;
			};

			vertexOutput vert(vertexInput IN)
			{
				vertexOutput o;
				o.vertex = UnityObjectToClipPos(IN.vertex);

				float4 clipSpace = mul(UNITY_MATRIX_MVP, IN.vertex);
				clipSpace.xy /= clipSpace.w;
				clipSpace.xy = 0.5 * clipSpace.xy + 0.5;
				o.screenPos = clipSpace.xy;

				return o;
			}

			float4 frag(vertexOutput OUT) : COLOR
			{
				float2 uv = OUT.screenPos * _ScreenParams.xy;
				float4 c = tex2D(_MainTex, OUT.screenPos);

				c.r = sin(uv.x * c.r + _Time.x * 500);
				c.g = sin(uv.x * c.g + _Time.x * 500);
				c.b = sin(uv.x * c.b + _Time.x * 500);

				return c;
			}

			ENDCG
		}
	}
}