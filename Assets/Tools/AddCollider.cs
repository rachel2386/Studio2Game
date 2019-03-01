using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCollider : MonoBehaviour
{
    public GameObject attachRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void Attach()
    {
        RecursiveAttach(attachRoot.transform);
    }

    void RecursiveAttach(Transform parent)
    {
        AttachCollider(parent);
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            RecursiveAttach(child);
        }
    }

    void AttachCollider(Transform trans)
    {
        if (!trans)
            return;

        var meshFilter = trans.GetComponent<MeshFilter>();
        if (!meshFilter || !trans.GetComponent<MeshRenderer>())
            return;

        var colls = trans.GetComponents<Collider>();

        if (colls.Length != 0)
            return;

        if (meshFilter.name == "Cube")
        {
            trans.gameObject.AddComponent<BoxCollider>();
        }
        else
        {
            trans.gameObject.AddComponent<MeshCollider>();

        }
        
    }

    
}
