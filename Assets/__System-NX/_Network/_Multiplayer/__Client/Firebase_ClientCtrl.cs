using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleJSON;
using System;
using Mirror;
using Mirror.SimpleWeb;
using NAXS.Event;


// Desc: Observe serverlist on Firebase, decide which server to join by server status and ServerZoneID from player
public class Firebase_ClientCtrl : MonoBehaviour
{
    public static Firebase_ClientCtrl instance;
#if UNITY_EDITOR
    public enum ServerType
    {
        LOCAL,
        REMOTE
    }
    public ServerType DevServerType;
    public ServerReadyObj LocalServerObj;
#endif


    SimpleFirebaseUnity.Firebase ServerListDB;
    FirebaseObserver ServerReadyListOvserver;
    public List<ServerReadyObj> ServerReadyList;
    public List<ServerReadyObj> ZoneMatchedServers;
    public MirrorFire_ClientNetManager ClientNetManager;
    // public WebsocketTransport websocketTransport;


    [Header("ServerZoneId")]
    public bool UseServerZoneId;
    public void SetUseServerZoneId(bool state)
    {
        UseServerZoneId = state;
    }
    int currentServerZoneId = 0;

    void Awake()
    {
        if (!Firebase_ClientCtrl.instance)
        {
            Firebase_ClientCtrl.instance = this;
        }
    }


    void Start()
    {
        ServerReadyList = new List<ServerReadyObj>();

#if UNITY_EDITOR
        // Testing usage
        if (DevServerType == ServerType.LOCAL)
        {
            Debug.Log("#### FBCC Start:: DevServerType=LOCAL");
            ServerReadyList.Clear();
            ServerReadyList.Add(LocalServerObj);
            ClientNetManager.SetServerCandidates(ServerReadyList);
            return;
        }
#endif


        ServerListDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_SERVERREADYLIST);
        FB_GetServerList();
        FB_StartServerListObservor();

        NXEvent.StartListening("ChangeServerZoneId", OnChangeServerZone);
        // Debug.Log("#### FBCC StartListening:: ChangeServerZoneId");
    }

    ///////////////////////////////////////////////

    public void OnChangeServerZone()
    {
        int newZoneId = NXEvent.GetInt("ChangeServerZoneId");
        // Debug.Log("#### FBCC OnChangeServerZone:: "+newZoneId);
        ExecuteChangeZone(newZoneId);
    }

    public void ExecuteChangeZone(int newZoneId)
    {
        Debug.Log("####FBCC ExecuteChangeZone"+newZoneId +" Current::"+currentServerZoneId);
        if (newZoneId != currentServerZoneId && currentServerZoneId != 99)
        {
            // Debug.Log("Changing Server...");
            if (ClientNetManager.isNetworkActive)
            {
                ClientNetManager.ForceDisconnect();
            }
            Debug.Log("####FBCC ExecuteChangeZone RefreshMatchedServers");
            RefreshMatchedServers(newZoneId);
        }
        else
        {
            Debug.Log("####FBCC ExecuteChangeZone no need");
            // Debug.Log("No need to change Server");
        }

        //21-05-07 Force change zone
        // if (ClientNetManager.isNetworkActive)
        //     ClientNetManager.ForceDisconnect();
        // RefreshMatchedServers(newZoneId);
    }

    void RefreshMatchedServers(int targetZoneId)
    {
        Debug.Log("####FBCC RefreshMatchedServers:: targetZoneId::" + targetZoneId+"   UseServerZoneId::"+UseServerZoneId);
        Debug.Log("####FBCC RefreshMatchedServers:: serverReadyList::" + ServerReadyList.Count);

        
        if (UseServerZoneId)
        {
            ZoneMatchedServers.Clear();
            for (int i = 0; i < ServerReadyList.Count; i++)
            {
                if (ServerReadyList[i].ServerZoneId == 99)
                {
                    //connect to server with zoneId 99 first
                    ZoneMatchedServers.Clear();
                    ZoneMatchedServers.Add(ServerReadyList[i]);
                    currentServerZoneId = 99;
                    break;
                }
                else if (ServerReadyList[i].ServerZoneId == targetZoneId)
                {
                    Debug.Log("####FBCC ZoneMatchedServers:: Add");
                    ZoneMatchedServers.Add(ServerReadyList[i]);
                }
            }
        }
        else
        {
            ZoneMatchedServers = ServerReadyList;
        }

        Debug.Log("####FBCC RefreshMatchedServers:: " + ZoneMatchedServers.Count);
        ClientNetManager.SetServerCandidates(ZoneMatchedServers);
    }



    ///////////////////////////////////////////////
    void DecodeServerList(string rawJson)
    {
        JSONNode parsed = JSON.Parse(rawJson);
        // var parsedArray = parsed.AsArray;
        // Debug.Log("####FBCC DecodeServerList parsing"+parsed.Count);
        ServerReadyList.Clear();
        foreach (JSONNode p in parsed)
        {
            // bool isReady = Convert.ToBoolean(p["IsReady"].ToString());
            // Debug.Log("####FBCCDecodeServerList  "+p["Ip"]+" "+p["Port"]+" "+p["ServerZoneId"]);
            ServerReadyList.Add(new ServerReadyObj(p["Ip"], p["Port"], p["ServerZoneId"]));
        }

        RefreshMatchedServers(currentServerZoneId);
    }

    void FB_GetServerList()
    {
        ServerListDB.OnGetSuccess += (SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot) =>
        {
            // Debug.Log("####FBCC [OnGetSuccess] GotServerList: " + snapshot.RawJson);
            DecodeServerList(snapshot.RawJson);
        };
        ServerListDB.OnGetFailed += FailHandler;
        ServerListDB.GetValue();
    }

    void FB_StartServerListObservor()
    {
        ServerReadyListOvserver = new FirebaseObserver(ServerListDB, 5f);
        ServerReadyListOvserver.OnChange += (SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot) =>
        {
            // Debug.Log("####FBCC [Observer] GotServerList: " + snapshot.RawJson);
            DecodeServerList(snapshot.RawJson);
        };
        ServerReadyListOvserver.Start();
    }


    void OKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        // Debug.Log("####FBCC [OK] from key: <" + sender.FullKey + ">");
    }

    void FailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        // Debug.LogError("####FBCC [ERR] from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }
}
