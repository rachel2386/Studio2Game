using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanRotation : Pan
{
    public GameObject pan;
    public GameObject panModel;

    public GameObject liftedAnchor;

    Camera cam;
    ShyInteractionSystem sis;


    Vector3 oriPanPosition;
    Quaternion oriPanRotation;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        oriPanPosition = pan.transform.position;
        oriPanRotation = pan.transform.rotation;

        cam = Camera.main;
        sis = GameObject.FindObjectOfType<ShyInteractionSystem>();
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

        }
        else
        {
            sis.forceHideCenterText = false;
            pan.transform.position = oriPanPosition;
            pan.transform.rotation = oriPanRotation;
        }

    }
}
