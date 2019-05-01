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
    public static int gameState = 0;
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


    void Awake()
    {
        if (gameState == 0)
        {
           //scene 1 active
            GameObject.Find("Scene1").SetActive(false);  
            GameObject.Find("Scene2").SetActive(false);
        }
        else if(gameState == 1) 
        {
           //scene 2 active
            GameObject.Find("Scene0").SetActive(false);  
            GameObject.Find("Scene2").SetActive(false);  
        }
        else if (gameState == 2)
        {
           //scene 0 active
            GameObject.Find("Scene0").SetActive(false);
            GameObject.Find("Scene1").SetActive(false);  
        }
    }

    void Start()
    {
        playMakerState = gameState;
        Timer = GameObject.Find("Timer");
        float secondsOnClock = Mathf.FloorToInt((int)3600 - totalTimeInSeconds);
        timerText = Timer.GetComponent<Text>();
        timerText.color = Color.black;
        myAS = Timer.GetComponent<AudioSource>();
        
       
        if (gameState == 1)
        {
           switchSceneNPC = GameObject.Find("SwitchSceneNPC").transform;
            MinutesOnClock = Mathf.RoundToInt(secondsOnClock / 60);
        }
        else if (gameState == 2)
        {
           MinutesOnClock = 0;
        }
 
        
    }

    // Update is called once per frame
    void Update()
    {
        
        TimerCountDown();
        
        // npc detects player after door opened
        // scene switches when player see npc
        if (gameState != 1) return;
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


        if (gameState == 1)
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
        else if (gameState == 2)
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

            if (gameState == 1)
            {
                HomeSceneManager.IntoIndex = 1;
                gameState = 2;
                SceneManager.LoadScene("HomeScene"); 
            }
            else if (gameState == 2)
            {
                SceneManager.LoadScene("Gym");
            }
            
            yield return null;
    }

    
}
