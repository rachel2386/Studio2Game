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
		Vector3 position = new Vector3(Random.Range(15f, 130f), Random.Range(-10f, 20f), Random.Range(-17f, 140f));
		Quaternion rotation = new Quaternion(Random.Range(-300f, 300f), Random.Range(-300f, 300f),Random.Range(-300f, 300f),Random.Range(-300f, 300f) );
		Transform next = Instantiate(model, position, rotation) as Transform;
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

		if (count == 15)
		{
			gameObject.MySendEventToAll("PlayBell");
		}

	}
}
