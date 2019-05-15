//LIVENDA CTAA VR FOR OPENVR - CINEMATIC TEMPORAL ANTI ALIASING
//VIRTUAL REALITY VERSION
//Copyright Livenda Labs 2018 V1.6

Shader "Hidden/VelocityBufferCTAAVR_VIVE"
{
	CGINCLUDE

	#include "UnityCG.cginc"

	uniform sampler2D _CameraDepthTexture;
	uniform float4 _CameraDepthTexture_TexelSize;
	uniform sampler2D _VelocityTex;
	uniform float4 _VelocityTex_TexelSize;
	uniform float4 _Corner;
	uniform float4x4 _CurrV;
	uniform float4x4 _CurrVP;
	uniform float4x4 _CurrM;
	uniform float4x4 _PrevVP;
	uniform float4x4 _PrevM;

	struct blit_v2f
	{
		float4 cs_pos : SV_POSITION;
		float2 ss_txc : TEXCOORD0;
		float2 vs_ray : TEXCOORD1;
	};

	blit_v2f blit_vert( appdata_img IN )
	{
		blit_v2f OUT;

		OUT.cs_pos = UnityObjectToClipPos(IN.vertex);
		OUT.ss_txc = IN.texcoord.xy;
		OUT.vs_ray = (2.0 * IN.texcoord.xy - 1.0) * _Corner.xy + _Corner.zw;

		return OUT;
	}

	float4 blit_frag_prepass( blit_v2f IN ) : SV_Target
	{
		//reconstruct
		float vs_dist = LinearEyeDepth(tex2D(_CameraDepthTexture, IN.ss_txc).x);
		float3 vs_pos = float3(IN.vs_ray, 1.0) * vs_dist;
		float4 ws_pos = mul(unity_CameraToWorld, float4(vs_pos, 1.0));

		//reproject
		float4 rp_cs_pos = mul(_PrevVP, ws_pos);
		float2 rp_ss_ndc = rp_cs_pos.xy / rp_cs_pos.w;
		float2 rp_ss_txc = 0.5 * rp_ss_ndc + 0.5;

		//velocity
		float2 ss_vel = IN.ss_txc - rp_ss_txc;

		// output
		return float4(ss_vel, 0.0, 0.0);
	}



	struct v2f
	{
		float4 cs_pos : SV_POSITION;
		float4 ss_pos : TEXCOORD0;
		float3 cs_xy_curr : TEXCOORD1;
		float3 cs_xy_prev : TEXCOORD2;
	};

	v2f process_vertex(float4 ws_pos_curr, float4 ws_pos_prev)
	{
		v2f OUT;

		OUT.cs_pos = mul(mul(_CurrVP, _CurrM), ws_pos_curr) * float4(1.0, -1.0, 1.0, 1.0);
		OUT.ss_pos = ComputeScreenPos(OUT.cs_pos);
		OUT.ss_pos.z = -mul(mul(_CurrV, _CurrM), ws_pos_curr).z;// COMPUTE_EYEDEPTH
		OUT.cs_xy_curr = OUT.cs_pos.xyw;
		OUT.cs_xy_prev = mul(mul(_PrevVP, _PrevM), ws_pos_prev).xyw * float3(1.0, -1.0, 1.0);

#if UNITY_UV_STARTS_AT_TOP
		OUT.cs_xy_curr.y = 1.0 - OUT.cs_xy_curr.y;
		OUT.cs_xy_prev.y = 1.0 - OUT.cs_xy_prev.y;
#endif

		return OUT;
	}

	v2f vert( appdata_base IN )
	{
		return process_vertex(IN.vertex, IN.vertex);
	}

	v2f vert_skinned( appdata_base IN )
	{
		return process_vertex(IN.vertex, float4(IN.normal, 1.0));// previous frame positions stored in normal data
	}

	float4 frag( v2f IN ) : SV_Target
	{
		float2 ss_txc = IN.ss_pos.xy / IN.ss_pos.w;
		float scene_z = tex2D(_CameraDepthTexture, ss_txc).x;
		float scene_d = LinearEyeDepth(scene_z);
		const float occlusion_bias = 0.03;

		// discard if occluded
		clip(scene_d - IN.ss_pos.z + occlusion_bias);

		// compute velocity in ndc
		float2 ndc_curr = IN.cs_xy_curr.xy / IN.cs_xy_curr.z;
		float2 ndc_prev = IN.cs_xy_prev.xy / IN.cs_xy_prev.z;


		return float4(0.5 * (ndc_curr - ndc_prev), 0.0, 0.0);
	}


	ENDCG

	SubShader
	{
		// 0: 
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }

			CGPROGRAM

			#pragma vertex blit_vert
			#pragma fragment blit_frag_prepass
			#pragma only_renderers ps4 xboxone d3d11 d3d9 xbox360 opengl
			#pragma target 3.0
			#pragma glsl

			ENDCG
		}

		// 1: 
		Pass
		{
			ZTest LEqual Cull Back ZWrite On
			Fog { Mode Off }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma only_renderers ps4 xboxone d3d11 d3d9 xbox360 opengl
			#pragma target 3.0
			#pragma glsl

			ENDCG
		}

		// 2:
		Pass
		{
			ZTest LEqual Cull Back ZWrite On
			Fog { Mode Off }

			CGPROGRAM

			#pragma vertex vert_skinned
			#pragma fragment frag
			#pragma only_renderers ps4 xboxone d3d11 d3d9 xbox360 opengl
			#pragma target 3.0
			#pragma glsl

			ENDCG
		}


	}

	Fallback Off
}
