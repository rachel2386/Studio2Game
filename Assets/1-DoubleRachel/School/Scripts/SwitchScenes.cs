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
    [HideInInspector]public bool doorOpened;
    [HideInInspector]public bool switchScene;
    private Transform switchSceneNPC;
    // Start is called before the first frame update
    public int playMakerState;
    private int totalTimeInSeconds;
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
           RenderSettings.ambientLight = new Color(0.57f, 0.65f, 0.73f);
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
           RenderSettings.ambientLight = new Color(1f, 0.56f, 0.36f);
            GameObject.Find("Scene0").SetActive(false);
            GameObject.Find("Scene1").SetActive(false);  
        }
    }

    void Start()
    {
        playMakerState = gameState;
        
        Timer = GameObject.Find("Timer");
        
        if (gameState == 0)
        {
            totalTimeInSeconds = 10 * 60;
        }
        if (gameState == 1)
        {
            totalTimeInSeconds = 5 * 60;
        }
        if (gameState == 2)
        {
            totalTimeInSeconds = 3600;
        }
        

        float secondsOnClock = Mathf.FloorToInt((int)3600 - totalTimeInSeconds);
        MinutesOnClock = Mathf.RoundToInt(secondsOnClock / 60);
        timerText = Timer.GetComponent<Text>();
        timerText.color = Color.black;
        myAS = Timer.GetComponent<AudioSource>();
       
        if (gameState == 1)
        {
          switchSceneNPC = GameObject.Find("SwitchSceneNPC").transform;
            
        }
       
 
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if(switchScene)
            StartCoroutine(SceneSwitch(3));

        if (gameState == 0)
        {
            TimerCountDown(7); 
            
        }
        else if(gameState == 1)
        {
            TimerCountDown(13); 
        }
        else
        {
            TimerCountDown(17);
        }

        
        // npc detects player after door opened
        // scene switches when player see npc
        if (gameState != 1) return;
        if (!doorOpened) return;
        print("door Opened");
        RaycastHit hit = new RaycastHit();
        Physics.SphereCast(switchSceneNPC.position, 1.6f, switchSceneNPC.forward, out hit);
        Debug.DrawRay(switchSceneNPC.position,switchSceneNPC.forward,Color.red);
        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                print("seePlayer!!");
                switchScene = true;
           
            }
        }
        
        




    }

    private float lateTimer = 0;
    void TimerCountDown(int hour)
    {
        int seconds =  Mathf.RoundToInt(timer % 60);
        int minutes =  (int)MinutesOnClock + Mathf.FloorToInt(timer / 60f);
        
            timer += Time.deltaTime;

        if (timer <= totalTimeInSeconds)
            {
               
                TickingSound(seconds, LastSecond); 
                LastSecond = Mathf.RoundToInt(timer % 60);
            }
            else
            {
                timer = timer;
                lateTimer += Time.deltaTime;
                timerText.color = Color.red;
                //StartCoroutine(SceneSwitch(2));
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

    void TickingSound(int second, int lastSecond)
    {
        if (second - lastSecond >= 0.9f || lastSecond == 59 && second == 0)
            if (!myAS.isPlaying)
                myAS.Play();
    }

    public IEnumerator SceneSwitch(int seconds)
    {
            yield return new WaitForSeconds(seconds);
              
            if (gameState == 0)
            {
                HomeSceneManager.IntoIndex = 0;
                gameState = 1;
                SceneManager.LoadScene("HomeScene"); 
                //SceneManager.LoadScene("School"); 
            }
            else if (gameState == 1)
            {
                //HomeSceneManager.IntoIndex = 1;
                gameState = 2;
               // SceneManager.LoadScene("HomeScene"); 
                SceneManager.LoadScene("DarkGym"); 
            }
            else if (gameState == 2)
            {
                SceneManager.LoadScene("Gym");
            }
            
            yield return null;
    }

    
}
