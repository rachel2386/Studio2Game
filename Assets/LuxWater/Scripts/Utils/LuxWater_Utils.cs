using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using System.Runtime.CompilerServices;
//#pragma warning disable 0660, 0661


static public class LuxWaterUtils {


	public struct GersterWavesDescription {
		public Vector3 intensity;
		public Vector4 steepness;
		public Vector4 amp;
		public Vector4 freq;
		public Vector4 speed;
		public Vector4 dirAB;
		public Vector4 dirCD;
	}


	static public void GetGersterWavesDescription (ref GersterWavesDescription Description, Material WaterMaterial ) {
		Description.intensity = WaterMaterial.GetVector("_GerstnerVertexIntensity");
		Description.steepness = WaterMaterial.GetVector("_GSteepness");
		Description.amp = WaterMaterial.GetVector("_GAmplitude");
		Description.freq = WaterMaterial.GetVector("_GFinalFrequency");
		Description.speed = WaterMaterial.GetVector("_GFinalSpeed");
		Description.dirAB = WaterMaterial.GetVector("_GDirectionAB");
		Description.dirCD = WaterMaterial.GetVector("_GDirectionCD");
	}


	//	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static public Vector3 GetGestnerDisplacement (
		Vector3 WorldPosition,
		GersterWavesDescription Description,
		float TimeOffset
	) {

		Vector2 xzVtx;
		xzVtx.x = WorldPosition.x;
		xzVtx.y = WorldPosition.z;

		Vector4 AB;
		Vector4 CD;
		
	//	half4 AB = steepness.xxyy * amp.xxyy * dirAB.xyzw;
		AB.x = Description.steepness.x * Description.amp.x * Description.dirAB.x;
		AB.y = Description.steepness.x * Description.amp.x * Description.dirAB.y;
		AB.z = Description.steepness.y * Description.amp.y * Description.dirAB.z;
		AB.w = Description.steepness.y * Description.amp.y * Description.dirAB.w;

	//	half4 CD = steepness.zzww * amp.zzww * dirCD.xyzw;
		CD.x = Description.steepness.z * Description.amp.z * Description.dirCD.x;
		CD.y = Description.steepness.z * Description.amp.z * Description.dirCD.y;
		CD.z = Description.steepness.w * Description.amp.w * Description.dirCD.z;
		CD.w = Description.steepness.w * Description.amp.w * Description.dirCD.w;


	//	half4 dotABCD = freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
		Vector4 dotABCD;
		dotABCD.x = Description.freq.x * (Description.dirAB.x * xzVtx.x + Description.dirAB.y * xzVtx.y);
		dotABCD.y = Description.freq.y * (Description.dirAB.z * xzVtx.x + Description.dirAB.w * xzVtx.y);
		dotABCD.z = Description.freq.z * (Description.dirCD.x * xzVtx.x + Description.dirCD.y * xzVtx.y);
		dotABCD.w = Description.freq.w * (Description.dirCD.z * xzVtx.x + Description.dirCD.w * xzVtx.y);
		
		Vector4 TIME;
	//	In case we do not use underwater rendering
		float time = Time.timeSinceLevelLoad + TimeOffset; //  Shader.GetGlobalVector("_Time").y + TimeOffset ; //Time.time; //timeSinceLevelLoad; //.time
	//	half4 TIME = _Time.yyyy * speed;
		TIME.x = time * Description.speed.x;
		TIME.y = time * Description.speed.y;
		TIME.z = time * Description.speed.z;
		TIME.w = time * Description.speed.w;

		Vector4 COS;
	//	half4 COS = cos (dotABCD + TIME);
		dotABCD.x += TIME.x;
		dotABCD.y += TIME.y;
		dotABCD.z += TIME.z;
		dotABCD.w += TIME.w;

		COS.x = (float)Math.Cos(dotABCD.x);
		COS.y = (float)Math.Cos(dotABCD.y);
		COS.z = (float)Math.Cos(dotABCD.z);
		COS.w = (float)Math.Cos(dotABCD.w);

		Vector4 SIN;
	//	half4 SIN = sin (dotABCD + TIME);
		SIN.x = (float)Math.Sin(dotABCD.x);
		SIN.y = (float)Math.Sin(dotABCD.y);
		SIN.z = (float)Math.Sin(dotABCD.z);
		SIN.w = (float)Math.Sin(dotABCD.w);

		Vector3 offsets;

	//	offsets.x = dot(COS, half4(AB.xz, CD.xz));
		offsets.x = (COS.x * AB.x + COS.y * AB.z + COS.z * CD.x + COS.w * CD.z) * Description.intensity.x;
	//	offsets.z = dot(COS, half4(AB.yw, CD.yw));
    	offsets.z = (COS.x * AB.y + COS.y * AB.w + COS.z * CD.y + COS.w * CD.w) * Description.intensity.z;
    //	offsets.y = dot(SIN, amp);
    	offsets.y = (SIN.x * Description.amp.x + SIN.y * Description.amp.y + SIN.z * Description.amp.z + SIN.w * Description.amp.w) * Description.intensity.y;

    	return (offsets);
	}
}



