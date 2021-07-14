using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorSys_ClientCtrl : NetworkBehaviour
{
    public NetworkManager NetManager;
    public bool AutoClientOnWebgl;
    

    void Start()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if(AutoClientOnWebgl){
            NetManager.StartClient();
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
    }

    override public void OnStartClient(){
        Debug.Log("MirrorSys_ClientCtrl:: OnStartClient");
    }

    override public void OnStopClient(){
        Debug.Log("MirrorSys_ClientCtrl:: OnStopClient");
    }


    public virtual void OnClientConnect(NetworkConnection conn) {

    }

    public virtual void OnClientDisconnect(NetworkConnection conn) {}

    public virtual void OnClientError(NetworkConnection conn, int errorCode) {}

    public virtual void OnClientNotReady(NetworkConnection conn) {}

    // public virtual void OnClientChangeScene(string newSceneName, LoadSceneMode sceneMode) {}

    public virtual void OnClientSceneChanged(NetworkConnection conn) {}
}
