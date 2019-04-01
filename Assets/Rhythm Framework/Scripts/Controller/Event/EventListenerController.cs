using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListenerController : EventModel.EventListener
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CallEventAction()
    {
        //You code here
        GetComponent<ObstaclePlacingController>().PlaceObstacle(GetComponent<ObstaclePlacingController>().streetParent);
    }
}
