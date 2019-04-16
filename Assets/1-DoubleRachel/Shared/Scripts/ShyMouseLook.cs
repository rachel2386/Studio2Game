using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityStandardAssets.Characters.FirstPerson;

[Serializable]
public class ShyMouseLook
{
    public enum ShyMode
    {
        DEFAULT,
        DYNAMIC,
        FORCED,
    }

    ShyFPSController controller;

    [HideInInspector]
    public bool needTempShowCursor = false;

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

    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck")]
    public bool angleCheck = true;

    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck"), EnableIf("angleCheck")]
    public AnimationCurve nearAngleCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck"), EnableIf("angleCheck")]
    public AnimationCurve farAngleCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [FoldoutGroup("Shy"), EnableIf("UseDynamicCheck"), EnableIf("angleCheck")]
    public AnimationCurve distanceCurve = AnimationCurve.Linear(0, 0, 1, 1);

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

        controller = character.gameObject.GetComponent<ShyFPSController>();
        
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
            sensitivityFactorX = 0.4f;
            sensitivityFactorY = 0.3f;
        }
        float yRot = LevelManager.Instance.PlayerActions.Look.X * XSensitivity * sensitivityFactorX;
        float xRot = LevelManager.Instance.PlayerActions.Look.Y * YSensitivity * sensitivityFactorY;

        if(controller.lockMouseLook || needTempShowCursor)
        {
            yRot = 0;
            xRot = 0;
        }

        //Debug.Log("x " + LevelManager.Instance.PlayerActions.Look.X);
        //Debug.Log("y " + LevelManager.Instance.PlayerActions.Look.Y);
        //if (Math.Abs(LevelManager.Instance.PlayerActions.Look.Y) > Math.Abs(LevelManager.Instance.PlayerActions.Look.X))
        //{
        //    Debug.Log("true");
        //}

        float vibIntensity = 0;
        bool isInShyAway = false;
        if (UseDynamicCheck)
        {
            var eyeLim = CheckEye(character, camera);
            // eye lim lower than current camera means camera should pitch down
            if (IsLowerThan(eyeLim, camera.localRotation))
            {
                isInShyAway = true;
                // if camera is about to pitch down
                // we should ignore the xRot if xRot goes up
                if (xRot > 0)
                {
                    xRot = 0;
                    if (Math.Abs(LevelManager.Instance.PlayerActions.Look.Y)> Math.Abs(LevelManager.Instance.PlayerActions.Look.X))
                    {
                        vibIntensity = 0.3f;
                    }
                      
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


        InControl.InputManager.ActiveDevice.Vibrate(vibIntensity);
        UpdateCursorLock();

        UpdateMouseLookInMenu();
    }

    public bool menuMode = false;
    public void SetMenuMode(bool mode)
    {
        menuMode = mode;
    }


    public float leftAng;    
    public float rightAng;
    public float topAng;
    public float bottomAng;

    void UpdateMouseLookInMenu()
    {
        var mousePosi = Input.mousePosition;
        var cam = Camera.main;
        var vp = cam.ScreenToViewportPoint(mousePosi);

        var horAng = Mathf.Lerp(leftAng, rightAng, vp.x);
        var verAng = Mathf.Lerp(bottomAng, topAng, vp.y);

        var charE = new Vector3(0, horAng, 0);
        var camE = new Vector3(verAng, 0, 0);

        controller.transform.localEulerAngles = charE;
        Camera.main.transform.localEulerAngles = camE;

        ForceSetRotationFromCurrentGameObject();
    }

    public void ForceSetRotationFromCurrentGameObject()
    {       
        m_CharacterTargetRot = controller.transform.localRotation;
        m_CameraTargetRot = Camera.main.transform.localRotation;
    }

    public static bool IsLineOfSightBlocked(Transform eye, Transform camera, Transform character)
    {
        bool ret = false;

        Ray ray = new Ray(eye.position, camera.position - eye.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000.0f))
        {
            if(hit.collider.gameObject != character.gameObject)
                return true;
        }

        return ret;
    }

    private Quaternion CheckEye(Transform character, Transform camera)
    {
        var eyes = GameObject.FindObjectsOfType<Eye>();
        

        // Debug.Log(camera.rotation.x / camera.rotation.w);
        Quaternion lowestQuaternion = camera.localRotation;
        bool foundOne = false;



        // for eyes in the view port
        foreach (var eye in eyes)
        {

            //if (backCheck && IsBehindEye(eye.transform, camera.GetComponent<Camera>()))
            //{
            //    continue;
            //}
            if (IsLineOfSightBlocked(eye.transform, camera, character))
                continue;

            var eyeInVp = camera.GetComponent<Camera>().WorldToViewportPoint(eye.transform.position);
            var vpTopInVp = eyeInVp;
            vpTopInVp.y = 1;
            var vpTopInWorld = camera.GetComponent<Camera>().ViewportToWorldPoint(vpTopInVp);
            var dirCamToTop = vpTopInWorld - camera.transform.position;
            var dirCamToEye = eye.transform.position - camera.transform.position;
            var thisAngleX = Vector3.SignedAngle(dirCamToTop, dirCamToEye, camera.right);
            var thisCamLimRot = camera.transform.localRotation * Quaternion.Euler(thisAngleX, 0, 0);
            Debug.DrawLine(camera.transform.position, vpTopInWorld);
            Debug.DrawLine(camera.transform.position, eye.transform.position);

            var backAngle = AngleBetweenEyeAndCameraPosi(eye.transform, camera.GetComponent<Camera>());
            var distance = Vector3.Distance(eye.transform.position, camera.position);
            thisCamLimRot = GetGradualRotation(
                thisCamLimRot, GetCameraMaxPitchRotation(camera),
                0, 180, backAngle, distance);

            // bool eyeInViewPortYZ = eyeInVp.y > 0 && eyeInVp.y < 1 && eyeInVp.z > 0;
            bool eyeInViewPortYZ = IsLowerThan(thisCamLimRot, camera.localRotation) && eyeInVp.z > 0;
            if (!eyeInViewPortYZ)
                continue;


            if (eyeInVp.x > 1 || eyeInVp.x < 0)
            {
                float offSetX = 0;
                bool inTheRight = eyeInVp.x > 1;
                offSetX = inTheRight ? eyeInVp.x - 1 : 0 - eyeInVp.x;
                
                //if (eye.gameObject.name == "Eye1")
                //    Debug.Log(offSetX);

                if (offSetX > whiskerDistance)
                    continue;

                // 0
                var rot0 = thisCamLimRot;
                // xWhiskerDistance
                var rot1 = GetCameraMaxPitchRotation(camera);
                var thisLerpedRot = Quaternion.Slerp(rot0, rot1, offSetX / whiskerDistance);
                                

                lowestQuaternion = GetLowerQuaternion(thisLerpedRot, lowestQuaternion);
                if (thisLerpedRot == lowestQuaternion)
                {
                    int a = 1;
                    a++;
                }
                foundOne = true;
            }
            else if (eyeInVp.x >= 0 && eyeInVp.x <= 1)
            {
                lowestQuaternion = GetLowerQuaternion(thisCamLimRot, lowestQuaternion);
                foundOne = true;
            }
        }

        

        return lowestQuaternion;
    }

    Quaternion GetCameraMaxPitchRotation(Transform camera)
    {
        var lea = camera.localEulerAngles;
        lea.x = -60;
        return Quaternion.Euler(lea);
    }

    Quaternion GetGradualRotation(Quaternion start, Quaternion end,
        float startAngle, float endAngle, float angle, float distance)
    {
        if (endAngle <= startAngle)
            return start;
        float angleRatio = (angle - startAngle) / (endAngle - startAngle);
        // var value = Quaternion.Slerp(start, end, curve.Evaluate(ratio));       


        var nearEvaluate = nearAngleCurve.Evaluate(angleRatio);
        var farEvaluate = farAngleCurve.Evaluate(angleRatio);

        var startDistance = 1;
        var endDistance = 5;
        var distanceRatio = (distance - startDistance) / (endDistance - startDistance);
        distanceRatio = Mathf.Clamp(distanceRatio, 0, 1);
        var distanceEvaluate = distanceCurve.Evaluate(distanceRatio);

        var mixedEvaluate = nearEvaluate + (farEvaluate - nearEvaluate) * distanceEvaluate;



        var value = Quaternion.SlerpUnclamped(start, end, mixedEvaluate);




        return value;
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

    float AngleBetweenEyeAndCameraForward(Transform eye, Camera camera)
    {
        var eyeForward = eye.forward;
        eyeForward.y = 0;

        var centerVpInWorld = camera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1));
        var camForward = centerVpInWorld - camera.transform.position;
        camForward.y = 0;

        return Vector3.Angle(eyeForward, -camForward);
    }

    // now we use -camToEye as return calculation
    // 0 means in frount of NPC
    // 180 means in back of NPC
    float AngleBetweenEyeAndCameraPosi(Transform eye, Camera camera)
    {
        var eyeForward = eye.forward;
        eyeForward.y = 0;

        var camToEye = eye.transform.position - camera.transform.position;
        camToEye.y = 0;

        return Vector3.Angle(eyeForward, -camToEye);
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
            if(needTempShowCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
                             
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