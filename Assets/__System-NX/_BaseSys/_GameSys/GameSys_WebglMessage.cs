using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameSys_WebglMessage : MonoBehaviour
{
    public static GameSys_WebglMessage singleton;


    public static bool OnGameStart;
    public static bool OnGameStartPlay;
    public static bool OnGameInteract;
    public static bool OnGameReady;
    public static bool OnGameEnd;    

    [ValueDropdown("EventTypes")]
    public string SendEventOnEnable = "OnGameStart";
    private static string[] EventTypes = new string[] { "None", "OnGameStart", "OnGameEnd" };

    void OnEnable()
    {
        if(!gameObject.activeSelf)
            return;
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLPluginJS.PassTextParam(SendEventOnEnable);
#endif
        
    }

    void Start()
    {
        if(!GameSys_WebglMessage.OnGameStart)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLPluginJS.PassTextParam("OnGameStart");
            GameSys_WebglMessage.OnGameStart = true;
#endif
        }
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if(!GameSys_WebglMessage.OnGameInteract)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                WebGLPluginJS.PassTextParam("OnGameInteract");
                GameSys_WebglMessage.OnGameInteract = true;
#endif
            }
        }
    }

    public void SendWebglMessage(string msg)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLPluginJS.PassTextParam(msg);
#endif
    }
}
