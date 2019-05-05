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
    public bool Clapping;
    public bool Cheering;
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
            Clapping = false;
            Cheering = false;
        }
        else if (Texting)
        {
            Talking = false;
            Clapping = false;
            Cheering = false;
            Laughing = false;
        }
        else if (Talking)
        {
            
            Texting = false;
            Clapping = false;
            Cheering = false;
            Laughing = false;
            
        }
        else if(Clapping)
        {
            Talking = false;
            Texting = false;
            Cheering = false;
            Laughing = false;
        }
        else if (Cheering)
        {
            Talking = false;
            Texting = false;
            Clapping = false;
            Laughing = false;
        }

        phone.SetActive(Texting);

        myAnim.SetBool("Laughing", Laughing);
        myAnim.SetBool("Texting", Texting);
        myAnim.SetBool("Talking", Talking);
        myAnim.SetBool("Clapping", Clapping);
        myAnim.SetBool("Cheering", Cheering);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
