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
        active = true;
        menu.LockerClicked(this);
    }

    public void OthersClicked()
    {
        active = false;
    }
    
    
}
