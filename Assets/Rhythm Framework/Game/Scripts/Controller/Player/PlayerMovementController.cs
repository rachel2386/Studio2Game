using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementController : PlayerModel.PlayerMovement

{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("obstacle")) {
            SceneManager.LoadScene("MusicGamePlay", LoadSceneMode.Single);
        }
    }
}
