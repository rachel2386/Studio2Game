using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDialogue : MonoBehaviour
{
    public float sphereRadius = 1.5f;

   private Rigidbody playerRB;

   private Text dialogueTxt;
   public GameObject DialogueBubble;
   private string message;
   public float letterTime;
   private bool textPlayed = false;

   private ParticleSystem txtParticles;
   private Canvas myCanvas;

   private NPCFollowOnOverlap followPlayer;
   
    
    void Start()
    {
       
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        
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
        
    }

    private bool addingTxt = false;

    private bool reducedSpeed = false;

    private bool _isfollowPlayerNotNull;

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
               
               //setUItocanvas
               SetDialogueState(myCanvas.transform, true);
              
            }
            else
            {
                StopCoroutine(PlayTxt());
                reducedSpeed = false;
                dialogueTxt.text = "";
                SetDialogueState(myCanvas.transform, false); 
            }


            if (dialogueTxt.text.ToCharArray().Length == message.ToCharArray().Length) // if coroutine finishes 
            {
               
                    if (Input.GetMouseButtonDown(0))
                    {
                        textPlayed = true;
                   
                    }
              
            }
            
            
        }
//        
//        else  if (_isfollowPlayerNotNull)
//        {
//            if(followPlayer.DetectPlayer())
//                textPlayed = true;
//        }
        else
        {
            addingTxt = false;
            textPlayed = false;
            txtParticles.Stop();
        }

       
    }

    bool DetectPlayer()
    {
        bool playerDetected = false;
        
        Collider[] stuffInSphere = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider t in stuffInSphere)
        {
            playerDetected = t.gameObject.CompareTag("Player");
        }

        Array.Clear(stuffInSphere, 0, stuffInSphere.Length);
        
        return playerDetected;
    }

    void SetDialogueState(Transform parent, bool activeState)
    {
        DialogueBubble.transform.SetParent(parent);
        DialogueBubble.SetActive(activeState);
        DialogueBubble.transform.localPosition = new Vector3(0, -90, 0);
        DialogueBubble.transform.localEulerAngles = new Vector3(0,0,0);
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
  
}
