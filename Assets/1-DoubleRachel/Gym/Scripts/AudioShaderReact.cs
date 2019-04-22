using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioShaderReact : MonoBehaviour {

    SpectrumAnalysis analyzer;
	 
	public float smoothing = 0.75f;

	public float lowFrequencyBound, highFrequencyBound;

	[Range(0f, 5f)] public float shaderTimeAmount;
	[Range(50f, 20000f)] public float shaderTimeLowBound, shaderTimeHighBound;
	[Range(0f, 1f)] public float shaderColorAmount;
	[Range(50f, 20000f)] public float shaderColorLowBound, shaderColorHighBound;
	
	public GameObject reactiveGameObject;
	[SerializeField] Material myMaterial;
	
    float normalizedEnergy = 0f;
	float prevEnergy = 0f;
	float smoothedEnergy = 0f;

	float energyGate = 0.5f;

	private float prevShaderTime;
	private float prevShaderColor;
	
	
	



	// Use this for initialization
	void Start () {

        analyzer = GetComponent<SpectrumAnalysis>();
		myMaterial = reactiveGameObject.GetComponent<Renderer> ().material;

		prevShaderTime = 0f;
		prevShaderColor = 0f;
		
	}
	
	// Update is called once per frame
	void Update () {
		

		normalizedEnergy = analyzer.GetWholeEnergy();
		
		

		//we're applying some additional smoothing here
		smoothedEnergy = Mathf.Lerp (prevEnergy, normalizedEnergy, smoothing);
		prevEnergy = smoothedEnergy;

		//Find the time value from the range of frequencies that we want
		float shaderTime = analyzer.GetEnergyFrequencyRange(shaderTimeLowBound, shaderTimeHighBound);
		shaderTime *= shaderTimeAmount;
		
		//Apply Smoothing
		shaderTime = Mathf.Lerp(prevShaderTime, shaderTime, smoothing);
		prevShaderTime = shaderTime;
		
		float shaderColor = analyzer.GetEnergyFrequencyRange(shaderColorLowBound, shaderColorHighBound);
		shaderColor *= shaderColorAmount;

		
		shaderColor = Mathf.Lerp(prevShaderColor, shaderColor, smoothing);
		prevShaderColor = shaderColor;

		//check to see if we have a material, then start adjusting shader values
		if (myMaterial != null) {
			myMaterial.SetFloat ("_AudioInput", smoothedEnergy);
			myMaterial.SetFloat ("_ShaderTime", shaderTime);
			myMaterial.SetFloat ("_ShaderColor", shaderColor);
		}
        
	}
}
