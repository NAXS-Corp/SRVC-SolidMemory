using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using System.Collections.Generic;
using System.Collections;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using NAXS.NXHelper;
using SimpleJSON;

public class StreamObj{
    public String Url;
    public bool State;
    public StreamObj(String _Url, bool _State){
        Url = _Url;
        State = _State;
    }
}

public class Firebase_StreamCtrl : Firebase_CtrlBase
{
    public bool ForceStartOnBuild = true;
    private string IdleVideoPath = "IdleVideo";
    public string BackupVideoUrl;

    [Header("CallBacks")]
    public StringEvent OnReceiveUrl;

    void Reset()
    {
        if(string.IsNullOrEmpty(DBPath))
            DBPath = gameObject.name;
    }

    protected override void AwakeChild()
    {
        #if !UNITY_EDITOR
        if(ForceStartOnBuild)
            if(this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
        #endif
    }


    protected override void Initialize()
    {
        if(!Firebase_Manager.instance) return;

        TargetDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_VIDEOSTREAM+"/"+DBPath);
        Debug.Log("TargetDB:: "+Firebase_Manager.PATH_VIDEOSTREAM+"/"+DBPath);

        TargetDB.OnGetSuccess += OnGetValue;
        TargetDB.OnGetFailed += OnGetFailed;
        TargetDB.GetValue();

        observer = new FirebaseObserver(TargetDB, ObserverRefreshRate);
        observer.OnChange += OnGetValue;
        observer.Start();
    }

    
    StreamObj DecodeStreamObj(string rawJson){
        JSONNode parsedData = JSON.Parse(rawJson);
        return new StreamObj(parsedData["Url"], parsedData["State"]);
    }


    void OnGetValue(Firebase sender, DataSnapshot snapshot){
            StreamObj stream = DecodeStreamObj(snapshot.RawJson);

            if (stream.State && Uri.IsWellFormedUriString(stream.Url, UriKind.Absolute))
            {
                OnReceiveUrl.Invoke(stream.Url);
            }
            else
            {
                TryGetIdleVideo();
            }
    }

    void OnGetFailed(Firebase sender, FirebaseError error){
            SendBackupUrl();
    }

    void TryGetIdleVideo(){
        Firebase idleTargetDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_VIDEOSTREAM+"/"+IdleVideoPath);
        idleTargetDB.OnGetSuccess += (Firebase sender, DataSnapshot snapshot) => {
            string gotUrl = snapshot.Value<string>();
            if (Uri.IsWellFormedUriString(gotUrl, UriKind.Absolute)){
                OnReceiveUrl.Invoke(gotUrl);
            }
        };

        idleTargetDB.OnGetFailed += (Firebase sender, FirebaseError error) => {
            SendBackupUrl();
        };
        idleTargetDB.GetValue();
    }

    void SendBackupUrl(){
        OnReceiveUrl.Invoke(BackupVideoUrl);
    }
}
