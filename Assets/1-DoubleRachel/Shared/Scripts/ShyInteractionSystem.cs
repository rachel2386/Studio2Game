using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShyInteractionSystem : MonoBehaviour
{
    ShyCamera shyCamera;
    [HideInInspector]
    public GameObject pickupRoot;    
    Camera cam;
    ShyUI uiLayer;
    Text centerText;
    
    string needToRefreshCenterText;
    public string NeedToRefreshCenterText
    {
        set { needToRefreshCenterText = value; }
        get { return needToRefreshCenterText; }
    }

    [HideInInspector]
    public GameObject curHeldObject;
    [HideInInspector]
    public GameObject curAimedObject;

    // used when the dialog or the other GUI is on
    bool IsWorking { get; set; }

    public bool forceHideCenterText = false;

    // Start is called before the first frame update
    void Start()
    {
        shyCamera = FindObjectOfType<ShyCamera>();
        pickupRoot = shyCamera.pickupRoot;
        cam = Camera.main;
        uiLayer = FindObjectOfType<ShyUI>();
        centerText = uiLayer.centerText.GetComponent<Text>();

        IsWorking = true;
    }

   

    // Update is called once per frame
    void Update()
    {
        UpdateInner();
    }

    public void SetIsWorking(bool work)
    {
        IsWorking = work;        
    }

    void UpdateInner()
    {
        ResetUI();

        if (IsWorking)
        {
            CheckIterableObject();
            CheckButton();
        }

        ApplyUI();
    }

    void ResetUI()
    {
        needToRefreshCenterText = "";
    }

    void ApplyUI()
    {
        centerText.text = needToRefreshCenterText;

        if (forceHideCenterText)
            centerText.text = "";
    }


    void CheckButton()
    {
        bool wasThrowPressed = LevelManager.Instance.PlayerActions.Fire.WasPressed;
        if (wasThrowPressed && curHeldObject && !curAimedObject)
        {
            var po = curHeldObject.GetComponent<ShyPickableObject>();
            var body = curHeldObject.GetComponent<Rigidbody>();

            curHeldObject.transform.SetParent(null);
            curHeldObject.transform.eulerAngles = po.OriRotation;
            curHeldObject.transform.localScale = po.OriLocalScale;

            if (body)
                body.isKinematic = false;

            curHeldObject = null;
        }
    }

    void CheckIterableObject()
    {

        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
        if (hits.Length > 0)
        {
            HandleInteractionS(hits);
        }

        
    }

    void HandleInteractionS(RaycastHit[] hits)
    {
        GameObject firstIO = null;
        GameObject firstPO = null;
        GameObject firstPBO = null;

        curAimedObject = null;

        foreach (var hit in hits)
        {
            var go = hit.collider.gameObject;
            var io = go.GetComponent<ShyInteractableObject>();
            // sometimes io can be the curHeldObject very occasionally
            // need to ignore this
            if (!io || !io.canInteract || io == curHeldObject)
                continue;            

            var po = io as ShyPickableObject;
            var pbo = io as ShyPutBackObject;

            if (io && !firstIO)
                firstIO = io.gameObject;

            if (po && !firstPO)
                firstPO = po.gameObject;

            if(pbo && !firstPBO)
                firstPBO = pbo.gameObject;
        }

        GameObject priorObject = null;
        if (firstPO)
            priorObject = firstPO;
        else if (firstPBO)
            priorObject = firstPBO;
        else
            priorObject = firstIO;

        curAimedObject = priorObject;
        HandleInteraction(priorObject);
    }

    void HandleInteraction(GameObject go)
    {
        if (go == null)
            return;

        bool wasPressed = LevelManager.Instance.PlayerActions.Fire.WasPressed;
        var io = go.GetComponent<ShyInteractableObject>();
        if (!io)
            return;        

        io.HandleInteraction();
        
    }
    

    public bool IsEmptyHand()
    {
        return curHeldObject == null;
    }
}
