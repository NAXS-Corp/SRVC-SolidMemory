using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using NAXS.NXHelper;
using NAXS.Event;
using Sirenix.OdinInspector;

public class Firebase_GetInt : Firebase_CtrlBase
{
    public IntEvent OnGetInt; 
    protected override void  Initialize(){
        TargetDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_BASE+"/"+DBPath);

        TargetDB.OnGetSuccess += OnGetValue;
        TargetDB.OnGetFailed += OnGetFailed;
        TargetDB.GetValue();

        if(ObserverRefreshRate > 0){
            observer = new FirebaseObserver(TargetDB, ObserverRefreshRate);
            observer.OnChange += OnGetValue;
            observer.Start();
        }
    }

    public void ResetDBPath(string newDBPath){
        DBPath = newDBPath;
        observer.Stop();
        Initialize();
    }

    void OnGetFailed(Firebase sender, FirebaseError err)
    {
        Debug.LogError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void OnGetValue(Firebase sender, DataSnapshot snapshot){
        Debug.Log("Firebase_GetInt OnGetValue "+snapshot.RawJson.ToString());
        int newInt = 0;
        int.TryParse(snapshot.RawJson, out newInt);
        Debug.Log("Firebase_GetInt OnGetValue "+newInt);
        OnGetInt.Invoke(newInt);
    }

    #if UNITY_EDITOR
    [Button]
    void Test(){
        if(observer != null)
            observer.Stop();
        Initialize();
    }
    #endif
}
