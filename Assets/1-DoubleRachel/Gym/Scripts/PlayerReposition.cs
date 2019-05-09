using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReposition : MonoBehaviour
{
    public Transform Position;
    public GameObject Player;
    public AudioSource StaticSound;
//    public ShyCamera Camera;
//    public float Timer = 1f;
//    private bool Countdown;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
//            Countdown = true;
            StaticSound.Play();
            Player.transform.position = Position.position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
//        Camera.runtimeProfile.grain.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
//        if (Countdown)
//        {
//            if (Timer > 0)
//            {
//                Timer-=Time.deltaTime;
//                Camera.runtimeProfile.bloom.enabled = true;
//            }
//            else
//            {
//                Countdown = false;
//            }
//        }
//        else
//        {
//            Camera.runtimeProfile.grain.enabled = false;
//            Timer = 1;
//        }
    }
}
