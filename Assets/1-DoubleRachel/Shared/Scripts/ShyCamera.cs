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
    
}
