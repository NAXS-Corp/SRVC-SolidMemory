using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
    public enum MovementMode
    {
        Default,
        Hoverable,
        Flyable
    }

    public class NXP_Movement : MonoBehaviour
    {
        #region Paramaters
        [Header("Ref")]
        private NXP_MainCtrl _NXPMain;
        private NXP_PlayerInput _NXPInput;
        private NXP_AnimatorCtrl _NXPAnimator;
        [SerializeField] private CharacterController _characterController; // The Unity's CharacterController

        [Header("Settings")]
        public MovementMode MovementMode;
        public NXPMovementSettings Settings;
        public bool isSimpleInput;

		// Variables for movement caculation
        private Vector3 _horizontalDirection;
        private float _targetHorizontalSpeed; // In meters/second
        private float _horizontalSpeed; // In meters/second
        private float _verticalSpeed; // In meters/second
        private float _maxVerticalSpeed;
        private float _groundedCounter;
        private Quaternion yawRotation;
        private Vector3 m_targetPost; // SimpleInput


		// INPUTS
        private Vector3 _movementInputDir;
        private Vector3 _lastMovementInput;
        private Vector3 _SimpleInputDir;
        private bool _hasMovementInput;
        private bool _jumpInput;
        private bool _jumpInputDown;
        private bool _runInput;


		//Public Params for Animator
        public Vector3 Velocity => _characterController.velocity;
        public Vector3 HorizontalVelocity => _characterController.velocity.SetY(0.0f);
        public Vector3 VerticalVelocity => _characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);
        public float MovingRate => Mathf.Clamp((_horizontalSpeed / Settings.MovementSettings.MaxHorizontalSpeed + _verticalSpeed / _maxVerticalSpeed), 0f, 1f);
        public bool IsGrounded { get; private set; }
        public bool IsMoveing => (_hasMovementInput) ? true : false;
        public bool IsIdle => !_hasMovementInput || _verticalSpeed > 0;
        #endregion

		#region Initialize
        public void Initialize(NXP_MainCtrl mainCtrl)
        {
            _NXPMain = mainCtrl;
            _NXPInput = _NXPMain._NXPInput;
            if (_NXPMain._NXPAnimator)
                _NXPAnimator = _NXPMain._NXPAnimator;
            if (_NXPMain.IsLocalPlayer)
            {

            }
            else
            {
                Destroy(_characterController);
            }
            InitializeMovementMode(MovementMode);
        }
        public void InitializeMovementMode(MovementMode mode)
        {
            MovementMode = mode;

            switch (MovementMode)
            {
                case MovementMode.Hoverable:
                    _maxVerticalSpeed = Settings.HoverSettings.HoverUpSpeed;
                    break;
                case MovementMode.Flyable:
                    Settings.FlySettings.IsFly = true;
                    // _maxVerticalSpeed = Settings.HoverSettings.HoverUpSpeed;
                    break;
                default:
                    _maxVerticalSpeed = Settings.MovementSettings.JumpSpeed;
                    break;
            }
        }
		#endregion




        // Main Update
        private void Update()
        {
            Update_MovementInput();
            Update_JumpInput();
            Update_RunInput();
            Update_Cheat();

        }
        private void FixedUpdate()
        {
            FixedUpdate_HorizontalMoveSpeed();
            FixedUpdate_HorizontalDirection();
            FixedUpdate_VerticalSpeed();
            FixedUpdate_FollowTarget();

			//Caculate the final movement, and move the characterController
            Vector3 movement = _horizontalSpeed * _horizontalDirection + _verticalSpeed * Vector3.up;
            _characterController.Move(movement * Time.deltaTime);

            FixedUpdate_OrientToTargetRotation(movement.SetY(0.0f));

            IsGrounded = _characterController.isGrounded;

            _NXPInput.UpdateInput();
            if (_NXPAnimator)
                _NXPAnimator.UpdateState();
        }

        public void Update_MovementInput()
        {
            // Calculate the move direction relative to the Camera's yaw rotation
            if (isSimpleInput)
            {

            }
            else
            {
                yawRotation = Quaternion.Euler(0.0f, _NXPMain.MainCam.rotation.eulerAngles.y, 0.0f);
            }
            
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;
            Vector3 movementInput;

            if (isSimpleInput)
            {
                movementInput = (right * _SimpleInputDir.x + forward * _SimpleInputDir.z);
                if ((transform.position - m_targetPost).sqrMagnitude < 0.2f)
                {
                    _SimpleInputDir = Vector3.zero;
                }
            }
            else
                movementInput = (right * _NXPInput.MoveInput.x + forward * _NXPInput.MoveInput.y);

            if (movementInput.sqrMagnitude > 1f)
            {
                movementInput.Normalize();
            }
            _movementInputDir = movementInput;


			//Check if has movement input
            bool hasMovementInput = movementInput.sqrMagnitude > 0.0f;

            if (_hasMovementInput && !hasMovementInput)
            {
                _lastMovementInput = _movementInputDir;
            }
            _hasMovementInput = hasMovementInput;
        }

        public void Update_JumpInput()
        {
            _jumpInput = _NXPInput.JumpInput;
            _jumpInputDown = _NXPInput.JumpInputDown;
        }

        public void Update_RunInput()
        {
            _runInput = _NXPInput.RunInput;
        }


        private void FixedUpdate_HorizontalMoveSpeed()
        {
            float runSpeed;
            // float 
            switch (MovementMode)
            {
                case MovementMode.Flyable:
                    runSpeed = Settings.FlySettings.HorizontalFlyRunMulti;
                    break;

                default:
                    runSpeed = Settings.MovementSettings.RunSpeed;
                    break;
            }
            Vector3 movementInputDir = _movementInputDir;

			//Max speed toward the direction
            _targetHorizontalSpeed = movementInputDir.magnitude * Settings.MovementSettings.MaxHorizontalSpeed;

			//
            if (_runInput)
            {
                _targetHorizontalSpeed *= runSpeed;
            }

			//Acceleration or decceleration
            float acceleration = _hasMovementInput ? Settings.MovementSettings.Acceleration : Settings.MovementSettings.Decceleration;

            _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
        }

        private void FixedUpdate_HorizontalDirection()
        {
            Vector3 moveDir;
            if (_hasMovementInput)
            {
                moveDir = _movementInputDir;
            }
            else
            {
                moveDir = _lastMovementInput;
            }

            if (moveDir.sqrMagnitude > 1f)
            {
                moveDir.Normalize();
            }

            _horizontalDirection = moveDir;
        }

        private void FixedUpdate_VerticalSpeed()
        {
            switch (MovementMode)
            {
                case MovementMode.Flyable:

                    if (IsGrounded)
                    {
                        Settings.FlySettings.IsFly = false;
                    }

                    // ////////////
                    // NOT FLYING
                    if (!Settings.FlySettings.IsFly)
                    {

                        if (_jumpInputDown)
                        {
                            // ////////////
                            // Idle to Fly
                            IsGrounded = false;
                            Settings.FlySettings.IsFly = true;
                            _verticalSpeed = Settings.FlySettings.FlyUpSpeedOnSpace * 2;
                            break;

                        }
                        else
                        {
                            // Normal gravity for short falling
                            _verticalSpeed = -Settings.GravitySettings.GroundedGravity;

                            // ////////////
                            // Check ground dist to float
                            RaycastHit hit;
                            Ray downRay = new Ray(transform.position, -Vector3.up);
                            if (Physics.Raycast(downRay, out hit))
                            {
                                if (hit.distance > Settings.FlySettings.FloatDownHeight)
                                {
                                    _verticalSpeed = -Settings.FlySettings.AirGravity;
                                    Settings.FlySettings.IsFly = true;
                                    IsGrounded = false;
                                    break;
                                }
                            }

                            break;
                        }
                    }


                    // ////////////
                    // IS FLYING
                    if (Settings.FlySettings.IsFly)
                    {
                        _groundedCounter = 0f;
                        _verticalSpeed = 0;

                        //Is Airborn
                        _verticalSpeed -= Settings.FlySettings.AirGravity;

                        // Press Space, Hovering up on air
                        if (_jumpInputDown)
                        {
                            _verticalSpeed += Settings.FlySettings.FlyUpSpeedOnSpace;
                            IsGrounded = false;
                        }

                        //Hold Space
                        if (_jumpInput)
                        {
                            _verticalSpeed += Settings.FlySettings.FlyUpSpeedOnSpace / 2f;
                            IsGrounded = false;
                        }



                        // Fly toward
                        float FlyTowardSpeed = 0;
                        if (_NXPInput.MoveInput.y != 0)
                        {
                            //3rd person
                            Vector3 camDir = transform.position - _NXPMain.MainCam.position;

                            //VR, 1st person
                            camDir = _NXPMain.MainCam.forward;
                            Debug.DrawRay(_NXPMain.MainCam.position, camDir, Color.red);

                            // float verticalDir = camDir.normalized.y * Mathf.Abs(camDir.normalized.y);
                            float camDirVertical = camDir.normalized.y;

                            //Slitely look down won't affect
                            if (camDirVertical < 0 && camDirVertical > -0.3f)
                            {
                                camDirVertical = 0;
                            }
                            else if (camDirVertical < -0.3f)
                            {
                                //fly down
                                camDirVertical = (camDirVertical + 0.3f) * 1.42f;
                                FlyTowardSpeed = camDirVertical * Settings.FlySettings.FlyDownSpeedByCamera * _NXPInput.MoveInput.y;
                            }
                            else
                            {
                                //fly up
                                FlyTowardSpeed = camDirVertical * Settings.FlySettings.FlyUpSpeedByCamera * _NXPInput.MoveInput.y;
                            }

                        }

                        _verticalSpeed += FlyTowardSpeed;


                        //Press Run
                        if (_runInput)
                        {
                            _verticalSpeed *= Settings.FlySettings.FlyMultiOnShift;
                        }

                        _verticalSpeed = Mathf.Clamp(_verticalSpeed, -Settings.FlySettings.MaxFallSpeed, Settings.FlySettings.MaxUpSpeed);
                    }
                    break;

                case MovementMode.Hoverable:
                    if (IsGrounded)
                    {
                        _verticalSpeed = -Settings.HoverSettings.GroundedGravity;
                        Settings.HoverSettings.HoverUpCount = 0;

                        if (_jumpInput)
                        {
                            // Debug.Log("Hover "+Settings.HoverSettings.HoverUpCount);
                            _verticalSpeed = Settings.HoverSettings.HoverUpSpeed;
                            IsGrounded = false;
                        }
                    }
                    else
                    {
                        //Is Airborn
                        if (_jumpInputDown) // Hovering up
                        {
                            // Debug.Log("Hover "+Settings.HoverSettings.HoverUpCount);
                            if (Settings.HoverSettings.HoverUpLimit == 0 || Settings.HoverSettings.HoverUpCount < Settings.HoverSettings.HoverUpLimit)
                            {
                                _verticalSpeed = Settings.HoverSettings.HoverUpSpeed;
                                IsGrounded = false;
                                Settings.HoverSettings.HoverUpCount += 1;
                            }
                        }
                        if (_jumpInput)
                        {
                            // Hold Height
                            _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, 0, Settings.HoverSettings.HoverAbortSpeed * Time.deltaTime);
                        }
                        else
                        {
                            // slowly drop
                            _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -Settings.HoverSettings.MaxFallSpeed, Settings.HoverSettings.AirGravity * Time.deltaTime);
                        }

                    }
                    break;
                default:
                    if (IsGrounded)
                    {
                        _verticalSpeed = -Settings.GravitySettings.GroundedGravity;

                        if (_jumpInput)
                        {
                            _verticalSpeed = Settings.MovementSettings.JumpSpeed;
                            IsGrounded = false;
                        }
                    }
                    else
                    {
                        if (!_jumpInput && _verticalSpeed > 0.0f)
                        {
                            // This is what causes holding jump to jump higher than tapping jump.
                            _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -Settings.GravitySettings.MaxFallSpeed, Settings.MovementSettings.JumpAbortSpeed * Time.deltaTime);
                        }

                        _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -Settings.GravitySettings.MaxFallSpeed, Settings.GravitySettings.Gravity * Time.deltaTime);
                    }
                    break;
            }
        }

        private void FixedUpdate_OrientToTargetRotation(Vector3 horizontalMovement)
        {
            if (horizontalMovement.sqrMagnitude > 0.0f)
            {
                float rotationSpeed = Mathf.Lerp(
                    Settings.RotationSettings.MaxRotationSpeed, Settings.RotationSettings.MinRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);

                Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }


        #region Simple Input
        ////////////////////
        // Simple Input
        ////////////////////
        public void MoveTo(Vector3 targetPost, Transform Cam)
        {
            m_targetPost = targetPost;

            float horizontalDir;
            float verticalDir;

            horizontalDir = targetPost.x - transform.position.x;
            verticalDir = targetPost.z - transform.position.z;

            // Debug.Log("MoveTo>> CamAngles: " + Cam.eulerAngles.y);
            // yawRotation = Quaternion.Euler(0f, Cam.eulerAngles.y, 0f);

            _SimpleInputDir = new Vector3(horizontalDir, 0f, verticalDir).normalized;
            //Debug.Log("MoveTo>> _SimpleInputDir.x: " + _SimpleInputDir.x + ", _SimpleInputDir.z: " + _SimpleInputDir.z);

            // transform.LookAt(Vector3.up * transform.position.y);
            
        }

        #endregion


        #region Extra functions
        ////////////////////
        // Public functions
        ////////////////////
        public void TransportTransform(Transform target)
        {
            _characterController.enabled = false;
            transform.position = target.position;
            transform.rotation = target.rotation;
            _characterController.enabled = true;
        }
        public void TransportV3(Vector3 target)
        {
            _characterController.enabled = false;
            transform.position = target;
            transform.rotation = Quaternion.identity;
            _characterController.enabled = true;
        }


        #region FollowTarget
        [Header("Fix Position")]
        private Transform m_followTarget;
        private Vector3 m_followLerpSpeedVector;
        private float m_initialLerpCount;
        private float m_initialLerpTime;

        public void SetFollowTarget(Transform target, float initialLerpTime, Vector3 lerpSpeed)
        {
            m_followTarget = target;
            m_initialLerpCount = initialLerpTime;
            m_initialLerpTime = initialLerpTime;
            m_followLerpSpeedVector = lerpSpeed;
        }

        public void RemoveFollowTarget(Transform target)
        {
            if (m_followTarget == target)
            {
                m_followTarget = null;
            }
        }


        void FixedUpdate_FollowTarget()
        {
            if (m_followTarget)
            {
                Vector3 moveDir = m_followTarget.position - transform.position;
                if (m_initialLerpCount > 0)
                {
                    // quickly follow up to the target
                    m_initialLerpCount -= Time.deltaTime;
                    _characterController.Move(moveDir / m_initialLerpTime * Time.deltaTime);
                }
                else
                {
                    // Adjust the postion by the lerp speed
                    _characterController.Move(Vector3.Scale(moveDir, m_followLerpSpeedVector) * Time.deltaTime);
                }
            }
        }
        #endregion
        #endregion


        #region Cheat

        [Header("Cheat")]
        public bool CheatActivate;
        private bool CheckCheat = false;
        private string[] CheatCode = new string[] { "g", "o", "d", "2", "0", "5", "0" };
        private int Cheatindex = 0;
        public NXPMovementSettings CheatSettings;
        private NXPMovementSettings originalSetting;

        void Update_Cheat()
        {
            if (CheatActivate)
            {
                if(Input.GetKeyDown(KeyCode.Z)){
                    CheatActivate = false;
                    CheckCheat = false;
                    this.Settings = originalSetting;
                    return;
                }

                if (!CheckCheat)
                {
                    // Check if any key is pressed
                    if (Input.anyKeyDown)
                    {
                        // Check if the next key in the code is pressed
                        if (Input.GetKeyDown(CheatCode[Cheatindex]))
                        {
                            // Add 1 to index to check the next key in the code
                            Cheatindex++;
                        }
                        // Wrong key entered, we reset code typing
                        else
                        {
                            Cheatindex = 0;
                        }
                    }

                    // If index reaches the length of the cheatCode string, 
                    // the entire code was correctly entered
                    if (Cheatindex == CheatCode.Length)
                    {
                        originalSetting = this.Settings;
                        // Cheat code successfully inputted!
                        // Unlock crazy cheat code stuff
                        var PlayerMovement = this.gameObject.GetComponent<NXP_Movement>();
                        PlayerMovement.Settings = CheatSettings;

                        Debug.Log("You're chicken!");
                        CheckCheat = true;
                    }
                }
            }
        }

        #endregion
    }

}
