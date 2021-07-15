using UnityEngine;
using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using NAXS.Event;

public class NXP_SimpleFPVPlayer : MonoBehaviour
{

    [Header("Basics")]
    public Transform MovingObj;
    [HideInInspector]public Transform mainCam;

    // [Header("DevSetting")]
    // public bool DevModeVR = false;
    // public float VRPlayerHeight = 0.7f;



    [Header("ROT")]
    public bool allowRotation = false;
    private Vector3 tempRot;
    public float smoothRotSpeed = 5f;
    public float cursorSensitivity = 0.025f;

    [Header("Moving")]
    public bool allowMovement = true;
    public float initialSpeed = 10f;
    public float increasedSpeed = 1.25f;
    public float smoothStopSpeed = 10f;
    public float smoothMoveSpeed = 10f;
    public float smoothIncreaseSpeed = 10f;
    // public float deltaIncreaseSpeed = 10f;
    Vector3 deltaDir;


    // [FoldoutGroup("Physic")]public bool UseCharacterController;
    [FoldoutGroup("Physic")] public CharacterController characterController;
    [FoldoutGroup("Physic")] public float Gravity;


    [FoldoutGroup("Camera")] public CinemachineVirtualCamera vcam;
    [FoldoutGroup("Camera")] public bool controlFOV;
    [FoldoutGroup("Camera")] public float FovStep = 0.5f;
    [FoldoutGroup("Camera")] public float FovLerp = 3f;
    float tempFov;

    //Tilt
    [FoldoutGroup("Tilt")] public bool allowTilt = true;
    [FoldoutGroup("Tilt")] public float TiltStep = 5f;
    [FoldoutGroup("Tilt")] public float TiltLerpSpeed = 1f;
    float tempTilt = 0;
    Quaternion targetTilt = Quaternion.identity;


    private bool cursorToggleAllowed = true;
    [FoldoutGroup("others")] public KeyCode cursorToggleButton = KeyCode.Escape;
    [FoldoutGroup("others")] public KeyCode rotationToggleButton = KeyCode.Z;

    private KeyCode forwardButton = KeyCode.W;
    private KeyCode backwardButton = KeyCode.S;
    private KeyCode rightButton = KeyCode.D;
    private KeyCode leftButton = KeyCode.A;

    private float currentSpeed = 0f;
    private bool moving = false;
    private bool togglePressed = false;
    private bool rotationPaused = false;

    Vector3 lastDelta;



    private void OnEnable()
    {
        Screen.lockCursor = true;
        Cursor.visible = false;
    }

    void Reset()
    {
        MovingObj = transform;
        mainCam = transform;
    }

    private void Start()
    {

        if (!mainCam)
        {
            mainCam = Camera.main.transform;
        }

        tempRot = MovingObj.rotation.eulerAngles;
        tempFov = vcam.m_Lens.FieldOfView;

        // Start event listeners
        SetupEventListeners();
        SetupCinemachine();
    }

    void SetupCinemachine()
    {
        if (!vcam) return;
        CinemachineCore.GetInputAxis = GetCMMouseAxis;
    }



    private void Update()
    {
        if (vcam)
        {
            //Disable Control if vcam not live
            if (!CinemachineCore.Instance.IsLive(vcam))
                return;
        }

        CalMove();
        DevInput();
        TiltCtrl();
        FOVCtrl();
    }


    void TiltCtrl()
    {
        // if(!allowTilt){
        //     return;
        // }
        // Debug.Log(Input.mouseScrollDelta.y);
        // if(Input.mouseScrollDelta.y != 0){
        //     tempTilt += Input.mouseScrollDelta.y * TiltStep;
        // }

        // // if(tempTilt > 60f){
        // //     tempTilt = 60f;
        // // }
        // // if(tempTilt < -60f){
        // //     tempTilt = -60f;
        // // }

        // Debug.Log(tempTilt);
        // targetTilt =  Quaternion.Euler(new Vector3(0, 0, tempTilt));
        // mainCam.localRotation = Quaternion.Lerp(mainCam.localRotation, targetTilt, Time.deltaTime * TiltLerpSpeed);
    }

    void FOVCtrl()
    {
        if (!controlFOV || !vcam) return;

        tempFov -= Input.mouseScrollDelta.y * FovStep;
        // Debug.Log("tempFov "+tempFov);
        tempFov = Mathf.Clamp(tempFov, 10f, 120f);
        vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, tempFov, Time.deltaTime * FovLerp);
    }

    private void CalMove()
    {
        if (allowMovement)
        {
            MoveUpdate();
        }

        //ROTATION
        if (allowRotation && !vcam)
        {
            Vector3 eulerAngles = MovingObj.transform.eulerAngles;
            tempRot.x += -Input.GetAxis("Mouse Y") * 359f * cursorSensitivity;
            tempRot.y += Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
            var rot = Quaternion.Lerp(MovingObj.transform.rotation, Quaternion.Euler(tempRot), Time.deltaTime * smoothRotSpeed);

            mainCam.transform.rotation = rot;
        }
    }

    public float GetCMMouseAxis(string m_axis)
    {
        if (!allowRotation) return 0;

        if (m_axis == "Mouse X")
        {
            return UnityEngine.Input.GetAxis("Mouse X");
        }
        if (m_axis == "Mouse Y")
        {
            return UnityEngine.Input.GetAxis("Mouse Y");
        }
        return 0;
    }

    void DevInput()
    {
        //TOGGLE
        if (cursorToggleAllowed)
        {
            if (Input.GetKeyDown(cursorToggleButton))
            {
                if (!togglePressed)
                {
                    togglePressed = true;
                    Cursor.visible = !Cursor.visible;
                    if (Cursor.visible)
                    {
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
            else togglePressed = false;
        }
        else
        {
            togglePressed = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(rotationToggleButton))
        {
            allowRotation = !allowRotation;
            if (allowRotation)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
#endif
    }

    void MoveUpdate()
    {

        Vector3 deltaPosition = Vector3.zero;
        float targetSpeedMulti = 1f;
        bool lastMoving = moving;
        moving = false;


        //Check Direction
        CheckMove(forwardButton, ref deltaPosition, mainCam.forward);
        CheckMove(backwardButton, ref deltaPosition, -mainCam.forward);
        CheckMove(rightButton, ref deltaPosition, mainCam.right);
        CheckMove(leftButton, ref deltaPosition, -mainCam.right);

        CheckMove(KeyCode.Q, ref deltaPosition, new Vector3(mainCam.forward.x, 0f, mainCam.forward.z));
        CheckMove(KeyCode.E, ref deltaPosition, mainCam.up * 0.75f);
        CheckMove(KeyCode.X, ref deltaPosition, mainCam.up * -0.5f);

        CheckMove(KeyCode.PageUp, ref deltaPosition, mainCam.forward);
        CheckMove(KeyCode.PageDown, ref deltaPosition, -mainCam.forward);
        CheckMove(KeyCode.RightArrow, ref deltaPosition, mainCam.right);
        CheckMove(KeyCode.LeftArrow, ref deltaPosition, -mainCam.right);

        CheckMove(KeyCode.Mouse0, ref deltaPosition, mainCam.forward);
        CheckMove(KeyCode.Mouse2, ref deltaPosition, mainCam.up * 0.75f);


        // Set Position Old
        if (moving)
        {
            //Just Start Moving, Reset Speed
            if (moving != lastMoving) currentSpeed = initialSpeed;
            lastDelta = deltaPosition;

            //Run
            if (Input.GetKey(KeyCode.LeftShift))
            {
                targetSpeedMulti = Mathf.Lerp(1, increasedSpeed, Time.deltaTime * smoothIncreaseSpeed);
                currentSpeed = initialSpeed * targetSpeedMulti;
            }
        }
        else if (currentSpeed > 0)
        {
            //Stopping
            currentSpeed -= Time.deltaTime * smoothStopSpeed;
            deltaPosition = lastDelta;
        }
        else
        {
            currentSpeed = 0f;
            lastDelta = deltaDir;
        }

        if (characterController)
        {
            //move
            characterController.Move(deltaPosition * currentSpeed * Time.deltaTime);

            //apply gravity
            Vector3 velocity = new Vector3(0, -Gravity * Time.deltaTime, 0);
            characterController.Move(velocity);
        }
        else
        {
            // set postion of transform directly
            Vector3 targetPos = MovingObj.transform.position + deltaPosition * currentSpeed * Time.deltaTime;
            MovingObj.transform.position = Vector3.Lerp(MovingObj.transform.position, targetPos, smoothMoveSpeed * Time.deltaTime);
        }
    }


    private void CheckMove(KeyCode keyCode, ref Vector3 deltaPosition, Vector3 directionVector)
    {
        if (Input.GetKey(keyCode))
        {
            // if(horizontalMoveOnly){
            //     // Vector3 horDir = new Vector3(1f, 0f, 1f);
            //     directionVector = Vector3.Scale(directionVector, new Vector3(1f, 0f, 1f));
            // }
            moving = true;
            deltaPosition += directionVector;
            // deltaPosition += directionVector * Time.deltaTime * deltaIncreaseSpeed;
        }
    }

    public void ResetRot(Vector3 targetEuler, bool doLerp)
    {
        if (!doLerp)
        {
        }
        transform.rotation = Quaternion.Euler(targetEuler);
        tempRot = targetEuler;
    }


    // NXEvent
    void SetupEventListeners()
    {
        NXEvent.StartListening("EnableCameraRotation", () =>
        {
            allowRotation = true;
        });
        NXEvent.StartListening("DisableCameraRotation", () =>
        {
            allowRotation = false;
        });
        NXEvent.StartListening("EnablePlayerMovement", () =>
        {
            allowMovement = true;
        });
        NXEvent.StartListening("DisablePlayerMovement", () =>
        {
            allowMovement = false;
        });
    }
}
