using System.Collections;
using System.Collections.Generic;
using InControl.NativeProfile;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightCountdown : MonoBehaviour
{
    public AudioSource Theme;
    public AudioSource lightswitch;
    public Light door;
    public GameObject lamp;
    private bool musicEnd;
    
    // Start is called before the first frame update
    void Start()
    {
        musicEnd = false;
        door.enabled = false;
        lamp.SetActive(false);
        StartCoroutine(LightWait());    
        print(Theme.clip.length);
        StartCoroutine(waitAudio()); 
//      StartCoroutine(SwitchScene()); 
        
        
    }

    IEnumerator LightWait()
    {
        yield return new WaitForSeconds(2);
        lightswitch.Play();
        lamp.SetActive(true);
        door.enabled = true;
    }


//    IEnumerator SwitchScene()
//    {
//        yield return new WaitForSeconds(20);
//        HomeSceneManager.IntoIndex = 2;
//        SceneManager.LoadScene("HomeScene");
    
//    }

    // Update is called once per frame
    void Update()
    {
        int jump = PlayerReposition.trigTime;
        if (jump > 0 && musicEnd)
        {
            HomeSceneManager.IntoIndex = 2;
            SceneManager.LoadScene("HomeScene");
        }
    }

    private IEnumerator waitAudio()
    {
        yield return new WaitForSeconds(Theme.clip.length);
        musicEnd = true;
    }
}
