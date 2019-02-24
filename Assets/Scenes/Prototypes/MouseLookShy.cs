using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;

[Serializable]
public class MouseLookShy
{
    public enum ShyMode
    {
        DEFAULT,
        DYNAMIC,
        FORCED,
    }


    #region Var: Original MouseLook 
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;    
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;
    #endregion

    #region Var: Shy MouseLook 
    

    [FoldoutGroup("Shy"), EnumToggleButtons]
    public ShyMode shyMode = ShyMode.DYNAMIC;


    public bool UseDynamicCheck
    {
        get
        {
            return shyMode == ShyMode.DYNAMIC;
        }            
    }
    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck")]
    public bool smoothShyDown = true;

    public bool UseForcedAngle
    {
        get
        {
            return shyMode == ShyMode.FORCED;
        }
    }    
    [FoldoutGroup("Shy"), EnableIf("UseForcedAngle")]
    public float forcedAngle = 60f;
    #endregion

    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
      
    }


    public void BeginLitimationMode(float to = 45)
    {
        shyMode = ShyMode.FORCED;       
        
        DOTween.To(() => forcedAngle, x => forcedAngle = x, to, 2);
        // MinimumX = eyeLimitationAngle;
    }



    public void LookRotation(Transform character, Transform camera)
    {
        // Debug.Log(camera.transform.eulerAngles);
        float sensitivityFactorX = 1;
        float sensitivityFactorY = 1;
        var inputClass = InControl.InputManager.ActiveDevice.DeviceClass;
        if(inputClass == InControl.InputDeviceClass.Controller)
        {
            sensitivityFactorX = 0.5f;
            sensitivityFactorY = 0.3f;
        }

       // Debug.Log("LevelManager.Instance.PlayerActions.Look.Y  " + LevelManager.Instance.PlayerActions.Look.Y);

        float yRot = LevelManager.Instance.PlayerActions.Look.X * XSensitivity * sensitivityFactorX;
        // float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
        float xRot = LevelManager.Instance.PlayerActions.Look.Y * YSensitivity * sensitivityFactorY;

       // Debug.Log("YRot:" + yRot);
        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }

       if(UseDynamicCheck)
        {
            CheckEye(camera);
        }        

        UpdateCursorLock();
    }


    private void CheckEye(Transform camera)
    {
        var eyes = GameObject.FindObjectsOfType<Eye>();

        bool inFrame = false;
        float vib = 0;
        foreach (var eye in eyes)
        {
            var vp =  camera.GetComponent<Camera>().WorldToViewportPoint(eye.transform.position);

            
            // Debug.Log(vp);
            if (vp.x > 0 && vp.x < 1 && vp.y < 1 && vp.z > 0)
            {
                var camTop = vp;
                camTop.y = 1;
                var camTopPoint = camera.GetComponent<Camera>().ViewportToWorldPoint(camTop);


                var dirCamTop = camTopPoint - camera.transform.position;
                var dirEye = eye.transform.position - camera.transform.position;

                Debug.DrawLine(camera.transform.position, camTopPoint);
                Debug.DrawLine(camera.transform.position, eye.transform.position);

                var an = Vector3.Angle(dirCamTop, dirEye);

                if(vp.y < 0.98 && LevelManager.Instance.PlayerActions.Look.Y > 0.4f)
                    vib = 0.3f;
                
                var lea = camera.transform.localEulerAngles;
                lea.x += an;

                float factor = 10.0f;
                if (vp.y > 0.95f)
                    factor = 30.0f;


                var frameRot = Quaternion.Slerp(camera.localRotation, Quaternion.Euler(lea),
                factor * Time.deltaTime);
                if (!smoothShyDown)
                    frameRot = Quaternion.Euler(lea);
                camera.localRotation = frameRot;
                m_CameraTargetRot = frameRot;


                inFrame = true;
            }
        }

        InControl.InputManager.ActiveDevice.Vibrate(vib);

        var step = 100.0f;
        if (inFrame)
        {


            return;

            // return;
            var lea = camera.transform.localEulerAngles;
            lea.x += step * Time.deltaTime;
            camera.transform.localEulerAngles = lea;

            m_CameraTargetRot = Quaternion.Euler(lea);
        }
    }


    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
       
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }


        Debug.Log(m_cursorIsLocked);
        if (m_cursorIsLocked)
        {
            // Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        float min = MinimumX;
        if (UseForcedAngle)
            min = forcedAngle;

        angleX = Mathf.Clamp(angleX, min, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}