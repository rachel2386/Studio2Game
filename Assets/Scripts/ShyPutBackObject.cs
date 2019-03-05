using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShyPutBackObject : ShyInteractableObject
{
    [PropertyOrder(-9)]
    public string emptyHandTooltip;
    [PropertyOrder(-9)]
    public string wrongThingTooltip;

    public GameObject putBackAnchor;
    public bool canPickAfterPut = false;

    [FoldoutGroup("Validation")]
    public bool needValidate = false;
    [FoldoutGroup("Validation"), ShowIf("needValidate")]
    public List<string> keyWords;

    #region FullEvent
    // if get enough number of objects put into the putback object
    // we invoke an event
    [FoldoutGroup("Full Event")]    
    public int invokeNumber = 1;
    [FoldoutGroup("Full Event")]
    public UnityEvent fullEvent = new UnityEvent();

    // Playmaker Related
    [FoldoutGroup("Full Event"), InlineButton("AutoAssignFsmToFullEvent", "Auto")]
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
    
    public bool Validate(GameObject go)
    {
        if (!needValidate)
            return true;

        bool ret = false;
        var inputName = go.name.ToLower();
        foreach(var keyWord in keyWords)
        {
            if (inputName.Contains(keyWord.ToLower()))
                return true;
        }

        return ret;
    }

    public override string GetTooltip()
    {
        var curHeld = sis.curHeldObject;
        if (!curHeld)
            return emptyHandTooltip;

        bool val = Validate(curHeld);
        if (!val)
            return wrongThingTooltip;
                     
        return defaultTooltip;
    }
 

    public void AddObjectToContainer(GameObject go)
    {
        container.Add(go);
        if(container.Count == invokeNumber)
        {
            fullEvent.Invoke();            
            if (fullMsgFsm)
            {
                fullMsgFsm.SendEvent(fullMsgEvent);
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
        if (!sis.curHeldObject)
            return;

        var po = sis.curHeldObject.GetComponent<ShyPickableObject>();
        var body = sis.curHeldObject.GetComponent<Rigidbody>();


        // wrong thing
        var validate = Validate(sis.curHeldObject);
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
}
