using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShyEvent
{
    [BoxGroup, LabelText("$ShyEventNameLabel")]
    public UnityEventWithGameObject unityEvent = new UnityEventWithGameObject();    
    [BoxGroup, InlineButton("AutoAssign", "Auto"), InlineButton("SelfAssign", "Self")]
    public PlayMakerFSM fsm;
    [BoxGroup,  ValueDropdown("EventList")]
    public string fsmEventSelect;

    [BoxGroup,  InlineButton("AddFsmEvent", "Add")]
    public string fsmEventAdd;

    public List<string> EventList()
    {
        List<string> events = new List<string>();
        events.Add("None");
        if(fsm)
        {
            foreach (var ev in fsm.FsmEvents)
            {
                events.Add(ev.Name);
            }
        }       
        return events;
    }

    [HideInInspector]
    public MonoBehaviour component;
    string ShyEventNameLabel;

    public void Invoke()
    {
        unityEvent.Invoke(component.gameObject);

        if(fsm)
            fsm.MySendEventToAll(fsmEventSelect);
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

    void AddFsmEvent()
    {
        if (!fsm)
            return;

        if (fsmEventAdd.Trim() == "")
            return; 
       
        if (!fsm.Fsm.HasEvent(fsmEventAdd))
        {
            var events = fsm.Fsm.Events;
            var newArray = new HutongGames.PlayMaker.FsmEvent[events.Length + 1];
            for (int i = 0; i < events.Length; i++)
            {
                newArray[i] = events[i];
            }
            newArray[newArray.Length - 1] = new HutongGames.PlayMaker.FsmEvent(fsmEventAdd);
            fsm.Fsm.Events = newArray;

        }

        fsmEventSelect = fsmEventAdd;
        fsmEventAdd = "";
    }
}
