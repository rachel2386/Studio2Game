Shader "Hidden/AdaptiveEnhanceVR_Oculus"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DELTAXp ("Pixel Width", Float) = 1
		_DELTAYp ("Pixel Height", Float) = 1 
		_Strength ("Strength", Range(0, 5.0)) = 0.60
		_DELTAMAXC ("Clamp", Range(0, 1.0)) = 0.05
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
				half _DELTAXp;
				half _DELTAYp;
				half _Strength;
				half _StrengthMAX;
				half _DELTAMAXC;
			
				
				uniform sampler2D _Motion0;
				uniform float _motionDelta;
				uniform float _AdaptiveSharpen;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 coords = i.uv;
					half4 color = tex2D(_MainTex, coords);
					half4 original = color;
					
					float4 mo1 = tex2D(_Motion0, i.uv  );
 					float2 ssVel =  mo1.xy ;
 					//ssVel *=  _motionDelta;
					
					half4 blur  = tex2D(_MainTex, coords + half2(0.5 *  _DELTAXp,       -_DELTAYp));
						  blur += tex2D(_MainTex, coords + half2(      -_DELTAXp, 0.5 * -_DELTAYp));
						  blur += tex2D(_MainTex, coords + half2(       _DELTAXp, 0.5 *  _DELTAYp));
						  blur += tex2D(_MainTex, coords + half2(0.5 * -_DELTAXp,        _DELTAYp));
					blur /= 4;
					
					float delta = lerp(_Strength, _StrengthMAX, saturate(pow(length(ssVel),1.10)*_AdaptiveSharpen) );
					
					half4 lumaStrength = half4(0.2126, 0.71521, 0.0722, 0) * (delta) * 0.666;

					half4 sharp = color - blur;
					color += clamp(dot(sharp, lumaStrength), -_DELTAMAXC, _DELTAMAXC);

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
				half _DELTAXp;
				half _DELTAYp;
				half _Strength;
				half _DELTAMAXC;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 coords = i.uv;
					half4 color = tex2D(_MainTex, coords);

					half4 blur  = tex2D(_MainTex, coords + half2(0.5 *  _DELTAXp,       -_DELTAYp));
						  blur += tex2D(_MainTex, coords + half2(      -_DELTAXp, 0.5 * -_DELTAYp));
						  blur += tex2D(_MainTex, coords + half2(       _DELTAXp, 0.5 *  _DELTAYp));
						  blur += tex2D(_MainTex, coords + half2(0.5 * -_DELTAXp,        _DELTAYp));
					blur /= 4;

					half4 lumaStrength = half4(0.2126, 0.7152, 0.0722, 0) * _Strength * 0.666;
					half4 sharp = color - blur;
					color += clamp(dot(sharp, lumaStrength), -_DELTAMAXC, _DELTAMAXC);

					return color;
				}

			ENDCG
		}

		//=====================================================================
	}

	FallBack off
}
