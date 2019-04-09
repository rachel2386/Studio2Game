using System.Collections;
using System.Collections.Generic;
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
    private bool SwitchSceneTimerStarts = false;
    
    public float time = 180;

    private Text timerText;
    private float LastSecond;

    private AudioSource myAS;
    private AudioClip[] clockTicks;

    void Start()
    {
        switchSceneNPC = GameObject.Find("SwitchSceneNPC").transform;
        timerText = GetComponent<Text>();
        timerText.color = Color.black;

        myAS = GetComponent<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            clockTicks[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/1-DoubleRachel/School/Audio/SFX/clocktick1.mp3" , typeof(AudioClip));//+ (i + 1) + ".mp3");
            print(clockTicks[i].name);
        }
        
        
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
            if (hit.transform.CompareTag("Player"))
            {
               print("seePlayer");
                SwitchSceneTimerStarts = true;
            }

            if (SwitchSceneTimerStarts)
                StartCoroutine(SceneSwitch(3));
        }
            
    }

    void TimerCountDown()
    {

        if (time >= 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.RoundToInt(time / 60f);
            float seconds = Mathf.Floor(time % 60);
            timerText.text = "TIME LEFT:" + minutes + "M" + seconds + "S"; 
            
            if(LastSecond -seconds >= 0.9f || seconds-LastSecond >= 58)
                if(!myAS.isPlaying)
                    myAS.Play();
           
                
                
      
        }
        else
        {
            time = time;
            timerText.color = Color.red;
            timerText.text = "YOU ARE LATE...AGAIN.";
            StartCoroutine(SceneSwitch(2));
        }

        LastSecond = Mathf.Floor(time % 60);;
    }

    public IEnumerator SceneSwitch(int seconds)
    {
            yield return new WaitForSeconds(seconds);
            HomeSceneManager.IntoIndex = 1;
            SceneManager.LoadScene("HomeScene"); 
            yield return null;

    }

    
}
