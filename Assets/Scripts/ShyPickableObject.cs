using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyPickableObject : ShyInteractableObject
{
    public Vector3 pickupRotaion;
    public Vector3 pickupScale;

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
        if(wasPressed)
            PickUp();
    }

    void PickUp()
    {
        var po = GetComponent<ShyPickableObject>();
        var body = GetComponent<Rigidbody>();

        transform.SetParent(sis.pickupRoot.transform);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = po.pickupRotaion;
        transform.localScale = po.pickupScale;

        if (body)
            body.isKinematic = true;

        sis.curHeldObject = gameObject;
    }


}
