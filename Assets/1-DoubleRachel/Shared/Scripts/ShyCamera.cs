using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class ShyCamera : MonoBehaviour
{
    public GameObject pickupRoot;
    public PostProcessingProfile runtimeProfile;

    private void Awake()
    {
        GetComponent<PostProcessingBehaviour>().profile = runtimeProfile;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int GetRandom1()
    {
        return Random.Range(0f, 1f) < 0.5f ? 1 : -1;
    }

    public Vector3 GetRandomVec3()
    {
        return new Vector3(Random.Range(5, 9) * GetRandom1(), Random.Range(0, 9) * GetRandom1(), 0);
    }

    

    [Title("Grain")]
    [Range(0.3f, 3)]
    public float minGrainSize;
    [Range(0.3f, 3)]
    public float maxGrainSize;

    [Range(0f, 1)]
    public float minGrainIntensity;
    [Range(0f, 1)]
    public float maxGrainIntensity;

    public float maxGrainDistance = 5;
    public AnimationCurve grainCurve = AnimationCurve.Linear(0, 1, 1, 0);

    bool distaceGrain = false;

    // Input Degree from 0 -> 1
    // 0 means min grain
    public void SetGrainDegree(float degree, float animationLerp = 1)
    {
        degree = Mathf.Clamp(degree, 0, 1);
        var sizeValue = Mathf.Lerp(minGrainSize, maxGrainSize, degree);
        SetGrainSize(sizeValue, animationLerp);

        var intensityValue = Mathf.Lerp(minGrainIntensity, maxGrainIntensity, degree);
        SetGrainIntensity(intensityValue, animationLerp);
    }
    
    public void SetGrainSize(float value, float animationLerp = 1)
    {
        SetPpeParam(PpeSetting.GRAIN_SIZE, value, animationLerp);
    }

    public void SetGrainIntensity(float value, float animationLerp = 1)
    {
        SetPpeParam(PpeSetting.GRAIN_INTENSITY, value, animationLerp);
    }

    public void SetPpeParam(PpeSetting ppes, float value, float animationLerp = 1)
    {
        ShyMiscTool.SetPpeParam(runtimeProfile, ppes, value, animationLerp);
    }

    public float GetPpeParam(PpeSetting ppes)
    {
        return ShyMiscTool.GetPpeParam(runtimeProfile, ppes);
    }

    void UpdateGrainOnDistance()
    {

    }

    void UpdateGrainOnDistanceObject(GameObject target)
    {
        var distance = ShyMiscTool.GetPlaneDistance(transform.position, target.transform.position);
        var normaledDistance = Mathf.InverseLerp(0, maxGrainDistance, distance);
        normaledDistance = Mathf.Lerp(0, 1, normaledDistance);
        var grainDegree = grainCurve.Evaluate(normaledDistance);
        SetGrainDegree(grainDegree);
    }
}
