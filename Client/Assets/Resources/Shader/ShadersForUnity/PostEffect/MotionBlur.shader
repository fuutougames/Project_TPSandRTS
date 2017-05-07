// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MotionBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DepthTex ("Depth Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _DepthTex;
	float4x4 _PrevVPMatrix;
	float4x4 _CurVPMatrix_I;

	struct v2f_simple
	{
		float4 pos			: POSITION;
		half2 uv 			: TEXCOORD0;
		float4 hPos 		: TEXCOORD1;
	};

	v2f_simple vert (appdata_img v)
	{
		v2f_simple o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.hPos = o.pos;
		o.uv = v.texcoord.xy;
		return o;
	}

	fixed4 frag (v2f_simple i) : COLOR
	{
		half2 texcoord = i.uv;
		fixed4 source = tex2D (_MainTex, texcoord);

		float depth = tex2D (_DepthTex, texcoord).r;

		// Get Pixel's World Position
		fixed4 H = fixed4(i.hPos.xy, depth, 1);
		fixed4 D = mul (_CurVPMatrix_I, H);
		fixed4 W = D / D.w;

		// Get Pixel's Old Screen Position
		fixed4 prevH = mul (_PrevVPMatrix, W);
		prevH /= prevH.w;

		// decide the sample step length and direction
		fixed2 velocity = (H.xy - prevH.xy) / 50.0f;

		// sample and blur
		fixed sampleNum = 5;
		for (int i = 1; i < 5; ++i)
		{
			texcoord += velocity;
			source += tex2D(_MainTex, texcoord);
		}

		fixed4 destination = source / sampleNum;
		return destination;
	}
	ENDCG

	SubShader {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG
		}
	} 
	FallBack Off
}
