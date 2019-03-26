using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;

public class ShyDistanceTrigger : MonoBehaviour
{
    public bool targetIsPlayer = true;
    [DisableIf("targetIsPlayer")]
    public GameObject target;

    public float distance = 0.5f;

    [FoldoutGroup("Debug")]
    public bool showArea = true;
    [FoldoutGroup("Debug")]
    public Color debugColor = new Color(1, 0.92f, 0.016f, 0.3f);


    // Events
    public ShyEvent enterEvent = new ShyEvent("On Enter");
    public ShyEvent leaveEvent = new ShyEvent("On Leave");

    [OnInspectorGUI]
    void SetShyEventParent()
    {
        enterEvent.component = this;
        leaveEvent.component = this;
    }


    float lastDistance = 10000;


    // Start is called before the first frame update
    void Start()
    {
        SetShyEventParent();
    }


    [OnInspectorGUI]
    void UpdateTarget()
    {
        if (targetIsPlayer)
            target = FindObjectOfType<ShyFPSController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        var curDis = Vector3.Distance(target.transform.position, transform.position);

        // Enter
        if(lastDistance > distance && curDis <= distance)
        {            
            enterEvent.Invoke();
        }
        // Leave
        else if(lastDistance <= distance && curDis > distance)
        {
            leaveEvent.Invoke();
        }
        lastDistance = curDis;
    }


    protected void UpdateEventNames(PlayMakerFSM fsm, List<string> names)
    {
        names.Clear();
        names.Add("None");
        if (fsm == null)
            return;


        var events = fsm.FsmEvents;
        foreach (var ev in events)
        {
            names.Add(ev.Name);
        }
    }


    void OnDrawGizmos()
    {
        if(showArea)
        {
            // Draw a yellow sphere at the transform's position           
            Gizmos.color = debugColor;
            Gizmos.DrawSphere(transform.position, distance);
        }
    }
}
