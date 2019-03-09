using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShyMiscTool
{
    public static void MySendMessageToAll(this PlayMakerFSM fsm,  string name)
    {
        if (!fsm)
            return;

        var go = fsm.gameObject;

        foreach(var i in go.GetComponents<PlayMakerFSM>())
        {
            i.SendEvent(name);
        }
    }
}
