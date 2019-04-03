using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public GameObject panCenter;
    public GameObject panEdge;
    public GameObject farthest;
    public GameObject jumpAnchorY;
    public GameObject foodRoot;

    protected List<GameObject> foodList = new List<GameObject>();

    float radius;
    public float Radius
    {
        get { return radius; }
    }

    float max;
    public float Max
    {
        get { return max; }
    }


    private void Awake()
    {
        radius = ShyMiscTool.GetPlaneDistance(panCenter.transform.position, panEdge.transform.position);
        max = ShyMiscTool.GetPlaneDistance(panCenter.transform.position, farthest.transform.position);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        var foodCompenents = foodRoot.GetComponentsInChildren<Food>();
        foreach (var com in foodCompenents)
            foodList.Add(com.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
