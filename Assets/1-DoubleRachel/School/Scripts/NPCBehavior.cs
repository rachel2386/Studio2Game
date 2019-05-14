using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Utility;
using Yarn;


//controls npc behavior 
//3 types of npc : auto, trigger, and stationary
//bool NPCRotate should be AutoBehavior  disabled when detects player. controlled through seePlayer script and ShyDistanceTriggerOnView script



public class NPCBehavior : MonoBehaviour
{
    
    #region Navmesh Agent Properties

    private NavMeshSurface navMesh;
    private NavMeshAgent myAgent;
    
    public List<Transform> Destinations = new List<Transform>();
    private Vector3 steeringTargetLastFrame;
   
    // patrol agent
    private int DestinationIndex = 0;
    
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

    
    private bool _greetedPlayer = false;
    private bool playerTriggered;
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

    public bool PlayerTriggered
    {
        get => playerTriggered;
        set => playerTriggered = value;
    }
    private float turnSpeed = 3f;
    private Vector3 _agentRotation;
    private Vector3 _rotationLastFrame;

    public GameObject exclamationMark;

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        
        
        exclamationMark.SetActive(false);
        
        playerTransform = GameObject.Find("PlayerBase").transform;
        
        AgentinitSpeed = myAgent.speed;
        
        if (!Stationary) return;
        randomRot = transform.eulerAngles.y;
        InvokeRepeating(nameof(generateRandomNum), 6, 7);
    }

   
    void Update()
    {
        _agentRotation = transform.eulerAngles;
        if (NpcRotate)
        {
          
            AgentBehavior();
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(playerTransform.position - transform.position, Vector3.up),
                Time.deltaTime *turnSpeed);
            
            if (!GreetedPlayer)
            FollowPlayer();
           
        }
        
       AnimatorUpdate();
       _rotationLastFrame = _agentRotation;
    }

    void AgentBehavior()
    {
        myAgent.updatePosition = false;
        myAgent.updateRotation = true;
        exclamationMark.SetActive(false);
        
        if (Trigger)
        {
//            transform.rotation = Quaternion.Slerp(transform.rotation, 
//                Quaternion.LookRotation(myAgent.nextPosition - transform.position, Vector3.up),
//                Time.deltaTime *turnSpeed);
            
            if (playerTriggered)
            {
               if(myAgent.destination != Destinations[0].position)
                myAgent.SetDestination(Destinations[0].position);
               if (!myAgent.pathPending && myAgent.remainingDistance <= 0.5f)
               {
                   myAgent.isStopped = true;
                   waiting = true;
               }

            }
            else
            {
                myAgent.isStopped = true;
            }
           
        }

        else if (Automatic)
        {
            
            
//            transform.rotation = Quaternion.Slerp(transform.rotation, 
//                Quaternion.LookRotation(myAgent.nextPosition - transform.position, Vector3.up),
//                Time.deltaTime *turnSpeed);
            //patrolling between points
            if (!myAgent.pathPending && myAgent.remainingDistance <= 0.5f)
            {
                StartCoroutine(wait(Random.Range(2,4)));
                if (DestinationIndex < Destinations.Count)
                {
                    DestinationIndex++;
                    DestinationIndex = DestinationIndex % Destinations.Count;
                }

                else
                    DestinationIndex = 0;
            }
            
            if(myAgent.destination != Destinations[DestinationIndex].position)
            myAgent.SetDestination(Destinations[DestinationIndex].position);


        }

      else if (Stationary)
        {
            if(myAgent.destination != Destinations[0].position)
                myAgent.SetDestination(Destinations[0].position);
              
                Vector3 newRot = new Vector3(transform.eulerAngles.x,
                    Mathf.LerpAngle(transform.eulerAngles.y, randomRot, Time.deltaTime * turnSpeed),
                    transform.eulerAngles.z);
                
                transform.eulerAngles = newRot;
             
               //Test Methods that didn't work out
               
               //Vector3 newDir = (transform.forward * Mathf.Cos(angleToTurn)).normalized;
               // Debug.DrawRay(transform.position,newDir,Color.blue);
                
                //float dotPrdt = Vector3.Dot(transform.forward, newDir);
               // print(dotPrdt);
               
                //if (Mathf.Abs(angleToTurn) >= 5)
                // {
                //myAnim.SetFloat("Turn", angleToTurn/180);
                // myAnim.PlayInFixedTime("Grounded", 0, degreesTurned / turnSpeed);
                //}
                // else
                //     myAnim.SetFloat("TurnAmount", 0);

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
                if (!waiting)
                {
                    myAgent.isStopped = false;
                    myAgent.speed = AgentinitSpeed;
                }
                else
                    myAgent.isStopped = true;
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
        {
            myAgent.isStopped = true;
            
        }

        




    }

    private void FollowPlayer()
    {
       
       
        if (myAgent.destination != playerTransform.position)
        {
            myAgent.ResetPath();
            myAgent.SetDestination(playerTransform.position);
        }
        exclamationMark.SetActive(true);
        
       if (Vector3.Distance(playerTransform.position,transform.position) <= 1.3f)
        {
            exclamationMark.SetActive(false);
            GreetedPlayer = true;
        }



    }
 
    private void AnimatorUpdate()
    {
        myAnim.SetFloat(Forward, myAgent.desiredVelocity.magnitude);
//        myAnim.SetFloat("Turn",(Mathf.Clamp(Mathf.Atan2(myAgent.desiredVelocity.x,myAgent.desiredVelocity.z),-1f,1f)),
//                                                0.1f,Time.deltaTime);
       
        if(!NpcRotate && !GreetedPlayer)
            myAnim.SetBool(HandWave, true);
        else
            myAnim.SetBool(HandWave,false);
        
        if(!NpcRotate && GreetedPlayer)
            myAnim.SetBool("Talking", true);
        else
            myAnim.SetBool("Talking", false);

        



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
            print("turning"); 
            degreesTurned = Random.Range(30, 89);
            randomRot = transform.eulerAngles.y + degreesTurned;
           myAnim.SetTrigger("TurnTrigger");
                //float angleToTurn = randomRot - transform.eulerAngles.y;
              //  myAnim.PlayInFixedTime("Turn", 0, angleToTurn/turnSpeed);
              //  myAnim.SetFloat("TurnAmount",angleToTurn/180);
         
        }
    }

    private bool waiting = false;
    IEnumerator wait(int seconds)
    {
        waiting = true;
        yield return new WaitForSeconds(seconds);
        waiting = false;
    }
}