using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxWater_SetToGerstnerHeight : MonoBehaviour {

	public Material WaterMaterial;
	public Vector3 Damping = new Vector3(0.3f, 1.0f, 0.3f);
	public float TimeOffset = 0.0f;
	public bool UpdateWaterMaterialPerFrame = false;
	
	private Transform trans;

	private Vector3 pos;
	private LuxWaterUtils.GersterWavesDescription Description;
	private bool ObjectIsVisible = false;


	void Start () {
		trans = this.transform;
		pos = trans.position;

	//	Get the Gestner Wave settings from the material and store them into or Description struct
		LuxWaterUtils.GetGersterWavesDescription(ref Description, WaterMaterial);
	}

	void OnBecameVisible () {
		ObjectIsVisible = true;
	}

	void OnBecameInvisible () {
		ObjectIsVisible = false;
	}
	
	void Update () {
		
	//	In case the object is rendered by any camera we have to update its position.
		if (ObjectIsVisible) {

		//	Check for material – you could add a check here if Gerstner Waves are enabled
			if (WaterMaterial == null) {
				return;
			}

			if (UpdateWaterMaterialPerFrame) {
			//	Update the Gestner Wave settings from the material if needed
				LuxWaterUtils.GetGersterWavesDescription(ref Description, WaterMaterial);
			}

		//	Get the offset of the Gerstner displacement. We have to pass:
		//	- a sample location in world space,
		//	- the Gestner Wave settings from the material sttored in our Description struct,
		//	- a time offset (in seconds) which lets us create an effect of the inertia of masses.
			Vector3 Offset = LuxWaterUtils.GetGestnerDisplacement(this.transform.position, Description, TimeOffset);
			
		//	We assume that the object itself does not move.
			var newPos = pos;
			newPos.x += Offset.x * Damping.x;
			newPos.y += Offset.y * Damping.y;
			newPos.z += Offset.z * Damping.z;

			trans.position = newPos;
		}

	}
}
