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

    public int fullNumber = 1;
    // if get enough number of objects put into the putback object
    // we invoke an event 
    [PropertyOrder(10)]
    public ShyEvent fullEvent = new ShyEvent("On Full");

    #endregion

    List<GameObject> container = new List<GameObject>();
    protected override void SetShyEventParent()
    {
        base.SetShyEventParent();
        fullEvent.component = this;
    }


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
        if(container.Count == fullNumber)
        {
            fullEvent.Invoke();            
            
        }
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
