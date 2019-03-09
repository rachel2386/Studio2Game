using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShySwitchObject : ShyInteractableObject
{
    public enum SwitchState
    {
        ON,
        OFF
    }

    public SwitchState switchState;

    #region Switch Event
    [FoldoutGroup("Switch Event"), PropertyOrder(10)]
    public UnityEventWithShyObject onToOffEvent = new UnityEventWithShyObject();
    [FoldoutGroup("Switch Event"), PropertyOrder(10)]
    public UnityEventWithShyObject offToOnEvent = new UnityEventWithShyObject();

    // Playmaker Related
    [FoldoutGroup("Switch Event"), PropertyOrder(10), 
        InlineButton("SelfAssignFsmToSwitchEvent", "Self"), 
        InlineButton("AutoAssignFsmToSwitchEvent", "Auto")]
    public PlayMakerFSM switchMsgFsm;

    List<string> switchEventNames = new List<string>();
    private List<string> SwitchEventNames
    {
        get
        {
            UpdateEventNames(switchMsgFsm, switchEventNames);
            return switchEventNames;
        }
    }
    [FoldoutGroup("Switch Event"), PropertyOrder(10), ValueDropdown("SwitchEventNames")]
    public string onToOffMsgEvent;
    [FoldoutGroup("Switch Event"), PropertyOrder(10), ValueDropdown("SwitchEventNames")]
    public string offToOnMsgEvent;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void HandleInteraction()
    {
        base.HandleInteraction();

        bool wasPressed = LevelManager.Instance.PlayerActions.Fire.WasPressed;
        if (wasPressed)
            SwitchClicked();
    }

    void SwitchClicked()
    {
        // In fact I think there is no need to validate here cuz it's ValidationMode.NONE
        // but to make it more uniform, I still make it here
        bool val = Validate();
        if (!val)
            return;

        string eventName = "";
        if(switchState == SwitchState.ON)
        {
            eventName = onToOffMsgEvent;
            onToOffEvent.Invoke(this);
        }
        else if(switchState == SwitchState.OFF)
        {
            eventName = offToOnMsgEvent;
            offToOnEvent.Invoke(this);
        }


        if (switchMsgFsm)
        {
            switchMsgFsm.MySendMessageToAll(eventName);
        }

        SwapSwitchState();
    }

    void SwapSwitchState()
    {
        if (switchState == SwitchState.ON)        
            switchState = SwitchState.OFF;        
        else
            switchState = SwitchState.ON;
    }
    

    protected override bool NeedValidateModeButton()
    {
        validationMode = ValidationMode.NONE;
        return false;
    }

    void AutoAssignFsmToSwitchEvent()
    {
        AutoAssignFsm(out switchMsgFsm);
    }

    void SelfAssignFsmToSwitchEvent()
    {
        SelfAssignFSM(out switchMsgFsm);
    }
}
