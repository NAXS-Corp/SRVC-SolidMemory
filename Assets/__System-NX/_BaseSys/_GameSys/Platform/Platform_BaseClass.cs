using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.XR;

namespace NAXS.GameSys
{
    public abstract class Platform_BaseClass : MonoBehaviour
    {
        public enum Platforms { Default, PCVR, WebGL };
        [TabGroup("PCVR")]
        public bool OverridePCVR;

        [TabGroup("WebGL")]
        public bool OverrideWebGL;
        private int vrCheckCount = 0;


        void Start()
        {
            XRDevice.deviceLoaded += OnXRDeviceLoaded;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (XRSettings.isDeviceActive && OverridePCVR)
            {
                // Debug.Log("Platform_BaseClass: Is PCVR");
                Setup(Platforms.PCVR);
                return;
            }
#endif

#if UNITY_WEBGL
            if(OverrideWebGL){
                // Debug.Log("Platform_BaseClass: Is WebGL");
                Setup(Platforms.WebGL);
                return;
            }
#endif

            // Debug.Log("Platform_BaseClass: Is Default");
            InvokeRepeating("VRCheck", 0f, 2f); //Check if VR is activated every 2 seconds
        }

        private void VRCheck()
        {
            if (XRSettings.isDeviceActive)
            {
                // Debug.Log("Platform_BaseClass: isDeviceActive");
                Setup(Platforms.PCVR);
                CancelInvoke("VRCheck");
                return;
            }

            if(vrCheckCount > 5){
                CancelInvoke("VRCheck");
            }
            vrCheckCount += 1;
        }

        void OnXRDeviceLoaded(string newLoadedDeviceName)
        {
            // Debug.Log("Platform_BaseClass: OnXRDeviceLoaded " + newLoadedDeviceName);
        }

        protected virtual void Setup(Platforms platform)
        {

        }
    }
}