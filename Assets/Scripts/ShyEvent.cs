using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShyEvent
{
    [LabelText("$ShyEventNameLabel")]
    public UnityEventWithGameObject unityEvent = new UnityEventWithGameObject();
    
    [Indent(-1), InlineButton("SelfAssign", "Self"), InlineButton("AutoAssign", "Auto")]
    public PlayMakerFSM fsm;
    [Indent(-1)]
    public string fsmEvent;

    [HideInInspector]
    public MonoBehaviour component;
    string ShyEventNameLabel;

    public void Invoke()
    {
        unityEvent.Invoke(component.gameObject);

        if(fsm)
            fsm.MySendEventToAll(fsmEvent);
    }



    public ShyEvent(string label)
    {        
        ShyEventNameLabel = label;
    }

    void SelfAssign()
    {
        fsm = component.GetComponent<PlayMakerFSM>();
    }

    void AutoAssign()
    {
        var main = GameObject.Find("MainFSM");
        if (!main)
        {
            Debug.Log("MainFSM not found");
            return;
        }

        var autoFSM = main.GetComponent<PlayMakerFSM>();
        if (!autoFSM)
        {
            Debug.Log("No PlayMakerFSM found on MainFSM");
            return;
        }

        if(autoFSM)
            fsm = autoFSM;
    }
}
