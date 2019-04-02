using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NPCBehavior : MonoBehaviour
{
    private NavMeshAgent myAgent;
    private Animator myAnim;
    private Vector3 steeringTargetLastFrame;

    private bool goingForward = true;
    private bool npcRotate = true;

    public bool NpcRotate
    {
        get => npcRotate;
        set => npcRotate = value;
    }

    public bool PlayerTriggered { get; set; } = false;

    private Vector3 AgentOffset;
    private Vector3 DestinationOffset;
    private float turnSpeed = 2f;
    
    public Transform myDestination;
    private Transform playerTransform;
    [Header(" NPC Type:")] 
    public bool Trigger;
    public bool Automatic;
    public bool Stationary;


    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        AgentOffset = transform.position;
        DestinationOffset = myDestination.position;
        playerTransform = GameObject.Find("PlayerBase").transform;

        if (!Stationary) return;
        randomRot = transform.eulerAngles.y;
        InvokeRepeating(nameof(generateRandomNum), 6, 7);
    }

    // Update is called once per frame

    void Update()
    {
        if (Trigger)
        {
            if (PlayerTriggered)
                myAgent.SetDestination(myDestination.position);
        }
        else if (Automatic)
        {
            myAgent.SetDestination(goingForward ? DestinationOffset : AgentOffset);
            //walking back and forth two points
            if (!myAgent.pathPending && myAgent.remainingDistance <= 0.5f)
            {
                goingForward = !goingForward;
            }
        }
        else if (Stationary)
        {
            if (npcRotate)
                AutoRotate();
            else
            {
                //Quaternion rot;
               // var rot = Quaternion.LookRotation(playerTransform.position, Vector3.up);
                transform.LookAt(Vector3.MoveTowards(transform.forward,playerTransform.position, 0.2f)); 
            }

        }
            
        

        myAgent.updatePosition = false;
        myAgent.updateRotation = true;
        float speed;
        if (npcRotate)
            speed = myAgent.desiredVelocity.magnitude;
        else speed = 0;
        //update animator 
        myAnim.SetFloat("Forward", speed);
        // look at walking dir    
        //transform.LookAt(myAgent.steeringTarget + transform.forward);
    }

    void AutoRotate()
    {
        // if(Rotating)

        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
            Mathf.MoveTowardsAngle(transform.eulerAngles.y, randomRot, turnSpeed),
            transform.eulerAngles.x);

        float angleToTurn = randomRot - transform.eulerAngles.y;
        if (angleToTurn >= 5)
        {
            myAnim.SetFloat("Turn", 0.8f);
            myAnim.PlayInFixedTime("Grounded", 0, degreesTurned / turnSpeed);
        }
        else
            myAnim.SetFloat("Turn", 0);
    }

    private void OnAnimatorMove()
    {
        myAgent.isStopped = !npcRotate;
        transform.position = myAgent.nextPosition;
    }

    private float randomRot;
    private float degreesTurned;

    void generateRandomNum()
    {
        if (npcRotate)
        {
            degreesTurned = Random.Range(30, 89);
            randomRot = transform.eulerAngles.y + degreesTurned;
        }
    }
}