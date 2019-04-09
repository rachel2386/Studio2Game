 using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using Yarn.Unity;
using InControl;


// This is not a singleton
public class ShyDialogManager : Yarn.Unity.DialogueUIBehaviour
{
    DialogueRunner dialogRunner;
    ShyDialogUI dialogUI;
    ShyDialogTrigger curDialogTrigger;
    ShyFPSController fpsController;
    ShyInteractionSystem sis;

    PlayMakerFSM dialogFSM;

    bool inDialog = false;
    bool inOption = false;
    bool needForceStop = false;




    /// The object that contains the dialogue and the options.
    /** This object will be enabled when conversation starts, and 
        * disabled when it ends.
        */
    GameObject dialogueContainer;

    /// The UI element that displays lines
    Text lineText;

    /// A UI element that appears after lines have finished appearing
    GameObject continuePrompt;

    /// A delegate (ie a function-stored-in-a-variable) that
    /// we call to tell the dialogue system about what option
    /// the user selected
    private Yarn.OptionChooser SetSelectedOption;

    /// How quickly to show the text, in seconds per character
    [Tooltip("How quickly to show the text, in seconds per character")]
    public float textSpeed = 0.025f;

    /// The buttons that let the user choose an option
    List<Button> optionButtons = new List<Button>();

    /// Make it possible to temporarily disable the controls when
    /// dialogue is active and to restore them when dialogue ends
    public RectTransform gameControlsContainer;

    void Awake()
    {        
    }

    private void Start()
    {
        dialogRunner = FindObjectOfType<DialogueRunner>();
        dialogUI = FindObjectOfType<ShyDialogUI>();
        fpsController = FindObjectOfType<ShyFPSController>();
        sis = FindObjectOfType<ShyInteractionSystem>();
        dialogFSM = GetComponent<PlayMakerFSM>();
        if (dialogUI)
        {
            dialogueContainer = dialogUI.container;
            lineText = dialogUI.content.GetComponent<Text>();
            continuePrompt = dialogUI.content;

            int index = 0;
            foreach (var option in dialogUI.options)
            {
                var btn = option.GetComponent<Button>();
                
                if(btn)
                {
                    int thisIndex = index;
                    optionButtons.Add(btn);
                    btn.onClick.AddListener(() => SetOption(thisIndex));
                    index++;
                }
            }
        }



        // Start by hiding the container, line and option buttons
        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

        lineText.gameObject.SetActive(false);

        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Hide the continue prompt if it exists
        if (continuePrompt != null)
            continuePrompt.SetActive(false);
    }

    /// Show a line of dialogue, gradually
    public override IEnumerator RunLine(Yarn.Line line)
    {
        // Show the text
        lineText.gameObject.SetActive(true);

        if (textSpeed > 0.0f)
        {
            // Display the line one character at a time
            var stringBuilder = new StringBuilder();

            foreach (char c in line.text)
            {
                if (needForceStop)
                    break;

                
                stringBuilder.Append(c);
                lineText.text = stringBuilder.ToString();
                yield return new WaitForSeconds(textSpeed);
            }
        }
        else
        {
            // Display the line immediately if textSpeed == 0
            lineText.text = line.text;
        }

        // Show the 'press any key' prompt when done, if we have one
        if (continuePrompt != null)
            continuePrompt.SetActive(true);

        // Wait for any user input
        while (!ContinueClicked() && !needForceStop)
        {
            yield return null;
        }

        // Hide the text and prompt
        lineText.gameObject.SetActive(false);

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

    }

    bool ContinueClicked()
    {
        return LevelManager.Instance.PlayerActions.Fire.WasPressed;
    }

    /// Show a list of options, and wait for the player to make a selection.
    public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                            Yarn.OptionChooser optionChooser)
    {
        inOption = true;
        // show cursor
        ShowCursor(true);

        // Do a little bit of safety checking
        if (optionsCollection.options.Count > optionButtons.Count)
        {
            Debug.LogWarning("There are more options to present than there are" +
                             "buttons to present them in. This will cause problems.");
        }

        // Display each option in a button, and make it visible
        int i = 0;
        foreach (var optionString in optionsCollection.options)
        {
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].GetComponentInChildren<Text>().text = optionString;
            i++;
        }

        // Record that we're using it
        SetSelectedOption = optionChooser;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (SetSelectedOption != null && !needForceStop)
        {
            yield return null;
        }


        inOption = false;
        // hide cursor
        ShowCursor(false);
        // Hide all the buttons
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    /// Called by buttons to make a selection.
    public void SetOption(int selectedOption)
    {

        // Call the delegate to tell the dialogue system that we've
        // selected an option.
        SetSelectedOption(selectedOption);

        // Now remove the delegate so that the loop in RunOptions will exit
        SetSelectedOption = null;
    }

    /// Run an internal command.
    public override IEnumerator RunCommand(Yarn.Command command)
    {
        // "Perform" the command
        Debug.Log("Command: " + command.text);
        var words = command.text.Split(' ');
        HandleCommand(words);

        yield break;
    }

    public void HandleCommand(string[] args)
    {
        if (args == null || args.Length == 0)
            return;

        // 1.Send FSM event to the DialogManager FSM
        var eventName = args[0];
        dialogFSM.MySendEventToAll(eventName);

        // 2.Also FSM event to the current trigger FSM if it exsist
        if (curDialogTrigger)
            curDialogTrigger.MySendEventToAll(eventName);
    }

    /// Called when the dialogue system has started running.
    public override IEnumerator DialogueStarted()
    {
        inDialog = true;
        Debug.Log("Dialogue starting!");

        if(curDialogTrigger && !curDialogTrigger.canStillInteract)
            sis.SetIsWorking(false);

        // Enable the dialogue controls.
        if (dialogueContainer != null)
            dialogueContainer.SetActive(true);

        // Hide the game controls.
        if (gameControlsContainer != null)
        {
            gameControlsContainer.gameObject.SetActive(false);
        }

        yield break;
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete()
    {
        inDialog = false;
        needForceStop = false;
        inOption = false;
        sis.SetIsWorking(true);

        RestoreLockState();

        if (curDialogTrigger)
            curDialogTrigger.completeEvent.Invoke();

        curDialogTrigger = null;
        Debug.Log("Complete!");

        // Hide the dialogue interface.
        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

        // Show the game controls.
        if (gameControlsContainer != null)
        {
            gameControlsContainer.gameObject.SetActive(true);
        }

        yield break;
    }  

    void UpdateLockStateWithTrigger(ShyDialogTrigger trigger)
    {
        fpsController.lockMove = trigger.lockMove;
        fpsController.lockMouseLook = trigger.lockMouseLook;
    }

    void RestoreLockState()
    {
        fpsController.lockMove = false;
        fpsController.lockMouseLook = false;
    }

    public void StartDialog(ShyDialogTrigger trigger, string talkToNode, bool needMatch)
    {
        if (!dialogRunner)
            return;        

        if (!needMatch || !inDialog || curDialogTrigger == trigger)
        {
            if (trigger)
            {
                UpdateLockStateWithTrigger(trigger);
            }

            curDialogTrigger = trigger;
            dialogRunner.StartDialogue(talkToNode);
        }       
    }

    public void StopDialog(ShyDialogTrigger trigger, bool needMatch)
    {
        if (!dialogRunner)
            return;

        
        if (!needMatch || curDialogTrigger == trigger)
        {

            RestoreLockState();

            // Be carefull, the Stop() here won't stop immediately
            // Hence, the complete callback will be involked later
            dialogRunner.Stop();
            needForceStop = true;
            // curDialogTrigger = null;
        }            
    }

    public void StopDialogLegacy(ShyDialogTrigger trigger, bool needMatch)
    {
        if (!dialogRunner)
            return;

        if(!needMatch || curDialogTrigger == trigger)
        {

            RestoreLockState();

            dialogRunner.Stop();
            // curDialogTrigger = null;
        }
    }

    public void ShowCursor(bool show)
    {
        bool isController = LevelManager.Instance.PlayerActions.ActiveDevice.DeviceClass == InputDeviceClass.Controller;

        // If is using controller, ignore the show cursor process
        if (show && !isController)
            fpsController.SetTempShowCursor(true);
        else
            fpsController.SetTempShowCursor(false);
    }
}
