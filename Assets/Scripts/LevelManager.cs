using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class LevelManager : MonoBehaviour
{

    static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance; }


    }


    public DoubleRachelPlayerActions PlayerActions
    {
        get { return playerActions; }
        set { playerActions = value; }
    }

    DoubleRachelPlayerActions playerActions;


    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            playerActions = DoubleRachelPlayerActions.CreateWithDefaultBindings();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugButtons();
    }

    void DebugButtons()
    {
        if(playerActions.Fire.IsPressed)
        {
            Debug.Log("Fire Pressed");
        }
    }
}
