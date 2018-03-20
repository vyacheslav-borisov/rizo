Shader "Custom/Blurring"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half	  _TexelUStep;
			half	  _TexelVStep;
			half	  _Factor;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color = 0;
				color += tex2D(_MainTex, i.uv + half2(-_TexelUStep,		-_TexelVStep));
				color += tex2D(_MainTex, i.uv + half2(0,				-_TexelVStep));
				color += tex2D(_MainTex, i.uv + half2(_TexelUStep,		-_TexelVStep));
				color += tex2D(_MainTex, i.uv + half2(_TexelUStep,		0));
				color += tex2D(_MainTex, i.uv + half2(_TexelUStep,		_TexelVStep));
				color += tex2D(_MainTex, i.uv + half2(0,				_TexelVStep));
				color += tex2D(_MainTex, i.uv + half2(-_TexelUStep,		_TexelVStep));
				color += tex2D(_MainTex, i.uv + half2(-_TexelUStep,		0));
				color.rgb /= 8;
				color.rgb *= _Factor;

				return color;
			}
			ENDCG
		}
	}
}
