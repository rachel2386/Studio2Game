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
    // Start is called before the first frame update
    void Start()
    {
        door.enabled = false;
        lamp.SetActive(false);
        StartCoroutine(LightWait());
        StartCoroutine(SwitchScene());
    }

    IEnumerator LightWait()
    {
        yield return new WaitForSeconds(2);
        lightswitch.Play();
        lamp.SetActive(true);
        door.enabled = true;
    }

    IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(20);
        HomeSceneManager.IntoIndex = 2;
        SceneManager.LoadScene("HomeScene");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
