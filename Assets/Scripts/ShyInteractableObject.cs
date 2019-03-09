using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventWithShyObject : UnityEvent<ShyInteractableObject>
{

}

[Serializable]
public class UnityEventWithGameObject : UnityEvent<GameObject>
{

}


public class ShyInteractableObject : MonoBehaviour
{

    
    public enum ValidationMode
    {
        NEED_EMPTY,
        NEED_HELD,
        NONE,
    }

    public enum Tooltip
    {
        NEED_EMPTY_HAND,
        WRONG_THING,
        HAND_IS_EMPTY,
    }

    [PropertyOrder(-10)]
    public bool canInteract = true;
    // the string 
    [PropertyOrder(-9)]
    public string defaultTooltip;
    [PropertyOrder(-9)]
    public string emptyHandTooltip;
    [PropertyOrder(-9)]
    public string wrongThingTooltip;

   
    #region Clicked Event
   
    [FoldoutGroup("Validation"), PropertyOrder(9), EnableIf("NeedValidateModeButton"), EnumToggleButtons]
    public ValidationMode validationMode = ValidationMode.NEED_EMPTY;
    [FoldoutGroup("Validation"), PropertyOrder(9), ShowIf("NeedValidateHeld")]
    public List<string> keyWords;

        
    // If the object is clickable and we found a click
    // send a click event
    [FoldoutGroup("Clicked Event"), PropertyOrder(10)]
    public UnityEventWithShyObject clickedEvent = new UnityEventWithShyObject();

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
        if (validationMode == ValidationMode.NONE)
            return defaultTooltip;
        else if (validationMode == ValidationMode.NEED_EMPTY)
        {
            if (!IsEmptyHand())                
                return GetTooltipString(Tooltip.NEED_EMPTY_HAND);
        }
        else if (validationMode == ValidationMode.NEED_HELD)
        {
            if (IsEmptyHand())
                return GetTooltipString(Tooltip.HAND_IS_EMPTY); 
            else if(!IsHeldObjectNeeded())
                return GetTooltipString(Tooltip.WRONG_THING);
        }

        return defaultTooltip;
    }

    

    public string GetTooltipString(Tooltip tooltip)
    {
        String ret = "";
        if(tooltip == Tooltip.NEED_EMPTY_HAND)
        {
            ret = GetNeedEmptyHandTooltip();
        }
        else if(tooltip == Tooltip.WRONG_THING)
        {
            ret = wrongThingTooltip.Length == 0 ? "Can't work" : wrongThingTooltip;
        }
        else if (tooltip == Tooltip.HAND_IS_EMPTY)
        {
            ret = emptyHandTooltip.Length == 0 ? "Need something" : emptyHandTooltip;
        }

        return ret;
    }

    public virtual string GetNeedEmptyHandTooltip()
    {
        return "There is already something in hand!";
    } 
    
    public virtual void Clicked()
    {
        bool validate = Validate();
        if (!validate)
            return;
        
        clickedEvent.Invoke(this);

        if (clickedMsgFsm)
        {
            clickedMsgFsm.MySendMessageToAll(clickedMsgEvent);
        }
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

    protected void SelfAssignFSM(out PlayMakerFSM targetFSM)
    {
        targetFSM = this.GetComponent<PlayMakerFSM>();
    }

    public virtual bool Validate()
    {
        bool ret = true;
        var curHeld = sis.curHeldObject;

        if (validationMode == ValidationMode.NONE)
        {
            ret = true;
        }
        else if(validationMode == ValidationMode.NEED_EMPTY)
        {
            ret = IsEmptyHand();
        }
        else if (validationMode == ValidationMode.NEED_HELD)
        {
            ret = IsHeldObjectNeeded();
        }           

        return ret;
    }

    public bool IsEmptyHand()
    {
        return sis.IsEmptyHand();
    }

    bool NeedValidateHeld()
    {
        return validationMode == ValidationMode.NEED_HELD;
    }

    protected virtual bool NeedValidateModeButton()
    {
        return true;
    }

    public bool IsHeldObjectNeeded()
    {
        bool ret = false;
        var curHeld = sis.curHeldObject;

        if (!curHeld)
            return false;

        var inputName = curHeld.name.ToLower();
        foreach (var keyWord in keyWords)
        {
            if (inputName.Contains(keyWord.ToLower()))
                return true;
        }

        return ret;
    }
    
}
