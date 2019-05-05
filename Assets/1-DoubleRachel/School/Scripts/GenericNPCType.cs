using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericNPCType : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("NPC Action")] 
    public bool Laughing;
    public bool Talking;
    public bool Texting;
    private Animator myAnim;
    public GameObject phone;
    void Start()
    {
        myAnim = GetComponent<Animator>();
        phone.SetActive(false);
        
        if (Laughing)
        {
            Talking = false;
            Texting = false;
            myAnim.SetBool("Laughing", true);
        }
        else if (Texting)
        {
            Talking = false;
            Laughing = false;
            myAnim.SetBool("Texting", true);
            phone.SetActive(true);
        }
        else if (Talking)
        {
            Laughing = false;
            Texting = false;
            myAnim.SetBool("Talking", true);
        }
        else
        {
            phone.SetActive(false);
            myAnim.SetBool("Laughing", false);
            myAnim.SetBool("Texting", false);
            myAnim.SetBool("Talking", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
