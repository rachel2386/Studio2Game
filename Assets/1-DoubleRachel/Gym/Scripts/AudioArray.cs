using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioArray : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PickUpSound()
    {
        int noteIndex = Random.Range(0, 3);
        _audioSource.PlayOneShot(sounds[noteIndex]);
    }
}
