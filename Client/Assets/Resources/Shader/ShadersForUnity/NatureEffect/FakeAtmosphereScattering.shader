// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NatureEffect/FakeAtmosphereScattering"
{
	Properties
	{
		_MainTex("Noise", 2D) = "white" {}
		_Color("Color", Color) = (0.2, 0.4, 0.8, 1.0)
		_Brightness("Brightness", float) = 1.0
		_RevertBound("RevertBound", float) = 0.44
		_Pow("Pow", float) = 0.0
		_FlowSpeedU("FlowSpeedU", float) = 0.1
		_FlowSpeedV("FlowSpeedV", float) = 0.1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Cull Back
		Blend SrcAlpha One

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Brightness;
			float _RevertBound;
			float _Pow;
			float _FlowSpeedU;
			float _FlowSpeedV;

			struct v2f
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				float4 vertex : TEXCOORD1;
				float3 normal : TEXCOORD2;
			};

			v2f vert(appdata_full input)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(input.vertex);
				o.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
				o.vertex = input.vertex;
				o.normal = input.normal;

				o.uv += half2(_Time.y * _FlowSpeedU, _Time.y * _FlowSpeedV);

				return o;
			}

			fixed4 frag(v2f input) : COLOR
			{
				fixed4 color = _Color;
				float3 viewDir = normalize(ObjSpaceViewDir(input.vertex));
				float3 N = normalize(input.normal);
				half dotVN = dot(N, viewDir);

				float alpha = abs(1 - dotVN);

				//it means
				//if alpha > _RevertBound:
				//    alpha = 2 * _RevertBound - alpha
				//else:
				//    alpha = alpha
				//the code down here is hard to understand but it's far more faster then if-else code above
				float2 tmp = float2(alpha, 2 * _RevertBound - alpha);
				alpha = lerp(tmp.y, tmp.x, step(alpha, _RevertBound));

				color.a = pow(alpha, _Pow);
				fixed4 noise = tex2D(_MainTex, input.uv);

				color.a *= _Brightness * (noise.r + noise.g + noise.b) / 3.0;
				return color;
			}

			ENDCG
		}
	}
}