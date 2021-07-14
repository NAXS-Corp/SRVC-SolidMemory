
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Mirror;
    using PlayFab;
    using PlayFab.Networking;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;
    using Mirror.SimpleWeb;
    using System;
    using NAXS;
    using NAXS.NXHelper;


    //Replacement of Playfab_MirrorServer
    public class NXM_ServerNetworkManager : NetworkManager
    {

        public UnityEvent OnMirrorStartServer = new UnityEvent();
        public UnityEvent OnMirrorStopServer = new UnityEvent();
        public PlayerEvent OnPlayerAdded = new PlayerEvent();
        public PlayerEvent OnPlayerRemoved = new PlayerEvent();
        public IntEvent OnPlayerChange;
        public BoolEvent OnServerIsFull;
        public class PlayerEvent : UnityEvent<string> { }

        public List<PlayfabConnection> Connections
        {
            get { return _playfabConnections; }
            private set { _playfabConnections = value; }
        }
        private List<PlayfabConnection> _playfabConnections = new List<PlayfabConnection>();



        /////////////
        //Overrides//
        /////////////

        public override void Awake()
        {
            
            
            if(GetCommandArg("MaxConnection") != null ){
                int maxConArg = int.Parse(GetCommandArg("MaxConnection"));
                if(maxConArg > 0){
                    Debug.Log("NXM_SNM:: ==== Set maxConArg: "+maxConArg+" ===");
                    maxConnections = maxConArg;
                }
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

        public override void Start()
        {
        
        }

        //////////////////////
        //Server-CallBack/////
        //////////////////////
        public override void OnStartServer()
        {
            OnMirrorStartServer.Invoke();
            _playfabConnections.Clear();
        }

        public override void OnStopServer()
        {
            OnMirrorStopServer.Invoke();
        }
        public override void OnServerConnect(NetworkConnection conn)
        {
            NXDebug.Log(string.Concat("NXM_SNM:: OnServerConnect:: ", numPlayers, " active players"));
			var playfabConn = _playfabConnections.Find(c => c.ConnectionId == conn.connectionId);
			if (playfabConn == null)
			{
                var newPlayfabConn = new PlayfabConnection()
				{
					Connection = conn,
					ConnectionId = conn.connectionId,
					LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId,
                    IsAuthenticated = true,
                    PlayFabId = conn.connectionId.ToString() //???
				};
				_playfabConnections.Add(newPlayfabConn);
                OnPlayerAdded.Invoke(newPlayfabConn.PlayFabId);
                OnPlayerChange.Invoke(Connections.Count);
                CheckServerFullState();
			}
        }

        bool isServerFull;

        void CheckServerFullState(){
            if(Connections.Count >= maxConnections){
                isServerFull = true;
                OnServerIsFull.Invoke(isServerFull);
                return;
            }

            if(isServerFull){
                if(Connections.Count <= maxConnections - 5){
                    isServerFull = false;
                    OnServerIsFull.Invoke(isServerFull);
                    return;
                }
            }
        }

        public override void OnServerError(NetworkConnection conn, int errorCode)
        {
            // base.OnServerError(conn, errorCode);
			try
			{
				// var error = netMsg.ReadMessage<ErrorMessage>();
				if (errorCode != 0)
				{
					NXDebug.Log(string.Format("NXM_SNM:: Unity Network Connection Status: code - {0}", errorCode));
				}
			}
			catch (Exception)
			{
				NXDebug.Log("NXM_SNM:: Unity Network Connection Status, but we could not get the reason, check the Unity Logs for more info.");
			}
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            NetworkServer.DestroyPlayerForConnection(conn);
            NXDebug.Log(string.Concat("NXM_SNM:: - OnServerDisconnect:: ", numPlayers, " active players"));
            
			var playfabConn = _playfabConnections.Find(c => c.ConnectionId == conn.connectionId);
			if (playfabConn != null)
			{
				if (!string.IsNullOrEmpty(playfabConn.PlayFabId))
				{
					OnPlayerRemoved.Invoke(playfabConn.PlayFabId);
				}
				_playfabConnections.Remove(playfabConn);
			}
            OnPlayerChange.Invoke(Connections.Count);
            CheckServerFullState();
        }

    
        public void ServerReplaceAllPlayer(GameObject newPrefab){
            playerPrefab = newPrefab;
            foreach(KeyValuePair<int, NetworkConnectionToClient> conn in NetworkServer.connections){
                ReplacePlayer(conn.Value, newPrefab);
            }
        }

        public void ReplacePlayer(NetworkConnection conn, GameObject newPrefab)
        {
            // Cache a reference to the current player object
            GameObject oldPlayer = conn.identity.gameObject;

            // Instantiate the new player object and broadcast to clients
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab));

            // Remove the previous player object that's now been replaced
            NetworkServer.Destroy(oldPlayer);
        }
        
    }

