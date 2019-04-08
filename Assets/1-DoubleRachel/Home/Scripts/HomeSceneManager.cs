using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    public static int IntoIndex = 0;

    ShyFPSController fpsController;
    ShyCamera shyCam;

    // Start is called before the first frame update
    void Start()
    {
        fpsController = FindObjectOfType<ShyFPSController>();
        shyCam = FindObjectOfType<ShyCamera>();

        if (IntoIndex == 0)
        {
            fpsController.lockMove = true;
            shyCam.useGrain = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
