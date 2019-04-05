using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public bool isOut = false;
    Pan pan;

    float panSize = 0;

    // Start is called before the first frame update
    void Start()
    {
        pan = GameObject.FindObjectOfType<Pan>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
    public Vector3 GetJumpDestination()
    {
        var radius = pan.Radius;
        var max = pan.Max;

        // ignore the height(y)
        var dir = new Vector3(transform.position.x - pan.panCenter.transform.position.x, 0,
            transform.position.z - pan.panCenter.transform.position.z);
        dir.Normalize();

        var mag = Random.Range(radius, max);

        // still without y
        var dest = pan.panCenter.transform.position + mag * dir;

        // apply y
        dest.y = pan.jumpAnchorY.transform.position.y;

        return dest;
    }

    // Deprecated    
    public Vector3 GetJumpSpeed()
    {

        var radius = pan.Radius;
        var max = pan.Max;

        // ignore the height(y)
        var dir = new Vector3(transform.position.x - pan.panCenter.transform.position.x, 0,
           transform.position.z - pan.panCenter.transform.position.z);
        dir.Normalize();

        // jump time
        var t = 0.5f;
        var speedY = 0.98f * t / 2.0f;

        var mag = Random.Range(radius, max);
        var speedXZ = dir * mag / t;

        return new Vector3(speedXZ.x, speedY, speedXZ.z);
    }
}
