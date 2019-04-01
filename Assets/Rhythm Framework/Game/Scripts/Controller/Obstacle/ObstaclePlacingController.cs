using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacingController : ObstacleModel.ObstaclePlacing
{

    public Transform streetParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Street")) {
            streetParent = other.gameObject.transform;
        }
        
    }

}
