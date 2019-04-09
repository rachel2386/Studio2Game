using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timeCountdown : MonoBehaviour
{
    public float time = 180;

    private Text myText;

    private SwitchScenes switchScenes;
    // Start is called before the first frame update
    private void Start()
    {
        myText = GetComponent<Text>();
        myText.color = Color.black;
        switchScenes = GetComponent<SwitchScenes>();
    }

    // Update is called once per frame
    private void Update()
    {
        myText.text = "Time Left:" + Mathf.RoundToInt(time / 60f) + "m" + Mathf.Floor(time % 60) + "s";
        if (time >= 0)
            time -= Time.deltaTime;
        else
        {
            time = time;
            myText.color = Color.red;
            myText.text = "You are late...Again.";
            StartCoroutine(switchScenes.SceneSwitch(2));
        }

        
       
    }
}
