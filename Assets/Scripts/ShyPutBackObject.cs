using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShyPutBackObject : ShyInteractableObject
{

    public GameObject putBackAnchor;
    public bool canPickAfterPut = false;
    
    #region FullEvent
    // if get enough number of objects put into the putback object
    // we invoke an event
    [FoldoutGroup("Full Event"), PropertyOrder(10)]    
    public int invokeNumber = 1;
    [FoldoutGroup("Full Event"), PropertyOrder(10)]
    public UnityEventWithShyObject fullEvent = new UnityEventWithShyObject();

    // Playmaker Related
    [FoldoutGroup("Full Event"), PropertyOrder(10), InlineButton("AutoAssignFsmToFullEvent", "Auto")]
    public PlayMakerFSM fullMsgFsm;

    List<string> fullEventNames = new List<string>();
    private List<string> FullEventNames
    {
        get
        {
            UpdateEventNames(fullMsgFsm, fullEventNames);
            return fullEventNames;
        }        
    }
    [FoldoutGroup("Full Event"), ValueDropdown("FullEventNames")]
    public string fullMsgEvent;
    #endregion

    List<GameObject> container = new List<GameObject>();
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    public override string GetTooltip()
    {
        if(IsEmptyHand())
            return GetTooltipString(Tooltip.HAND_IS_EMPTY);
        else if(!IsHeldObjectNeeded())
            return GetTooltipString(Tooltip.WRONG_THING);

        return defaultTooltip;
    }
 

    public void AddObjectToContainer(GameObject go)
    {
        container.Add(go);
        if(container.Count == invokeNumber)
        {
            fullEvent.Invoke(this);            
            if (fullMsgFsm)
            {
                fullMsgFsm.MySendEventToAll(fullMsgEvent);
            }
        }
    }


    void AutoAssignFsmToFullEvent()
    {
        AutoAssignFsm(out fullMsgFsm);
    }


    public override void HandleInteraction()
    {
        base.HandleInteraction();

        bool wasPressed = LevelManager.Instance.PlayerActions.Fire.WasPressed;
        if (wasPressed)
            PutBack();
    }


    void PutBack()
    {
        // nothing in hand
        if (IsEmptyHand())
            return;

        var po = sis.curHeldObject.GetComponent<ShyPickableObject>();
        var body = sis.curHeldObject.GetComponent<Rigidbody>();


        // wrong thing
        var validate = Validate();
        if (!validate)
            return;

        // right thing
        sis.curHeldObject.transform.SetParent(null);
        sis.curHeldObject.transform.eulerAngles = po.OriRotation;
        sis.curHeldObject.transform.localScale = po.OriLocalScale;
        sis.curHeldObject.transform.position = putBackAnchor ? putBackAnchor.transform.position : transform.position;

        AddObjectToContainer(sis.curHeldObject);

        if (!canPickAfterPut)
        {
            po.canInteract = false;
        }

        if (body)
            body.isKinematic = false;

        sis.curHeldObject = null;
    }


    protected override bool NeedValidateModeButton()
    {
        validationMode = ValidationMode.NEED_HELD;
        return false ;
    }
}
