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
    public bool doorOpened;
    private Transform switchSceneNPC;
    // Start is called before the first frame update
   
    public int totalTimeInSeconds = 180;
    private float MinutesOnClock;
    private float timer = 0;
    private Text timerText;
    private float LastSecond;

    private AudioSource myAS;
    private AudioClip[] clockTicks;

    void Start()
    {
        switchSceneNPC = GameObject.Find("SwitchSceneNPC").transform;
        
        float secondsOnClock = Mathf.FloorToInt((int)3600 - totalTimeInSeconds);
        MinutesOnClock = Mathf.RoundToInt(secondsOnClock/60);
        print(MinutesOnClock);
        
        timerText = GetComponent<Text>();
        timerText.color = new Color(0.2f,0.3f,0.35f);

        myAS = GetComponent<AudioSource>();
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
        if (doorOpened)
        {
            RaycastHit hit = new RaycastHit();
            Physics.SphereCast(switchSceneNPC.position, 1.6f, switchSceneNPC.forward, out hit);
            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Player"))
                {
                    print("seePlayer");
                    StartCoroutine(SceneSwitch(3));
                }
            }
        
        }
          
            
    }

    void TimerCountDown()
    {

        if (timer <= totalTimeInSeconds)
        {

            timer += Time.deltaTime;
            int seconds =  Mathf.RoundToInt(timer % 60);
            int minutes =  (int)MinutesOnClock + Mathf.FloorToInt(timer / 60f);
            int hour=8;

            string secTxt;
            if (seconds < 10)
                secTxt = '0' + seconds.ToString();
            else
                secTxt = seconds.ToString();
            timerText.text = '0' + hour.ToString() + ':' + minutes + ':' +  secTxt;
            //"TIME LEFT:" + minutes + "M" + seconds + "S"; 
            
            if(seconds-LastSecond>= 0.9f || LastSecond-seconds >= 58)
                if(!myAS.isPlaying)
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

    public IEnumerator SceneSwitch(int seconds)
    {
            yield return new WaitForSeconds(seconds);
            HomeSceneManager.IntoIndex = 1;
            SceneManager.LoadScene("HomeScene"); 
            yield return null;

    }

    
}
