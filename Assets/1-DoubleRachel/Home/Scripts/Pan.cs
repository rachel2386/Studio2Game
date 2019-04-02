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
    public float[] beats;
    public float threshouldTime = 0.3f;


    AudioSource bgm;
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

        bgm = radio.GetComponent<AudioSource>();
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(radio.GetComponent<AudioSource>().time);
        CheckMusicProgress();
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
        bgm.Play();
        
        
    }

    public void RefreshGrade()
    {
        // the reference is either the next beat or the last trigger beat
        var refLast = lastTriggerTime;

        float span = 0;
        if (lastTriggerIndex != beats.Length - 1)
        {
            span = beats[lastTriggerIndex + 1] - beats[lastTriggerIndex];
        }
        else
            span = bgm.clip.length - beats[lastTriggerIndex] + beats[0];
        var refNext = lastTriggerTime + span;

        var dt = Mathf.Min(Mathf.Abs(Time.time - refLast), Mathf.Abs(Time.time - refNext));
        //Debug.Log("Now: " + Time.time);
        //Debug.Log("Deviation: " + dt);

        // 0 best    dt = 0
        // 10 worst  dt = threshouldTime

        var grade = dt / threshouldTime * 10.0f;
        grade = Mathf.Clamp(grade, 0, 10);
        lastGrade = grade;
    }

    float lastTriggerTime = -1;
    int lastTriggerIndex = 0;
    

    int beatIndex = 0;
    void CheckMusicProgress()
    {
        var pt = bgm.time % bgm.clip.length;        
        

        if (pt > beats[beatIndex]
            && !(beatIndex == 0 && pt > beats[1]))
        {
            
            lastTriggerTime = Time.time;
            lastTriggerIndex = beatIndex;

            // Debug.Log("LastTriggerTime: " + lastTriggerTime);

            beatIndex++;
            beatIndex %= beats.Length;
            radio.MySendEventToAll("SHAKE");
        }
        
    }

}
