using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventModel{
    public class EventListener : MonoBehaviour
    {
        public int layer;
        public int currentBeatNumber;
        

        public void EventAction(int objLayer, int currentBeat) {

            

            if (layer == objLayer && currentBeatNumber != currentBeat) {
                GetComponent<EventListenerController>().CallEventAction();
                currentBeatNumber = currentBeat;
            }

            
        }
        
        
    }
}


