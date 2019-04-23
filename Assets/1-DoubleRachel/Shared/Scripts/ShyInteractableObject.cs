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
        INVALID
    }

    public enum Tooltip
    {
        NEED_EMPTY_HAND,
        WRONG_THING,
        HAND_IS_EMPTY,
        IS_INVALID
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
    [PropertyOrder(-9)]
    public string invalidTooltip;




    [FoldoutGroup("Validation"), PropertyOrder(9), EnableIf("NeedValidateModeButton"), EnumToggleButtons]
    public ValidationMode validationMode = ValidationMode.NEED_EMPTY;
    [FoldoutGroup("Validation"), PropertyOrder(9), ShowIf("NeedValidateHeld")]
    public List<string> keyWords;

    #region Clicked Event
    // If the object is clickable and we found a click
    // send a click event
    [PropertyOrder(10)]
    public ShyEvent clickedEvent = new ShyEvent("On Clicked");
    #endregion

    [OnInspectorGUI]
    protected virtual void SetShyEventParent()
    {
        clickedEvent.component = this;
    }

    protected ShyInteractionSystem sis;

    // Start is called before the first frame update
    protected void Start()
    {
        sis = FindObjectOfType<ShyInteractionSystem>();
        SetShyEventParent();
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
        else if(validationMode == ValidationMode.INVALID)
        {
            return GetTooltipString(Tooltip.IS_INVALID);
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
        else if(tooltip == Tooltip.IS_INVALID)
        {
            ret = invalidTooltip.Length == 0 ? "?" : invalidTooltip;
        }

        return ret;
    }

    public virtual string GetNeedEmptyHandTooltip()
    {
        return ">_<";
    } 
    
    public virtual void Clicked()
    {
        bool validate = Validate();
        if (!validate)
            return;
        
        clickedEvent.Invoke();        
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
        else if(validationMode == ValidationMode.INVALID)
        {
            return false;
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
