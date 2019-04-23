using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class ShyAudioEvent : MonoBehaviour
{
    public string trackEventID;

    public GameObject targetFSM;
    public string[] eventNameList;

    int curIndex = -1;

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        Koreographer.Instance.RegisterForEvents(trackEventID, FireEventDebugLog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FireEventDebugLog(KoreographyEvent koreoEvent)
    {
        Debug.Log("Koreography Event Fired.");

        
        if(koreoEvent.HasTextPayload())
        {
            var textPayload = koreoEvent.GetTextValue();
            if(targetFSM)
                targetFSM.MySendEventToAll(textPayload);

            Debug.Log("Payload: " + textPayload);
        }

        curIndex++;
        if (curIndex >= eventNameList.Length)
            return;
        var eventName = eventNameList[curIndex];
        if (targetFSM)
            targetFSM.MySendEventToAll(eventName);
    }


}
