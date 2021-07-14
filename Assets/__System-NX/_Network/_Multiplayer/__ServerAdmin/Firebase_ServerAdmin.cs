using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Sirenix.OdinInspector;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using SimpleJSON;


[Serializable]
public class ServerStatusObj{
    // public string ServerId;
    public string Ip;
    public int Port;
    public string SesseionId;
    public string ServerId;
    public string Status;
    // public string PlayfabState;
    public bool IsReady;
    // public string BuildId;
    public int PlayerCount;
    public int ServerZoneId;
    public string ServerCommand;

    public ServerStatusObj (string _Ip, int _Port, string _ServerId, string _SessionId, string _Status, bool _IsReady){
        Ip = _Ip;
        Port = _Port;
        ServerId = _ServerId;
        Status = _Status;
        IsReady = _IsReady;
        SesseionId = _SessionId;
    }
}

[Serializable]
public class ServerReadyObj{
    // public string ServerId;
    public string Ip;
    public int Port;
    public int ServerZoneId;

    public ServerReadyObj (string _Ip, int _Port, int _ServerZoneId){
        Ip = _Ip;
        Port = _Port;
        ServerZoneId = _ServerZoneId;
    }
}

[Serializable]
public class ServerCollection{
    public ServerStatusObj[] servers;
}

public class Firebase_ServerAdmin : MonoBehaviour
{
	public string buildId;
    SimpleFirebaseUnity.Firebase ServerListDB;
    FirebaseObserver ServerListObserver;
    SimpleFirebaseUnity.Firebase CurrentServerDB;
    FirebaseObserver CurrentServerObvserver;
    public List<ServerStatusObj> ServerList;
    public bool AutoAddServerToFirebase;

    void Start()
    {
        Debug.Log("Firebase_Manager.PATH_SERVERLIST:: "+Firebase_Manager.PATH_SERVERLIST);
        ServerListDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_SERVERLIST);

        FB_GetServerList();
        FB_StartServerListObservor();
    }


    void FB_StartServerListObservor(){
        ServerListObserver = new FirebaseObserver(ServerListDB, 1f);
        ServerListObserver.OnChange += (SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot) =>
        {
            Debug.Log("[OnGetSuccess] GotServerList: " + snapshot.RawJson);
            DecodeServerList(snapshot.RawJson);
        };
        ServerListObserver.Start();
    }

    void FB_StopServerListObservor(){
        ServerListObserver.Stop();
    }

    void FB_GetServerList(){
        ServerListDB.OnGetSuccess += (SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot) =>
        {
            Debug.Log("[OnGetSuccess] GotServerList: " + snapshot.RawJson);
            DecodeServerList(snapshot.RawJson);
        };
        ServerListDB.OnGetFailed += FB_FailHandler;
        ServerListDB.GetValue();
    }

    void DecodeServerList(string rawJson){
        JSONNode parsed = JSON.Parse(rawJson);
        var parsedArray = parsed.AsArray;
        Debug.Log("parse"+parsed.Count);
        ServerList.Clear();
        foreach(JSONNode p in parsed){
            Debug.Log(p["Ip"]+" "+p["Port"]);
            bool isReady = Convert.ToBoolean(p["IsReady"].ToString());
            ServerList.Add(new ServerStatusObj(p["Ip"], p["Port"], p["ServerId"], p["SessionId"], p["Status"], isReady));
        }
    }

	public void FB_AddToServerList(string ip, int port, string serverId, string sessionId){
        string jsonStr = "{ \"Ip\": \""+ ip +"\", \"Port\": \""+ port +"\", \"SessionId\": \""+ sessionId +"\"}";
        ServerListDB.Child(serverId).UpdateValue(jsonStr);
    }
    
    void FB_OKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        Debug.Log("[OK] from key: <" + sender.FullKey + ">");
    }

    void FB_FailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        Debug.LogError("[ERR] from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

	// ================================
	// Test
	// ================================
}
