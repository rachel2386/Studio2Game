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
            testGo.transform.parent = null;
            testGo.transform.localScale = go.transform.lossyScale;
            testGo.transform.parent = transform;
            go.SetActive(false);
            testGo.tag = "Pseudo";
        }
    }

    [Button]
    public void RestoreTest()
    {
        foreach (var go in children)
        {            
            go.SetActive(true);
        }


        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        int i = 0;
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
            Debug.Log("haha");
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            if(child.tag == "Pseudo")
                DestroyImmediate(child.gameObject);
        }
        //for(int i = transform.childCount; i > 0; i--)
        //{
        //    DestroyImmediate(transform.GetChild(0).gameObject);
        //}
    }
}
