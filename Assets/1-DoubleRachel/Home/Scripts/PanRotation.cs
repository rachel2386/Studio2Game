using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PanRotation : Pan
{
   

    public GameObject pan;
    public GameObject panModel;

    public GameObject liftedAnchor;

    Camera cam;
    ShyInteractionSystem sis;
    ShyUI shyUI;


    Vector3 oriPanPosition;
    Quaternion oriPanRotation;


    public PostProcessingProfile postProcessingProfile;
    public float oriVignetteIntensity;
    public float finalVignetteIntensity = 0.4f;
    public float oriDepthParam;
    public float finalDepthParam = 14f;

    //public float oriGrainSize = 0.3f;
    //public float finalGrainSize = 2;
    //public float oriGrainIntensity = 0;
    //public float finalGrainIntensity = 3;
    public float grainSize = 1.6f;
    public float grainIntensity = 1;


    public float finalCurtainHeight = 180f;
    public float curtainIntoTime = 3.0f;
    public float curtainOutTime = 1.5f;

    public float oriBgmVolume;
    public float finalBgmVolume;

    public float intoLerpFactor;
    public float outLerpFactor;


    [Header("Progress")]
    public float timeToFinish = 30;
    public float knockDecrease = 0.05f;

    public AudioSource bgm;
    ShyDialogManager dialogManager;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        oriPanPosition = pan.transform.position;
        oriPanRotation = pan.transform.rotation;

        dialogManager = FindObjectOfType<ShyDialogManager>();

        cam = Camera.main;
        sis = GameObject.FindObjectOfType<ShyInteractionSystem>();
        shyUI = GameObject.FindObjectOfType<ShyUI>();

        InitPostProcessingParams();        
    }
    
    public float rotateSpeedY = 1;
    public float rotateSpeedZ = 1;

    public float rotateAmplitudeY = 45;
    public float rotateAmplitudeZ = 45;

    // Update is called once per frame

    float yRotationTime = 0;
    float zRotationTime = 0;
    void Update()
    {
        bool aimedPan = false;
        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
        foreach(var hit in hits)
        {
            if(hit.collider.gameObject == gameObject)
            {
                aimedPan = true;
                break;
            }
        }

        // Into
        if(aimedPan && LevelManager.Instance.PlayerActions.Fire.IsPressed)
        {
            var ori = pan.transform.eulerAngles;
            yRotationTime += Time.deltaTime;
            zRotationTime += Time.deltaTime;
            ori.y = Mathf.Sin(yRotationTime * rotateSpeedY) * rotateAmplitudeY;
            ori.z = Mathf.Cos(zRotationTime * rotateSpeedZ) * rotateAmplitudeZ;
            pan.transform.eulerAngles = ori;

            sis.forceHideCenterText = true;
            pan.transform.position = liftedAnchor.transform.position;

            // FOV
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, intoLerpFactor * Time.deltaTime);
            // Post Processing Effects
            // SetProcessingParams(false, true, finalVignetteIntensity, finalDepthParam);
            var lerpFactor = intoLerpFactor * Time.deltaTime;
            SetPpeParam(PpeSetting.VIGNETTE_INTENSITY, finalVignetteIntensity, lerpFactor);
            SetPpeParam(PpeSetting.DEPTH_OF_FIELD_APERTURE, finalDepthParam, lerpFactor);
            // Curtain
            var neededCurtainHeight = Mathf.MoveTowards(currentCurtainHeight, finalCurtainHeight, finalCurtainHeight / curtainIntoTime * Time.deltaTime);

            if (!dialogManager.IsInDialog())
                SetCurtainHeight(neededCurtainHeight);
            // BGM
            bgm.volume = finalBgmVolume;

            //Add Progress
            shyUI.ShowProgress(true);
            AddProgress();
        }
        // Out
        else
        {
            sis.forceHideCenterText = false;
            pan.transform.position = oriPanPosition;
            pan.transform.rotation = oriPanRotation;

            // FOV            
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, outLerpFactor * Time.deltaTime);
            // Post Processing Effects
            // SetProcessingParams(false, false, oriVignetteIntensity, oriDepthParam);
            var lerpFactor = outLerpFactor * Time.deltaTime;
            SetPpeParam(PpeSetting.VIGNETTE_INTENSITY, oriVignetteIntensity, lerpFactor);
            SetPpeParam(PpeSetting.DEPTH_OF_FIELD_APERTURE, oriDepthParam, lerpFactor);

            // Curtain
            var neededCurtainHeight = Mathf.MoveTowards(currentCurtainHeight, 0, finalCurtainHeight / curtainOutTime * Time.deltaTime);

            if(!dialogManager.IsInDialog())
                SetCurtainHeight(neededCurtainHeight);
            // BGM
            bgm.volume = oriBgmVolume;

            // Progress
            shyUI.ShowProgress(false);
        }

        // Debug.Log(shyUI.topCurtain.GetComponent<RectTransform>().sizeDelta.y);
        // RefreshGrainEffect();
    }

    void AddProgress()
    {
        shyUI.AddProgress(Time.deltaTime * 1 / timeToFinish);
    }

    public void EnableGrainEffect(bool enabled)
    {
        postProcessingProfile.grain.enabled = enabled;
    }

    //void RefreshGrainEffect()
    //{
    //    // 1 - > 0
    //    float perc = 1 - (float)foodList.Count / (float)oriFoodCount;

    //    SetPpeParam(PpeSetting.GRAIN_SIZE, oriGrainSize + (finalGrainSize - oriGrainSize) * perc);
    //    SetPpeParam(PpeSetting.GRAIN_INTENSITY, oriGrainIntensity + (finalGrainIntensity - oriGrainIntensity) * perc);
    //}

    void SetPpeParam(PpeSetting ppes, float value, float lerpFactor = 1)
    {
        ShyMiscTool.SetPpeParam(postProcessingProfile, ppes, value, lerpFactor);
    }

    float GetPpeParam(PpeSetting ppes)
    {
        return ShyMiscTool.GetPpeParam(postProcessingProfile, ppes);
    }

    float currentCurtainHeight = 0;
    void SetCurtainHeight(float height)
    {
        currentCurtainHeight = height;
        shyUI.SetCurtainHeight(height);
    }


    void InitPostProcessingParams()
    {
        SetPpeParam(PpeSetting.VIGNETTE_INTENSITY, oriVignetteIntensity);
        SetPpeParam(PpeSetting.DEPTH_OF_FIELD_APERTURE, oriDepthParam);

        SetPpeParam(PpeSetting.GRAIN_INTENSITY, grainIntensity);
        SetPpeParam(PpeSetting.GRAIN_SIZE, grainSize);

        ShyMiscTool.SetPpeActivate(postProcessingProfile, PpeSetting.DEPTH_OF_FIELD_APERTURE, false);
    }
    

    public void KnockDoorStart(int index)
    {
        
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.05f);
        seq.AppendCallback(() =>
        {
            cam.MySendEventToAll("SHAKE");
            EnableGrainEffect(true);
        });

        ShyMiscTool.SetPpeActivate(postProcessingProfile, PpeSetting.DEPTH_OF_FIELD_APERTURE, false);
    }

    public void KnockDoorEnd(int index)
    {
        shyUI.SuddenDecreaseProgress(knockDecrease);
        if(index == 0 || index == 1)
        {
            
            var seq = DOTween.Sequence();
            seq.AppendInterval(0.15f);
            seq.AppendCallback(() =>
            {
                EnableGrainEffect(false);
                
            });

            SendEventGoodMode();
        }
        else
        {
            SendEventBadMode();

            var seq = DOTween.Sequence();
            seq.AppendInterval(0.25f);
            seq.AppendCallback(() => {
                EnableGrainEffect(false);           
                ShyMiscTool.SetPpeActivate(postProcessingProfile, PpeSetting.DEPTH_OF_FIELD_APERTURE, true);
            });
            
        }
    }
}
