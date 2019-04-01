using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public GameObject panCenter;
    public GameObject panEdge;
    public GameObject farthest;
    public GameObject jumpAnchorY;
    public GameObject radio;

    public GameObject foodRoot;
    List<GameObject> foodList = new List<GameObject>();
  

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

    // 10 means good
    // 0 means bad.
    float lastGrade = 10;
    public float LastGrade
    {
        get { return lastGrade; }
        set { lastGrade = value; }
    }

    private void Awake()
    {
        radius = ShyMiscTool.GetPlaneDistance(panCenter.transform.position, panEdge.transform.position);
        max = ShyMiscTool.GetPlaneDistance(panCenter.transform.position, farthest.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        var foodCompenents = foodRoot.GetComponentsInChildren<Food>();
        foreach (var com in foodCompenents)
            foodList.Add(com.gameObject);

        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Return and remove the food from the list
    public GameObject PopRandomFood()
    {
        if (foodList.Count == 0)
            return null;

        var ranIndex = Random.Range(0, foodList.Count);

        var ret = foodList[ranIndex];
        foodList.RemoveAt(ranIndex);

        return ret;
    }

    public void SendEventGoodMode()
    {
        foreach(var food in foodList)
        {
            food.MySendEventToAll("FOOD_TOSS");
        }
    }

    public void SendEventBadMode()
    {
        var spiltFood = PopRandomFood();
        if (spiltFood)
        {
            spiltFood.MySendEventToAll("SPILT");
            spiltFood.transform.SetParent(null);
        }
           

        foreach (var food in foodList)
        {
            food.MySendEventToAll("FOOD_TOSS");
        }
    }

    public void StartMusic()
    {       
        StartCoroutine(MusicTrigger());
    }

    public void RefreshGrade()
    {
        var dt = Time.time - lastTriggerTime;
        if (dt > musicInterval / 2)
            dt = musicInterval - dt;

        // 0 best    dt = 0
        // 10 worst  dt = 0.5


        var grade = dt / 0.5f * 10.0f;
        grade = Mathf.Clamp(grade, 0, 10);
        lastGrade = grade;
    }

    float lastTriggerTime = 0;
    public float musicInterval = 1.77f;
    IEnumerator MusicTrigger()
    {
        while(true)
        {
            yield return new WaitForSeconds(musicInterval);
            lastTriggerTime = Time.time;
            radio.MySendEventToAll("SHAKE");
        }
    }

}
