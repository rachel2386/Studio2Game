using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyMouseExcludedEvent : MonoBehaviour
{
    public GameObject[] excludeList;

    Camera cam;

    public string excludedClick;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        bool currentIsOn = IsMouseOnExcludeList();
        if (!currentIsOn && LevelManager.Instance.PlayerActions.Fire.WasPressed)
        {
            DoExcludedClicked();
        }
    }

    void DoExcludedClicked()
    {
        gameObject.MySendEventToAll(excludedClick);
    }

    bool IsMouseOnExcludeList()
    {        
        bool ret = false;
        // Debug.Log(Input.mousePosition);
        RaycastHit[] hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        hit = Physics.RaycastAll(ray, 100);

        foreach (var h in hit)
        {
            var go = h.collider.gameObject;
            
            foreach(var goInList in excludeList)
            {
                if (goInList == go)
                    return true;
            }
        }

        return ret;
    }
}
