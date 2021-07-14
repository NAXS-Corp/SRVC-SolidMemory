using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace NAXS.NXPlayer
{
    public class NXP_3PVCamCtrl : NXP_CameraCtrlBase
    {
		public CinemachineVirtualCameraBase VCam;
        protected override void UpdateChild()
        {
        }

        void Start()
        {
            if(!VCam)
                VCam = GetComponent<CinemachineVirtualCameraBase>();
            if(VCam is CinemachineStateDrivenCamera)
            {
                CinemachineStateDrivenCamera sdc = VCam as CinemachineStateDrivenCamera;
				sdc.m_LookAt = FollowTarget;
				sdc.m_Follow = FollowTarget;
				sdc.m_AnimatedTarget = _NXPMain._3rdAnimator;
            }else{
                VCam.Follow = FollowTarget;
                VCam.LookAt = FollowTarget;
            }
        }

//         void OnEnable()
//         {
            
//             // CinemachineCore.GetInputAxis += LookInputOverride;
//         }
        
        
// 		void OnDisable()
// 		{
//             // CinemachineCore.GetInputAxis -= LookInputOverride;
// 		}
// 		public bool UseTouchControls()
// 		{
// 			//Assume Touch Controls are wanted for iOS and Android builds
// #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
// 			return true;
// #else
// 			return false;
// #endif
// 		}


// 		// Handles the Cinemachine delegate
// 		float LookInputOverride(string axis)
// 		{
			
// 			if (axis == "Vertical")
// 			{	
// 				var lookVertical = _NXPMain._NXPInput.CameraInput.y * -1;
// 				if (UseTouchControls())
// 				{
// 					// lookVertical *= m_CameraLookSensitivity.touchVerticalSensitivity;
// 				}
// 				else
// 				{
// 					lookVertical *= cursorSensitivity;
// 				}
// 				return lookVertical;
// 			}

// 			if (axis == "Horizontal")
// 			{
// 				var lookHorizontal = _NXPMain._NXPInput.CameraInput.x;
// 				if (UseTouchControls())
// 				{
// 					// lookHorizontal *= m_CameraLookSensitivity.touchHorizontalSensitivity;
// 				}
// 				else
// 				{
// 					lookHorizontal *= cursorSensitivity;
// 				}
// 				return lookHorizontal;
// 			}
// 			return 0;
// 		}
		
    }
}