using BeatModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatActionController : BeatModel.BeatAction
{
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GameObject.Find("AudioPlayer");
        musicPlayerController = musicPlayer.GetComponent<MusicPlayerController>();
        beatController = GetComponent<BeatPlayerController>();
        //beatOrdered = beatController.beatOrdered;
    }

    // Update is called once per frame
    void Update()
    {
        beatOrdered = beatController.beatOrdered;
        currentBeat = Mathf.RoundToInt(((musicPlayerController.currentTime / 60) * musicBPM) * beatInterTimes);
        if (currentBeat >= 0 && musicPlayerController.source.isPlaying)
        {
            CallBeatAction();
        }

    }
}
