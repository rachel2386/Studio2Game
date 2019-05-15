//CTAA SPS VelocityShaderCTAASPS SHADER V1.6

Shader "Hidden/VelocityShaderCTAASPS"
{
	CGINCLUDE	

	#pragma only_renderers ps4 xboxone d3d11 d3d9 xbox360 opengl glcore gles3 metal vulkan
	#pragma target 3.0

	#pragma multi_compile CAMERA_PERSPECTIVE CAMERA_ORTHOGRAPHIC	

	#include "UnityCG.cginc"	

#if UNITY_VERSION < 550
	#define STEREO_ARRAY
	#define STEREO_INDEX(x) x
#else
	#define STEREO_ARRAY [2]
	#define STEREO_INDEX(x) x[unity_StereoEyeIndex] 
#endif

	uniform sampler2D_half _VelocityTex;
	uniform float4 _VelocityTex_TexelSize;
	uniform float4 _ProjectionExtents STEREO_ARRAY;
	uniform float4x4 _CurrV STEREO_ARRAY;
	uniform float4x4 _CurrVP STEREO_ARRAY;
	uniform float4x4 _CurrM;
	uniform float4x4 _PrevVP STEREO_ARRAY;
	uniform float4x4 _PrevVP_NoFlip STEREO_ARRAY;
	uniform float4x4 _PrevM;

	uniform sampler2D_float _CameraDepthTexture;
	uniform float4 _CameraDepthTexture_TexelSize;

	#if UNITY_REVERSED_Z
	#define ZCMP_GT(a, b) (a < b)
	#else
	#define ZCMP_GT(a, b) (a > b)
	#endif

	float depth_resolve_linear(float z)
	{
		#if CAMERA_ORTHOGRAPHIC
		#if UNITY_REVERSED_Z
				return (1.0 - z) * (_ProjectionParams.z - _ProjectionParams.y) + _ProjectionParams.y;
		#else
				return z * (_ProjectionParams.z - _ProjectionParams.y) + _ProjectionParams.y;
		#endif
		#else
				return LinearEyeDepth(z);
		#endif
	}

	float depth_sample_linear(float2 uv)
	{
		return depth_resolve_linear(tex2D(_CameraDepthTexture, uv).x);
	}

	struct blit_v2f
	{
		float4 cs_pos : SV_POSITION;
		float2 ss_txc : TEXCOORD0;
		float2 vs_ray : TEXCOORD1;
	};

	blit_v2f blit_vert(appdata_img IN)
	{
		blit_v2f OUT;

	#if UNITY_VERSION < 540
		OUT.cs_pos = UnityObjectToClipPos(IN.vertex);
	#else
		OUT.cs_pos = UnityObjectToClipPos(IN.vertex);
	#endif

	#if UNITY_SINGLE_PASS_STEREO
		OUT.ss_txc = UnityStereoTransformScreenSpaceTex(IN.texcoord.xy);
	#else
		OUT.ss_txc = IN.texcoord.xy;
	#endif
		OUT.vs_ray = (2.0 * IN.texcoord.xy - 1.0) * STEREO_INDEX(_ProjectionExtents).xy + STEREO_INDEX(_ProjectionExtents).zw;

		return OUT;
	}

	float4 blit_frag_prepass(blit_v2f IN) : SV_Target
	{
		
		float vs_dist = depth_sample_linear(IN.ss_txc);
	#if CAMERA_PERSPECTIVE
		float3 vs_pos = float3(IN.vs_ray, 1.0) * vs_dist;
	#elif CAMERA_ORTHOGRAPHIC
		float3 vs_pos = float3(IN.vs_ray, vs_dist);
	#else
		#error "missing keyword CAMERA_..."
	#endif

	#if UNITY_VERSION < 540
		float4 ws_pos = mul(unity_CameraToWorld, float4(vs_pos, 1.0));
	#else
		float4 ws_pos = mul(unity_CameraToWorld, float4(vs_pos, 1.0));
	#endif
				
		float4 rp_cs_pos = mul(STEREO_INDEX(_PrevVP_NoFlip), ws_pos);
		float2 rp_ss_ndc = rp_cs_pos.xy / rp_cs_pos.w;
		float2 rp_ss_txc = 0.5 * rp_ss_ndc + 0.5;
		
	#if UNITY_SINGLE_PASS_STEREO
		float2 ss_vel = IN.ss_txc - UnityStereoTransformScreenSpaceTex(rp_ss_txc);
	#else
		float2 ss_vel = IN.ss_txc - rp_ss_txc;
	#endif

		
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

		const float occlusion_bias = 0.03;

		OUT.cs_pos = mul(mul(STEREO_INDEX(_CurrVP), _CurrM), ws_pos_curr);
		OUT.ss_pos = ComputeScreenPos(OUT.cs_pos);
		OUT.ss_pos.z = -mul(mul(STEREO_INDEX(_CurrV), _CurrM), ws_pos_curr).z - occlusion_bias;// COMPUTE_EYEDEPTH
		OUT.cs_xy_curr = OUT.cs_pos.xyw;
		OUT.cs_xy_prev = mul(mul(STEREO_INDEX(_PrevVP), _PrevM), ws_pos_prev).xyw;

	#if UNITY_UV_STARTS_AT_TOP
		OUT.cs_xy_curr.y = -OUT.cs_xy_curr.y;
		OUT.cs_xy_prev.y = -OUT.cs_xy_prev.y;
	#endif

		return OUT;
	}

	v2f vert(appdata_base IN)
	{
		return process_vertex(IN.vertex, IN.vertex);
	}

	v2f vert_skinned(appdata_base IN)
	{
		return process_vertex(IN.vertex, float4(IN.normal, 1.0));
	}

	float4 frag(v2f IN) : SV_Target
	{
		float2 ss_txc = IN.ss_pos.xy / IN.ss_pos.w;
		float scene_d = depth_sample_linear(ss_txc);
		
		clip(scene_d - IN.ss_pos.z);
		
		float2 ndc_curr = IN.cs_xy_curr.xy / IN.cs_xy_curr.z;
		float2 ndc_prev = IN.cs_xy_prev.xy / IN.cs_xy_prev.z;
		
	#if UNITY_SINGLE_PASS_STEREO
		return float4(0.5 * (ndc_curr - ndc_prev) * unity_StereoScaleOffset[unity_StereoEyeIndex].xy, 0.0, 0.0);
	#else
		return float4(0.5 * (ndc_curr - ndc_prev), 0.0, 0.0);
	#endif
	}

	
	ENDCG

	SubShader
	{
		
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }

			CGPROGRAM

			#pragma vertex blit_vert
			#pragma fragment blit_frag_prepass

			ENDCG
		}
		
		Pass
		{
			ZTest LEqual Cull Back ZWrite On
			Fog { Mode Off }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
		
		Pass
		{
			ZTest LEqual Cull Back ZWrite On
			Fog { Mode Off }

			CGPROGRAM

			#pragma vertex vert_skinned
			#pragma fragment frag

			ENDCG
		}

		
	}

	Fallback Off
}