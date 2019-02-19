using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachedGoal : MonoBehaviour
{
    // Start is called before the first frame update
    Animator MyAnim;
    private AudioSource[] myAudios;
   
   private bool _doorOpen = false;
   private bool _doorClosed = false;
    void Start()
    {
        
        MyAnim = GetComponent<Animator>();
       myAudios = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            _doorOpen = true;
            _doorClosed = false;


        }
        
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _doorClosed = true;
            _doorOpen = false;

        }
        
    }

    private void Update()
    {
       Debug.Log("DoorOpen = " + _doorOpen);
       Debug.Log("DoorClose = " + _doorClosed);
        if (_doorOpen)
        {
            
            if (!myAudios[0].isPlaying)
            {
                myAudios[0].Play();
               

            }  
        }

        

        if (_doorClosed)
        {
           
            if (!myAudios[1].isPlaying)
            {
                myAudios[1].Play();
                
            }
        }
        
        MyAnim.SetBool("DoorOpen",_doorOpen);
        MyAnim.SetBool("DoorClose", _doorClosed);
    }
}
