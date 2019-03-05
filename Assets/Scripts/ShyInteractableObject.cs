using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShyInteractableObject : MonoBehaviour
{
    [PropertyOrder(-10)]
    public bool canInteract = true;
    // the string 
    [PropertyOrder(-9)]
    public string defaultTooltip;


    #region Clicked Event


    public event Action<ShyInteractableObject> OnClicked;
    // If the object is clickable and we found a click
    // send a click event
    [FoldoutGroup("Clicked Event"), PropertyOrder(10)]
    public UnityEvent clickedEvent = new UnityEvent();

    // Playmaker Related
    [FoldoutGroup("Clicked Event"), PropertyOrder(10), InlineButton("AutoAssignFSMToClickedEvent", "Auto")]
    public PlayMakerFSM clickedMsgFsm;

    List<string> clickedEventNames = new List<string>();
    private List<string> ClickedEventNames
    {
        get
        {
            UpdateEventNames(clickedMsgFsm, clickedEventNames);
            return clickedEventNames;
        }
    }
    [FoldoutGroup("Clicked Event"), PropertyOrder(10), ValueDropdown("ClickedEventNames")]
    public string clickedMsgEvent;
    #endregion


    protected ShyInteractionSystem sis;

    // Start is called before the first frame update
    protected void Start()
    {
        sis = FindObjectOfType<ShyInteractionSystem>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void CheckIfClickedInUpdate()
    {  

    }

    public virtual void HandleInteraction()
    {
        sis.NeedToRefreshCenterText = GetTooltip();
        bool wasPressed = LevelManager.Instance.PlayerActions.Fire.WasPressed;

        if (wasPressed)
        {
            Clicked();
        }
    }


    public virtual string GetTooltip()
    {
        return defaultTooltip;
    }

    public virtual void Clicked()
    { 
        if(OnClicked != null)
            OnClicked.Invoke(this);
    }

    protected void UpdateEventNames(PlayMakerFSM fsm, List<string> names)
    {
        names.Clear();
        names.Add("None");
        if (fsm == null)
            return;
        
        
        var events = fsm.FsmEvents;
        foreach (var ev in events)
        {
            names.Add(ev.Name);
        }
    }

    void AutoAssignFSMToClickedEvent()
    {
        AutoAssignFsm(out clickedMsgFsm);
    }

    protected void AutoAssignFsm(out PlayMakerFSM targetFsm)
    {
        targetFsm = null;
        var main = GameObject.Find("MainFSM");
        if (!main)
        {
            Debug.Log("MainFSM not found");
            return;
        }

        var fsm = main.GetComponent<PlayMakerFSM>();
        if (!fsm)
        {
            Debug.Log("No PlayMakerFSM found on MainFSM");
            return;
        }

        targetFsm = fsm;
    }


}
