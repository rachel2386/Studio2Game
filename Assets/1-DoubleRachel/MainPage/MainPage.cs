using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {      
        StartCoroutine(DelayLoadScene());
    }

    IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(0.5f);

        if(!needGotoSelectScene)
            SceneManager.LoadScene("School");
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {        
        CheckIfOptionKeyPressed();
    }

    bool needGotoSelectScene = false;
    void CheckIfOptionKeyPressed()
    {
        if (needGotoSelectScene)
            return;

        if(Input.GetKey(KeyCode.A))
        {
            needGotoSelectScene = true;
            SceneManager.LoadScene("SelectScene");
        }
    }
}
