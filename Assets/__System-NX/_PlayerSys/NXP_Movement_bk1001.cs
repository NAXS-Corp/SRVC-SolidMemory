using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
	// public enum MovementMode{
	// 	Default,
	// 	Hoverable,
	// 	Ghost
	// }

	public class NXP_Movement_bk1001 : MonoBehaviour
	{
// 		[Header("Ref")]
// 		private NXP_MainCtrl _NXPMain;
// 		private NXP_PlayerInput _NXPInput;
// 		[SerializeField]private CharacterController _characterController; // The Unity's CharacterController
// 		private NXP_AnimatorCtrl _NXPAnimator;

// 		[Header("Settings")]
// 		public MovementMode MovementMode;
// 		public MovementSettings MovementSettings;
// 		public GravitySettings GravitySettings;
// 		public RotationSettings RotationSettings;
// 		public HoverSettings HoverSettings;
// 		public float ControlRotationSensitivity = 3.0f;

// 		private float _targetHorizontalSpeed; // In meters/second
// 		private float _horizontalSpeed; // In meters/second
// 		private float _verticalSpeed; // In meters/second
// 		private float _maxVerticalSpeed;

// 		private Vector2 _controlRotation; // X (Pitch), Y (Yaw)
// 		private Vector3 _movementInput;
// 		private Vector3 _lastMovementInput;
// 		private bool _hasMovementInput;
// 		private bool _jumpInput;
// 		private bool _jumpInputDown;
// 		private bool _runInput;

// 		public Vector3 Velocity => _characterController.velocity;
// 		public Vector3 HorizontalVelocity => _characterController.velocity.SetY(0.0f);
// 		public Vector3 VerticalVelocity => _characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);
// 		public float MovingRate => Mathf.Clamp((_horizontalSpeed / MovementSettings.MaxHorizontalSpeed + _verticalSpeed / _maxVerticalSpeed), 0f, 1f);
// 		public bool IsGrounded { get; private set; }
// 		public bool IsMoveing => (_hasMovementInput) ? true : false;
// 		public bool IsIdle => !_hasMovementInput || _verticalSpeed > 0;

// // NOTE: Move & Rotation
// // Update: GetMovementInput > SetMovementInput
// // FixedUpdate: UpdateHorizontalSpeed > speed calculation > _characterController.Move()

// //
// 		public void Initialize(NXP_MainCtrl mainCtrl)
// 		{
// 			// Debug.Log("NXP Movement Init"+mainCtrl);
// 			_NXPMain = mainCtrl;
// 			_NXPInput = _NXPMain._NXPInput;
// 			if(_NXPMain._NXPAnimator)
// 				_NXPAnimator = _NXPMain._NXPAnimator;
// 			if(_NXPMain.IsLocalPlayer){

// 			}else{
// 				Destroy(_characterController);
// 			}
// 			InitializeMovementMode(MovementMode);
// 		}



// 		// Main Update
// 		private void Update()
// 		{
// 			// UpdateControlRotation();
// 			SetMovementInput(GetMovementInput());
// 			SetJumpInput(_NXPInput.JumpInput, _NXPInput.JumpInputDown);
// 			SetRunInput(_NXPInput.RunInput);
// 		}

// 		private void FixedUpdate()
// 		{
// 			UpdateHorizontalSpeed();
// 			UpdateVerticalSpeed();

// 			Vector3 movement = _horizontalSpeed * GetMovementDirection() + _verticalSpeed * Vector3.up;
// 			_characterController.Move(movement * Time.deltaTime);

// 			OrientToTargetRotation(movement.SetY(0.0f));

// 			IsGrounded = _characterController.isGrounded;

// 			if(_NXPAnimator)
// 				_NXPAnimator.UpdateState();
// 			_NXPInput.UpdateInput();
// 		}


// 		public void SetMovementInput(Vector3 movementInput)
// 		{
// 			bool hasMovementInput = movementInput.sqrMagnitude > 0.0f;

// 			if (_hasMovementInput && !hasMovementInput)
// 			{
// 				_lastMovementInput = _movementInput;
// 			}

// 			_movementInput = movementInput;
// 			_hasMovementInput = hasMovementInput;
// 		}

// 		public void SetJumpInput(bool jumpInput)
// 		{
// 			_jumpInput = jumpInput;
// 		}
		
// 		public void SetJumpInput(bool jumpInput, bool jumpInputDown)
// 		{
// 			_jumpInput = jumpInput;
// 			_jumpInputDown = jumpInputDown;
// 		}

// 		public void SetRunInput(bool runInput){
// 			_runInput = runInput;
// 		}

// 		// public Vector2 GetControlRotation()
// 		// {
// 		// 	return _controlRotation;
// 		// }

// 		// public void SetControlRotation(Vector2 controlRotation)
// 		// {
// 		// 	// Adjust the pitch angle (X Rotation)
// 		// 	float pitchAngle = controlRotation.x;
// 		// 	pitchAngle %= 360.0f;
// 		// 	pitchAngle = Mathf.Clamp(pitchAngle, RotationSettings.MinPitchAngle, RotationSettings.MaxPitchAngle);

// 		// 	// Adjust the yaw angle (Y Rotation)
// 		// 	float yawAngle = controlRotation.y;
// 		// 	yawAngle %= 360.0f;

// 		// 	_controlRotation = new Vector2(pitchAngle, yawAngle);
// 		// }

// 		private void UpdateHorizontalSpeed()
// 		{
// 			Vector3 movementInput = _movementInput;
// 			if (movementInput.sqrMagnitude > 1.0f)
// 			{
// 				movementInput.Normalize();
// 			}

// 			_targetHorizontalSpeed = movementInput.magnitude * MovementSettings.MaxHorizontalSpeed;
// 			if(_runInput){
// 				_targetHorizontalSpeed *= MovementSettings.RunSpeed;
// 			}
// 			float acceleration = _hasMovementInput ? MovementSettings.Acceleration : MovementSettings.Decceleration;


// 			_horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
			
// 		}

// 		public void InitializeMovementMode(MovementMode mode)
// 		{
// 			MovementMode = mode;
			
// 			switch (MovementMode)
// 			{
// 				case MovementMode.Hoverable:
// 					_maxVerticalSpeed = HoverSettings.HoverUpSpeed;
// 					break;
// 				default:
// 					_maxVerticalSpeed = MovementSettings.JumpSpeed;
// 					break;
// 			}
// 		}

// 		private void UpdateVerticalSpeed()
// 		{
// 			switch (MovementMode)
// 			{
// 				case MovementMode.Ghost:
					
// 				case MovementMode.Hoverable:
// 					if (IsGrounded)
// 					{
// 						_verticalSpeed = -HoverSettings.GroundedGravity;
// 						HoverSettings.HoverUpCount = 0;

// 						if (_jumpInput)
// 						{
// 							// Debug.Log("Hover "+HoverSettings.HoverUpCount);
// 							_verticalSpeed = HoverSettings.HoverUpSpeed;
// 							IsGrounded = false;
// 						}
// 					}
// 					else
// 					{
// 						//Is Airborn
// 						if(_jumpInputDown) // Hovering up
// 						{
// 							// Debug.Log("Hover "+HoverSettings.HoverUpCount);
// 							if(HoverSettings.HoverUpLimit == 0 || HoverSettings.HoverUpCount < HoverSettings.HoverUpLimit){
// 								_verticalSpeed = HoverSettings.HoverUpSpeed;
// 								IsGrounded = false;
// 								HoverSettings.HoverUpCount += 1;
// 							}
// 						}
// 						if (_jumpInput) 
// 						{
// 							// Hold Height
// 							_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, 0, HoverSettings.HoverAbortSpeed * Time.deltaTime);
// 						}else{
// 							// slowly drop
// 							_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -HoverSettings.MaxFallSpeed, HoverSettings.Gravity * Time.deltaTime);
// 						}

// 					}
// 					break;
// 				default:
// 					if (IsGrounded)
// 					{
// 						_verticalSpeed = -GravitySettings.GroundedGravity;

// 						if (_jumpInput)
// 						{
// 							_verticalSpeed = MovementSettings.JumpSpeed;
// 							IsGrounded = false;
// 						}
// 					}
// 					else
// 					{
// 						if (!_jumpInput && _verticalSpeed > 0.0f)
// 						{
// 							// This is what causes holding jump to jump higher than tapping jump.
// 							_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, MovementSettings.JumpAbortSpeed * Time.deltaTime);
// 						}

// 						_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, GravitySettings.Gravity * Time.deltaTime);
// 					}
// 					break;
// 			}
// 		}

// 		private Vector3 GetMovementDirection()
// 		{
// 			// Vector3 moveDir = _hasMovementInput ? _movementInput : _lastMovementInput;
// 			Vector3 moveDir;
// 			if(_hasMovementInput){
// 				Vector3 camDir = _NXPMain.MainCam.position - transform.position;
// 				// Quaternion camRot = Quaternion.LookRotation(camDir, Vector3.up);
// 				// Quaternion inputRot = Quaternion.EulerRotation
// 				// moveDir = _movementInput * Quaternion.Euler(0, 90, 0);
// 				// moveDir = _movementInput.normalized + camDir.normalized;
// 				moveDir = _movementInput;
// 			}else{
// 				moveDir = _lastMovementInput;
// 			}

// 			if (moveDir.sqrMagnitude > 1f)
// 			{
// 				moveDir.Normalize();
// 			}

// 			return moveDir;
// 		}

// 		private void OrientToTargetRotation(Vector3 horizontalMovement)
// 		{
// 			// float rotationSpeed = Mathf.Lerp(RotationSettings.MaxRotationSpeed, RotationSettings.MinRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);
// 			// Quaternion targetRotation = Quaternion.Euler(0, _NXPMain.MainCam.rotation.eulerAngles.y, 0);
// 			// transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
// 			// Debug.Log("OrientToTargetRotation "+ _NXPMain.MainCam.rotation.eulerAngles.y+" / "+rotationSpeed+" / "+ _horizontalSpeed + " / "+_targetHorizontalSpeed);

// 			if (horizontalMovement.sqrMagnitude > 0.0f)
// 			{
// 				float rotationSpeed = Mathf.Lerp(
// 					RotationSettings.MaxRotationSpeed, RotationSettings.MinRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);

// 				Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
// 				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
// 			}
// 			else 
// 			{
// 				Quaternion targetRotation = Quaternion.Euler(0.0f, _controlRotation.y, 0.0f);
// 				transform.rotation = targetRotation;
// 			}

// 		}



		
// 		public void OnCharacterFixedUpdate()
// 		{
// 			// _playerCamera.SetPosition(Character.transform.position);
// 			// _playerCamera.SetControlRotation(Character.GetControlRotation());
// 		}

// 		// private void UpdateControlRotation()
// 		// {
// 		// 	Vector2 camInput = _NXPInput.CameraInput;
// 		// 	Vector2 controlRotation = GetControlRotation();

// 		// 	// Adjust the pitch angle (X Rotation)
// 		// 	float pitchAngle = controlRotation.x;
// 		// 	pitchAngle -= camInput.y * ControlRotationSensitivity;

// 		// 	// Adjust the yaw angle (Y Rotation)
// 		// 	float yawAngle = controlRotation.y;
// 		// 	yawAngle += camInput.x * ControlRotationSensitivity;

// 		// 	controlRotation = new Vector2(pitchAngle, yawAngle);
// 		// 	SetControlRotation(controlRotation);
// 		// }

// 		private Vector3 GetMovementInput()
// 		{
// 			// Calculate the move direction relative to the character's yaw rotation
// 			Quaternion yawRotation = Quaternion.Euler(0.0f, _NXPMain.MainCam.rotation.eulerAngles.y, 0.0f);
// 			Vector3 forward = yawRotation * Vector3.forward;
// 			Vector3 right = yawRotation * Vector3.right;
// 			Vector3 movementInput = (forward * _NXPInput.MoveInput.y + right * _NXPInput.MoveInput.x);

// 			if (movementInput.sqrMagnitude > 1f)
// 			{
// 				movementInput.Normalize();
// 			}

// 			return movementInput;
// 		}

// 		////////////////////
// 		// Public functions
// 		////////////////////
// 		public void Transport(Transform target)
// 		{
// 			_characterController.enabled = false;
// 			transform.position = target.position;
// 			transform.rotation = target.rotation;
// 			_characterController.enabled = true;
// 		}
	}


	// [System.Serializable]
	// public class MovementSettings
	// {
	// 	public float Acceleration = 25.0f; // In meters/second
	// 	public float Decceleration = 25.0f; // In meters/second
	// 	public float RunSpeed = 3f; // Multiplier
	// 	public float MaxHorizontalSpeed = 8.0f; // In meters/second
	// 	public float JumpSpeed = 10.0f; // In meters/second
	// 	public float JumpAbortSpeed = 10.0f; // In meters/second
	// }

	// [System.Serializable]
	// public class GravitySettings
	// {
	// 	public float Gravity = 20.0f; // Gravity applied when the player is airborne
	// 	public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
	// 	public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
	// }

	// [System.Serializable]
	// public class HoverSettings
	// {
	// 	[HideInInspector]
	// 	public int HoverUpCount = 0;
	// 	public int HoverUpLimit = 3;
	// 	public float HoverUpSpeed = 5f;
	// 	public float HoverAbortSpeed = 10f;
	// 	public float Gravity = 3.0f; // Gravity applied when the player is airborne
	// 	public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
	// 	public float MaxFallSpeed = 5.0f; // The max speed at which the player can fall
	// }

	// [System.Serializable]
	// public class RotationSettings
	// {
	// 	// [Header("Control Rotation")]
	// 	// public float MinPitchAngle = -45.0f;
	// 	// public float MaxPitchAngle = 75.0f;

	// 	// [Header("Character Orientation")]
	// 	// [SerializeField] private bool _useControlRotation = false;
	// 	// [SerializeField] private bool _orientRotationToMovement = true;
	// 	public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
	// 	public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)

	// 	// public bool UseControlRotation { get { return _useControlRotation; } set { SetUseControlRotation(value); } }
	// 	// public bool OrientRotationToMovement { get { return _orientRotationToMovement; } set { SetOrientRotationToMovement(value); } }

	// 	// private void SetUseControlRotation(bool useControlRotation)
	// 	// {
	// 	// 	_useControlRotation = useControlRotation;
	// 	// 	_orientRotationToMovement = !_useControlRotation;
	// 	// }

	// 	// private void SetOrientRotationToMovement(bool orientRotationToMovement)
	// 	// {
	// 	// 	_orientRotationToMovement = orientRotationToMovement;
	// 	// 	_useControlRotation = !_orientRotationToMovement;
	// 	// }
	// }
}
