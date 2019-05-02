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
		Vector3 position = new Vector3(Random.Range(-43f, 46f), Random.Range(-10f, 10f), 0);
		Transform next = Instantiate(model, position, transform.rotation) as Transform;
		next.parent = transform;
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
