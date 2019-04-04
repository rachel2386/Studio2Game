using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallSpawner : MonoBehaviour
{

	public Transform model;
	public float spawnRate = 1.0f;
	public AudioClip bell;
	float progress = 0.0f;
	private int count;

	void Start()
	{
		count = 0;
	}

	void Spawn()
	{
		Transform next = Instantiate(model, transform.position, transform.rotation) as Transform;
		next.parent = transform;
		next.localPosition = Vector3.zero;
		count += 1;
	}

	void Update()
	{
		progress += spawnRate * Time.deltaTime;
		if (progress >= 1.0f)
		{
			Spawn();
			progress -= 1.0f;
		}

		if (count == 10)
		{
			gameObject.MySendEventToAll("PlayBell");
		}

	}
}
