using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyMouseEvent : MonoBehaviour
{

    public GameObject target;
    public string enterEvent;
    public string exitEvent;

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame


    bool lastMouseOn = false;

    bool isMouseOn()
    {
        bool ret = false;
        // Debug.Log(Input.mousePosition);
        RaycastHit[] hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        hit = Physics.RaycastAll(ray, 100);

        foreach (var h in hit)
        {
            var go = h.collider.gameObject;
            if (gameObject == go)
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    void Update()
    {
        bool currentIsOn = isMouseOn();

        if(lastMouseOn && !currentIsOn)
        {
            DoExit();
        }
        else if(!lastMouseOn && currentIsOn)
        {
            DoEnter();
        }

        lastMouseOn = currentIsOn;
    }

    

    void DoExit()
    {
        target.MySendEventToAll(exitEvent);
    }

    void DoEnter()
    {
        target.MySendEventToAll(enterEvent);
    }
}
