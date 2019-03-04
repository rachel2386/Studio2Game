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
    void Start()
    {
        oriRotation = transform.eulerAngles;
        oriLocalScale = transform.localScale; 
    }

    // Update is called once per frame
    void Update()
    {

    }
}
