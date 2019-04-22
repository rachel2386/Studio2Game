using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    bool active = false;
    LockerMenu menu;

    public ShyMouseEvent[] innerObjects;

    // Start is called before the first frame update
    void Start()
    {
        menu = FindObjectOfType<LockerMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInnerObjectInteractiveState();
    }

    void UpdateInnerObjectInteractiveState()
    {
        foreach(var sme in innerObjects)
        {
            sme.inUse = active;
        }
    }

    public void Clicked()
    {
        if (!active)
            gameObject.MySendEventToAll("LOCKER_CLICKED");

        if (active)
            gameObject.MySendEventToAll("ACTIVE_CLICKED");

        active = true;
        menu.LockerClicked(this);

       
    }

    public void OthersClicked()
    {
        if (active)
            gameObject.MySendEventToAll("LOCKER_OTHER_CLICKED");

        active = false;        
    }
    
    
}
