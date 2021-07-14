using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Mirror.SimpleWeb;
using NAXS;
using NAXS.Event;
using NAXS.MirrorSys;
// Internal Dependencies: Firebase_ServerAdmin/ClientCtrl

public class MirrorFire_ClientNetManager : NetworkManager
{
    


    [Header("Customize")]
    public List<ServerReadyObj> ServerCandidates;
    public ServerReadyObj ConnectedServer;
    public string BackupAddress = "114.32.6.212";
    public bool AutoSwitchOfflinePlayer = true;
    // private WebsocketTransport _websocketTransport;
    private SimpleWebTransport _websocketTransport;

    //Reconnect
    [FoldoutGroup("Reconnection")]
    bool clientConnected = false;
    [FoldoutGroup("Reconnection")]
    bool tryBackupServer;
    [FoldoutGroup("Reconnection")]
    // string primaryAddress;
    public List<ServerReadyObj> BackUpServers;

    //Custom Callback
    [Header("UnityEvent")]
    public UnityEvent OnClientConnectExternal;
    [Header("UnityEvent")]
    public UnityEvent OnClientDisconnectExternal;


    /////////////
    //Overrides//
    /////////////


    public override void Awake()
    {
        // NXDebug.Log("MirrorFire_ClientNetManager AWAKE");
        // if(transport.GetType() == typeof(WebsocketTransport))
        // {
        //     _websocketTransport = transport as WebsocketTransport;
        // }
        NXDebug.Log("MirrorFire_ClientNetManager AWAKE");
        if(transport.GetType() == typeof(SimpleWebTransport))
        {
            _websocketTransport = transport as SimpleWebTransport;
        }
    }


    /////////////
    //Firebase Integration//
    /////////////

    public void SetServerCandidates(List<ServerReadyObj> newList){
        NXDebug.Log("==== MirroryFire_ClientNetManager:: SetServerList "+newList.Count);
        ServerCandidates = newList;
        if(!clientConnected){
            currentServerIdx = -1; //Reset
            StartConnectProcess();
        }
    }


    //////////////////////
    //Client-CallBack//
    //////////////////////

    public override void OnClientConnect(NetworkConnection conn)
    {

        NXDebug.Log("MirrorSys_ClientNetManager:: OnClientConnect");
        clientConnected = true;

        if (!clientLoadedScene)
        {
            // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
            if (!ClientScene.ready) ClientScene.Ready(conn);

            if (autoCreatePlayer)
            {
                NXDebug.Log("MirrorSys_ClientNetManager:: AddPlayer");
                ClientScene.AddPlayer(conn);
            }
            OnClientConnectExternal.Invoke();
        }
    }

    /// Called on clients when disconnected from a server.
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        NXDebug.Log("MirrorSys_ClientNetManager:: OnClientDisconnect " + NetworkClient.isConnected);
        clientConnected = false;

        // if (AutoSwitchOfflinePlayer && MirrorSys_NetPlayerManager.singleton)
        //     MirrorSys_NetPlayerManager.singleton.SwitchToOfflinePlayer();

        StopClient();

        StopAllCoroutines();
        StartCoroutine(ReconnectOnce());
        OnClientDisconnectExternal.Invoke();
    }

    public void ForceDisconnect(){
        clientConnected = false;
        MirrorSys_NetPlayerManager_v2.instance.OnDisconnectPlayers();
        StopClient();
    }

    /// Called on clients when a network error occurs.
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        clientConnected = false;
        NXDebug.Log("MirrorSys_ClientNetManager:: OnClientError" + NetworkClient.isConnected);
    }


    public override void OnClientNotReady(NetworkConnection conn)
    {
        clientConnected = false;
        NXDebug.Log("MirrorSys_ClientNetManager:: OnClientNotReady" + NetworkClient.isConnected);
    }


    /////////////
    //Methods//
    /////////////


    int currentServerIdx = -1;
    void StartConnectProcess(){
        NXDebug.Log("MirrorSys_ClientNetManager:: StartConnectProcess ");
        if(ServerCandidates.Count == 0) return;

        // Iterate through server list
        currentServerIdx += 1;

        //Loop back to first server in list
        if(currentServerIdx >= ServerCandidates.Count)
            currentServerIdx = 0; 

        networkAddress = ServerCandidates[currentServerIdx].Ip;
        // _websocketTransport.port = Convert.ToUInt16(ServerCandidates[currentServerIdx].Port); 
        _websocketTransport.port = (ushort)ServerCandidates[currentServerIdx].Port; 
        NXDebug.Log("MirrorSys_ClientNetManager:: StartClient:: "+networkAddress+" "+_websocketTransport.port);
        StartClient();
    }

    void StartBackupProcess(){

    }
    //Client Reconnect
    IEnumerator ReconnectOnce()
    {
        yield return new WaitForSeconds(0.5f);

        // TrySwitchServer();
        NXDebug.Log("MirrorSys_ClientNetManager:: ReconnectOnce ");
        StopAllCoroutines();
        StartConnectProcess();
    }

    void TrySwitchServer()
    {
        if (tryBackupServer && !string.IsNullOrEmpty(BackupAddress))
        {
            networkAddress = BackupAddress;
        }
        else
        {
            // networkAddress = primaryAddress;
        }
        tryBackupServer = !tryBackupServer;
        
    }

    
    /////////////
    //Editor//
    /////////////

    [Button]
    void StartClient_Direct(){
        StartClient();
    }

}
