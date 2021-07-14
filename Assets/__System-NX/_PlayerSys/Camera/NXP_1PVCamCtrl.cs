using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
    public class NXP_1PVCamCtrl : NXP_CameraCtrlBase
    {
        [Header("1PV Options")]
        public float CameraHeight = 1f;
        public float HeightFollowSpeed = 3f;


        // Update is called once per frame
        protected override void UpdateChild()
        {
            // if(FollowTarget)
            //     transform.position = FollowTarget.position;
            float targetY = _NXPMain.transform.position.y + CameraHeight;
            targetY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * HeightFollowSpeed);
            transform.position = new Vector3(_NXPMain.transform.position.x, targetY, _NXPMain.transform.position.z);


            
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            _targetRotation.x += _NXPMain._NXPInput.CameraInput.y * 359f * cursorSensitivity * -1f;
            _targetRotation.y += _NXPMain._NXPInput.CameraInput.x * 359f * cursorSensitivity;
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, MinPitchAngle, MaxPitchAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime* smoothRotSpeed);
            
        }
    }
}