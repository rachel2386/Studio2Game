using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Utility;


//To-do:
//turning animation

public class NPCBehavior : MonoBehaviour
{
    #region Navmesh Agent Properties

    private NavMeshAgent myAgent;
    public Transform myDestination;
    private Vector3 steeringTargetLastFrame;
    private Vector3 AgentOffset;
    private Vector3 DestinationOffset;
    private Transform playerTransform;
    private float AgentinitSpeed;

    #endregion

    #region Animator Properties

    private Animator myAnim;
    private static readonly int Forward = Animator.StringToHash("Forward");
    private static readonly int HandWave = Animator.StringToHash("HandWave");

    #endregion

    #region Agent Types

    [Header(" NPC Type:")] public bool Trigger;
    public bool Automatic;
    public bool Stationary;

    #endregion

    private bool goingForward = true;
    private bool _greetedPlayer = false;
    private bool _npcRotate = true;
    private float approachPlayerSpeed = 1f;
    public bool NpcRotate
    {
        private get { return _npcRotate; }
        set { _npcRotate = value; }
    }

    public bool GreetedPlayer
    {
        get => _greetedPlayer;
        set => _greetedPlayer = value;
    }

    public bool PlayerTriggered { get; set; } = false;


    private float turnSpeed = 2f;


    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        AgentOffset = myAgent.transform.position;
        // print("agent position =" + transform.position);
        DestinationOffset = myDestination.position;
        playerTransform = GameObject.Find("PlayerBase").transform;
        AgentinitSpeed = myAgent.speed;
        if (!Stationary) return;
        randomRot = transform.eulerAngles.y;
        InvokeRepeating(nameof(generateRandomNum), 6, 7);
    }

    // Update is called once per frame

    
    
    void Update()
    {
        if (NpcRotate)
        {
            AgentBehavior();
        }
        else
        {
            FollowPlayer();   
        }
        
       AnimatorUpdate();
 
    }

    void AgentBehavior()
    {
        myAgent.updatePosition = false;
        myAgent.updateRotation = true;
        
        if (Trigger)
        {
            if (PlayerTriggered)
            {
                myAgent.SetDestination(myDestination.position);
                if (!myAgent.pathPending && myAgent.remainingDistance <= 0.5f)
                    myAgent.isStopped = true;
             }
           
        }

        if (Automatic)
        {
            myAgent.SetDestination(goingForward ? DestinationOffset : AgentOffset);
            //walking back and forth two points
            if (!myAgent.pathPending && myAgent.remainingDistance <= 0.5f)
            {
                goingForward = !goingForward;
            }
        }

        if (Stationary)
        {
          
                myAgent.SetDestination(myDestination.position);
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

       
    }

    void NPCSpeed()
    {

//        if (!NpcRotate && GreetedPlayer || myAgent.pathPending ) // pathPending
//            myAgent.isStopped = true;

        if (!myAgent.pathPending)
        {
            if (NpcRotate)
            {
                myAgent.isStopped = false;
                myAgent.speed = AgentinitSpeed;
            }
            else
            {
                if ( !GreetedPlayer)
                {
                   myAgent.speed = approachPlayerSpeed;
                }
                else
                    myAgent.isStopped = true;
           
            }
        }
        else
            myAgent.isStopped = true;




    }

    private void FollowPlayer()
    {
       
        transform.rotation = Quaternion.LookRotation(playerTransform.position - transform.position, Vector3.up);
        if (GreetedPlayer) return;
        if (myAgent.destination != playerTransform.position)
        {
            myAgent.ResetPath();
            myAgent.SetDestination(playerTransform.position);
        }
       if (Vector3.Distance(playerTransform.position,transform.position) <= 1.3f)
        {
            GreetedPlayer = true;
        }



    }
 
    private void AnimatorUpdate()
    {
        myAnim.SetFloat(Forward, myAgent.desiredVelocity.magnitude);
        
        if(!NpcRotate && !GreetedPlayer)
            myAnim.SetBool(HandWave, true);
        else
            myAnim.SetBool(HandWave,false);
            
        
            
    }

    private void OnAnimatorMove()
    {
        
        transform.position = myAgent.nextPosition;
        NPCSpeed();
       
    }

    private float randomRot;
    private float degreesTurned;

    void generateRandomNum()
    {
        if (NpcRotate)
        {
            degreesTurned = Random.Range(30, 89);
            randomRot = transform.eulerAngles.y + degreesTurned;
        }
    }

  
}