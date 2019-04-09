﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSoundTrigger : MonoBehaviour
{
    public AudioSource audio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gate")
        {
            audio.Play();
            print("1111");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
