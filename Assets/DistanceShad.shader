// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DistanceShad"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Blend One Zero
		//Zwrite Off
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag2
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				//float dist : DISTANCE;
				float2 depth : TEXCOORD0;
				//float computeDepth : DEPTH;
			};

			//float4x4 _WorldToLocalMatrix;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.dist = distance(_WorldSpaceCameraPos.xyz, mul(_Object2World, v.vertex).xyz);
				//o.dist = distance(_PersCamPos.xyz, mul(_Object2World, v.vertex).xyz);
				o.depth = o.vertex.zw;
				//o.computeDepth = COMPUTE_DEPTH_01;
				return o;
			}

			half4 frag2 (v2f i) :SV_Target{
				//return i.depth.x / i.depth.y;
				return EncodeFloatRGBA(i.depth.x / i.depth.y);
				//return DECODE_EYEDEPTH(i.computeDepth);
				//return i.computeDepth;
				//return clamp(i.dist * 0.04f, 0.0f, 1.0f);
			}
			ENDCG
		}
	}
}
