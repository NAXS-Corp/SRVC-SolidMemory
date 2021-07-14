using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NAXS.Event;

namespace NAXS.NXPlayer
{
    public class NXP_PlayerInput : MonoBehaviour
    {
        // public InputActionAsset NXP_ControlAssets; // Can separate 2 assets to manage different controller
        private NXP_MainCtrl NXPMain;
        public float MoveAxisDeadZone = 0.2f;

        // public bool isVRPlayer;

        // private InputActionMap VRMap;
        // private InputActionMap PCMap;

        private PlayerInput m_PlayerInput;
        private InputAction m_Jump;
        public bool ActiveInputOnStart = true;
        private InputAction m_Run;
        private InputAction m_Move;
        private InputAction m_Option;
        // private List<InputAction> m_CustomAnim = new List<InputAction>();
        private InputAction m_CustomAnim0;
        private InputAction m_CustomAnim1;
        private InputAction m_CustomAnim2;
        private InputAction m_CustomAnim3;
        private InputAction m_CustomAnim4;

        private bool IsInputActive;
        private bool isOptionLastTime;

        public Vector2 MoveInput { get; private set; }
        private Vector2 m_moveInput;
        public Vector2 LastMoveInput { get; private set; }
        public Vector2 CameraInput { get; private set; }
        // public bool JumpInput { get; private set; }
        // public bool JumpInputDown { get; private set; }
        public bool JumpInput;
        public bool JumpInputDown;
        public bool RunInput { get; private set; }

        // public List<bool> CustomAnimInput;
        public bool CustomAnim0;
        public bool CustomAnim1;
        public bool CustomAnim2;
        public bool CustomAnim3;
        public bool CustomAnim4;

        public bool HasMoveInput { get; private set; }

        void Start()
        {
            IsInputActive = ActiveInputOnStart;
            // SetCursorLock(true);
            // StartSimulation();
            // InputMapSetup();
            NXEvent.StartListening("SetNXPInput", InputActive);
        }

        void Update()
        {
            if (m_Option.WasPressedThisFrame())
            {
                NXEvent.EmitEvent("CallVRMenu");
            }
        }

        void SetCursorLock(bool Lock)
        {
            lockMode = Lock;
            if (lockMode)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                lockRotation = false;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                lockRotation = true;
            }
        }



#region SimulationInput
        /////////////////////
        //Input Simulation
        /////////////////////

        [Header("SimulationDetails")]
        private bool EnableTestSimulation = false;

        public void InputActive()
        {
            IsInputActive = NXEvent.GetBool("SetNXPInput");
        }

        public void Initialize(NXP_MainCtrl mainCtrl)
        {
            NXPMain = mainCtrl;
        }
        public Vector2 SimuRoundTime = new Vector2(2f, 10f);
        public bool ForwardOnly;


        public void UpdateInput()
        {
            // Debug.Log("NXP_PlayerInput>> UpdateInput()");
            //Get Input data
            // Vector2 m_moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            // CameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            // JumpInput = Input.GetButton("Jump");
            // JumpInputDown = Input.GetButtonDown("Jump");
            // RunInput = Input.GetButton("Run");

            // New Input System -- 20210315
            if (m_PlayerInput == null)
            {
                m_PlayerInput = FindObjectOfType<PlayerInput>();
                m_Jump = m_PlayerInput.actions["Jump"];
                m_Run = m_PlayerInput.actions["Run"];
                m_Move = m_PlayerInput.actions["Move"];
                m_Option = m_PlayerInput.actions["Option"];
                
                // Custom Animations
                m_CustomAnim0 = m_PlayerInput.actions["CustomAnim0"];
                m_CustomAnim1 = m_PlayerInput.actions["CustomAnim1"];
                m_CustomAnim2 = m_PlayerInput.actions["CustomAnim2"];
                m_CustomAnim3 = m_PlayerInput.actions["CustomAnim3"];
                m_CustomAnim4 = m_PlayerInput.actions["CustomAnim4"];

                // foreach (InputAction CustomAnim in m_PlayerInput.actions)
                // {
                //     if (CustomAnim.name.Contains("CustomAnim"))
                //     {
                //         Debug.Log("NXP_PlayerInput: "+CustomAnim.name);
                //         m_CustomAnim.Add(CustomAnim);
                //     }
                // }
            }

            // Debug.Log("PlayerInput>> isGamestart: outside");

            // Debug.Log("PlayerInput>> isGamestart: inside");
            if (IsInputActive)
            {
                JumpInput = m_Jump.IsPressed();
                JumpInputDown = m_Jump.IsPressed();
                RunInput = m_Run.IsPressed();
                m_moveInput = m_Move.ReadValue<Vector2>();

                CustomAnim0 = m_CustomAnim0.IsPressed();
                CustomAnim1 = m_CustomAnim1.IsPressed();
                CustomAnim2 = m_CustomAnim2.IsPressed();
                CustomAnim3 = m_CustomAnim3.IsPressed();
                CustomAnim4 = m_CustomAnim4.IsPressed();

                // if (CustomAnimInput == null)
                // {
                //     foreach (InputAction CustomAnim in m_CustomAnim)
                //     {
                //         CustomAnimInput.Add(CustomAnim.IsPressed());
                //     }
                // }
                // else
                // {
                //     for (int i=0; i<m_CustomAnim.Count; i++)
                //     {
                //         bool CustomAnimInputUpdate = CustomAnimInput[i];
                //         CustomAnimInputUpdate = m_CustomAnim[i].IsPressed();
                //         CustomAnimInput[i] = CustomAnimInputUpdate;
                //     }
                // }
            }
            else
            {
                JumpInput = false;
                JumpInputDown = false;
                RunInput = false;
                m_moveInput = new Vector2(0, 0);

                CustomAnim0 = false;
                CustomAnim1 = false;
                CustomAnim2 = false;
                CustomAnim3 = false;
                CustomAnim4 = false;

                // if (CustomAnimInput == null)
                // {
                //     for (int i=0; i<m_CustomAnim.Count; i++)
                //     {
                //         CustomAnimInput.Add(false);
                //     }
                // }
                // else
                // {
                //     for (int i=0; i<m_CustomAnim.Count; i++)
                //     {
                //         bool CustomAnimInputUpdate = CustomAnimInput[i];
                //         CustomAnimInputUpdate = false;
                //         CustomAnimInput[i] = CustomAnimInputUpdate;
                //     }
                // }
            }

            
#if UNITY_EDITOR
            //LockRotation update
            if (lockRotation)
            {
                CameraInput = Vector2.zero;
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SetCursorLock(!lockMode);
            }
#endif

            // Check Move axis dead zone
            if (Mathf.Abs(m_moveInput.x) < MoveAxisDeadZone)
                m_moveInput.x = 0.0f;

            if (Mathf.Abs(m_moveInput.y) < MoveAxisDeadZone)
                m_moveInput.y = 0.0f;

            // Assign 
            MoveInput = m_moveInput;
            SimulationUpdate();

            // HasMoveInput
            bool hasMoveInput = MoveInput.sqrMagnitude > 0.0f;
            if (HasMoveInput && !hasMoveInput)
            {
                LastMoveInput = MoveInput;
            }
            HasMoveInput = hasMoveInput;
        }
        public Vector2 SimuDir = new Vector2(0f, 1f);
        public Vector2 SimuCamRange = new Vector2(0.3f, 0f);

        // private

        private bool lockMode = true;
        private bool lockRotation = false;

        private float simuCounter;
        private Vector2 SimuMove;
        private Vector2 SimuCam;
        private bool SimuJump;
        bool simuRun;
        public Transform simuCamObj;
        // bool lastEnableSimulation;

        void SimulationUpdate()
        {
// #if UNITY_EDITOR
//             if (Input.GetKey(KeyCode.LeftShift))
//             {
//                 if (Input.GetKeyDown(KeyCode.T))
//                 {
//                     EnableTestSimulation = !EnableTestSimulation;
//                 }
//             }
// #endif

            if (EnableTestSimulation)
            {
                MoveInput = SimuMove;
                CameraInput = SimuCam;
                RunInput = simuRun;
                JumpInput = SimuJump;
            }
        }

        public void StartSimulation()
        {
            EnableTestSimulation = true;
            StartCoroutine(SimulationRound());
        }
        IEnumerator SimulationRound()
        {
            Debug.Log("NXP_PlayerInput SimulationRound ");
            SimuMove = new Vector2(Random.Range(-SimuDir.x, SimuDir.x), Random.Range(-SimuDir.y, SimuDir.y));
            SimuCam = new Vector2(Random.Range(-SimuCamRange.x, SimuCamRange.x), Random.Range(-SimuCamRange.y, SimuCamRange.y));
            if (simuCamObj)
                simuCamObj.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            if (ForwardOnly)
            {
                SimuMove = new Vector2(0, Random.Range(SimuDir.y / 2, SimuDir.y));
                SimuCam = new Vector2(Random.Range(-SimuCamRange.x, SimuCamRange.x), 0);
            }

            if (Random.Range(0f, 1f) > 0.7f)
            {
                simuRun = true;
            }
            else
            {
                simuRun = false;
            }

            if (Random.Range(0f, 1f) > 0.7f)
            {
                SimuJump = true;
            }
            else
            {
                SimuJump = false;
            }


            yield return new WaitForSeconds(Random.Range(SimuRoundTime.x, SimuRoundTime.y));
            // if (EnableTestSimulation)
            // {
            //     StartCoroutine(SimulationRound());
            // }
        }
    #endregion
    }

}
