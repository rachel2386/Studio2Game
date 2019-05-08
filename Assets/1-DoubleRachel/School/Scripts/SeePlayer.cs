using System.Collections;
using System.Collections.Generic;
using MultiplayerWithBindingsExample;
using UnityEngine;

public class SeePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform player;

    public bool ConsiderPlayerEyeContact;
    public bool PlayerSeen { get; set; }

    private float angleOfView = 90;
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
      
        if(PlayerSeen)
            print("seenPlayer");
        //testForDot
//        dotProduct = Vector3.Dot(transform.forward.normalized, (testObject.position-transform.position).normalized);
//        print(dotProduct);
   }

    void LookForPlayer()
    {
        Vector3 playerDir = Vector3.Normalize(player.position - transform.position);
        float angle = Vector3.Angle(playerDir, transform.forward);

        if (!ConsiderPlayerEyeContact)
        {
           PlayerSeen = angle <= angleOfView;
        }

        else
        {
            PlayerSeen = angle <= angleOfView && PlayerEyeContact();
            
        }

    }

    bool PlayerEyeContact()
    {
       Debug.DrawRay(player.position, player.forward, Color.blue);
       Debug.DrawRay(player.position, transform.position - player.position, Color.cyan);
       
        bool playerSeeNPC = Vector3.Angle(player.forward, transform.position - player.position) <= angleOfView;
        return playerSeeNPC;
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