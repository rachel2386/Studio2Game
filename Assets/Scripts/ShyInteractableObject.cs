using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ShyInteractableObject : MonoBehaviour
{
    public bool canInteract = true;
    // the string 
    public string tooltip;
        
    public event Action<ShyInteractableObject> OnClicked;

    protected ShyInteractionSystem sis;

    // Start is called before the first frame update
    protected void Start()
    {
        sis = FindObjectOfType<ShyInteractionSystem>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void CheckIfClickedInUpdate()
    {  

    }


    public virtual string GetTooltip()
    {
        return tooltip;
    }

    public virtual void Clicked()
    { 
        if(OnClicked != null)
            OnClicked.Invoke(this);
    }

    
}
