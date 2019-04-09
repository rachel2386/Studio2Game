using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyPickableObject : ShyInteractableObject
{
    public Vector3 pickupRotaion;
    public Vector3 pickupScale = new Vector3(1, 1, 1);
    public bool canThrow = true;

    // Sometimes we want to use the pickable object just as an interactableObject
    // to evoke clicked event
    // but we don't want to pick up it
    public bool canPickUp = true;
    public bool pickParent = false;

    Vector3 oriRotation;
    public Vector3 OriRotation
    {
        get
        {
            return oriRotation;
        }
    }



    Vector3 oriLocalScale;
    public Vector3 OriLocalScale
    {
        get
        {
            return oriLocalScale;
        }
    }


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        oriRotation = transform.eulerAngles;
        oriLocalScale = transform.localScale; 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void HandleInteraction()
    {
        base.HandleInteraction();

        bool wasPressed = LevelManager.Instance.PlayerActions.Fire.WasPressed;
        if(wasPressed && canPickUp)
            PickUp();
    }

    void PickUp()
    {
        bool validate = Validate();
        if (!validate)
            return;


        var po = GetComponent<ShyPickableObject>();
        var body = GetComponent<Rigidbody>();


        var dealTarget = transform;
        var oriParent = transform.parent;
        var parentOriLocalScale = transform.parent.localScale;
        if(pickParent)
        {
            dealTarget = transform.parent;
        }

        dealTarget.SetParent(sis.pickupRoot.transform);
        dealTarget.localPosition = Vector3.zero;
        dealTarget.localEulerAngles = po.pickupRotaion;

        if (pickParent)
            dealTarget.localScale = parentOriLocalScale;
        else
            transform.localScale = po.pickupScale;


        if (body)
            body.isKinematic = true;

        if(pickParent)
        {
            foreach(var rb in oriParent.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
        }

        sis.curHeldObject = dealTarget.gameObject;
    }


}
