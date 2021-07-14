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
    public class UI_LoginCtrl : MonoBehaviour
    {
        public bool isVR;
        private bool LoginUIState;

        [Header("Login UI")]
        public CanvasGroup LoginUI;
        public LoginUIList[] LoginUISettings;
        private int CheckLoginUI = 0;
        [Serializable]
        public struct LoginUIList
        {
            public string Id;
            public Image PlayLog;
        }
        // Start is called before the first frame update
        void Start()
        {
            ShowLoginUI();
            // CheckInputField();
        }

        // Update is called once per frame
        void Update()
        {
            HotKeyUpdate();
        }


        void HotKeyUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (LoginUIState)
                {
                    LoginUINext();
                }
            }
        }

        // Login UI
        public void ShowLoginUI()
        {
            UI_Methods.ShowCursor();
            LoginUIState = true;
            LoginUI.DOFade(1, 0.5f);
            LoginUI.gameObject.SetActive(true);

            // VR
            if (isVR)
            {
                NXEvent.SetData("VRRaycast", true);
                NXEvent.EmitEvent("VRRaycast");
            }

            CheckInputField();
        }
        public InputField IDInput;
        public void LoginUINext()
        {
            if (LoginUI.isActiveAndEnabled == true)
            {
                string ToStringCheckLoginUI;
                ToStringCheckLoginUI = CheckLoginUI.ToString();
                foreach (var LoginList in LoginUISettings)
                {
                    if (ToStringCheckLoginUI == LoginList.Id)
                    {
                        LoginList.PlayLog.DOFade(0, 0.3f).OnComplete(() =>
                        {
                            LoginList.PlayLog.gameObject.SetActive(false);
                        });
                    }
                }

                CheckLoginUI++;
                // Debug.Log(CheckLoginUI);
                ToStringCheckLoginUI = CheckLoginUI.ToString();
                Debug.Log(ToStringCheckLoginUI);
                foreach (var LoginList in LoginUISettings)
                {
                    if (ToStringCheckLoginUI == LoginList.Id)
                    {
                        Debug.Log("Changed.");
                        LoginList.PlayLog.DOFade(0, 0.3f).OnComplete(() =>
                        {
                            LoginList.PlayLog.DOFade(1, 0.3f);
                            LoginList.PlayLog.gameObject.SetActive(true);

                            // After set gameobject active, check FocusInput: IDInput & setusernameandend + eventque 
                            CheckInputField();
                        });
                    }
                }

                

            }
            else
            {
                return;
            }
        }

        // public GameSys_EventQue EventQueue;
        public UnityEvent OnLoginDoneEvent;

        public void CheckInputField()
        {
            Debug.Log("UI_LoginCtrl>> LoginUISettings.Length: " + LoginUISettings.Length);
            if (CheckLoginUI == LoginUISettings.Length - 1)
            {
                if (isVR)
                {
                    return;
                }
                else
                {
                    UI_Methods.FocusInput(IDInput);           
                }
            }
            else if (CheckLoginUI > LoginUISettings.Length - 1)
            {
                SetUsernameAndEnd();
                OnLoginDoneEvent.Invoke();
                // EventQueue.StartEventQue();
            }
        }

        public void SetUsernameAndEnd()
        {
#if UNITY_EDITOR
            Debug.Log("UI_LoginCtrl: SetUsernameAndEnd "+ IDInput.text);
#endif
            NXEvent.SetData("OnUsernameInput", IDInput.text);
            NXEvent.EmitEvent("OnUsernameInput");
            NXEvent.EmitEvent("OnLoginComplete"); // ShowChatroomUI in UIMaster
            // UI_Methods.HideCursor();
            // if(UI_MasterManager.instance)
            // {
            //     Debug.Log("UI_LoginCtrl Master instance show chatroom");
            //     UI_MasterManager.instance.ShowChatroomUI();
            // }
            // else{
            //     Debug.LogError("UI_LoginCtrl Master instance not found");
            // }
            HideLoginUI();
        }

        public void HideLoginUI()
        {
#if UNITY_EDITOR
            Debug.Log("UI_LoginCtrl HideLoginUI");
#endif
            LoginUIState = false;
            
            // VR
            if (isVR)
            {
                NXEvent.SetData("VRRaycast", false);
                NXEvent.EmitEvent("VRRaycast");
            }
            
            LoginUI.DOFade(0, 0.5f).OnComplete(() =>
            {
                // Destroy(this.gameObject);
                // LoginUI.gameObject.SetActive(false);
            });
        }

    }
}