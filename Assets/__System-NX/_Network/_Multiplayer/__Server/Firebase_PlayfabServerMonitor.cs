using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.Networking;
using PlayFab.MultiplayerAgent.Model;
using SimpleFirebaseUnity;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.Events;
using Mirror;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Firebase_PlayfabServerMonitor : Firebase_CtrlBase
{
    private bool isInitialized;
    IDictionary<string, string> ServerConfig;
    public float ServerReportRate = 300;
    public ServerStatusObj m_ServerStatusObj;
    public SimpleFirebaseUnity.Firebase ServerListDB;
    private SimpleFirebaseUnity.Firebase ServerObjDB;
    public SimpleFirebaseUnity.Firebase MyServerReadyDB;
    private FirebaseObserver ServerObjObservor;

    public UnityEvent OnCommandShutdown;

    [Header("Dev/Editor")]
    public bool Dev_NoPlayFab;
    public bool Dev_SetLocalServerInfo;
    

    // =================================================================
    void Awake()
    {
        
        if(GetCommandArg("ZoneId") != null){
            m_ServerStatusObj.ServerZoneId = int.Parse(GetCommandArg("ZoneId"));
        }
        if(GetCommandArg("RandomZoneId") != null){
            int randomRange = int.Parse(GetCommandArg("RandomZoneId"));
            m_ServerStatusObj.ServerZoneId = Random.Range(0, randomRange);
        }
    }

    private string GetCommandArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
        
    // =================================================================
    protected override void StartChild()
    {
        Debug.Log("====FB_PSM StartChild::");
        DBInitialize();
        StartDB();
        StartCoroutine(ServerCheck());

        #if UNITY_EDITOR
        EditorApplication.quitting += OnQuit;
        #endif
    }
    

    void DBInitialize(){
        if(isInitialized) return;

        #if !UNITY_EDITOR
        Dev_NoPlayFab = false;
        // m_ServerStatusObj = new ServerStatusObj("----", 9999, "_ServerId", "_SessionId", "Test", true);
        // m_ServerStatusObj.ServerZoneId = DefaultZoneId;
        #endif


        Debug.Log("====FB_PSM DBInitialize:: "+Firebase_Manager.PATH_SERVERLIST);
        ServerListDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_SERVERLIST);

        ServerListDB.OnSetSuccess += FB_OKHandler;
        ServerListDB.OnSetFailed += FB_FailHandler;


        GetPlayFabServerConfig();
        
        if(string.IsNullOrEmpty(m_ServerStatusObj.ServerId)) return;

        MyServerReadyDB = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_SERVERREADYLIST).Child(GetServerFBPath);
        ServerObjDB = ServerListDB.Child(GetServerFBPath);
        ServerObjDB.OnSetSuccess += FB_OKHandler;
        ServerObjDB.OnSetFailed += FB_FailHandler;
        ServerObjDB.OnUpdateSuccess += FB_OKHandler;
        ServerObjDB.OnUpdateFailed += FB_FailHandler;


        isInitialized = true;
    }

    void StartDB(){
        Debug.Log("====FB_PSM StartDB");
        FB_StartServerObjObservor();
        FB_UpdateServerStatus("FirebaseStart");

        // Dev usage
        #if UNITY_EDITOR
        if(Dev_NoPlayFab){
            FB_UpdateServerInfo("LocalReady", true);
        }
        #endif
    }

    IEnumerator ServerCheck(){
        yield return new WaitForSeconds(ServerReportRate);
        if(!isServerFull && NetworkManager.singleton.isNetworkActive){
            FB_UpdateServerInfo("ServerChecked", true);
        }else{
            FB_UpdateServerReadyList(false);
        }
        StartCoroutine(ServerCheck());
    }

    void FB_StartServerObjObservor(){
        // if(ServerObjDB == null) DBInitialize();
        Debug.Log("====FB_PSM ServerObjObservor");

        ServerObjObservor = new FirebaseObserver(ServerObjDB, ObserverRefreshRate);
        ServerObjObservor.OnChange += (SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot) =>
        {
            Debug.Log("====FB_PSM ServerObjObservor OnChange: " + snapshot.RawJson);
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
            List<string> keys = snapshot.Keys;

            if(dict.ContainsKey("ServerCommand")){
                if(dict["ServerCommand"].ToString() == "SHUTDOWN"){
                    Debug.Log("====FB_PSM ServerCommand SHUTDOWN");
                    OnCommandShutdown.Invoke();
                }
            }
            if(dict.ContainsKey("ServerZoneId")){
                // Debug.Log("====FB_PSM OnServerZoneId");
                // Debug.Log("====FB_PSM OnServerZoneId :: "+ System.Int32.Parse(dict["ServerZoneId"].ToString()));
                Debug.Log("====FB_PSM OnServerZoneId :: "+ int.Parse(dict["ServerZoneId"].ToString()));
                m_ServerStatusObj.ServerZoneId = int.Parse(dict["ServerZoneId"].ToString());                
                FB_UpdateServerReadyList(!isServerFull);
            }
        };
        ServerObjObservor.Start();
    }

    void GetPlayFabServerConfig(){

        #if UNITY_EDITOR
        if(Dev_NoPlayFab)
            return;
        #endif
        
        ServerConfig = PlayFabMultiplayerAgentAPI.GetConfigSettings();
        ServerConfig.TryGetValue(PlayFabMultiplayerAgentAPI.PublicIpV4AddressKey, out m_ServerStatusObj.Ip);
        ServerConfig.TryGetValue(PlayFabMultiplayerAgentAPI.ServerIdKey, out m_ServerStatusObj.ServerId);
        ServerConfig.TryGetValue(PlayFabMultiplayerAgentAPI.SessionIdKey, out m_ServerStatusObj.SesseionId);
        
        IEnumerable<GamePort> getPort = PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo().GamePortsConfiguration;
        List<GamePort> gamePortList = getPort.ToList();
        m_ServerStatusObj.Port = gamePortList[0].ClientConnectionPort;

        Debug.Log("==== GetServerConfig:: "+ m_ServerStatusObj.Ip+" / "+m_ServerStatusObj.Port);
    }


	private void FB_UpdateServerInfo(string Status, bool isVMReady)
	{
        if(ServerObjDB == null) DBInitialize();

        bool isReadyForConnect;
        if(!isVMReady) isReadyForConnect = false;
        else  isReadyForConnect = !isServerFull;

        GetPlayFabServerConfig();
        m_ServerStatusObj.IsReady = isReadyForConnect;
        FB_UpdateServerReadyList(isReadyForConnect);
        
        if(Status == "OnQuit"){
            ServerObjDB.Delete();
        }else{
            #if UNITY_EDITOR
            if(!Dev_NoPlayFab)
                m_ServerStatusObj.Status = Status;
            #else
                m_ServerStatusObj.Status = PlayFabMultiplayerAgentAPI.CurrentState.CurrentGameState.ToString();
            #endif

            string jsonStr = JsonUtility.ToJson(m_ServerStatusObj);
            ServerObjDB.UpdateValue(jsonStr);
        }

        Debug.Log("==== FB_UpdateServerInfo finished:: IP:"+m_ServerStatusObj.Ip+" / Port:"+m_ServerStatusObj.Port);
	}
    
    bool isServerFull;
    public void OnServerFullChange(bool isFull){
        isServerFull= isFull;
        FB_UpdateServerReadyList(!isFull);
    }


    void FB_UpdateServerReadyList(bool isReady){
        Debug.Log("==== FB_UpdateServerReadyList1" +isReady.ToString());
        if(MyServerReadyDB == null) Initialize();
        Debug.Log("==== FB_UpdateServerReadyList2" +isReady.ToString());

        if(isReady){
            string jsonData = JsonUtility.ToJson(new ServerReadyObj(m_ServerStatusObj.Ip, m_ServerStatusObj.Port, m_ServerStatusObj.ServerZoneId));
            MyServerReadyDB.UpdateValue(jsonData);
        }else{
            MyServerReadyDB.Delete();
        }
    }

    void FB_UpdateServerStatus(string status){
        if(ServerObjDB == null) DBInitialize();

        if(m_ServerStatusObj.Status == status)
            return; // no need to update

        Debug.Log("==== FB_UpdateServerStatus:: "+ status);

        if(Dev_NoPlayFab)
            m_ServerStatusObj.Status = status;
        else
            m_ServerStatusObj.Status = PlayFabMultiplayerAgentAPI.CurrentState.CurrentGameState.ToString();

        ServerObjDB.Child("Status").SetValue(m_ServerStatusObj.Status);
    }


    void FB_UpdatePlayerCount(int playerCount){
        if(ServerObjDB == null) DBInitialize();
        Debug.Log("==== FB_UpdatePlayerCount:: "+ playerCount);
        m_ServerStatusObj.PlayerCount = playerCount;
        ServerObjDB.Child("PlayerCount").SetValue(playerCount);
    }

    string GetServerFBPath{
        get{
            return m_ServerStatusObj.ServerId;
        }
    }

    public void OnServerStatusChange (string Status){
        Debug.Log("==== OnServerStatusChange:: "+ Status +" ");
        if(Status == "Start")
        { 
            // Firebase not ready at this stage. Sending data will crash Playfab
            return;
        }
        else if(Status == "StandBy")
        {
            // Server in standby mode
            // FB_UpdateServerInfo(Status, true);
            FB_UpdateServerStatus("StandBy");
        }
        else if(Status == "OnStartServer")
        {
            // Server in standby mode
            FB_UpdateServerInfo(Status, true);
        }
        else if(Status == "Active")
        {
            // Server is activated and ready for player
            // FB_UpdateServerInfo(Status, true);
            FB_UpdateServerStatus("Active");
        }
        else if(Status == "OnStopServer")
        {
            // Server in standby mode
            FB_UpdateServerInfo(Status, false);
        }
        else if(Status == "Shutdown")
        {
            FB_UpdateServerInfo(Status, false);
        }else{
            FB_UpdateServerStatus(Status);
        }
        
    }

    public void OnServerPlayerChange (int connectionCount){
        FB_UpdatePlayerCount(connectionCount);
    }

    void OnDestroy()
    {
        FB_UpdateServerStatus("Destroyed");
    }
    

    void OnApplicationQuit(){
        OnQuit();
    }

    void OnQuit(){
        FB_UpdateServerInfo("OnQuit", false);
    }


}
