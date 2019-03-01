using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoGameObject : MonoBehaviour
{
    public GameObject[] children;

    private void Awake()
    {
        foreach(var go in children)
        {
            go.transform.SetParent(this.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void TestMode()
    {
        foreach (var go in children)
        {
            var testGo = Instantiate(go, go.transform.position, go.transform.rotation, this.transform);
            testGo.transform.localScale = go.transform.lossyScale;
            go.SetActive(false);         
        }
    }

    [Button]
    public void RestoreTest()
    {
        foreach (var go in children)
        {            
            go.SetActive(true);
        }

        for(int i = transform.childCount; i > 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
