Shader "Custom/VFXOverlay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}		
	}

	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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
			sampler2D _TransparentTex;
			sampler2D _ParticlesTex;
			sampler2D _BlurTex;
			sampler2D _SkanLinesTex;

			half     _TransparentAlpha;
			half     _ParticlesAlpha;
			half     _BlurAlpha;
			half     _SkanLinesAlpha;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 mainColor = tex2D(_MainTex, i.uv);
				fixed4 blurColor = tex2D(_BlurTex, i.uv);
				fixed4 scanLinesColor = tex2D(_SkanLinesTex, i.uv);
				fixed4 transparentColor = tex2D(_TransparentTex, i.uv) * _TransparentAlpha;
				fixed4 particlesColor = tex2D(_ParticlesTex, i.uv) * _ParticlesAlpha;

				//blurColor = blurColor - scanLinesColor;

				fixed4 result = mainColor;
				result = result * (1 - transparentColor.a) + transparentColor;
				result = result * (1 - particlesColor.a) + particlesColor;
				result += (blurColor - scanLinesColor) * _BlurAlpha;

				return result;
			}

			/*fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainColor = tex2D(_MainTex, i.uv);
				fixed4 blurColor = tex2D(_BlurTex, i.uv);
				fixed4 scanLinesColor = tex2D(_SkanLinesTex, i.uv);
				fixed4 transparentColor = tex2D(_TransparentTex, i.uv);

				mainColor *= 1 - transparentColor.a;
				
				fixed4 result = mainColor;
				result += transparentColor * _TransparentAlpha;
				result += (blurColor - scanLinesColor) * _BlurAlpha;	

				return result;
			}*/
			ENDCG
		}
	}
}
