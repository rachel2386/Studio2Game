using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCountdown : MonoBehaviour
{
    public AudioSource panic;
    public AudioSource lightswitch;
    public Light door;
    // Start is called before the first frame update
    void Start()
    {
        door.enabled = false;
        panic.Play();
        StartCoroutine(LightWait());
    }

    IEnumerator LightWait()
    {
        yield return new WaitUntil(() => panic.isPlaying == false);
        yield return new WaitForSeconds(2);
        lightswitch.Play();
        door.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
