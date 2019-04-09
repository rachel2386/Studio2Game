using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PopUpDialogue : MonoBehaviour
{
    private GameObject _playerObject;
    private Rigidbody playerRB;
    private ShyFPSController _playerController;
    private NPCFollowOnOverlap followPlayer;

    #region Dialogue Variables

    private Canvas myCanvas;
    private Text dialogueTxt;
    private GameObject DialogueBubble;

    [Header("Dialogue Bubble Properties")]
    //dialogueBox properties
    public Vector3 bubblePos = new Vector3(0, -100, 0);

    public Vector2 bubbleSize = new Vector2(650, 150);

    public int textSize = 30;

    //for text animation
    private string message;
    public float letterTime;
    private bool textPlayed = false;

    //audio
    public AudioClip dialogue;
    private AudioSource audioSource;

    //dialogue particles 
    private ParticleSystem txtParticles;

    #endregion


    public float sphereRadius = 1.5f;
    public static bool checkSceneSwitched = false;

    private bool _isPlayerObjectNotNull;
    private SeePlayer playerSeen;

    void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        //playerRB = _playerObject.GetComponent<Rigidbody>();
        _playerController = _playerObject.GetComponent<ShyFPSController>();

        followPlayer = GetComponent<NPCFollowOnOverlap>();
        _isfollowPlayerNotNull = followPlayer != null;

        txtParticles = GetComponentInChildren<ParticleSystem>();
        txtParticles.transform.localPosition = new Vector3(0, 1.35f, 0.06f);
        txtParticles.transform.localEulerAngles = Vector3.zero;

        myCanvas = FindObjectOfType<Canvas>();
        DialogueBubble = transform.Find("Dialogue").gameObject; //GetComponentInChildren<Image>().gameObject;
        DialogueBubble.SetActive(false);


        dialogueTxt = DialogueBubble.GetComponentInChildren<Text>();
        message = dialogueTxt.text;
        dialogueTxt.text = "";

        audioSource = GetComponent<AudioSource>();
        playerSeen = GetComponent<SeePlayer>();
    }

    private bool addingTxt = false;

    private bool reducedSpeed = false;

    private bool _isfollowPlayerNotNull;

    private bool isFollowing = false;


    // Update is called once per frame
    void Update()
    {
       
        if (playerSeen.PlayerSeen)
        {
            Debug.Log("player found");
            if (DetectPlayer())
            {
                if (!txtParticles.isPlaying)
                    txtParticles.Play();


                ReducePlayerSpeed(reducedSpeed);

                if (!textPlayed)
                {
                    reducedSpeed = true;

                    if (!addingTxt)
                        StartCoroutine(PlayTxt());

                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(dialogue, 1F);

                    SetDialogueState(myCanvas.transform, true);
                }
                else
                {
                    addingTxt = false;
                    StopCoroutine(PlayTxt());
                    reducedSpeed = false;
                    dialogueTxt.text = "";
                    SetDialogueState(transform, false);
                }


                if (dialogueTxt.text.ToCharArray().Length == message.ToCharArray().Length) // if coroutine finishes 
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        textPlayed = true;
                        isFollowing = true;
                        if (gameObject.CompareTag("Respawn"))
                            SwitchScene();
                    }
                }
            }

            else
            {
                if (_isfollowPlayerNotNull)
                {
                    if (followPlayer.DetectPlayer())
                        if (isFollowing)
                            textPlayed = true;
                }
                else
                {
                    textPlayed = false;
                    txtParticles.Stop();
                }
            }
        }
    }

    bool DetectPlayer()
    {
        bool playerDetected = false;

        Collider[] stuffInSphere = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider t in stuffInSphere)
        {
            if (t.gameObject.CompareTag("Player"))
                playerDetected = true;

//            playerDetected = t.gameObject.CompareTag("Player");
//            if (playerDetected)
//                break;
        }

        Array.Clear(stuffInSphere, 0, stuffInSphere.Length);

        return playerDetected;
    }

    void SetDialogueState(Transform parent, bool activeState)
    {
        DialogueBubble.transform.SetParent(parent);
        DialogueBubble.SetActive(activeState);
        DialogueBubble.transform.localPosition = bubblePos;
        DialogueBubble.transform.localEulerAngles = new Vector3(0, 0, 0);
        DialogueBubble.GetComponent<RectTransform>().sizeDelta = bubbleSize;
        dialogueTxt.fontSize = textSize;
    }

    void ReducePlayerSpeed(bool speedReduced)
    {
        if (speedReduced)
        {
            _playerController.disablePlayerMovement = true;
            //playerRB.velocity = Vector3.zero;
        }
        else _playerController.disablePlayerMovement = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, sphereRadius);
    }

    IEnumerator PlayTxt()
    {
        addingTxt = true;
        foreach (char letter in message.ToCharArray())
        {
            dialogueTxt.text += letter;
            yield return new WaitForSecondsRealtime(letterTime);
        }
    }

   
    void SwitchScene()
    {
        checkSceneSwitched = true;
        SceneManager.LoadScene(1);
    }
}