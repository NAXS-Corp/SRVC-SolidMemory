using UnityEngine;
using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using NAXS.Event;

public class NXP_SimpleFPVPlayer : MonoBehaviour
{

    [Header("Basics")]
    public Transform MovingObj;
    public Transform mainCam;

    [Header("DevSetting")]
    // public bool DevModeVR = false;
    // public float VRPlayerHeight = 0.7f;
    public bool horizontalMoveOnly;

    

    [Header("ROT")]
    public bool allowRotation = false;
	private Vector3 tempRot;
    public float smoothRotSpeed = 5f;
    public float cursorSensitivity = 0.025f;

    [Header("Moving")]
    public bool allowMovement = true;
    public float initialSpeed = 10f;
    public float increaseSpeed = 1.25f;
    public float smoothStopSpeed = 10f;
    public float smoothMoveSpeed = 10f;
    public float deltaIncreaseSpeed = 10f;
    Vector3 deltaDir;


    [FoldoutGroup("Character Controller")]public bool UseCharacterController;
    [FoldoutGroup("Character Controller")]public CharacterController characterController;

    
    [FoldoutGroup("Fov")]public bool controlFOV;
    [FoldoutGroup("Fov")]public float FovStep = 0.5f;
    [FoldoutGroup("Fov")]public float FovLerp = 3f;
    float tempFov;

    //Tilt
    [FoldoutGroup("Tilt")]public bool allowTilt = true;
    [FoldoutGroup("Tilt")]public float TiltStep = 5f;
    [FoldoutGroup("Tilt")]public float TiltLerpSpeed = 1f;
    float tempTilt = 0;
    Quaternion targetTilt = Quaternion.identity;


    private bool cursorToggleAllowed = true;
    [FoldoutGroup("others")]public KeyCode cursorToggleButton = KeyCode.Escape;
    [FoldoutGroup("others")]public KeyCode rotationToggleButton = KeyCode.Z;

    private KeyCode forwardButton = KeyCode.W;
    private KeyCode backwardButton = KeyCode.S;
    private KeyCode rightButton = KeyCode.D;
    private KeyCode leftButton = KeyCode.A;
    
    private float currentSpeed = 0f;
    private bool moving = false;
    private bool togglePressed = false;
    private bool rotationPaused = false;



    //Cinemachine Integration
    private CinemachineVirtualCamera vcam;

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
        vcam = GetComponent<CinemachineVirtualCamera>();

        if(!mainCam){
            mainCam = Camera.main.transform;
        }
        
		tempRot = MovingObj.rotation.eulerAngles;
        tempFov = vcam.m_Lens.FieldOfView;

        // Start event listeners
        SetupEventListeners();
    }

    private void Update()
    {
        if(vcam)
        {
            //Disable Control if vcam not live
            if(!CinemachineCore.Instance.IsLive(vcam))
                return;
        }

        CalMove();
        DevInput();
        TiltCtrl();
        FOVCtrl();
    }


    void TiltCtrl(){
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

    void FOVCtrl(){
        if(!controlFOV || !vcam) return;
        
        tempFov -= Input.mouseScrollDelta.y * FovStep;
        // Debug.Log("tempFov "+tempFov);
        tempFov = Mathf.Clamp(tempFov, 10f, 120f);
        vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, tempFov, Time.deltaTime * FovLerp);
    }

    private void CalMove(){
        if (allowMovement)
        {
            MoveUpdate();
        }

        //ROTATION
        if (allowRotation)
        {
            Vector3 eulerAngles = MovingObj.transform.eulerAngles;
			tempRot.x += -Input.GetAxis("Mouse Y") * 359f * cursorSensitivity;
			tempRot.y += Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
			MovingObj.transform.rotation = Quaternion.Lerp(MovingObj.transform.rotation, Quaternion.Euler(tempRot), Time.deltaTime* smoothRotSpeed);
        }

    }

    void DevInput(){
        //TOGGLE
        if (cursorToggleAllowed)
        {
            if (Input.GetKeyDown(cursorToggleButton))
            {
                if (!togglePressed)
                {
                    togglePressed = true;
                    Cursor.visible = !Cursor.visible;
                    if(Cursor.visible){
                        Cursor.lockState = CursorLockMode.None;
                    }else{
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
        if (Input.GetKeyDown(rotationToggleButton)) {
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

    void MoveUpdate(){
        Vector3 deltaPosition = Vector3.zero;
        float SpeedIncrease = 0f;
        bool lastMoving = moving;
        moving = false;


        //Check Direction
        CheckMove(forwardButton, ref deltaPosition, mainCam.forward);
        CheckMove(backwardButton, ref deltaPosition, -mainCam.forward);
        CheckMove(rightButton, ref deltaPosition, mainCam.right);
        CheckMove(leftButton, ref deltaPosition, -mainCam.right);

        CheckMove(KeyCode.Q, ref deltaPosition, new Vector3(mainCam.forward.x, 0f, mainCam.forward.z));
        CheckMove(KeyCode.E, ref deltaPosition, mainCam.up* 0.75f);
        CheckMove(KeyCode.X, ref deltaPosition, mainCam.up* -0.5f);

        CheckMove(KeyCode.PageUp, ref deltaPosition, mainCam.forward);
        CheckMove(KeyCode.PageDown, ref deltaPosition, -mainCam.forward);
        CheckMove(KeyCode.RightArrow, ref deltaPosition, mainCam.right);
        CheckMove(KeyCode.LeftArrow, ref deltaPosition, -mainCam.right);
        
        CheckMove(KeyCode.Mouse0, ref deltaPosition, mainCam.forward);
        CheckMove(KeyCode.Mouse2, ref deltaPosition, mainCam.up* 0.75f);

        // deltaDir += deltaPosition;
        // deltaDir.x = Mathf.Clamp(deltaDir.x, -2f, 2f);
        // deltaDir.y = Mathf.Clamp(deltaDir.y, -2f, 2f);
        // deltaDir.z = Mathf.Clamp(deltaDir.z, -2f, 2f);

        // //smooth Stop
        // deltaDir = Vector3.Lerp(deltaDir, Vector3.zero, Time.deltaTime);

        
        //speed input
        if(Input.GetKey(KeyCode.LeftShift)){
            SpeedIncrease = increaseSpeed;
        }
        if (moving)
            currentSpeed += SpeedIncrease * Time.deltaTime;


        // Set Position Old
        if (moving)
        {
            //Just Start Moving, Reset Speed
            if (moving != lastMoving)
                currentSpeed = initialSpeed;
            lastDelta = deltaPosition;
            
            // MovingObj.transform.position += deltaPosition * currentSpeed * Time.deltaTime;
            // Vector3 targetPos = MovingObj.transform.position + deltaDir*currentSpeed * Time.deltaTime;

            // Vector3 targetPos = MovingObj.transform.position + deltaPosition * currentSpeed * Time.deltaTime;
            // MovingObj.transform.position = Vector3.Lerp(MovingObj.transform.position, targetPos, smoothMoveSpeed* Time.deltaTime);            
        }
        else if(currentSpeed > 0){
            //Stopping
            currentSpeed -= Time.deltaTime * smoothStopSpeed;     
            // MovingObj.transform.position += lastDelta * currentSpeed * Time.deltaTime;
            deltaPosition = lastDelta;
        }else{
            currentSpeed = 0f;
            lastDelta = deltaDir;
        }


        if(UseCharacterController && characterController){
            characterController.Move(deltaPosition * currentSpeed * Time.deltaTime);
            return;
        }

        Vector3 targetPos = MovingObj.transform.position + deltaPosition * currentSpeed * Time.deltaTime;
        MovingObj.transform.position = Vector3.Lerp(MovingObj.transform.position, targetPos, smoothMoveSpeed* Time.deltaTime);
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

    public void ResetRot(Vector3 targetEuler, bool doLerp){
        if(!doLerp){
        }
            transform.rotation = Quaternion.Euler(targetEuler);
        tempRot = targetEuler;
    }


    // NXEvent
    void SetupEventListeners(){
        NXEvent.StartListening("EnableCameraRotation", () => {
            allowRotation = true;
        });
        NXEvent.StartListening("DisableCameraRotation", () => {
            allowRotation = false;
        });
        NXEvent.StartListening("EnablePlayerMovement", () => {
            allowMovement = true;
        });
        NXEvent.StartListening("DisablePlayerMovement", () => {
            allowMovement = false;
        });
    }    
}
