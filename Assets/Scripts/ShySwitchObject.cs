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
    [PropertyOrder(10)]
    public ShyEvent onToOffEvent = new ShyEvent("On OnToOff");
    [PropertyOrder(10)]
    public ShyEvent offToOnEvent = new ShyEvent("On OffToOn");
    #endregion

    protected override void SetShyEventParent()
    {
        
        base.SetShyEventParent();
        onToOffEvent.component = this;
        offToOnEvent.component = this;
    }

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

        
        if(switchState == SwitchState.ON)
        {
            onToOffEvent.Invoke();
        }
        else if(switchState == SwitchState.OFF)
        {
            offToOnEvent.Invoke();
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

}
