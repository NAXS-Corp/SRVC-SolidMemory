using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
    public class NXP_SetFixedTarget : MonoBehaviour
    {
        public bool AutoStart = true;
        public Transform FixedTarget;
        public float InitialLerpTime = 3f;
        // public float LerpSpeed = 1f;
        public Vector3 LerpSpeedVector = Vector3.one;

        void Reset()
        {
            FixedTarget = this.transform;
        }

        void OnEnable()
        {
            if(AutoStart){
                SetTarget();
            }
        }

        public void SetTarget(){
            if(NXP_MainCtrl.LOCAL._NXPMovement){
                NXP_MainCtrl.LOCAL._NXPMovement.SetFollowTarget(FixedTarget, InitialLerpTime, LerpSpeedVector);
            }
        }

        public void RemoveTarget(){
            if(NXP_MainCtrl.LOCAL._NXPMovement){
                NXP_MainCtrl.LOCAL._NXPMovement.RemoveFollowTarget(FixedTarget);
            }
        }
        void OnDisable()
        {
            if(AutoStart){
                RemoveTarget();
            }
        }
    }
}