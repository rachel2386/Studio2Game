using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyMouseEvent : MonoBehaviour
{
    public bool inUse = true;
    public GameObject checkRayObject = null;
    public GameObject target;
    public string enterEvent;
    public string exitEvent;

    public string hoverEvent;
    public string notHoverEvent;

    public string clickedEvent;
    public string releasedEvent;
    public string anyClickEvent;

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

        if (!inUse)
            return false;

        bool ret = false;
        // Debug.Log(Input.mousePosition);
        RaycastHit[] hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        hit = Physics.RaycastAll(ray, 100);

        GameObject check = checkRayObject == null ? gameObject : checkRayObject;

        foreach (var h in hit)
        {
            var go = h.collider.gameObject;
            if (go == check)
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

        if(!currentIsOn)
        {
            DoNotHover();
            if(lastMouseOn)
                DoExit();
        }
        else if(currentIsOn)
        {
            DoHover();
            if(!lastMouseOn)
                DoEnter();
        }

        if(LevelManager.Instance.PlayerActions.Fire.WasPressed)
        {
            DoAnyClick();
        }

        if(currentIsOn && LevelManager.Instance.PlayerActions.Fire.WasPressed)
        {
            DoClicked();
        }

        if(LevelManager.Instance.PlayerActions.Fire.WasReleased)
        {
            DoRelease();
        }

        lastMouseOn = currentIsOn;
    }

    

    void DoExit()
    {
        //if (!inUse)
        //    return;

        target.MySendEventToAll(exitEvent);
    }

    void DoEnter()
    {
        //if (!inUse)
        //    return;

        target.MySendEventToAll(enterEvent);
    }

    void DoClicked()
    {
        //if (!inUse)
        //    return;

        target.MySendEventToAll(clickedEvent);
    }

    void DoRelease()
    {
        target.MySendEventToAll(releasedEvent);
    }

    public void TestHaha()
    {
        Debug.Log("Haha");
    }

    void DoAnyClick()
    {
        target.MySendEventToAll(anyClickEvent);
    }

    void DoHover()
    {
        target.MySendEventToAll(hoverEvent);
    }

    void DoNotHover()
    {
        target.MySendEventToAll(notHoverEvent);
    }
}
