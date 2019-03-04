using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShyPutBackObject : ShyInteractableObject
{
    public GameObject putBackAnchor;

    public bool needValidate = false;
    [ShowIf("needValidate")]
    public List<string> keyWords;

    [PropertyOrder(-1)]
    public string emptyHandTooltip;
    [PropertyOrder(-1)]
    public string wrongThingTooltip;

    // whether the object can pick up again
    // after it has been put on this putback object
    public bool canPickAfterPut = false;


    // if get enough number of objects put into the putback object
    // we invoke an event
    [FoldoutGroup("Full Event")]
    public int invokeNumber = 1;
    [FoldoutGroup("Full Event")]
    public UnityEvent fullEvent = new UnityEvent();
    [FoldoutGroup("Full Event")]
    public PlayMakerFSM fullMsgFsm;


    List<string> avaEventNames = new List<string>();
    private List<string> AvaEventNames
    {
        get
        {
            UpdateAvaEventNames();
            return avaEventNames;
        }        
    }
    [FoldoutGroup("Full Event"), ValueDropdown("AvaEventNames")]
    public string fullMsgName;

    List<GameObject> container = new List<GameObject>();
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public bool Validate(GameObject go)
    {
        if (!needValidate)
            return true;

        bool ret = false;
        var inputName = go.name.ToLower();
        foreach(var keyWord in keyWords)
        {
            if (inputName.Contains(keyWord.ToLower()))
                return true;
        }

        return ret;
    }

    public override string GetTooltip()
    {
        var curHeld = sis.CurHeldObject;
        if (!curHeld)
            return emptyHandTooltip;

        bool val = Validate(curHeld);
        if (!val)
            return wrongThingTooltip;
                     
        return tooltip;
    }
 

    public void AddObjectToContainer(GameObject go)
    {
        container.Add(go);
        if(container.Count == invokeNumber)
        {
            fullEvent.Invoke();

            
        }
    }

    void UpdateAvaEventNames()
    {
        // Debug.Log("update names called");
        avaEventNames.Clear();
        var events = fullMsgFsm.FsmEvents;
        foreach (var ev in events)
        {
            avaEventNames.Add(ev.Name);
        }
    }
}
