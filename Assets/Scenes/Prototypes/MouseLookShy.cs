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
    public float whiskerDistance = 3;

    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck"), DisableIf("smooth")]
    [PropertyTooltip("The lerp smooth speed when shy away if the standard smooth is not turned on")]
    public float shySmooth = 5;


    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck"), EnableIf("smooth")]
    [PropertyTooltip("The lerp factor multiplied to the standard smooth when it's a shy process")]
    public float smoothFactor = 0.5f;



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
        float sensitivityFactorX = 1;
        float sensitivityFactorY = 1;
        var inputClass = InControl.InputManager.ActiveDevice.DeviceClass;
        if (inputClass == InControl.InputDeviceClass.Controller)
        {
            sensitivityFactorX = 0.5f;
            sensitivityFactorY = 0.3f;
        }
        float yRot = LevelManager.Instance.PlayerActions.Look.X * XSensitivity * sensitivityFactorX;
        float xRot = LevelManager.Instance.PlayerActions.Look.Y * YSensitivity * sensitivityFactorY;

        bool isInShyAway = false;
        if (UseDynamicCheck)
        {
            var eyeLim = CheckEye(camera);
            // eye lim lower than current camera means camera should pitch down
            if (IsLowerThan(eyeLim, camera.localRotation))
            {
                isInShyAway = true;
                // if camera is about to pitch down
                // we should ignore the xRot if xRot goes up
                if (xRot > 0)
                {
                    xRot = 0;
                }

                // if the standard smooth is on
                // we should not slerp the destination here
                // or else we will make a double lerp, which will make a pretty slow animation
                if (smooth)
                {
                    m_CameraTargetRot = eyeLim;
                }                
                else
                {
                    m_CameraTargetRot = Quaternion.Slerp(camera.localRotation, eyeLim,
                        shySmooth * Time.deltaTime);
                }                    
            }
        }


        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            // if is in shy away, maybe we need to make it shy away faster than regular lerp
            var factor = isInShyAway ? smoothFactor : 1.0f;
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime * factor);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime * factor);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }

        

        UpdateCursorLock();
    }


    private Quaternion CheckEye(Transform camera)
    {
        var eyes = GameObject.FindObjectsOfType<Eye>();
        float vibIntensity = 0;

        // Debug.Log(camera.rotation.x / camera.rotation.w);
        Quaternion lowestQuaternion = camera.localRotation;
        bool foundOne = false;



        // for eyes in the view port
        foreach (var eye in eyes)
        {

            if (IsBehindEye(eye.transform, camera.GetComponent<Camera>()))
            {
                continue;
            }

            var eyeInVp = camera.GetComponent<Camera>().WorldToViewportPoint(eye.transform.position);

            var vpTopInVp = eyeInVp;
            vpTopInVp.y = 1;
            var vpTopInWorld = camera.GetComponent<Camera>().ViewportToWorldPoint(vpTopInVp);

            var dirCamToTop = vpTopInWorld - camera.transform.position;
            var dirCamToEye = eye.transform.position - camera.transform.position;

            var thisAngleX = Vector3.Angle(dirCamToTop, dirCamToEye);

            if (eyeInVp.y > 0 && eyeInVp.y < 1 && eyeInVp.z > 0
                && (eyeInVp.x > 1 || eyeInVp.x < 0))
            {
                float offSetX = 0;
                if (eyeInVp.x > 1)
                {
                    offSetX = eyeInVp.x - 1;
                    eyeInVp.x = 1;
                }
                else if (eyeInVp.x < 0)
                {
                    offSetX = 0 - eyeInVp.x;
                    eyeInVp.x = 0;
                }

                if (eye.gameObject.name == "Eye1")
                {
                    Debug.Log(offSetX);
                }
                if (offSetX > whiskerDistance)
                    continue;

                // 0
                var rot0 = camera.transform.localRotation * Quaternion.Euler(thisAngleX, 0, 0);
                // xWhiskerDistance
                var lea = camera.transform.localEulerAngles;
                lea.x = -89;
                var rot1 = Quaternion.Euler(lea);

                var thisRot = Quaternion.Slerp(rot0, rot1, offSetX / whiskerDistance);

                var rot0X = rot0.eulerAngles.x;
                var thisRotX = thisRot.eulerAngles.x;

                lowestQuaternion = GetLowerQuaternion(thisRot, lowestQuaternion);
                if (thisRot == lowestQuaternion)
                {
                    int a = 1;
                    a++;
                }
                foundOne = true;
            }
            else if (eyeInVp.y > 0 && eyeInVp.y < 1 && eyeInVp.z > 0 && eyeInVp.x >= 0 && eyeInVp.x <= 1)
            {
                var thisRot = camera.transform.localRotation * Quaternion.Euler(thisAngleX, 0, 0);
                lowestQuaternion = GetLowerQuaternion(thisRot, lowestQuaternion);
                foundOne = true;
            }
        }

        //if(foundOne)
        //{
        //    camera.transform.localRotation = lowestQuaternion;
        //    m_CameraTargetRot = lowestQuaternion;
        //}

        return lowestQuaternion;
    }

    Quaternion GetLowerQuaternion(Quaternion q1, Quaternion q2)
    {
        var x1 = q1.x / q1.w;
        var x2 = q2.x / q2.w;

        return x2 > x1 ? q2 : q1;
    }

    bool IsLowerThan(Quaternion q1, Quaternion q2)
    {
        var x1 = q1.x / q1.w;
        var x2 = q2.x / q2.w;

        return x1 > x2;
    }

    float AngleBetweenEyeAndCamera(Transform eye, Camera camera)
    {
        var eyeForward = eye.forward;
        eyeForward.y = 0;

        var centerVpInWorld = camera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1));
        var camForward = centerVpInWorld - camera.transform.position;
        camForward.y = 0;

        return Vector3.Angle(eyeForward, -camForward);
    }

    bool IsBehindEye(Transform eye, Camera camera)
    {
        bool ret = false;

        var eyeForward = eye.forward;
        eyeForward.y = 0;

        var camToEye = eye.transform.position - camera.transform.position;
        camToEye.y = 0;

        var angle = Vector3.Angle(eyeForward, camToEye);
        if (Mathf.Abs(angle) < 50)
            ret = true;

        return ret;

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


        // Debug.Log(m_cursorIsLocked);
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