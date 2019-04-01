using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetPathController : StreetModel.StreetPath
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckNewStreet")) {
            PlaceAnother();

        } else if (other.gameObject.CompareTag("CheckStreetEnd")) {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
