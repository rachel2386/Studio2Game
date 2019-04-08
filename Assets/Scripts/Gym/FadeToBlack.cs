using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour {

	private float alpha;
	
	// Use this for initialization
	void Start () {
		alpha = 0.0f;		
	}
	
	// Update is called once per frame
	void Update () {
		
		GetComponent<Image>().color = new Color(0,0,0,alpha);
		if (alpha < 1.5)
		{
			alpha += 0.01f;
		}	
		
		if (alpha >= 1.5)
		{
			SceneManager.LoadScene (0);
			Debug.Log("Loaded Scene 0");

		}	

		
	}
}
