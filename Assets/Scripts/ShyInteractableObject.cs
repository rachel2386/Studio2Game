using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ShyInteractableObject : MonoBehaviour
{
    // the string 
    public string tooltip;

    public event Action<ShyInteractableObject> OnClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void CheckIfClickedInUpdate()
    {  

    }

    public virtual void Clicked()
    { 
        if(OnClicked != null)
            OnClicked.Invoke(this);
    }

    
}
