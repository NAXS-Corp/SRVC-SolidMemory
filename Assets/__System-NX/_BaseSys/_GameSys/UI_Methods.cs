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
    // Static Methods for UI
    public class UI_Methods : MonoBehaviour
    {

        public static void ShowCursor()
        {
            // Debug.Log("GameSys_WebglCursor:: FreeCursor");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // Debug.Log("UI_Methods ShowCursor");
            // cursorLocked = false;
        }

        public static void HideCursor()
        {
            // Debug.Log("UI_Methods HideCursor");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // cursorLocked = true;
        }

        public static void FocusInput(InputField inputField)
        {
            inputField.ActivateInputField();
            inputField.interactable = true;
            inputField.Select();
            EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
            // inputField.OnPointerClick (null);
        }
    }
}