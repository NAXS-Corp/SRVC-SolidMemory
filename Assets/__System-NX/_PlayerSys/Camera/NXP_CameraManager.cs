using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.NXPlayer{
    public class NXP_CameraManager : MonoBehaviour
    {
	    public enum CamMode {_1pv, _3pv};
        public CamMode m_CamMode;
        [Header("Ref")]
        private NXP_MainCtrl _NXPMain;
        public NXP_CameraCtrlBase _1pvCam;
        public NXP_CameraCtrlBase _3pvCam;

        [Header("Settings")]
        public NXP_CameraCtrlBase DefaultCamCtrl;
        public NXP_CameraCtrlBase CurCamera{
            get; set;
        }



        void Start()
        {
            // Initialize();
        }

        public void Initialize(NXP_MainCtrl mainCtrl){
            Debug.Log("NXP NXP_CameraManager:: Initialize "+mainCtrl);
            _NXPMain = mainCtrl;


            switch (m_CamMode)
            {
                case CamMode._1pv:
                    SwitchCam(_1pvCam);
                    break;
                case CamMode._3pv:
                    SwitchCam(_3pvCam);
                    break;
                default:
                    SwitchCam(DefaultCamCtrl);
                    break;
            }
        }


        [Button]
        void ToogleSwitch(){
            NXP_CameraCtrlBase TCam;
            if(CurCamera == _1pvCam) TCam = _3pvCam;
            else TCam = _1pvCam; 
            SwitchCam(TCam);
        }
        void SwitchCam(NXP_CameraCtrlBase targetCtrl){
                SetActive(_1pvCam, false);
                SetActive(_3pvCam, false);
                SetActive(targetCtrl, true);
                targetCtrl.Initialize(_NXPMain);
                CurCamera = targetCtrl;
        }

        void SetActive(NXP_CameraCtrlBase targetCtrl, bool state){
            if(targetCtrl) 
                targetCtrl.gameObject.SetActive(state);
        }
    }
}