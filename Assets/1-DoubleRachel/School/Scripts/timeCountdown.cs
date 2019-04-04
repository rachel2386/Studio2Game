using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timeCountdown : MonoBehaviour
{
    public float time = 180;

    private Text myText;
    // Start is called before the first frame update
    private void Start()
    {
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        myText.text = "Time Left:" + Mathf.RoundToInt(time / 60f) + "m" + Mathf.FloorToInt(time % 60) + "s";
        if (time >= 0)
            time -= Time.deltaTime;
        else
            time = 0;
       
    }
}
