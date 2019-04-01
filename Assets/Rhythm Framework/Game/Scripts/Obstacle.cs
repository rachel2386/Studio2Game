using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObstacleModel{
    public class ObstaclePlacing : MonoBehaviour
    {
        public GameObject obstacle;
        public GameObject cloneObstacle;
        public Color color;
    
        public void PlaceObstacle(Transform parent) {
            cloneObstacle = Instantiate(obstacle, transform.position,transform.rotation);
            cloneObstacle.transform.SetParent(parent);
            cloneObstacle.GetComponent<MeshRenderer>().material.color = color;
        }
    }
}


