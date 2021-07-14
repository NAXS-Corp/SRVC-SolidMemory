using UnityEngine;

using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

using System.Collections.Generic;
using System.Collections;
using System;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class Firebase_Manager : MonoBehaviour
{
    string DATABASE_URL = "https://naxs-pf-dev.firebaseio.com";
    string API_KEY = "AIzaSyBkuUd-8PozIYgVoZOWs-1x-O7nQFCooww";
    public string pathBase = "UnityConfig/AF-EV20";
    public static Firebase_Manager instance;
    public static string PATH_BASE;
    public static string PATH_OBJCTRL;
    public static string PATH_SERVERLIST;
    public static string PATH_SERVERREADYLIST;
    public static string PATH_VIDEOSTREAM;
    public static string PATH_NOTIFICATION;
    public string pathObjCtrl = "ObjCtrl";
    public string pathServerList = "ServerList";
    public string pathServerReadyList = "ServerReadyList";
    public string pathVideoStream = "StreamList";
    public string pathNotification = "Notification";
    [HideInInspector]public Firebase firebase;
    public static Firebase RTDB;
    bool isInitialized;


    void Awake()
    {
        Debug.Log("======== Firebase_Manager Awake");
        if(!Firebase_Manager.instance)
        {
            Firebase_Manager.instance = this;
            Initialize();
        }else{
            Destroy(this);
        }
    }

    public void Initialize()
    {
        if(isInitialized) return;
        Debug.Log("======== Firebase_Manager Initialize");
        firebase =  Firebase.CreateNew(DATABASE_URL, API_KEY);
        RTDB = firebase;
        SetPath();
        isInitialized = true;
    }


    public void SetPath(){
        Debug.Log("======== Firebase_Manager SetPath");
        Firebase_Manager.PATH_BASE = pathBase;
        Firebase_Manager.PATH_OBJCTRL = string.Concat(pathBase, "/" , pathObjCtrl);
        Firebase_Manager.PATH_SERVERLIST = string.Concat(pathBase, "/" , pathServerList);
        Firebase_Manager.PATH_VIDEOSTREAM = string.Concat(pathBase, "/" , pathVideoStream);
        Firebase_Manager.PATH_NOTIFICATION = string.Concat(pathBase, "/" , pathNotification);
        Firebase_Manager.PATH_SERVERREADYLIST = string.Concat(pathBase, "/" , pathServerReadyList);
    }

    

    //////////////////////
    // Firebase Callbacks
    //////////////////////
    
    void GetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Get from key: <" + sender.FullKey + ">");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);

        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (keys != null)
            foreach (string key in keys)
            {
                DebugLog(key + " = " + dict[key].ToString());
            }
    }

    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void UpdateOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Update from key: <" + sender.FullKey + ">");
    }

    void UpdateFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Update from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void DelOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Del from key: <" + sender.FullKey + ">");
    }

    void DelFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Del from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void PushOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Push from key: <" + sender.FullKey + ">");
    }

    void PushFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Push from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void GetRulesOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] GetRules");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);
    }

    void GetRulesFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] GetRules,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void GetTimeStamp(Firebase sender, DataSnapshot snapshot)
    {
        long timeStamp = snapshot.Value<long>();
        DateTime dateTime = Firebase.TimeStampToDateTime(timeStamp);

        DebugLog("[OK] Get on timestamp key: <" + sender.FullKey + ">");
        DebugLog("Date: " + timeStamp + " --> " + dateTime.ToString());
    }

    void DebugLog(string str)
    {
        // #if UNITY_EDITOR
        Debug.Log(str);
        // #endif
    }

    void DebugWarning(string str)
    {
        Debug.LogWarning(str);
    }

    void DebugError(string str)
    {
        Debug.LogError(str);
    }
}
