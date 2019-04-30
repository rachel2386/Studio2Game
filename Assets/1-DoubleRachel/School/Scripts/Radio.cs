using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    public OptionChange curOption;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Add()
    {
        if(curOption)
            curOption.Add();
    }

    public void Decrease()
    {
        if(curOption)
            curOption.Decrease();
    }
}
