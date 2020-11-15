// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradient"
{
	Properties
	{
		 _Color1("Color1", Color) = (1.0, 1.0, 1.0, 1.0)
		 _Color2("Color2", Color) = (0.0, 0.0, 0.0, 0.0)
	}

		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color1;
			fixed4 _Color2;
			fixed4 _Color3;
			float _Weights;
			float _Top;
			float _Bottom;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
				float lp = 0.0;

				lp = 1 - i.uv.y;
				col = lerp(_Color1, _Color2, lp);

				return col;
			}

			ENDCG
		}
	}
}