using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

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
    
    public static void SetPpeParam(PostProcessingProfile profile, PpeSetting st, float value, float lerpFactor = 1)
    {
        if (st == PpeSetting.DEPTH_OF_FIELD_APERTURE)
        {
            var setting = profile.depthOfField.settings;
            setting.aperture = Mathf.Lerp(
                setting.aperture, value, lerpFactor);
            profile.depthOfField.settings = setting;

        }
        else if (st == PpeSetting.GRAIN_SIZE)
        {
            var setting = profile.grain.settings;
            setting.size = Mathf.Lerp(
                setting.size, value, lerpFactor);
            profile.grain.settings = setting;
        }
        else if (st == PpeSetting.VIGNETTE_INTENSITY)
        {
            var setting = profile.vignette.settings;
            setting.intensity = Mathf.Lerp(
                setting.intensity, value, lerpFactor);
            profile.vignette.settings = setting;
        }
        else if (st == PpeSetting.GRAIN_INTENSITY)
        {
            var setting = profile.grain.settings;
            setting.intensity = Mathf.Lerp(
                setting.intensity, value, lerpFactor);
            profile.grain.settings = setting;
        }
    }


    public static float GetPpeParam(PostProcessingProfile profile, PpeSetting st)
    {
        if (st == PpeSetting.DEPTH_OF_FIELD_APERTURE)
        {
            var setting = profile.depthOfField.settings;
            return setting.aperture;

        }
        else if (st == PpeSetting.GRAIN_SIZE)
        {
            var setting = profile.grain.settings;
            return setting.size;
        }
        else if (st == PpeSetting.VIGNETTE_INTENSITY)
        {
            var setting = profile.vignette.settings;
            return setting.intensity;
        }
        else if (st == PpeSetting.GRAIN_INTENSITY)
        {
            var setting = profile.grain.settings;
            return setting.intensity;
        }
        else
            return 0;
    }
}
