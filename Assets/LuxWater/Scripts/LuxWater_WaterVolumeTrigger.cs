using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuxWater {

	public class LuxWater_WaterVolumeTrigger : MonoBehaviour {
		public Camera cam;
		public bool active = true;

		void OnEnable () {
			if (cam == null) {
				var camera = this.GetComponent<Camera>();
				if (camera != null) {
					cam = camera;
				}
				else {
					active = false;
				}
			}
		}
	}
}
