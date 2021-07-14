using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Sirenix.OdinInspector;
// using Mirror.Websocket;
using Mirror.SimpleWeb;

[System.Serializable]
public enum MirrorSys_Roles{
    Client,
    Server,
    Host
}

public class MirrorSys_NetManager : NetworkManager
{
    [Header("Customize")]
    public string BackupAddress = "114.32.6.212";
    public bool startClientOnWebgl = true;
    public bool AutoSwitchOfflinePlayer = true;


    //Reconnect
    [FoldoutGroup("Reconnection")]
    public bool clientAutoReconnect = true;
    [FoldoutGroup("Reconnection")]
    bool clientConnected = false;
    [FoldoutGroup("Reconnection")]
    bool tryBackupServer;
    [FoldoutGroup("Reconnection")]
    string primaryAddress;


#if UNITY_EDITOR
    [Header("Editor Dev")]
    [BoxGroup("Dev Settings")]
    [EnumToggleButtons]
    public MirrorSys_Roles EditorRole;
    public bool AutoStartRole = true;
    [BoxGroup("Dev Settings")]
    public int EditorPort = 7779;
#endif

    //Custom Callback
    [FoldoutGroup("UnityEvent")]
    public UnityEvent OnClientConnectExternal;
    [FoldoutGroup("UnityEvent")]
    public UnityEvent OnClientDisconnectExternal;


    /////////////
    //Overrides//
    /////////////


    public override void Start()
    {
    
        primaryAddress = networkAddress;


#if UNITY_EDITOR
        if(transport.GetType() == typeof(SimpleWebTransport))
        {
            SimpleWebTransport _transport = transport as SimpleWebTransport;
            _transport.port = (ushort)EditorPort;
        }

        if (AutoStartRole)
        {
            switch (EditorRole)
            {
                case MirrorSys_Roles.Server:
                    Debug.Log("MirrorSys_NaxsNetManager:: StartServer");
                    StartServer();
                    break;
                case MirrorSys_Roles.Client:
                    Debug.Log("MirrorSys_NaxsNetManager:: StartClient");
                    StartClient();
                    break;
                case MirrorSys_Roles.Host:
                    StartHost();
                    break;
                default:
                    break;
            }
        }
#endif

#if UNITY_SERVER
        if (autoStartServerBuild)
        {
            StartServer();
            return;
        }
#endif

#if UNITY_WEBGL
        if (startClientOnWebgl)
        {
            Debug.Log("MirrorSys_NaxsNetManager:: StartClient");
            StartClient();
            return;
        }
#endif
    }


    //////////////////////
    //Client-CallBack//
    //////////////////////

    public override void OnClientConnect(NetworkConnection conn)
    {

        Debug.Log("MirrorSys_NaxsNetManager:: OnClientConnect");
        clientConnected = true;

        if (!clientLoadedScene)
        {
            // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
            if (!ClientScene.ready) ClientScene.Ready(conn);
            if (autoCreatePlayer)
            {
                Debug.Log("MirrorSys_NaxsNetManager:: AddPlayer");
                ClientScene.AddPlayer(conn);
            }
            OnClientConnectExternal.Invoke();
        }
    }

    /// Called on clients when disconnected from a server.
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("MirrorSys_NaxsNetManager:: OnClientDisconnect " + NetworkClient.isConnected);
        clientConnected = false;
        if (AutoSwitchOfflinePlayer && MirrorSys_NetPlayerManager.singleton)
            MirrorSys_NetPlayerManager.singleton.SwitchToOfflinePlayer();
        StopClient();

        StopAllCoroutines();
        StartCoroutine(ReconnectOnce());
        OnClientDisconnectExternal.Invoke();
    }

    /// Called on clients when a network error occurs.
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        clientConnected = false;
        Debug.Log("MirrorSys_NaxsNetManager:: OnClientError" + NetworkClient.isConnected);
    }


    public override void OnClientNotReady(NetworkConnection conn)
    {
        clientConnected = false;
        Debug.Log("MirrorSys_NaxsNetManager:: OnClientNotReady" + NetworkClient.isConnected);
    }


    /////////////
    //Methods//
    /////////////


    //Client Reconnect

    IEnumerator ReconnectOnce()
    {
        yield return new WaitForSeconds(0.5f);

        TrySwitchServer();
        Debug.Log("MirrorSys_NaxsNetManager:: ReconnectOnce " + networkAddress);
        StopAllCoroutines();
        StartClient();
    }

    void TrySwitchServer()
    {
        if (tryBackupServer && !string.IsNullOrEmpty(BackupAddress))
        {
            networkAddress = BackupAddress;
        }
        else
        {
            networkAddress = primaryAddress;
        }
        tryBackupServer = !tryBackupServer;
    }


    //////////////////////
    //Server-CallBack/////
    //////////////////////
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log(string.Concat("+ OnServerConnect:: ", numPlayers, " active players"));
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayerForConnection(conn);
        Debug.Log(string.Concat("- OnServerDisconnect:: ", numPlayers, " active players"));
    }
}
