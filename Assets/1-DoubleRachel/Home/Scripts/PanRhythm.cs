using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanRhythm : Pan
{
    public GameObject radio;

    public float[] beats;
    public float threshouldTime = 0.3f;
    public float lastTossTime = -1;
    public float foodBottomColor = -3f;
    public float expandMax = 0.3f;
    public float heightChangeMax = -0.5f;

    AudioSource bgm;
   
    // 10 means good
    // 0 means bad.
    float lastGrade = 10;
    public float LastGrade
    {
        get { return lastGrade; }
        set { lastGrade = value; }
    }



    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        bgm = radio.GetComponent<AudioSource>();
        
        ResetFoodMat();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(radio.GetComponent<AudioSource>().totalTimeInSeconds);
        CheckMusicProgress();

        UpdateFoodMat();
    }




    

    bool inCook = false;
    public void StartMusic()
    {
        bgm.Play();
        StartCook();
    }

    public void StartCook()
    {
        inCook = true;

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
        //Debug.Log("Now: " + Time.totalTimeInSeconds);
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

    float lastProgress = -1;
    void CheckMusicProgress()
    {
        if (!bgm.isPlaying)
            return;


        var pt = bgm.time % bgm.clip.length;

        if (pt < lastProgress)
            lastProgress = -1;
        
        //if (pt > beats[beatIndex]
        //    && !(beatIndex == 0 && pt > beats[1]))
        if(lastProgress < beats[beatIndex] && pt >= beats[beatIndex])
        {
            BeatEncountered(beatIndex);
            lastTriggerTime = Time.time;
            lastTriggerIndex = beatIndex;

            // Debug.Log("LastTriggerTime: " + lastTriggerTime);

            beatIndex++;
            beatIndex %= beats.Length;
            
        }

        lastProgress = pt;
    }

    public void UnpauseBGM()
    {
        bgm.UnPause();
    }

    bool firstTurnOn = true;
    void BeatEncountered(int index)
    {
        Debug.Log(index);

        if (firstTurnOn && index == 0)
            firstTurnOn = false;
        else
            radio.MySendEventToAll("SHAKE");       


    }

  

    void UpdateFoodMat()
    {
        if (!inCook)
            return;
        
        foreach (var go in foodList)
        {
           
            var bC = go.GetComponent<Renderer>().material.GetFloat("_bottomColor");
            var expand = go.GetComponent<Renderer>().material.GetFloat("_expand");
            var heightChange  = go.GetComponent<Renderer>().material.GetFloat("_heightChange");

            /// heightChange           
            go.GetComponent<Renderer>().material.SetFloat("_bottomColor", Mathf.Lerp(bC, foodBottomColor, 0.15f * Time.deltaTime));
            go.GetComponent<Renderer>().material.SetFloat("_expand", Mathf.Lerp(expand, expandMax, 0.15f * Time.deltaTime));
            go.GetComponent<Renderer>().material.SetFloat("_heightChange", Mathf.Lerp(heightChange, heightChangeMax, 0.15f * Time.deltaTime));

            // go.GetComponent<Renderer>().material.color = Color.Lerp(c, Color.black, 0.15f * Time.deltaTime);
        }
        
    }


    public void PanTossed()
    {
        lastTossTime = Time.time;
        ResetFoodMat();
    }

    void ResetFoodMat()
    {
        foreach (var go in foodList)
        {
            // go.GetComponent<Renderer>().material.color = Color.white;
            go.GetComponent<Renderer>().material.SetFloat("_bottomColor", 1);
            go.GetComponent<Renderer>().material.SetFloat("_expand", 0);
            go.GetComponent<Renderer>().material.SetFloat("_heightChange", 0);
        }
    }
}

