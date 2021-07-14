using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using System.Collections.Generic;
using System.Collections;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using NAXS.NXHelper;


public class Firebase_ObjCtrl : Firebase_CtrlBase
{
    public GameObject TargetObject;
    [FoldoutGroup("Events")]
    public BoolEvent ToggleEvent;

    [FoldoutGroup("Events")]
    public UnityEvent OnTrueEvent;
    [FoldoutGroup("Events")]
    public UnityEvent OnFalseEvent;


    void Reset()
    {
        TargetObject = this.gameObject;
    }

    protected override void  Initialize(){
        TargetDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_OBJCTRL+"/"+DBPath);

        TargetDB.OnGetSuccess += OnGetValue;
        TargetDB.GetValue();

        observer = new FirebaseObserver(TargetDB, ObserverRefreshRate);
        observer.OnChange += OnGetValue;
        observer.Start();
    }


    void OnGetValue(Firebase sender, DataSnapshot snapshot){
        var value = snapshot.Value<bool>();
        ToggleEvent.Invoke(value);
        if(TargetObject)
            TargetObject.SetActive(value);
        if(value)
            OnTrueEvent.Invoke();
        else
            OnFalseEvent.Invoke();
        #if UNITY_EDITOR
        Debug.Log("Firebase_ObjCtrl OnGetValue "+gameObject.name+" "+value.ToString());
        #endif
    }
    
}
