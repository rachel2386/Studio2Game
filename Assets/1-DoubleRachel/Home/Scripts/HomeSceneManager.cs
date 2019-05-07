using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    public HomeSceneSetting initSetting;
    public static int IntoIndex = 2;

    ShyFPSController fpsController;
    ShyCamera shyCam;

    public GameObject objectOnUI;
    public GameObject lookDownUI;
    public GameObject arrowRoot;

    public Transform[] playerBornPosis;

    PanRotation panRotation;

    public GameObject screwDriver;
    public GameObject remote;
    

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
        if (initSetting)
            IntoIndex = initSetting.InitHomeIndex;

        screwDriver.SetActive(IntoIndex == 1);
        remote.SetActive(IntoIndex == 2);
        arrowRoot.SetActive(IntoIndex == 2);

        InitPosition(fpsController, playerBornPosis[IntoIndex].position);
        InitRotation(fpsController, playerBornPosis[IntoIndex]);
        

        shyCam.useGrain = false;
        if (IntoIndex == 0)
        {
            fpsController.lockMove = true;

            objectOnUI.SetActive(false);
            lookDownUI.SetActive(true);
        }
        else if (IntoIndex == 1)
        {
            fpsController.lockMove = false;            

            objectOnUI.SetActive(true);
            objectOnUI.MySendEventToAll("SHOW");
            // objectOnUI.transform.GetChild(0).gameObject.SetActive(true);
            lookDownUI.SetActive(false);         
        }
        else if(IntoIndex == 2)
        {
            fpsController.lockMove = false;
            objectOnUI.SetActive(true);
            lookDownUI.SetActive(false);
        }
        else if(IntoIndex == 3)
        {
            fpsController.lockMove = true;
            objectOnUI.SetActive(false);
            lookDownUI.SetActive(false);
        }

    }

    void InitPosition(ShyFPSController controller, Vector3 posi)
    {
        controller.GetComponent<CharacterController>().enabled = false;
        fpsController.transform.position = posi;
        controller.GetComponent<CharacterController>().enabled = true;

        if(IntoIndex == 3)
        {
            var cam = controller.GetComponentInChildren<ShyCamera>();
            var lp = cam.transform.localPosition;
            lp.y = 0.28f;
            cam.transform.localPosition = lp;
        }

        controller.GetMouseLook().ForceSetRotationFromCurrentGameObject(controller.transform);
    }

    void InitRotation(ShyFPSController controller, Transform startPoint)
    {
        var se = startPoint.eulerAngles;
        controller.transform.eulerAngles = new Vector3(0, se.y, 0);
        controller.GetComponentInChildren<ShyCamera>().transform.localEulerAngles = new Vector3(se.x, 0, 0);
        controller.GetMouseLook().ForceSetRotationFromCurrentGameObject(controller.transform);
    }

    // Update is called once per frame
    void Update()
    {
        // fpsController.transform.position = playerBornPosis[1].position;
        // panRotation.PanRotationUpdateStateByLevel(IntoIndex);
    }

    public void SetSchoolSceneIndex(int value)
    {
        SwitchScenes.gameState = value;
    }
}
