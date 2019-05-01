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

   
   

    [Title("Grain")]
    public bool useGrain = false;

    [Range(0.3f, 3)]
    public float minGrainSize;
    [Range(0.3f, 3)]
    public float maxGrainSize;

    [Range(0f, 1)]
    public float minGrainIntensity;
    [Range(0f, 1)]
    public float maxGrainIntensity;

    [Tooltip("The lerp speed when grain value is changed")]
    public float grainLerpSpeed = 0.3f;
   
    [Tooltip("The max distance that make grain begin to work")]
    public float grainEffectDistance = 5;
    public AnimationCurve grainDistanceCurve = AnimationCurve.Linear(0, 1, 1, 0);

    
    public bool debug = false;

    [Tooltip("Whether disable the grain if it's blocked")]
    public bool considerOccusion = true;
    [Tooltip("Only grain when the npc is in the player's FOV")]
    public bool considerPlayerSeeNpc = true;
    [Tooltip("Only grain when the player is in the npc's view within an angle")]
    public bool considerNpcSeePlayer = false;
    [ShowIf("considerNpcSeePlayer"), Range(0, 360)]
    public float npcViewAngle = 180;

    List<Eye> eyeList = new List<Eye>();


    ShyFPSController fpsController;
    Camera cam;

    private void Awake()
    {
        GetComponent<PostProcessingBehaviour>().profile = runtimeProfile;

        InitPPE();
    }

    // Start is called before the first frame update
    void Start()
    {
        fpsController = FindObjectOfType<ShyFPSController>();
        cam = GetComponent<Camera>();
        InitEyes();
        
    }

    void InitPPE()
    {
        runtimeProfile.grain.enabled = false;
        runtimeProfile.depthOfField.enabled = false;
        runtimeProfile.vignette.enabled = false;
    }

    void InitEyes()
    {
        var eyes = Resources.FindObjectsOfTypeAll<Eye>();
        if (eyes != null)
        {
            foreach (var eye in eyes)
                eyeList.Add(eye);
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrain();
    }

 
    int GetRandom1()
    {
        return Random.Range(0f, 1f) < 0.5f ? 1 : -1;
    }

    public Vector3 GetRandomVec3()
    {
        return new Vector3(Random.Range(5, 9) * GetRandom1(), Random.Range(0, 9) * GetRandom1(), 0);
    }


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



    void UpdateGrain()
    {
        if (!useGrain)
            return;

        float closestDistance = float.MaxValue;
        // the closest eye that the player can see without occlusion

        var eyes = FindObjectsOfType<Eye>();
        foreach (var eye in eyes)
        {
            var eyeTransform = eye.transform;
            var npcTransform = eyeTransform.parent;
            var playerTransform = fpsController.transform;
            if (!npcTransform.gameObject.activeSelf)
                continue;

            if (considerOccusion && ShyMouseLook.IsLineOfSightBlocked(eyeTransform, transform, playerTransform))
                continue;
            
           
            if (considerPlayerSeeNpc)
            {
                var eyeInVp = cam.WorldToViewportPoint(eye.transform.position);
                if (eyeInVp.x > 1 || eyeInVp.x < 0 || eyeInVp.z < 0)
                    continue;
            }

            if (considerNpcSeePlayer)
            {
                var toPlayer = Quaternion.LookRotation(playerTransform.position - npcTransform.position);
                var turnQ = Quaternion.Inverse(npcTransform.rotation) * toPlayer;

                if (debug)
                    Debug.Log("NPC View: " + turnQ.eulerAngles);

                var turnE = turnQ.eulerAngles;
                var halfAngle = turnE.y > 180 ? 360 - turnE.y : turnE.y;

                if (halfAngle * 2 > npcViewAngle)
                    continue;
            }



            var distance = ShyMiscTool.GetPlaneDistance(eyeTransform.position, transform.position);
            if (distance < closestDistance)
                closestDistance = distance;
        }

        UpdateGrainOnDistance(closestDistance);
    }



    void UpdateGrainOnDistance(float distance)
    {        
        var normaledDistance = Mathf.InverseLerp(0, grainEffectDistance, distance);
        normaledDistance = Mathf.Clamp(normaledDistance, 0f, 1f);

        if (debug)
        {
            Debug.Log("Distance: " + distance);
            Debug.Log("NormaledDistance: " + normaledDistance);
        }
           

        var grainDegree = grainDistanceCurve.Evaluate(normaledDistance);


        SetGrainDegree(grainDegree, Time.deltaTime * grainLerpSpeed);

        if (grainDegree == 0)
            SetPpeActivate(PpeSetting.GRAIN_INTENSITY, false);
    }

    public void SetPpeActivate(PpeSetting st, bool enable)
    {
        ShyMiscTool.SetPpeActivate(runtimeProfile, st, enable);
    }
}
