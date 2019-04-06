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
    #region Navmesh Agent Properties

    private NavMeshAgent myAgent;
    public Transform myDestination;
    private Vector3 steeringTargetLastFrame;
    private Vector3 AgentOffset;
    private Vector3 DestinationOffset;
    private Transform playerTransform;

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
                if (NpcRotate)
                    myAgent.SetDestination(myDestination.position);
            }
        }
        else if (Automatic)
        {
            // print("agent position updated=" + transform.position);
            if (NpcRotate)
                myAgent.SetDestination(goingForward ? DestinationOffset : AgentOffset);
            //walking back and forth two points
            if (!myAgent.pathPending && myAgent.remainingDistance <= 0.5f)
            {
                goingForward = !goingForward;
            }
        }
        else if (Stationary)
        {
            if (NpcRotate)
            {
                AutoRotate();
                myAgent.SetDestination(myDestination.position);
            }

            
//            else
//                transform.LookAt(playerTransform.position);
        }

        myAgent.updatePosition = false;
        myAgent.updateRotation = true;

        AnimatorUpdate();

        if (!NpcRotate)
        {
            transform.LookAt(playerTransform);
            FollowPlayer();   
        }
        else
            myAnim.SetBool(HandWave, false);
       
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
       
        if (!GreetedPlayer)
        {
            if (myAgent.destination != playerTransform.position)
            {
                myAgent.ResetPath();
                myAgent.SetDestination(playerTransform.position);
            }
            myAnim.SetBool(HandWave, true);
            
            if (!myAgent.pathPending && myAgent.remainingDistance <= 1f)
            {
                    myAnim.SetBool(HandWave, false);
                    myAgent.SetDestination(myDestination.position);
                    GreetedPlayer = true;
            }
        }
       
       
      
    }
 
    private void AnimatorUpdate()
    {
       
        myAnim.SetFloat(Forward, myAgent.desiredVelocity.magnitude);
       
    }

    private void OnAnimatorMove()
    {
       if (!NpcRotate && GreetedPlayer || myAgent.pathPending)
            myAgent.isStopped = true;
        else
            myAgent.isStopped = false;
        
        transform.position = myAgent.nextPosition;
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