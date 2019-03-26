/*

 * 		Original Code by: NOT_lonely (www.facebook.com/notlonely92)
 * 		Code Revision by: sluice (www.sluicegaming.com)
 *
 */ 
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour {
	private Transform[] Childs;
	private Transform Joint01;
	public enum OpenStyle
	{
		BUTTON, 
		AUTOMATIC
	}
	
	[Serializable]
	public class DoorControls
	{
		public float openingSpeed = 1;
		public float closingSpeed = 1.3f;
		[Range(0,1)]
		public float closeStartFrom = 0.6f;
		public OpenStyle openMethod; //Open by button or automatically?
		public KeyCode openButton = KeyCode.E; //Button on the keyboard to open the door
		public bool autoClose = false; //Automatically close the door. Forced to true when in AUTOMATIC mode.
	}
	[Serializable]
	public class AnimNames //names of the animations, which you use for every action
	{
		public string OpeningAnim = "Door_open";
		public string LockedAnim = "Door_locked";
	}
	[Serializable]
	public class DoorSounds 
	{
		public bool enabled = true;
		public AudioClip open;
		public AudioClip close;
		public AudioClip closed;
		[Range(0, 1.0f)]
		public float volume = 1.0f;
		[Range(0, 0.4f)]
		public float pitchRandom = 0.2f;
	}
	[Serializable]
	public class DoorTexts
	{
		public bool enabled = false;
		public string openingText = "Press [BUTTON] to open";
		public string closingText = "Press [BUTTON] to close";
		public string lockText = "You need a key!";
		public GameObject TextPrefab; 
	}
	[Serializable]
	public class KeySystem
	{
		public bool enabled = false;
		[HideInInspector]
		public bool isUnlock = false;
		[Tooltip("If you have a padlock model, you can put the prefab here.")]
		public GameObject LockPrefab;
	}
	
	[Tooltip("player's head with collider in trigger mode. Type your tag here (usually it is MainCamera)")]
	public string PlayerHeadTag = "MainCamera";
	[Tooltip("Empty gameObject in the door knobs area. It needed to open the door if "+ "'"+ "Open by button"+"'"+" type is selected. If you don't want to put this object in this slot manually, you can simply create the object with the name " +"'"+"doorKnob"+"'" +" and put it in the door prefab.")]
	public Transform knob; //Empty GO in the door knobs area.
	
	public DoorControls controls = new DoorControls();
	public AnimNames AnimationNames = new AnimNames();
	public DoorSounds doorSounds = new DoorSounds();
	public DoorTexts doorTexts = new DoorTexts();
	public KeySystem keySystem = new KeySystem();

	Transform player;
	bool Opened = false;
	bool inZone = false; 
	Canvas TextObj;
	Text theText;
	AudioSource SoundFX;
	Animation doorAnimation;
	Animation LockAnim;


	
	void Start (){

//Border reparenting

		Childs = GetComponentsInChildren<Transform> ();

		foreach (Transform Child in Childs) {
			if (Child.name == "Joint01") {
				Joint01 = Child.transform;
			}
		}

		foreach(Transform Child in Childs){
			if(Child.name == "Door_bottom01"){
				print(gameObject.name + " bottom has been found");
				Child.parent = Joint01;
			}
		}
//--------------------------------

		if(controls.openMethod == OpenStyle.AUTOMATIC)
			controls.autoClose = true;

		if(PlayerHeadTag == "")
			Debug.LogError("You need to set a tag!");

		if (GameObject.FindWithTag (PlayerHeadTag) != null) {
			player = GameObject.FindWithTag (PlayerHeadTag).transform;
		} 
		else {
			Debug.LogWarning(gameObject.name + ": You need to set your player's camera tag to " + "'"+PlayerHeadTag+"'." + " The " + "'" + gameObject.name+"'" +" can't open/close if you don't set this tag");
		}


		AddText();
		AddLock();
		AddAudioSource();
		DetectDoorKnob ();
		doorAnimation = GetComponent<Animation>();
	}

	void AddText(){
		if(doorTexts.enabled){
			if(doorTexts.TextPrefab == null)
			{
				Debug.LogWarning(gameObject.name + ": Text prefab missing, if you want see the text, please, put the text prefab in Text Prefab slot");
				return;
			}
			GameObject go = Instantiate (doorTexts.TextPrefab, Vector3.zero, new Quaternion (0, 0, 0, 0)) as GameObject;
			TextObj = go.GetComponent<Canvas>();
			
			theText = TextObj.GetComponentInChildren<Text>();
		}
	}

	void AddLock(){
		if(!keySystem.enabled)
			return;

		if(keySystem.LockPrefab == null){
			Debug.LogWarning(gameObject.name + ": you can set a padlock prefab.");
		}
		else {
			LockAnim = keySystem.LockPrefab.GetComponent<Animation> ();
			keySystem.enabled = true;
		}
	}

	void AddAudioSource()
	{
		GameObject go = new GameObject("SoundFX");
		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;
		go.transform.parent = transform;
		SoundFX = go.AddComponent<AudioSource>();
		SoundFX.volume = doorSounds.volume;
		SoundFX.spatialBlend = 1;
		SoundFX.playOnAwake = false;
		SoundFX.clip = doorSounds.open;
	}

	void DetectDoorKnob()
	{
		if(knob != null)
			return;

		Transform[] children = GetComponentsInChildren<Transform>(true);
		
		foreach(Transform child in children)
		{
			if(child.name == "doorKnob")
			{
				knob = child;
				return;
			}
		}
		
		if(knob == null)
			Debug.LogWarning("Door has no set knob!");
	}
	
	void Update () {
		if (!doorAnimation.isPlaying && SoundFX.isPlaying) {
			SoundFX.Stop();
		}
		if(!inZone)
		{
			HideHint();
			return;
		}

		if(controls.openMethod == OpenStyle.AUTOMATIC && !Opened)
			OpenDoor();

		if(PLayerIsLookingAtDoorKnob())
		{
			if(controls.openMethod == OpenStyle.BUTTON)
				ShowHint();
		}
		else
		{
			HideHint();
			return;
		}

		if(!Input.GetKeyDown(controls.openButton) || controls.openMethod != OpenStyle.BUTTON)
			return;
		
		if(Opened)
		{
			if(!controls.autoClose)
				CloseDoor();
		}
		else
		{
			if(keySystem.enabled)
			{
				if(keySystem.isUnlock)
					OpenLockDoor();
				else
					PlayClosedFXs();
			}
			else
			{
				OpenDoor();
			}
		}
	}

	bool PLayerIsLookingAtDoorKnob()
	{

//		Vector3 forward = player.TransformDirection (Vector3.back);
//		Vector3 thisTransform = knob.position - Camera.main.transform.position;
		Vector3 forward = player.TransformDirection (Vector3.back);
		Vector3 thisTransform = knob.position - player.transform.position;

		float dotProd = Vector3.Dot (forward.normalized, thisTransform.normalized);
		return(dotProd < 0 && dotProd< -0.9f);
	}
	
	void OpenLockDoor()
	{
		if(keySystem.LockPrefab != null){
			LockAnim.Play("Lock_open");
			Invoke("OpenDoor", 1);
		} 
		else
		{
			OpenDoor();
		}
	}
	
	public void Unlock()
	{
		keySystem.isUnlock = true;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag != PlayerHeadTag)
			return;
		
		inZone = true;
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag != PlayerHeadTag) 
			return;


		if(Opened && controls.autoClose)
			CloseDoor();
		
		inZone = false;
	}


	void ShowHint()
	{
		if (Opened) 
		{
			if(!controls.autoClose)
				CloseText();
		}
		else
		{
			if(keySystem.enabled && !keySystem.isUnlock)
				LockText();
			else
				OpenText();
		}
	}

	void HideHint()
	{
		if(controls.openMethod == OpenStyle.BUTTON)
			HideText();
	}

	#region AUDIO
	/*
	 * 	AUDIO
	 */ 
	void PlaySFX(AudioClip clip)
	{
		if(!doorSounds.enabled)
			return;

		SoundFX.pitch = UnityEngine.Random.Range (1-doorSounds.pitchRandom, 1+doorSounds.pitchRandom);
		SoundFX.clip = clip;
		SoundFX.Play();
	}
	
	void PlayClosedFXs(){
		if (doorSounds.closed != null) {
			SoundFX.clip = doorSounds.closed;
			SoundFX.Play();
			if(doorAnimation[AnimationNames.LockedAnim] != null){
				doorAnimation.Play(AnimationNames.LockedAnim);
				doorAnimation[AnimationNames.LockedAnim].speed = 1;
				doorAnimation [AnimationNames.LockedAnim].normalizedTime = 0;
			}


		}
	}

	void CloseSound()
	{
		if(doorAnimation[AnimationNames.OpeningAnim].speed < 0 && doorSounds.close != null)
			PlaySFX(doorSounds.close);
	}
	#endregion

	#region TEXT
	/*
	 * 	TEXT
	 */ 

	public void OpenText()
	{
		ShowText(doorTexts.openingText);
	}
	
	void LockText()
	{
		ShowText(doorTexts.lockText);
	}
	
	void CloseText()
	{
		ShowText(doorTexts.closingText);
	}
	
	void ShowText(string txt)
	{
		if(!doorTexts.enabled)
			return;

		string tempTxt = txt;

		if(controls.openMethod == OpenStyle.BUTTON)
			tempTxt = txt.Replace("[BUTTON]", "'" + controls.openButton.ToString() + "'");
		
		TextObj.enabled = false;
		theText.text = tempTxt;
		TextObj.enabled = true;
	}

	void HideText()
	{
		if(!doorTexts.enabled)
			return;
		if (doorTexts.TextPrefab != null) {
			TextObj.enabled = false;
		} else {
			doorTexts.enabled = false;
		}
	}
	#endregion
	
	void OpenDoor()
	{
		doorAnimation.Play(AnimationNames.OpeningAnim);
		doorAnimation[AnimationNames.OpeningAnim].speed = controls.openingSpeed;
		doorAnimation [AnimationNames.OpeningAnim].normalizedTime = doorAnimation [AnimationNames.OpeningAnim].normalizedTime;

		if(doorSounds.open != null)
			PlaySFX(doorSounds.open);
		
		Opened = true;
		if (controls.openMethod == OpenStyle.BUTTON) 
			HideText();

		keySystem.enabled = false;
	}
	
	void CloseDoor()
	{
		if (doorAnimation[AnimationNames.OpeningAnim].normalizedTime < 0.98f && doorAnimation [AnimationNames.OpeningAnim].normalizedTime > 0) 
		{
			doorAnimation[AnimationNames.OpeningAnim].speed = -controls.closingSpeed;
			doorAnimation[AnimationNames.OpeningAnim].normalizedTime = doorAnimation [AnimationNames.OpeningAnim].normalizedTime;
			doorAnimation.Play (AnimationNames.OpeningAnim);
		} 
		else 
		{
			doorAnimation[AnimationNames.OpeningAnim].speed = -controls.closingSpeed;
			doorAnimation[AnimationNames.OpeningAnim].normalizedTime = controls.closeStartFrom;
			doorAnimation.Play (AnimationNames.OpeningAnim);
		}
		if(doorAnimation[AnimationNames.OpeningAnim].normalizedTime > controls.closeStartFrom){
			doorAnimation[AnimationNames.OpeningAnim].speed = -controls.closingSpeed;
			doorAnimation[AnimationNames.OpeningAnim].normalizedTime = controls.closeStartFrom;
			doorAnimation.Play (AnimationNames.OpeningAnim);
		}
		Opened = false;

		if (controls.openMethod == OpenStyle.BUTTON && !controls.autoClose) 
			HideText();
	}
	
}