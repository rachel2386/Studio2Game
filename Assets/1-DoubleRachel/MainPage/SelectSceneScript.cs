using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotoHome(int index)
    {
        HomeSceneManager.IntoIndex = index;
        SceneManager.LoadScene("HomeScene"); 
    }

    public void GotoSchool(int index)
    {
        SwitchScenes.gameState = index;
        SceneManager.LoadScene("School");
    }

    public void GotoGym(bool isNormal)
    {
        if(isNormal)
            SceneManager.LoadScene("Gym");
        else
            SceneManager.LoadScene("DarkGym");
    }
}
