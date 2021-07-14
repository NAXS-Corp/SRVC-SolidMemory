using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using NAXS.NXHelper;
using NAXS.Event;
using Sirenix.OdinInspector;

public class Firebase_Notification : Firebase_CtrlBase
{
    public StringEvent OnGetNotification;
    protected override void Initialize()
    {
        TargetDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_NOTIFICATION + "/" + DBPath);

        TargetDB.OnGetSuccess += OnGetValue;
        TargetDB.GetValue();

        if (ObserverRefreshRate > 0)
        {
            observer = new FirebaseObserver(TargetDB, ObserverRefreshRate);
            observer.OnChange += OnGetValue;
            observer.Start();
        }
    }

    public void ResetDBPath(string newDBPath)
    {
        DBPath = newDBPath;
        observer.Stop();
        Initialize();
    }

    void OnGetValue(Firebase sender, DataSnapshot snapshot)
    {
        var notification = snapshot.Value<string>();
        OnGetNotification.Invoke(notification);
    }

#if UNITY_EDITOR
    [Button]
    void Test()
    {
        if (observer != null)
            observer.Stop();
        Initialize();
    }
#endif
}
