using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
    public class NXP_1stPersonCamera : NXP_CameraCtrlBase
    {
        [Header("1st Person View")]
        public float CameraHeight = 1f;
        public float HeightFollowSpeed = 3f;
        
        
        protected override void UpdateChild()
        {
            PositionUpdate();
            RotationUpdate();
        }

        void RotationUpdate()
        {
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            _targetRotation.x += _NXPMain._NXPInput.CameraInput.y * 359f * cursorSensitivity * -1f;
            _targetRotation.y += _NXPMain._NXPInput.CameraInput.x * 359f * cursorSensitivity;
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, MinPitchAngle, MaxPitchAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime* smoothRotSpeed);
        }

        void PositionUpdate()
        {
            float targetPosY = _NXPMain.transform.position.y + CameraHeight;
            targetPosY = Mathf.Lerp(transform.position.y, targetPosY, Time.deltaTime * HeightFollowSpeed);
            transform.position = new Vector3 (_NXPMain.transform.position.x, targetPosY, _NXPMain.transform.position.z);
        }
    }
}