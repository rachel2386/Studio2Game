using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShyInteractionSystem : MonoBehaviour
{
    ShyCamera shyCamera;
    GameObject pickupRoot;
    Camera cam;
    ShyUI uiLayer;
    Text centerText;
    string needToRefreshCenterText;

    GameObject curHeldObject;
    public GameObject CurHeldObject
    {
        get
        {
            return curHeldObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        shyCamera = FindObjectOfType<ShyCamera>();
        pickupRoot = shyCamera.pickupRoot;
        cam = Camera.main;
        uiLayer = FindObjectOfType<ShyUI>();
        centerText = uiLayer.centerText.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIterableObject();
    }

    void CheckIterableObject()
    {
        needToRefreshCenterText = "";

        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
        if (hits.Length > 0)
        {
            HandleInteractionS(hits);
        }

        centerText.text = needToRefreshCenterText;
    }

    void HandleInteractionS(RaycastHit[] hits)
    {
        GameObject firstIO = null;
        GameObject firstPO = null;
        GameObject firstPBO = null;

        foreach(var hit in hits)
        {
            var go = hit.collider.gameObject;
            var io = go.GetComponent<ShyInteractableObject>();
            if (!io || !io.canInteract)
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

        needToRefreshCenterText = io.GetTooltip();
        if (wasPressed)
        {
            io.Clicked();

            var po = io as ShyPickableObject;
            var pbo = io as ShyPutBackObject;
            // Shy PickableObject
            if (po)
            {
                PickUp(go);
            }
            // Shy PutBackObject
            else if (pbo)
            {
                PutBack(go);
            }
        }
    }

    void PutBack(GameObject go)
    {
        // nothing in hand
        if (!curHeldObject)
            return;
            
        var po = curHeldObject.GetComponent<ShyPickableObject>();
        var body = curHeldObject.GetComponent<Rigidbody>();
        var pbo = go.GetComponent<ShyPutBackObject>();

        // wrong thing
        var validate = pbo.Validate(curHeldObject);
        if (!validate)
            return;

        // right thing
        curHeldObject.transform.SetParent(null);
        curHeldObject.transform.eulerAngles = po.OriRotation;
        curHeldObject.transform.localScale = po.OriLocalScale;
        curHeldObject.transform.position = pbo.putBackAnchor ? pbo.putBackAnchor.transform.position : pbo.transform.position;

        pbo.AddObjectToContainer(curHeldObject);

        if(!pbo.canPickAfterPut)
        {
            po.canInteract = false;
        }

        if (body)
            body.isKinematic = false;

        curHeldObject = null;
    }

    void PickUp(GameObject go)
    {
        var po = go.GetComponent<ShyPickableObject>();
        var body = go.GetComponent<Rigidbody>();

        go.transform.SetParent(pickupRoot.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = po.pickupRotaion;
        go.transform.localScale = po.pickupScale;

        if (body)
            body.isKinematic = true;

        curHeldObject = go;
    }
}
