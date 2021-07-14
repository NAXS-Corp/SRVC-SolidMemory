using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
	public static class CharacterAnimatorParamId
	{
		public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
		public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
		public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
		public static readonly int IsMoving = Animator.StringToHash("IsMoving");
		public static readonly int IsRunning = Animator.StringToHash("IsRunning");
		//public static List<int> IsCustomAnim;
		public static readonly int IsCustomAnim0 = Animator.StringToHash("CustomAnim0");
		public static readonly int IsCustomAnim1 = Animator.StringToHash("CustomAnim1");
		public static readonly int IsCustomAnim2 = Animator.StringToHash("CustomAnim2");
		public static readonly int IsCustomAnim3 = Animator.StringToHash("CustomAnim3");
		public static readonly int IsCustomAnim4 = Animator.StringToHash("CustomAnim4");
	}

	// [CreateAssetMenu(fileName = "PlayerController", menuName = "NaughtyCharacter/PlayerController")]
	public class NXP_AnimatorCtrl : MonoBehaviour
	{
		public bool SetBoolState;
		public bool SetFloatState = true;
		public Animator _animator;
		public NXP_Movement _NXPMovement;
		public NXP_PlayerInput _NXPPlayerInput;

		void Start()
		{

		}

		void SetCustomAnim()
		{
			// if (CharacterAnimatorParamId.IsCustomAnim == null && _NXPPlayerInput.CustomAnimInput != null)
			// {
			// 	for (int i=0; i<_NXPPlayerInput.CustomAnimInput.Count; i++)
			// 	{
			// 		CharacterAnimatorParamId.IsCustomAnim.Add(Animator.StringToHash("CustomAnim"+i));
			// 	}
			// }

			_animator.SetBool(CharacterAnimatorParamId.IsCustomAnim0, _NXPPlayerInput.CustomAnim0);
			_animator.SetBool(CharacterAnimatorParamId.IsCustomAnim1, _NXPPlayerInput.CustomAnim1);
			_animator.SetBool(CharacterAnimatorParamId.IsCustomAnim2, _NXPPlayerInput.CustomAnim2);
			_animator.SetBool(CharacterAnimatorParamId.IsCustomAnim3, _NXPPlayerInput.CustomAnim3);
			_animator.SetBool(CharacterAnimatorParamId.IsCustomAnim4, _NXPPlayerInput.CustomAnim4);
		}

		public void UpdateState()
		{
			
			float normHorizontalSpeed = _NXPMovement.HorizontalVelocity.magnitude / _NXPMovement.Settings.MovementSettings.MaxHorizontalSpeed;
			float jumpSpeed = _NXPMovement.Settings.MovementSettings.JumpSpeed;
			float normVerticalSpeed = _NXPMovement.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);

			_animator.SetBool(CharacterAnimatorParamId.IsGrounded, _NXPMovement.IsGrounded);

			if(SetBoolState){
				bool isMoving = false, isRunning = false;
				if(normHorizontalSpeed > 0f) isMoving = true;
				if(normHorizontalSpeed > 1.5f) isRunning = true;

				_animator.SetBool(CharacterAnimatorParamId.IsMoving, isMoving);
				_animator.SetBool(CharacterAnimatorParamId.IsRunning, isRunning);

				SetCustomAnim();

				// for (int i=0; i<_NXPPlayerInput.CustomAnimInput.Count; i++)
				// {
				// 	_animator.SetBool(CharacterAnimatorParamId.IsCustomAnim[i], _NXPPlayerInput.CustomAnimInput[i]);
				// }
				return;
			}
			if(SetFloatState){
				_animator.SetFloat(CharacterAnimatorParamId.HorizontalSpeed, normHorizontalSpeed);
				_animator.SetFloat(CharacterAnimatorParamId.VerticalSpeed, normVerticalSpeed);
			}

		}
	}
}