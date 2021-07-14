using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
	////////////////////
	// MovementSetting
	////////////////////

	[CreateAssetMenu(fileName = "NXPMovement" , menuName = "NAXS/Movement Setting" , order = 1)]
	public class NXPMovementSettings : ScriptableObject 
	{
		public MovementSettings MovementSettings;
		public FlySettings FlySettings;
		public HoverSettings HoverSettings;
		public GravitySettings GravitySettings;
		public RotationSettings RotationSettings;
	}

	[System.Serializable]
	public class MovementSettings
	{
		public float Acceleration = 25.0f; // In meters/second
		public float Decceleration = 25.0f; // In meters/second
		public float RunSpeed = 3f; // Multiplier
		public float MaxHorizontalSpeed = 8.0f; // In meters/second
		public float JumpSpeed = 10.0f; // In meters/second
		public float RunJumpSpeed = 10.0f; // In meters/second
		public float JumpAbortSpeed = 10.0f; // In meters/second
	}

	[System.Serializable]
	public class GravitySettings
	{
		public float Gravity = 20.0f; // Gravity applied when the player is airborne
		public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
		public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
	}

	[System.Serializable]
	public class HoverSettings
	{
		[HideInInspector]
		public int HoverUpCount = 0;
		public int HoverUpLimit = 3;
		public float HoverUpSpeed = 5f;
		public float HoverAbortSpeed = 10f;
		public float AirGravity = 3.0f; // Gravity applied when the player is airborne
		public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
		public float MaxFallSpeed = 5.0f; // The max speed at which the player can fall
	}
	[System.Serializable]
	public class FlySettings
	{
		// [HideInInspector]
		public float FlyUpSpeedOnSpace = 6f;
		public float FlyMultiOnShift = 2f;
		// public float FlyAbortSpeed = 6f;
		public float FlyUpSpeedByCamera = 3f;
		public float FlyDownSpeedByCamera = 4f;
		public float AirGravity = 0.5f; // Gravity applied when the player is airborne
		public float GroundedGravity = 3.0f; // A constant gravity that is applied when the player is grounded
		public float FloatDownHeight = 5f;
		public float MaxFallSpeed = 15f; // The max speed at which the player can fall
		public float MaxUpSpeed = 15f; // The max speed at which the player can fall
		public bool IsFly{get;set;}
		public float HorizontalFlyRunMulti = 2f;
	}

	[System.Serializable]
	public class RotationSettings
	{
		// [Header("Control Rotation")]
		// public float MinPitchAngle = -45.0f;
		// public float MaxPitchAngle = 75.0f;

		// [Header("Character Orientation")]
		// [SerializeField] private bool _useControlRotation = false;
		// [SerializeField] private bool _orientRotationToMovement = true;
		public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
		public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)

		// public bool UseControlRotation { get { return _useControlRotation; } set { SetUseControlRotation(value); } }
		// public bool OrientRotationToMovement { get { return _orientRotationToMovement; } set { SetOrientRotationToMovement(value); } }

		// private void SetUseControlRotation(bool useControlRotation)
		// {
		// 	_useControlRotation = useControlRotation;
		// 	_orientRotationToMovement = !_useControlRotation;
		// }

		// private void SetOrientRotationToMovement(bool orientRotationToMovement)
		// {
		// 	_orientRotationToMovement = orientRotationToMovement;
		// 	_useControlRotation = !_orientRotationToMovement;
		// }
	}
}
