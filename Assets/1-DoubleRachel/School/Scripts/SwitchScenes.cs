using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScenes : MonoBehaviour
{
    public static int gameState = 1;
    public bool doorOpened;
    private Transform switchSceneNPC;
    // Start is called before the first frame update
    public int playMakerState;
    public int totalTimeInSeconds = 180;
    private GameObject Timer;
    private float MinutesOnClock;
    private float timer = 0;
    private Text timerText;
    private int LastSecond;

    private AudioSource myAS;
    private AudioClip[] clockTicks;

    void Start()
    {
        playMakerState = gameState;
        if (gameState == 0) 
        switchSceneNPC = GameObject.Find("SwitchSceneNPC").transform;

        Timer = GameObject.Find("Timer");
        float secondsOnClock = Mathf.FloorToInt((int)3600 - totalTimeInSeconds);

        MinutesOnClock = gameState == 0 ? Mathf.RoundToInt(secondsOnClock / 60) : 0;
        
        timerText = Timer.GetComponent<Text>();
        timerText.color = Color.black;

        myAS = Timer.GetComponent<AudioSource>();
//        for (int i = 0; i < 5; i++)
//        {
//            clockTicks[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/1-DoubleRachel/School/Audio/SFX/clocktick1.mp3" , typeof(AudioClip));//+ (i + 1) + ".mp3");
//            print(clockTicks[i].name);
//        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        TimerCountDown();
        
        // npc detects player after door opened
        // scene switches when player see npc
        if (gameState != 0) return;
        if (!doorOpened) return;
        RaycastHit hit = new RaycastHit();
        Physics.SphereCast(switchSceneNPC.position, 1.6f, switchSceneNPC.forward, out hit);
        Debug.DrawRay(switchSceneNPC.position,switchSceneNPC.forward,Color.red);
        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                print("seePlayer");
                StartCoroutine(SceneSwitch(3));
            }
        }




    }

    void TimerCountDown()
    {
        int seconds =  Mathf.RoundToInt(timer % 60);
        int minutes =  (int)MinutesOnClock + Mathf.FloorToInt(timer / 60f);
        int hour = 0;


        if (gameState == 0)
        {
            if (timer <= totalTimeInSeconds)
            {
                timer += Time.deltaTime;
                hour = 8;

                if (seconds - LastSecond >= 0.9f || LastSecond == 59 && seconds == 0)
                    if (!myAS.isPlaying)
                        myAS.Play();
            }
            else
            {
                timer = timer;
                timerText.color = Color.red;
                timerText.text = "09:00:00";
                StartCoroutine(SceneSwitch(2));
            }

            LastSecond = Mathf.RoundToInt(timer % 60);
        }
        else if (gameState == 1)
        {
            timer += Time.deltaTime;
            hour = 16;
        }
        
        string secTxt;
        if (seconds < 10)
            secTxt = '0' + seconds.ToString();
        else
            secTxt = seconds.ToString();

        string minText;
        if (minutes < 10)
            minText = '0' + minutes.ToString();
        else
            minText = minutes.ToString();
        
        string hourText;
        if (hour < 10)
            hourText = '0' + hour.ToString();
        else
            hourText = hour.ToString();
        
        timerText.text = hourText + ':' + minText + ':' +  secTxt;


    }

    public IEnumerator SceneSwitch(int seconds)
    {
            yield return new WaitForSeconds(seconds);
            HomeSceneManager.IntoIndex = 1;
            SceneManager.LoadScene("HomeScene"); 
            yield return null;

    }

    
}
