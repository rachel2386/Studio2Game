﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPCFollowOnOverlap : MonoBehaviour
{
    private ParticleSystem speechParticle;
    private Transform playerTransform;
    private Transform NPCTransform;
    Animator NPCAnim;
    bool seePlayer = false;
    float NPCspeed = 0;
   public float sphereRadius = 1.6f;
    void Start()
    {
        playerTransform = GameObject.Find("PlayerBase").transform;
        NPCTransform = transform;
        NPCAnim = GetComponent<Animator>();
        speechParticle = GetComponentInChildren<ParticleSystem>();
        
    }

    float NPCx = 0f;
    float NPCz = 0f;
    Vector3 newNPCPos = new Vector3(0, 0, 0);
    private static readonly int Forward = Animator.StringToHash("Forward");

    void Update()
    {
        
      
        if (DetectPlayer())
        {
            if(!speechParticle.isPlaying)
            speechParticle.Play();
                
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
            speechParticle.Stop();
        }
        
        UpdateAnimator();
    }

    private void OnDrawGizmos()
    {
        
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position,sphereRadius);
        
    }

    public bool DetectPlayer()
    {
       
        Collider[] stuffInSphere = Physics.OverlapSphere(NPCTransform.position, sphereRadius);

        foreach (Collider t in stuffInSphere)
        {
            if (t.CompareTag("Player"))
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
