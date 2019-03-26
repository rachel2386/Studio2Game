using UnityEngine;
using System.Collections;

public class KeyScript : MonoBehaviour {
	public string PlayerTag = "Player";
	public GameObject Door;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other){
		if(other.transform.gameObject.tag == PlayerTag){
			GetKey();
		}
	}
	void GetKey(){
		DoorScript doorScript = Door.GetComponent<DoorScript> ();
		doorScript.keySystem.isUnlock = true;
		Destroy (gameObject);
	}
}
