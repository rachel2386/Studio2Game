using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollowOnOverlap : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform playerTransform;
    private Transform NPCTransform;
    bool seePlayer = false;
   
    void Start()
    {
        playerTransform = GameObject.Find("PlayerBase").transform;
        NPCTransform = transform;
    }

    // Update is called once per frame
    float NPCx = 0f;
    float NPCz = 0f;
    Vector3 newNPCPos = new Vector3(0, 0, 0);
    void Update()
    {
        
        if (DetectPlayer())
        {
           
            if (Vector3.Distance(NPCTransform.position, playerTransform.position) >= 0.2f)
            {
                 NPCx= Mathf.MoveTowards(NPCTransform.position.x, playerTransform.position.x, 1f * Time.deltaTime);  
                 NPCz= Mathf.MoveTowards(NPCTransform.position.z, playerTransform.position.z -playerTransform.localScale.z,1f * Time.deltaTime);  
            }
            else
            {
                
                NPCx = NPCTransform.position.x;
                NPCz = NPCTransform.position.z;

            }

            newNPCPos.x = NPCx;
            newNPCPos.z = NPCz;
            newNPCPos.y = NPCTransform.position.y;
            NPCTransform.position = newNPCPos;

        }
    }

    private void OnDrawGizmos()
    {
        if (NPCTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(NPCTransform.position,1);
        }
    }

    bool DetectPlayer()
    {
       
        Collider[] stuffInSphere = Physics.OverlapSphere(NPCTransform.position, 1.5f);

        for (int i = 0; i < stuffInSphere.Length; i++)
        {
            if (stuffInSphere[i].CompareTag("Player"))
            {
                seePlayer = true;
            }
        }

        return seePlayer;
    }
}
