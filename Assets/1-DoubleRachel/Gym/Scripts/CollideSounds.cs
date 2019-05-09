using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideSounds : MonoBehaviour {

	private AudioSource audioSource;
	public AudioClip[] fallinbottle = new AudioClip[5];
	public AudioClip[] fallinwater = new AudioClip[1];
	private AudioClip bottleClip;
	private AudioClip waterClip;

	
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Water") == true)
		{
			int index = Random.Range(0, fallinwater.Length);
			waterClip = fallinwater[index];
			audioSource.clip = waterClip;
			audioSource.Play();
			print("11");
		}
	}

	private void OnCollisionEnter(Collision other)
	{	
		if (other.gameObject.CompareTag("Bottle") == true)
		{
			int index = Random.Range(0, fallinbottle.Length);
			bottleClip = fallinbottle[index];
			audioSource.clip = bottleClip;
			audioSource.Play();
		}
	}
}
