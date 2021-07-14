using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

public abstract class Firebase_CtrlBase : MonoBehaviour
{
    public bool InitializeOnAwake = false;
    public string DBPath;
    public float ObserverRefreshRate = 1f;
    Firebase_Manager FirebaseManager;
    // Start is called before the first frame update
    // Firebase objects
    protected Firebase TargetDB;
    protected FirebaseObserver observer;
    public bool StopObserveOnDestroy;
    void Awake()
    {
        if(!Firebase_Manager.instance)
        {
            Debug.LogError("=========== Can't find Firebase_Manager singleton");
            // return;
            var newObj = new GameObject("Firebase_Manager");
            var newManager = newObj.AddComponent<Firebase_Manager>();
            newManager.Initialize();
        }

        if(InitializeOnAwake)
            Initialize();
            
        AwakeChild();
    }

    void Start()
    {
        if(!InitializeOnAwake)
            Initialize();
        StartChild();
    }

    void OnDisable()
    {
        if(!StopObserveOnDestroy && observer != null)
            observer.Stop();
    }

    void OnDestroy()
    {
        if(StopObserveOnDestroy && observer != null)
            observer.Stop();
    }

    protected virtual void AwakeChild(){}
    protected virtual void StartChild(){}
    protected virtual void Initialize(){}
    
    protected void FB_OKHandler(Firebase sender, DataSnapshot snapshot)
    {
        Debug.Log("[Firebase: OK] from key: <" + sender.FullKey + ">");
    }

    protected void FB_FailHandler(Firebase sender, FirebaseError err)
    {
        Debug.LogError("[Firebase: ERR] from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

}
