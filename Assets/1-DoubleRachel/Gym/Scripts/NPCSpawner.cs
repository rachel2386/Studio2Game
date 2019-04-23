using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public Transform model;
    public float spawnRate = 1.0f;
    float progress = 0.0f;
    private int count;
    public GameObject gate;
    private Vector3 startPos;
    private Vector3 endPos;
    private float distance = 10f;
    private float lerpTime = 3;
    private float currentLerpTime = 0;
    private bool openGate = false;

    public void SwitchScene()
    {
        HomeSceneManager.IntoIndex = 2;}

    void Start()
    {
        count = 0;
        startPos = gate.transform.position;
        endPos = gate.transform.position + Vector3.forward * distance * 5;
    }

    void Spawn()
    {
        Transform next = Instantiate(model, transform.position, transform.rotation) as Transform;
        next.parent = transform;
        next.localPosition = Vector3.zero;
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

        if (count == 10)
        {
            openGate = true;
            if (openGate == true)
            {
                currentLerpTime += Time.deltaTime;
                if (currentLerpTime >= lerpTime)
                {
                    currentLerpTime = lerpTime;
                }

                float Perc = currentLerpTime / lerpTime;
                gate.transform.position = Vector3.Lerp(startPos, endPos, Perc);
                
                if (Vector3.Distance(gate.transform.position, endPos) < 0.3)
                {
                    //A並B
                    if (flag)
                    {
                        flag = false;
                        gate.GetComponent<AudioSource>().Play();
                    }
                }
            }
            }
    }    
        
    
    // Start is called before the first frame update
    bool flag = true;
    // Update is called once per frame

}
