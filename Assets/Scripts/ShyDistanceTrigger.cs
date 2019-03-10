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


    // Enter start
    [FoldoutGroup("Enter Event")]
    public UnityEventWithGameObject enterEvent = new UnityEventWithGameObject();
    
    [FoldoutGroup("Enter Event"), InlineButton("SelfAssignFsmToEnterEvent", "Self")]
    public PlayMakerFSM enterMsgFsm;
    List<string> enterEventNames = new List<string>();
    private List<string> EnterEventNames
    {
        get
        {
            UpdateEventNames(enterMsgFsm, enterEventNames);
            return enterEventNames;
        }
    }
    [FoldoutGroup("Enter Event"), ValueDropdown("EnterEventNames")]
    public string enterMsgEvent;
    // Enter end


    // Leave start
    [FoldoutGroup("Leave Event")]
    public UnityEventWithGameObject leaveEvent = new UnityEventWithGameObject();

    [FoldoutGroup("Leave Event"), InlineButton("SelfAssignFsmToLeaveEvent", "Self")]
    public PlayMakerFSM leaveMsgFsm;
    List<string> leaveEventNames = new List<string>();
    private List<string> LeaveEventNames
    {
        get
        {
            UpdateEventNames(leaveMsgFsm, leaveEventNames);
            return leaveEventNames;
        }
    }
    [FoldoutGroup("Leave Event"), ValueDropdown("LeaveEventNames")]
    public string leaveMsgEvent;
    // Leave end


    float lastDistance = 10000;


    // Start is called before the first frame update
    void Start()
    {
        
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
            enterEvent.Invoke(gameObject);
            if(enterMsgFsm)
                enterMsgFsm.MySendEventToAll(enterMsgEvent);
        }
        // Leave
        else if(lastDistance <= distance && curDis > distance)
        {
            leaveEvent.Invoke(gameObject);
            if(leaveMsgFsm)
                leaveMsgFsm.MySendEventToAll(leaveMsgEvent);
        }
        lastDistance = curDis;
    }

    protected void SelfAssignFSM(out PlayMakerFSM targetFSM)
    {
        targetFSM = this.GetComponent<PlayMakerFSM>();
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

    void SelfAssignFsmToEnterEvent()
    {
        SelfAssignFSM(out enterMsgFsm);
    }

    void SelfAssignFsmToLeaveEvent()
    {
        SelfAssignFSM(out leaveMsgFsm);
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
