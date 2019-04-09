using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableTofu : MonoBehaviour
{
    PanRotation pr;

    // Start is called before the first frame update
    void Start()
    {
        pr = GameObject.FindObjectOfType<PanRotation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {
        if(pr)
            pr.PickableTofuClicked(this);
    }
}
