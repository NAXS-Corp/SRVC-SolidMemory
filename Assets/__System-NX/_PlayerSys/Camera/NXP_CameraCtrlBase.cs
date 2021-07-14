using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
    public abstract class NXP_CameraCtrlBase : MonoBehaviour
    {
        protected NXP_MainCtrl _NXPMain;
        protected bool initialized;
        [Header("Basic")]
        public Transform FollowTarget;
        [Header("ROT")]
        public bool PauseRotation = false;
        protected Vector3 _targetRotation;
        public float smoothRotSpeed = 5f;
        public float cursorSensitivity = 0.025f;
        public float MinPitchAngle = -45.0f;
        public float MaxPitchAngle = 75.0f;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        public virtual void Initialize(NXP_MainCtrl _mainCtrl){
            _NXPMain = _mainCtrl;
            initialized = true;
            
            // Sync position & rotation with existing camera, prevent camera clipping
            Transform mainCam = Camera.main.transform;
            if(mainCam){
                transform.position = mainCam.position;
                transform.rotation = mainCam.rotation;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!initialized)    
                return;
            
            UpdateChild();
        }

        protected virtual void UpdateChild() {}
    }
}