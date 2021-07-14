using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

public class GameSys_WebglCursor : MonoBehaviour
{
    public bool CaptureAllKeyboardInput = false;
    public bool HideCursorOnStart = true;
    public bool FreeCursorOnEsc;
    public bool LockCursorOnClick;
    bool cursorLocked;

    void Start()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        // disable WebGLInput.captureAllKeyboardInput so elements in web page can handle keabord inputs
        WebGLInput.captureAllKeyboardInput = CaptureAllKeyboardInput;
        #endif
        
        if(HideCursorOnStart)
            LockAndHideCursor();
    }

    void Update()
    {
        if(LockCursorOnClick)
        {
            if(!cursorLocked && Input.GetMouseButtonDown(0))
                LockAndHideCursor();
        }

        if(FreeCursorOnEsc){
            if(Input.GetKeyDown(KeyCode.Escape)){
                FreeCursor();
            }
        }
    }

    public void FreeCursor( ){
        Debug.Log("GameSys_WebglCursor:: FreeCursor");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cursorLocked = false;
    }

    public void LockAndHideCursor()
    {
        Debug.Log("GameSys_WebglCursor:: LockAndHideCursor");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cursorLocked = true;
    }
}
