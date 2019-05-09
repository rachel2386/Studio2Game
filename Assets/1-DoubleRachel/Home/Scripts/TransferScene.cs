using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HomeSceneManager.IntoIndex = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
