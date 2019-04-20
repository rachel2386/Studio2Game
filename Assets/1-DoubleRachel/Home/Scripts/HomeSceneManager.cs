using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    public static int IntoIndex = 1;

    ShyFPSController fpsController;
    ShyCamera shyCam;

    public GameObject objectOnUI;
    public GameObject lookDownUI;

    public Transform[] playerBornPosis;

    PanRotation panRotation;

    public GameObject screwDriver;
    

    public int GetIntoIndex()
    {
        return IntoIndex;
    }

    private void Awake()
    {
        fpsController = FindObjectOfType<ShyFPSController>();
        shyCam = FindObjectOfType<ShyCamera>();

        panRotation = FindObjectOfType<PanRotation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IntoIndex == 0)
        {
            fpsController.lockMove = true;
            shyCam.useGrain = false;

            objectOnUI.SetActive(false);
            lookDownUI.SetActive(true);

            InitPosition(fpsController, playerBornPosis[0].position);
            fpsController.transform.rotation = playerBornPosis[0].rotation;

            screwDriver.SetActive(false);
        }
        else if (IntoIndex == 1)
        {
            fpsController.lockMove = false;
            shyCam.useGrain = false;

            objectOnUI.SetActive(true);
            lookDownUI.SetActive(false);

            InitPosition(fpsController, playerBornPosis[1].position);
            fpsController.transform.rotation = playerBornPosis[1].rotation;

            screwDriver.SetActive(true);

            
        }

    }

    void InitPosition(ShyFPSController controller, Vector3 posi)
    {
        controller.GetComponent<CharacterController>().enabled = false;
        fpsController.transform.position = posi;
        controller.GetComponent<CharacterController>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // fpsController.transform.position = playerBornPosis[1].position;
        // panRotation.PanRotationUpdateStateByLevel(IntoIndex);
    }
}
