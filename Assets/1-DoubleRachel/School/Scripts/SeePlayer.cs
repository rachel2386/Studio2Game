using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform player;

    public bool PlayerSeen { get; set; }
    
    //test
    //public Transform testObject;
    //public float dotProduct;
   // public float newY;
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        LookForPlayer();
        
        //testForDot
//        dotProduct = Vector3.Dot(transform.forward.normalized, (testObject.position-transform.position).normalized);
//        print(dotProduct);
   }

    void LookForPlayer()
    {
        Vector3 playerDir = Vector3.Normalize(player.position - transform.position);
        float angle = Vector3.Angle(playerDir, transform.forward);
        PlayerSeen = angle <= 90;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (player != null)
        Gizmos.DrawRay(transform.position, Vector3.Normalize(player.position - transform.position));
        //Gizmos.DrawRay(transform.position, testObject.position-transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}