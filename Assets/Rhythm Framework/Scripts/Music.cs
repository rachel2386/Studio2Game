using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicModel {
   // [ExecuteAlways]
    public class MusicPlayer : MusicPlayerStatusController
    {
        public AudioClip clip;
        public AudioSource source;
        public bool musicStarted;

        public Sprite pauseSpriteNormal;
        public Sprite pauseSpriteClicked;
        public Sprite playSpriteNormal;
        public Sprite playSpriteClicked;

        public GameObject playButton;

        public AudioSource delayedAudioSource;
        public float offset;
        public bool canPlayDelayed;


        public void StartMusic()
        {

            if (!musicStarted)
            {
                source.clip = clip;
                //source.time = 0;
                source.Play();
                if (offset != 0) {
                    source.volume = 0;
                }
            }
            else {
                PauseMusic();
            }
            musicStarted = true;

        }

        public void ResumeMusic() {
            source.UnPause();
        }

        public void StopMusic() {

            source.Stop();
            
        }

        public void PauseMusic()
        {
            if (source.isPlaying)
            {
                source.Pause();
            }
            else
            {
                ResumeMusic();
            }
        }

        public void ForwardMusic() {
            if (source.time + 5 <= clip.length)
            {
                source.time += 5;
            }
            else {
                return;
            }
            
        }

        public void BackwardMusic() {
            
            if (source.time >= 5)
            {
                source.time -= 5;
            }
            else
            {
                source.time -= source.time;
            }
        }


    }
    //[ExecuteAlways]
    public class MusicPlayerStatusController : MonoBehaviour {


        public float musicTime;
        public float musicTimePercentage;

        public Slider musicTimerSlider;

        public void GetMusicTime(AudioSource source) {
            musicTime = source.time;
            
        }
        public void GetMusicTimePercentage(AudioClip clip)
        {
            musicTimePercentage = (( musicTime / clip.length) * 100);
            
        }

        public void SetMusicTimer() {
            musicTimerSlider.value = musicTimePercentage;
        }
    }
}

