using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeFromWhite : MonoBehaviour
{
    private float alpha;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().color = new Color(249,241,229,alpha);
        if (alpha > 0)
        {
            alpha -= 0.01f;
        }

    }
}
