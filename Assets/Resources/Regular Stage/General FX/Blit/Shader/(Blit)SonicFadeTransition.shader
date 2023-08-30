// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Add Color"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Red("Red", Range(-1.0, 1.0)) = 0.0
		_Green("Green", Range(-1.0, 1.0)) = 0.0
		_Blue("Blue", Range(-1.0, 1.0)) = 0.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent-1"}
		Pass
		{
           Stencil
             {
                 Ref 2
                 Comp Always
                 Pass Replace
             }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}
			
			sampler2D _MainTex;
			float _Red;
			float _Green;
			float _Blue;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				col.r += _Red;
				col.g += _Green;
				col.b += _Blue;

				return col;
			}
			ENDCG
		}
	}
}
