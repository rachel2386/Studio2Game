using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPCFollowOnOverlap : MonoBehaviour
{

    private Transform playerTransform;
    private Transform NPCTransform;
    Animator NPCAnim;
    bool seePlayer = false;
    float NPCspeed = 0;
   
    void Start()
    {
        playerTransform = GameObject.Find("PlayerBase").transform;
        NPCTransform = transform;
        NPCAnim = GetComponent<Animator>();
    }

    float NPCx = 0f;
    float NPCz = 0f;
    Vector3 newNPCPos = new Vector3(0, 0, 0);
    private static readonly int Forward = Animator.StringToHash("Forward");

    void Update()
    {
        
      
        if (DetectPlayer())
        {
            
            if (Vector3.Distance(NPCTransform.position, playerTransform.position) <= 1f)
            {
                NPCspeed = Mathf.Lerp(NPCspeed, 0f, 2*Time.deltaTime);
            }
            else if(Vector3.Distance(NPCTransform.position, playerTransform.position) <= 1.5f)
            {

                NPCspeed = Mathf.Lerp(NPCspeed, 0.02f, Time.deltaTime); //Mathf.MoveTowards(NPCspeed, 0.02f, Time.deltaTime/4);
            }
            else
            {

                NPCspeed = Mathf.Lerp(NPCspeed, 0.06f, Time.deltaTime);

            }

            NPCx= Mathf.MoveTowards(NPCTransform.position.x, playerTransform.position.x,NPCspeed );  
            NPCz= Mathf.MoveTowards(NPCTransform.position.z, playerTransform.position.z -playerTransform.localScale.z,NPCspeed);  
            
            newNPCPos.x = NPCx;
            newNPCPos.z = NPCz;
            newNPCPos.y = NPCTransform.position.y;
            NPCTransform.position = newNPCPos;
            NPCTransform.LookAt(newNPCPos);

        }
        else
        {
            NPCspeed = 0;
        }
        
        UpdateAnimator();
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

    void UpdateAnimator()
    {
        //print("Distance" + Vector3.Distance(NPCTransform.position, playerTransform.position) );
        //print("npcSpeed" + NPCspeed);
        NPCAnim.SetFloat(Forward, NPCspeed);
    }
}
