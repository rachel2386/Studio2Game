using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Utility;


//To-do:

//Change speed of npc when approaching player
//add waving animation 
//turning animation
//sound
public class NPCBehavior : MonoBehaviour
{
    private NavMeshAgent myAgent;
    private Animator myAnim;
    private Vector3 steeringTargetLastFrame;

    private bool goingForward = true;
    private bool npcRotate = true;
    private bool greetedPlayer = false;

    public bool NpcRotate
    {
        get => npcRotate;
        set => npcRotate = value;
    }

    public bool GreetedPlayer
    {
        get => greetedPlayer;
        set => greetedPlayer = value;
    }

    public bool PlayerTriggered { get; set; } = false;

    private Vector3 AgentOffset;
    private Vector3 DestinationOffset;
    private float turnSpeed = 2f;

    public Transform myDestination;
    private Transform playerTransform;
    [Header(" NPC Type:")] public bool Trigger;
    public bool Automatic;
    public bool Stationary;


    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        AgentOffset = myAgent.transform.position;
        // print("agent position =" + transform.position);
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
                {
                    if(NpcRotate)
                    myAgent.SetDestination(myDestination.position);
                }

                
            }
            else if (Automatic)
            {
                // print("agent position updated=" + transform.position);
                if(npcRotate)
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
                    transform.LookAt(playerTransform.position);
            }
    
        myAgent.updatePosition = false;
        myAgent.updateRotation = true;
       
        var speed = myAgent.desiredVelocity.magnitude;
       //update animator 
        myAnim.SetFloat(Forward, speed);
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
        //if (Mathf.Abs(angleToTurn) >= 5)
       // {
           //myAnim.SetFloat("Turn", angleToTurn/180);
           // myAnim.PlayInFixedTime("Grounded", 0, degreesTurned / turnSpeed);
       //}
       // else
       //     myAnim.SetFloat("Turn", 0);
    }

    public void FollowPlayer()
    {
        NpcRotate = false;
        if (!GreetedPlayer)
        {
          //  myDestination.position = playerTransform.position;
            //myAgent.CalculatePath(myDestination.position,myAgent.path);
            if(!myAgent.pathPending && myAgent.destination != playerTransform.position)
                myAgent.ResetPath();
                
            myAgent.SetDestination(playerTransform.position);
           if (!myAgent.pathPending && myAgent.remainingDistance <= 1f)
            {
                myAgent.ResetPath();
                myAgent.SetDestination(myDestination.position);
                GreetedPlayer = true;
            }
        }
        else
        {
           // myDestination.position = DestinationOffset;
            //myAgent.CalculatePath(myDestination.position,myAgent.path);
        }
        // 

    }


    private void OnAnimatorMove()
    {
        if (!npcRotate && GreetedPlayer) 
            myAgent.Stop();
        else myAgent.Resume();
        transform.position = myAgent.nextPosition;
    }

    private float randomRot;
    private float degreesTurned;
    private static readonly int Forward = Animator.StringToHash("Forward");

    void generateRandomNum()
    {
        if (npcRotate)
        {
            degreesTurned = Random.Range(30, 89);
            randomRot = transform.eulerAngles.y + degreesTurned;
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}