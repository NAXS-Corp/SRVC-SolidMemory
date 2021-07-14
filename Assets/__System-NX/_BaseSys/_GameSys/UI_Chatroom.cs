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
    public class UI_Chatroom : MonoBehaviour
    {
        [System.Serializable]
        public enum Modes { auto, show, hide };
        public Modes chatroomMode;
        private Modes lastMode;
        private Modes lastModeOnHide;

        [Header("UIElement")]
        public GameObject ChatSysObject;
        public CanvasGroup ChatroomPanel;
        public Image ChatroomBG;
        public InputField ChatInput;
        public GameObject ChatInputHolder;
        // public Canvas ChatInputCanvas;
        [Header("Input")]
        public string placeHolderText = "[Enter] to chat, [\\] to toggle chatroom";
        bool ChatUIState = false;

        void Start()
        {
            ModeSetup(chatroomMode);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                ToggleInputState();
            if (Input.GetKeyDown(KeyCode.Backslash)){
                SwitchChatroomMode();
            }
        }

        void SwitchChatroomMode(){
            Modes newMode;
            switch (chatroomMode)
            {
                case Modes.auto:
                    newMode = Modes.show;
                    break;
                case Modes.show:
                    newMode = Modes.hide;
                    break;
                case Modes.hide:
                    // newMode = Modes.auto;
                    newMode = Modes.show;
                    break;
                default:
                    newMode = Modes.auto;
                    break;
            }
            ModeSetup(newMode);
        }

        void ModeSetup(Modes newMode){
            lastMode = chatroomMode;
            chatroomMode = newMode;
            switch (chatroomMode)
            {
                case Modes.auto:
                    ChatroomPanel.gameObject.SetActive(true);
                    ChatroomBG.CrossFadeAlpha(0f, 0.3f, true);
                    ChatroomPanel.DOFade(1f, 0.3f);
                    break;
                case Modes.show:
                    ChatroomPanel.gameObject.SetActive(true);
                    ChatroomBG.CrossFadeAlpha(0.8f, 0.3f, true);
                    ChatroomPanel.DOFade(1f, 0.3f);
                    break;
                case Modes.hide:
                    ChatroomPanel.DOFade(0f, 0.5f).OnComplete(() =>
                    {
                        ChatroomPanel.gameObject.SetActive(false);
                    });
                    break;
            }
        }

        public void SetInputSate(bool state)
        {
            ChatUIState = state;
            if (ChatUIState)
            {
                //show
                ChatInput.gameObject.SetActive(true);
                ChatInput.text = "";
                UI_Methods.FocusInput(ChatInput);
                UI_Methods.ShowCursor();
                ChatInputHolder.SetActive(false);
            }
            else
            {
                //hide
                ChatInput.DeactivateInputField();
                ChatInput.gameObject.SetActive(false);
                ChatInput.text = placeHolderText;
                // if (!isMaterUIShowed)
                UI_Methods.HideCursor();
                ChatInputHolder.SetActive(true);
            }
        }

        void ToggleInputState()
        {
            var newState = !ChatUIState;
            SetInputSate(newState);
        }

        ////////////////////////////
        // API
        public void HideAndDisable(){
            ChatSysObject.SetActive(false);
            this.enabled = false;
            lastModeOnHide = chatroomMode;
            ModeSetup(Modes.hide);
        }

        public void EnableAndShow(){
            ChatSysObject.SetActive(true);
            this.enabled = true;
            ModeSetup(lastModeOnHide);
        }

    }

}
