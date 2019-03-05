using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUpDialogue : MonoBehaviour
{
    public float sphereRadius = 1.5f;

   private Rigidbody playerRB;

   private Text dialogueTxt;
   private GameObject DialogueBubble;
   private string message;
   public float letterTime;
   public AudioClip dialogue;
   private AudioSource audioSource;
   private bool textPlayed = false;

   private ParticleSystem txtParticles;
   private Canvas myCanvas;

   private NPCFollowOnOverlap followPlayer;

  public static bool checkSceneSwitched = false;
    
    void Start()
    {

        if (!checkSceneSwitched)
            playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        else
            playerRB = null;
        
        followPlayer = GetComponent<NPCFollowOnOverlap>();
        _isfollowPlayerNotNull = followPlayer != null;
       
        txtParticles = GetComponentInChildren<ParticleSystem>();
        txtParticles.transform.localPosition = new Vector3(0,1.35f,0.06f);
        txtParticles.transform.localEulerAngles = Vector3.zero;

        myCanvas = FindObjectOfType<Canvas>();
        DialogueBubble = transform.Find("Dialogue").gameObject;//GetComponentInChildren<Image>().gameObject;
        DialogueBubble.SetActive(false);
        
        
        dialogueTxt = DialogueBubble.GetComponentInChildren<Text>();
        message = dialogueTxt.text;
        dialogueTxt.text = "";

        audioSource = GetComponent<AudioSource>();

    }

    private bool addingTxt = false;

    private bool reducedSpeed = false;

    private bool _isfollowPlayerNotNull;

    private bool isFollowing = false;

    // Update is called once per frame
    void Update()
    {
        
        if (DetectPlayer())
        {
            
            if(!txtParticles.isPlaying)
                txtParticles.Play();
            
            
            ReducePlayerSpeed(reducedSpeed);
            
            if (!textPlayed)
            {
             
               reducedSpeed = true;
              
               if (!addingTxt)
               StartCoroutine(PlayTxt()); 
               
               if (!audioSource.isPlaying)
               audioSource.PlayOneShot(dialogue, 1F);
               
               SetDialogueState(myCanvas.transform, true);
              
            }
            else
            {
                addingTxt = false;
                StopCoroutine(PlayTxt());
                reducedSpeed = false;
                dialogueTxt.text = "";
                SetDialogueState(transform, false); 
            
            }


            if (dialogueTxt.text.ToCharArray().Length == message.ToCharArray().Length) // if coroutine finishes 
            {
               
                    if (Input.GetMouseButtonDown(0))
                    {
                        textPlayed = true;
                        isFollowing = true;
                        if(gameObject.CompareTag("Respawn"))
                            SwitchScene();
                    }
              
            }
            
            
        }
        
        else
        {
            if (_isfollowPlayerNotNull)
            {
                if(followPlayer.DetectPlayer())
                    if (isFollowing)
                        textPlayed = true;
            }
            else
            {
                textPlayed = false;
                txtParticles.Stop();
            }

            
        }

       
    }

    bool DetectPlayer()
    {
        bool playerDetected = false;
        
        Collider[] stuffInSphere = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider t in stuffInSphere)
        {
            if (t.gameObject.CompareTag("Player"))
                playerDetected = true;
           
//            playerDetected = t.gameObject.CompareTag("Player");
//            if (playerDetected)
//                break;
        }

        Array.Clear(stuffInSphere, 0, stuffInSphere.Length);
        
        return playerDetected;
    }

    void SetDialogueState(Transform parent, bool activeState)
    {
        DialogueBubble.transform.SetParent(parent);
        DialogueBubble.SetActive(activeState);
        DialogueBubble.transform.localPosition = new Vector3(0, -100, 0);
        DialogueBubble.transform.localEulerAngles = new Vector3(0,0,0);
        DialogueBubble.transform.localScale = new Vector3(1,1,1);
    }

    void ReducePlayerSpeed(bool speedReduced)
    {
        if (speedReduced)
        playerRB.velocity = Vector3.zero;
     
    }
    
   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position,sphereRadius);
        
    }

   IEnumerator PlayTxt()
   {
       addingTxt = true;
        foreach( char letter in message.ToCharArray())
         {
             dialogueTxt.text += letter;
             yield return  new WaitForSecondsRealtime(letterTime);
         }


    }

   void SwitchScene()
   {
       checkSceneSwitched = true;
       SceneManager.LoadScene(1);
   }
  
}
