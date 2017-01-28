Shader "Unlit/WaterShader"
{
	Properties
	{
		_MainTex ("Delay", 2D) = "white" {}
		_WaterColor ("Water Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {
			"RenderType"="Transparent"
			"Queue"="Transparent+10"
			"IgnoreProjector"="True" 
		}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha, One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float h : POSITION1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.h = v.vertex.y;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 _WaterColor;

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _WaterColor;
				fixed delay = tex2D(_MainTex, i.uv).r;
				float h = i.h + 1;
				h = ceil(h * 8)/8;

				float f = abs(i.h) * 4;
				if (delay < f) {
					col.rgb += fixed3(f, f, f);// lerp(col.rgb, fixed3(1, 1, 1), h);
				}
				col.rgb *= h;
				col.a = 0.5f + 0.5*abs(i.h);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
