using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShyMiscTool
{
    // Send FSM event to all FSMs that is in the same game object with the input fsm / monobehavior
    public static void MySendEventToAll(this MonoBehaviour mb,  string name)
    {
        if (!mb)
            return;

        var go = mb.gameObject;

        MySendEventToAll(go, name);
    }

    // Send FSM event to all FSMs that is in the game object
    public static void MySendEventToAll(this GameObject go, string name)
    {
        if (!go)
            return;

        foreach (var i in go.GetComponents<PlayMakerFSM>())
        {
            i.SendEvent(name);
        }
    }

    public static float GetPlaneDistance(Vector3 go1, Vector3 go2)
    {
        go1.y = 0;
        go2.y = 0;

        return Vector3.Distance(go1, go2);
    }
    

}
