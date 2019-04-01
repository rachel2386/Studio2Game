using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatStructureController : BeatModel.BeatStructure
{

    // Start is called before the first frame update
    void Start()
    {
        musicPlayerController = musicPlayer.GetComponent<MusicPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentBeat = Mathf.RoundToInt(((musicPlayerController.currentTime / 60) * musicBPM) * beatInterTimes);
        if (currentBeat >= 0 && musicPlayerController.source.isPlaying) {
            try
            {
                SetBeatColor();
            }
            catch {
                print("Loop");
            }
            
        }
        
    }
}
