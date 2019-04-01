using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//[runInEditMode]
public class MusicPlayerController : MusicModel.MusicPlayer
{
    public float currentTime;
    
    // Start is called before the first frame update
    void Start()
    {

        //StartMusic();
        //ResumeMusic();
        //PauseMusic();

        try
        {
            delayedAudioSource = GameObject.Find("AudioPlayerDelayed").GetComponent<AudioSource>();
        }
        catch {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (delayedAudioSource != null) {
            delayedAudioSource.clip = source.clip;
        }
        

        if (source.isPlaying)
        {

            //StartMusic();
            if (offset > 0)
            {
                offset -= Time.deltaTime;
            }
            if (offset < 0)
            {
                canPlayDelayed = true;
            }
            try
            {


                if (canPlayDelayed && !delayedAudioSource.isPlaying)
                {
                    delayedAudioSource.Play();
                }
            }
            catch {
                source.volume = 1;
            }
            currentTime = source.time;
            GetMusicTime(source);
            GetMusicTimePercentage(clip);
            try
            {
                SetMusicTimer();
            }
            catch {
                return;
            }
            
            
            try
            {
                playButton.GetComponent<Image>().sprite = playSpriteNormal;
                SpriteState ss = new SpriteState();
                ss.pressedSprite = playSpriteClicked;
                playButton.GetComponent<Button>().spriteState = ss;
            }
            catch {
                return;
            }
            
        }
        else {
            try
            {
                playButton.GetComponent<Image>().sprite = pauseSpriteNormal;
                SpriteState ss = new SpriteState();
                ss.pressedSprite = pauseSpriteClicked;
                playButton.GetComponent<Button>().spriteState = ss;

            }
            catch
            {
                return;
            }
            
        }

        if (Input.GetKeyUp(KeyCode.Q)) {
            BackwardMusic();
        }else if (Input.GetKeyUp(KeyCode.E)) {
            ForwardMusic();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (source.isPlaying)
            {
                StartMusic();
            }
            else {
                PauseMusic();
            }
        }
    }
}
