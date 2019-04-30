using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioBtn : MonoBehaviour
{
    Radio radio;
    
    // Start is called before the first frame update
    void Start()
    {
        radio = FindObjectOfType<Radio>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {

        radio.curOption = GetComponentInChildren<OptionChange>();
    }
}
