using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerMenu : MonoBehaviour
{
    public Locker[] lockers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LockerClicked(Locker locker)
    {
        foreach(Locker l in lockers)
        {
            if (l != locker)
                l.OthersClicked();
        }
    }
}
