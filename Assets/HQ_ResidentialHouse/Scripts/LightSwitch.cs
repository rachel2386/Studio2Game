using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LightSwitch : MonoBehaviour {
	public GameObject[] Lights;
	[Tooltip("Reflection probes, which will be refreshed after ON/OFF light.")]
	public ReflectionProbe[] ReflectionProbes;
	[Tooltip("Put your emissive objects here. For example, a bulb with emission map.")]
	public Renderer[] EmissiveObjects;
	private Color[] EmissColors;
	[Tooltip("player's head with collider in trigger mode. Type your tag here (usually it is MainCamera)")]
	public string PlayerHeadTag = "MainCamera";
	[Tooltip ("Start state of the light (ON or OFF)")]
	public bool LightsON = true;
	[Tooltip("Button on the keyboard to switch the light")]
	public KeyCode SwithButton = KeyCode.E;
	[Tooltip("Prefab with the canvas and text object that will shown when the player looks at the light switch")]
	public GameObject TextPrefab;
	private Canvas TextObj;
	private Text theText;
	[Tooltip("Animation name")]
	public string AnimName = "LightSwitch";

	public AudioClip SwitchSound;
	[Range(0, 1)]
	public float Volume = 1;
	private AudioSource SoundFX;

	private bool inZone = false;
	private Animation Anim;
	private float _timer = 0.5f;
	private Vector3 forward;
	private Vector3 thisTransform;
	private Transform player;
	
	void Start () {
		if (GameObject.FindWithTag (PlayerHeadTag) != null) {
			player = GameObject.FindWithTag (PlayerHeadTag).transform;
		} 
		else {
			Debug.LogWarning(gameObject.name + ": You need to set your player's camera tag to " + "'"+PlayerHeadTag+"'." + " The " + "'" + gameObject.name + "'" +" can't switch on/off if you don't set this tag");
		}

		if (TextPrefab == null) {
			Debug.LogWarning (gameObject.name + ": Text prefab is missing. If you want see the text, please, put the text prefab in Text Prefab slot");
		} else {
			GameObject go = Instantiate (TextPrefab, Vector3.zero, new Quaternion (0, 0, 0, 0)) as GameObject;
			TextObj = go.GetComponent<Canvas> ();
			theText = TextObj.GetComponentInChildren<Text> ();
			theText.text = "Press " + "'" + SwithButton + "'" + " button to toggle the light";
			TextObj.gameObject.SetActive (false);
		}


		Anim = GetComponent<Animation> ();
		foreach (GameObject _light in Lights) {
			if (_light) {
				if (LightsON) {
					_light.SetActive (true);
					Anim [AnimName].normalizedTime = 0;
					Anim [AnimName].speed = -1;
					Anim.Play ();

				} else {
					_light.SetActive (false);
					Anim [AnimName].normalizedTime = 1;
					Anim [AnimName].speed = 1;
					Anim.Play ();
				}
			}
		}
		foreach (ReflectionProbe _probe in ReflectionProbes) {
			if (_probe) {
				_probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
				_probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
				Invoke ("BakeProbes", _timer);
			}
		}
	
		EmissColors = new Color[EmissiveObjects.Length];

		for(int i = 0; i < EmissiveObjects.Length; i++){
			EmissColors[i] = EmissiveObjects[i].material.GetColor("_EmissionColor");
		}

		if (!LightsON) {
			DisableEmission ();
		} 
		else {
			StartCoroutine (EnableEmissionLate());
			//Debug.Log("Emission enabled");
		}
		AddAudioSource ();
	}

	void AddAudioSource(){
		GameObject go = new GameObject("SoundFX");
		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;
		go.transform.parent = transform;
		SoundFX = go.AddComponent<AudioSource>();
		SoundFX.volume = Volume;
		SoundFX.spatialBlend = 1;
		SoundFX.playOnAwake = false;
		if (SwitchSound != null) {
			SoundFX.clip = SwitchSound;
		}
	}

	IEnumerator EnableEmissionLate () {
		yield return null;
		for(int i = 0; i < EmissiveObjects.Length; i++){
			EmissiveObjects[i].material.SetColor ("_EmissionColor", EmissColors[i]);
			DynamicGI.SetEmissive(EmissiveObjects[i], EmissColors[i] * 0.36f);
		}
	}

	void DisableEmission(){

		for (int i = 0; i < EmissiveObjects.Length; i++) {
				EmissiveObjects[i].material.SetColor ("_EmissionColor", Color.black);
				DynamicGI.SetEmissive(EmissiveObjects[i], Color.black * 0);
		}

	}
	void EnableEmission(){
		for(int i = 0; i < EmissiveObjects.Length; i++){
				EmissiveObjects[i].material.SetColor ("_EmissionColor", EmissColors[i]);
				DynamicGI.SetEmissive(EmissiveObjects[i], EmissColors[i] * 0.36f);
		}
	}

	void BakeProbes(){
		foreach (ReflectionProbe _probe in ReflectionProbes) {
			if (_probe) {
				_probe.RenderProbe ();
			}
		}
	}
	void Light_Off(){
		foreach (GameObject _light in Lights) {
			if(_light){
				_light.SetActive (false);
				LightsON = false;
				Anim [AnimName].normalizedTime = 1;
				Anim [AnimName].speed = 1;
				Anim.Play ();
				if(SwitchSound != null){
					SoundFX.Play();
				}
			}
		}
		Invoke("BakeProbes", _timer);
		/*
		foreach (ReflectionProbe _probe in ReflectionProbes) {
			if (_probe) {
				Invoke("BakeProbes", _timer);
			}
		}
		*/

	}
	void Light_On(){
		foreach (GameObject _light in Lights) {
			if(_light){
				_light.SetActive (true);
				LightsON = true;
				Anim [AnimName].normalizedTime = 0;
				Anim [AnimName].speed = -1;
				Anim.Play ();
				if(SwitchSound != null){
					SoundFX.Play();
				}
			}
		}
		Invoke("BakeProbes", _timer);
		/*
		foreach (ReflectionProbe _probe in ReflectionProbes) {
			if (_probe) {
				Invoke("BakeProbes", _timer);
			}
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
		if(inZone){
			forward = player.TransformDirection(Vector3.back);
			thisTransform = transform.position - player.transform.position;
			if (Vector3.Dot (forward.normalized, thisTransform.normalized) < 0 && Vector3.Dot (forward.normalized, thisTransform.normalized)< -0.9f) {
				if(TextPrefab != null){
					TextObj.gameObject.SetActive (true);
				}
				if(Input.GetKeyDown(SwithButton) && inZone){
					if(LightsON){
						Light_Off();
						DisableEmission();
						//Debug.Log("OFF");
					}
					else{
						Light_On();
						EnableEmission();
						//Debug.Log("ON");
					}
				}
			}
			else if(TextPrefab != null){
				TextObj.gameObject.SetActive (false);
			}



		}
	}
	void OnTriggerEnter(Collider other){
		if (other.tag != PlayerHeadTag) {
			return;
		}
		
		inZone = true;
		//Debug.Log ("IN ZONE");
	}
	void OnTriggerExit(Collider other){
		if (other.tag != PlayerHeadTag) {
			return;
		}
		
		inZone = false;
		if (TextPrefab != null) {
			TextObj.gameObject.SetActive (false);
		}

	}
}
