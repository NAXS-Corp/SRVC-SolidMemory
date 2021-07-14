using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using UnityEngine.Events;

public class Firebase_ObjList : Firebase_CtrlBase
{
    public Dictionary<string, object> ObjStates;
    public UnityEvent OnGetUpdate;

    protected override void  Initialize(){
        TargetDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_OBJCTRL+"/"+DBPath);

        TargetDB.OnGetSuccess += OnGetValue;
        TargetDB.GetValue();

        observer = new FirebaseObserver(TargetDB, ObserverRefreshRate);
        observer.OnChange += OnGetValue;
        observer.Start();
    }

    void OnGetValue(Firebase sender, DataSnapshot snapshot){
        Debug.Log("Firebase_ObjList OnGetValue "+snapshot.RawJson);

        
        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;
        // if (keys != null)
        //     foreach (string key in keys)
        //     {
        //         Debug.Log(key + " = " + dict[key].ToString());
        //     }

        ObjStates = snapshot.Value<Dictionary<string, object>>();
        OnGetUpdate.Invoke();
    }
}
