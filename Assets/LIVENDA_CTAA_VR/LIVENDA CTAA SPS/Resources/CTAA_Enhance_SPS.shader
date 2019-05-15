//LIVENDA CTAA - CINEMATIC TEMPORAL ANTI ALIASING
//Copyright Livenda Labs 2019 V1.6

Shader "Hidden/CTAA_Enhance_SPS"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AEXCTAA ("Pixel Width", Float) = 1
		_AEYCTAA ("Pixel Height", Float) = 1 
		_AESCTAA ("Strength", Range(0, 5.0)) = 0.60
		_AEMAXCTAA ("Clamp", Range(0, 1.0)) = 0.05
	}

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				half _AEXCTAA;
				half _AEYCTAA;
				half _AESCTAA;
				half _StrengthMAX;
				half _AEMAXCTAA;
			
				
				uniform sampler2D _Motion0;
				uniform float _motionDelta;

				uniform sampler2D _Motion0Dynamic;
				uniform float _motionDeltaDynamic;
				uniform float _AdaptiveEnhanceStrength;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 coords = i.uv;
					half4 color = tex2D(_MainTex, coords);
					half4 original = color;
					
					float4 mo1 = tex2D(_Motion0, i.uv  );
 					float2 ssVel = ( mo1.xy * 2 -1 ) * mo1.z;
 					ssVel *=  _motionDelta;

 					float4 mo2 = tex2D(_Motion0Dynamic, i.uv  );
 					float2 ssVel2 = ( mo2.xy * 2 -1 ) * mo2.z;
 					ssVel2 *=  _motionDeltaDynamic;

 					ssVel += ssVel2;
					
					half4 blur  = tex2D(_MainTex, coords + half2(0.5 *  _AEXCTAA,       -_AEYCTAA));
						  blur += tex2D(_MainTex, coords + half2(      -_AEXCTAA, 0.5 * -_AEYCTAA));
						  blur += tex2D(_MainTex, coords + half2(       _AEXCTAA, 0.5 *  _AEYCTAA));
						  blur += tex2D(_MainTex, coords + half2(0.5 * -_AEXCTAA,        _AEYCTAA));
					blur /= 4;
					
					float delta = lerp(_AESCTAA, _StrengthMAX, saturate(length(ssVel)*_AdaptiveEnhanceStrength) );
					
					half4 lumaStrength = half4(0.2126, 0.7152, 0.0722, 0) * (delta) * 0.666;

					half4 sharp = color - blur;
					color += clamp(dot(sharp, lumaStrength), -_AEMAXCTAA, _AEMAXCTAA);

					return color; 
				}

			ENDCG
		}

		//=====================================================================

		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				half _AEXCTAA;
				half _AEYCTAA;
				half _AESCTAA;
				half _AEMAXCTAA;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 coords = i.uv;
					half4 color = tex2D(_MainTex, coords);

					half4 blur  = tex2D(_MainTex, coords + half2(0.5 *  _AEXCTAA,       -_AEYCTAA));
						  blur += tex2D(_MainTex, coords + half2(      -_AEXCTAA, 0.5 * -_AEYCTAA));
						  blur += tex2D(_MainTex, coords + half2(       _AEXCTAA, 0.5 *  _AEYCTAA));
						  blur += tex2D(_MainTex, coords + half2(0.5 * -_AEXCTAA,        _AEYCTAA));
					blur /= 4;

					half4 lumaStrength = half4(0.2126, 0.7152, 0.0722, 0) * _AESCTAA * 0.666;
					half4 sharp = color - blur;
					color += clamp(dot(sharp, lumaStrength), -_AEMAXCTAA, _AEMAXCTAA);

					return color;
				}

			ENDCG
		}

		//=====================================================================
	}

	FallBack off
}
