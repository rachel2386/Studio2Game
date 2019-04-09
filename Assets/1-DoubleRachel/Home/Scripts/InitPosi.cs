using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPosi : MonoBehaviour
{
    public Transform reference;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start In InitPosi");
        transform.position = reference.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
