// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PostEffect/DepthRenderer" {
	SubShader {
		Tags { "RenderType"="Opaque" }

		ZTest LEqual

		CGINCLUDE
		#include "UnityCG.cginc"

		struct v2f
		{
			float4 vertex	:	POSITION;
			float depth 	: 	TEXCOORD0;
		};

		v2f vert(appdata_base v)
		{
			v2f o;

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.depth = COMPUTE_DEPTH_01;

			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{

			fixed depth = i.depth;
			fixed4 color = fixed4(depth, depth, depth, 1.0);
			return color;
			
		}

		ENDCG

		Pass
		{
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	} 
}
