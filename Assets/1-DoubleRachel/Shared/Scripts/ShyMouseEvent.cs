﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyMouseEvent : MonoBehaviour
{
    public bool inUse = true;
    public GameObject target;
    public string enterEvent;
    public string exitEvent;
    public string clickedEvent;

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


        if(currentIsOn && LevelManager.Instance.PlayerActions.Fire.WasPressed)
        {
            DoClicked();
        }

        lastMouseOn = currentIsOn;
    }

    

    void DoExit()
    {
        if (!inUse)
            return;

        target.MySendEventToAll(exitEvent);
    }

    void DoEnter()
    {
        if (!inUse)
            return;

        target.MySendEventToAll(enterEvent);
    }

    void DoClicked()
    {
        if (!inUse)
            return;

        target.MySendEventToAll(clickedEvent);
    }

    public void TestHaha()
    {
        Debug.Log("Haha");
    }
}
