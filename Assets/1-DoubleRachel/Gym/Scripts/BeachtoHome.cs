using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeachtoHome : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SwitchScene()
    {
        HomeSceneManager.IntoIndex = 3;
        SceneManager.LoadScene("HomeScene");
    }
    

}
