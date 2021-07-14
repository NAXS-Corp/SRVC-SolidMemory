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


// Report server state to Firebase
// replacement of Firebase_PlayfabServerMonitor.cs
public class NXM_ServerStatusReporter : Firebase_CtrlBase
{
    private bool isInitialized;
    IDictionary<string, string> ServerConfig;
    public float ServerReportRate = 300;
    public ServerStatusObj m_ServerStatusObj;
    private SimpleFirebaseUnity.Firebase DB_ServerList;
    private SimpleFirebaseUnity.Firebase DB_ServerListObj; // This server in the firebase ServerList
    private SimpleFirebaseUnity.Firebase DB_ServerReadyObject; // This server in the firebase ServerRadyList
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
        Debug.Log("NXM_SSR / StartChild::");
        DBInitialize();

        #if UNITY_EDITOR
        EditorApplication.quitting += OnQuit;
        #endif
    }
    

    // Set firebase path & callbacks
    void DBInitialize(){
        if(isInitialized) return;

        #if !UNITY_EDITOR
        Dev_NoPlayFab = false;
        // m_ServerStatusObj = new ServerStatusObj("----", 9999, "_ServerId", "_SessionId", "Test", true);
        // m_ServerStatusObj.ServerZoneId = DefaultZoneId;
        #endif
        GetPlayFabServerConfig();


        Debug.Log("NXM_SSR / DBInitialize / ServerID:"+m_ServerStatusObj.ServerId);
        DB_ServerList = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_SERVERLIST);
        DB_ServerList.OnSetSuccess += FB_OKHandler;
        DB_ServerList.OnSetFailed += FB_FailHandler;
        
        if(string.IsNullOrEmpty(m_ServerStatusObj.ServerId)) {
            Debug.Log("NXM_SSR / DBInitialize / Failed: No ServerId from playfab");
            return;
        };

        Debug.Log("NXM_SSR / DBInitialize / Listen to changes on ServerList");
        DB_ServerReadyObject = Firebase_Manager.instance.firebase.Child(Firebase_Manager.PATH_SERVERREADYLIST).Child(GetServerFBPath);
        DB_ServerListObj = DB_ServerList.Child(GetServerFBPath);
        DB_ServerListObj.OnSetSuccess += FB_OKHandler;
        DB_ServerListObj.OnSetFailed += FB_FailHandler;
        DB_ServerListObj.OnUpdateSuccess += FB_OKHandler;
        DB_ServerListObj.OnUpdateFailed += FB_FailHandler;

        DBStartListening();
        StartCoroutine(ServerCheck());

        isInitialized = true;
    }

    void DBStartListening(){
        Debug.Log("NXM_SSR / DBStartListening");
        ServerObjObservor = new FirebaseObserver(DB_ServerListObj, ObserverRefreshRate);
        ServerObjObservor.OnChange += FB_OnServerObjChange;
        ServerObjObservor.Start();

        FB_UpdateServerStatus("FirebaseStartListening");

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

        Debug.Log("NXM_SSR / GetPlayFabServerConfig / "+ m_ServerStatusObj.Ip+" / "+m_ServerStatusObj.Port);
    }

    // =================
    // Firebase callbacks
    void FB_OnServerObjChange(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot){
            Debug.Log("NXM_SSR / ServerObjObservor OnChange: " + snapshot.RawJson);

            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
            List<string> keys = snapshot.Keys;

            if(dict.ContainsKey("ServerCommand")){
                if(dict["ServerCommand"].ToString() == "SHUTDOWN"){
                    Debug.Log("NXM_SSR / ServerCommand SHUTDOWN");
                    OnCommandShutdown.Invoke();
                }
            }
            
            Debug.Log("NXM_SSR / OnServerZoneId  / "+ int.Parse(dict["ServerZoneId"].ToString()));
            m_ServerStatusObj.ServerZoneId = int.Parse(dict["ServerZoneId"].ToString());          
            m_ServerStatusObj.Port = int.Parse(dict["Port"].ToString());                
            FB_UpdateServerReadyList(!isServerFull);
    }

    // ====================
    // Firebase Methods

	private void FB_UpdateServerInfo(string Status, bool isVMReady)
	{
        if(DB_ServerListObj == null) DBInitialize();

        bool isReadyForConnect;
        if(!isVMReady) isReadyForConnect = false;
        else  isReadyForConnect = !isServerFull;

        GetPlayFabServerConfig();
        m_ServerStatusObj.IsReady = isReadyForConnect;
        FB_UpdateServerReadyList(isReadyForConnect);
        
        if(Status == "OnQuit"){
            Debug.Log("NXM_SSR / FB_UpdateServerInfo / Delete from list");
            DB_ServerListObj.Delete();
            DB_ServerReadyObject.Delete();
        }else if (Status == "Terminated"){
            DB_ServerListObj.Delete();
        }else{
            #if UNITY_EDITOR
            if(!Dev_NoPlayFab)
                m_ServerStatusObj.Status = Status;
            #else
                m_ServerStatusObj.Status = PlayFabMultiplayerAgentAPI.CurrentState.CurrentGameState.ToString();
            #endif

            string jsonStr = JsonUtility.ToJson(m_ServerStatusObj);
            DB_ServerListObj.UpdateValue(jsonStr);
        }

        Debug.Log("NXM_SSR / FB_UpdateServerInfo finished / IP:"+m_ServerStatusObj.Ip+" / Port:"+m_ServerStatusObj.Port);
	}
    
    bool isServerFull;
    public void OnServerFullChange(bool isFull){
        isServerFull= isFull;
        FB_UpdateServerReadyList(!isFull);
    }


    void FB_UpdateServerReadyList(bool isReady){
        Debug.Log("NXM_SSR / FB_UpdateServerReadyList1 " +isReady.ToString());
        if(DB_ServerReadyObject == null) Initialize();
        Debug.Log("NXM_SSR / FB_UpdateServerReadyList2 " +isReady.ToString());

        if(isReady){
            string jsonData = JsonUtility.ToJson(new ServerReadyObj(m_ServerStatusObj.Ip, m_ServerStatusObj.Port, m_ServerStatusObj.ServerZoneId));
            DB_ServerReadyObject.UpdateValue(jsonData);
        }else{
            DB_ServerReadyObject.Delete();
        }
    }

    void FB_UpdateServerStatus(string status){
        if(DB_ServerListObj == null) DBInitialize();

        if(m_ServerStatusObj.Status == status)
            return; // no need to update

        Debug.Log("NXM_SSR / FB_UpdateServerStatus / "+ status);

        if(Dev_NoPlayFab)
            m_ServerStatusObj.Status = status;
        else
            m_ServerStatusObj.Status = PlayFabMultiplayerAgentAPI.CurrentState.CurrentGameState.ToString();

        DB_ServerListObj.Child("Status").SetValue(m_ServerStatusObj.Status);
    }


    void FB_UpdatePlayerCount(int playerCount){
        if(DB_ServerListObj == null) DBInitialize();
        Debug.Log("NXM_SSR / FB_UpdatePlayerCount / "+ playerCount);
        m_ServerStatusObj.PlayerCount = playerCount;
        DB_ServerListObj.Child("PlayerCount").SetValue(playerCount);
    }

    string GetServerFBPath{
        get{
            return m_ServerStatusObj.ServerId;
        }
    }

    // ===============
    // Public Methods & API
    public void OnServerStatusChange (string Status){
        Debug.Log("NXM_SSR / OnServerStatusChange / "+ Status +" ");
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
        else if(Status == "OnStartMirrorServer")
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
        else if(Status == "OnStopMirrorServer")
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
