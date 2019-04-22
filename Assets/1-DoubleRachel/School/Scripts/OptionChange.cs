using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionChange : MonoBehaviour
{
    public string[] options;
    public int curIndex = 0;


    SimpleHelvetica sh;
    // Start is called before the first frame update
    void Start()
    {
        sh = GetComponent<SimpleHelvetica>();
        RefreshContent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Add()
    {
        curIndex++;
        curIndex %= options.Length;

        RefreshContent();
    }

    public void Decrease()
    {
        curIndex--;
        curIndex += options.Length;
        curIndex %= options.Length;

        RefreshContent();
    }

    void RefreshContent()
    {
        sh.Text = options[curIndex];
        sh.GenerateText();
    }
}
