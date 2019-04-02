using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour
{

    public AudioClip[] tones;
	
    public float maxSpeed = 1.0f;

    private AudioSource _audioSource;

    void Start() {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        int noteIndex = GetCollisionStrength(collision);
		
        _audioSource.PlayOneShot(tones[noteIndex]);
    }
    
    int GetCollisionStrength(Collision collision)
    {
        //project the velocity onto the normal of the platform
        Vector3 normal = collision.contacts[0].normal;
        Vector3 bounceAmount = Vector3.Project(collision.relativeVelocity, normal);
        float speed = bounceAmount.magnitude;
		
        //we want to map the ball's speed to a 0-1 range
        float clampedSpeed = Mathf.Clamp(speed / maxSpeed, 0.0f, 0.99f);

        //we want to round down in order to get the correct index of our tone array
        return Mathf.FloorToInt(clampedSpeed * tones.Length);
    }
//    public AudioClip bounce;
//    // Start is called before the first frame update
//    void Start()
//    {
//        GetComponent<AudioSource>().playOnAwake = false;
//        GetComponent<AudioSource>().clip = bounce;
//    }
//
//    private void OnCollisionEnter(Collision other)
//    {
//        GetComponent<AudioSource>().Play();
//    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
