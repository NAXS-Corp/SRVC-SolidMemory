using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using NAXS.Event;
using UnityEngine.Events;
using NAXS.NXHelper;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NAXS.UI
{
    public class VRUI_LoginGame : MonoBehaviour
    {
        public InputField IDInput;

        public GameSys_EventQue EventQueue;

        void Start()
        {

        }

        void Update()
        {

        }

        public void SetUsernameAndEnd()
        {
#if UNITY_EDITOR
            Debug.Log("UI_LoginCtrl: SetUsernameAndEnd "+ IDInput.text);
#endif
            NXEvent.SetData("OnUsernameInput", IDInput.text);
            NXEvent.EmitEvent("OnUsernameInput");
            NXEvent.EmitEvent("OnLoginComplete"); // ShowChatroomUI in UIMaster

            NXEvent.SetData("VRRaycast", false);
            NXEvent.EmitEvent("VRRaycast");

            EventQueue.StartEventQue();
        }

    }
}