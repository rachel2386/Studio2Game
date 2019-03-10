using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShyDialogTrigger : MonoBehaviour
{
    [Tooltip("Not used curretly")]
    public string characterName = "";

    [FormerlySerializedAs("startNode")]
    public string talkToNode = "";

    public bool lockMove = false;
    public bool lockMouseLook = false;
    

    [Header("Optional")]    
    public TextAsset scriptToLoad;

    // Use this for initialization
    void Start()
    {
        if (scriptToLoad != null)
        {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().AddScript(scriptToLoad);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartDialog()
    {
        StartDialogWithMatch(true);
    }

    public void StartDialogForce()
    {
        StartDialogWithMatch(false);
    }

    // Stop and hide all the UI instantly
    // Made by TonTron
    public void StopDialog()
    {
        StopDialogWithMatch(true);
    }

    public void StopDialogForce()
    {
        StopDialogWithMatch(false);
    }

    // needMatch means the trigger started the dialog should be the same that is shown currently
    // So that the others won't disturb the current session
    public void StartDialogWithMatch(bool needMatch = true)
    {
        FindObjectOfType<ShyDialogManager>().StartDialog(this, talkToNode, needMatch);
    }

    public void StopDialogWithMatch(bool needMatch = true)
    {
        FindObjectOfType<ShyDialogManager>().StopDialog(this, needMatch);
    }


    // Maked as stopped
    // but still wait for the next input
    // The default stop offerred by Yarn
    // needMatch means the trigger started the dialog should be the same that is shown currently
    // So that the others won't stop the current session by mistake
    public void StopDialogLecacyWithMatch(bool needMatch = true)
    {
        FindObjectOfType<ShyDialogManager>().StopDialogLegacy(this, needMatch);
    }
}
