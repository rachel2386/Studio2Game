using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShyDialogOption : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string enterChangeText = "";
    public void PointerEnter()
    {
        if(enterChangeText != "")
            text.text = enterChangeText;

        enterChangeText = "";
    }
}
