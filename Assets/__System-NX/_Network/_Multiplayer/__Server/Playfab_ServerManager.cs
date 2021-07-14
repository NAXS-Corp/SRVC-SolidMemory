using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using UnityEngine.Events;
using PlayFab;
using PlayFab.Networking;
using PlayFab.MultiplayerAgent.Model;
using Sirenix.OdinInspector;
using NAXS.NXHelper;


public class Playfab_ServerManager : MonoBehaviour
{
    public enum ServerType
    {
        LOCAL,
        REMOTE,
		LOCAL_HOST
    }
    public ServerType m_ServerType;

	// public Configuration configuration;

	private List<ConnectedPlayer> _connectedPlayers;
	// public Playfab_MirrorServer MirrorServer;
	public Playfab_MirrorServer MirrorServer;

	public NetworkManager networkManager;

	public bool playFabDebugging = false;
	public bool AutoShutdown = true;

	[Header("Editor Setup")]
	public bool AutoStart = true;

	[FoldoutGroup("Callbacks")]
	public StringEvent OnServerStatusChange;
	[FoldoutGroup("Callbacks")]
	public IntEvent OnServerPlayerChange;
	// public UnityEvent OnServerReady;
	// public UnityEvent OnServerShutDown;
	// public UnityEvent OnServerQuit;
	void Awake()
	{
		#if !UNITY_EDITOR
		AutoStart = true;
		// AutoShutdown = true;
		m_ServerType = ServerType.REMOTE;
		#endif
	}
	

	void Start()
	{
		if(!AutoStart) return;

		if (m_ServerType == ServerType.REMOTE)
		{
			StartRemoteServer();
		}
		else if (m_ServerType == ServerType.LOCAL)
		{
			networkManager.StartServer();
		}
		else if (m_ServerType == ServerType.LOCAL_HOST)
		{
			networkManager.StartHost();
		}
	}

	[Button]
	private void StartRemoteServer()
	{
		Debug.Log("[ServerStartUp].StartRemoteServer");
		_connectedPlayers = new List<ConnectedPlayer>();
		PlayFabMultiplayerAgentAPI.Start();
		OnServerStatusChange.Invoke("Start");
		PlayFabMultiplayerAgentAPI.IsDebugging = playFabDebugging;
		PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
		PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
		PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
		PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;
		PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
		// PlayFabMultiplayerAgentAPI.OnSessionConfigUpdate += OnSeeeionConfigUpdate;

		MirrorServer.OnPlayerAdded.AddListener(OnPlayerAdded);
		MirrorServer.OnPlayerRemoved.AddListener(OnPlayerRemoved);
		MirrorServer.OnMirrorStartServer.AddListener(OnMirrorStartServer);
		MirrorServer.OnMirrorStopServer.AddListener(OnMirrorStopServer);

		StartCoroutine(ReadyForPlayers());
		StartCoroutine(ShutdownServerInXTime());
	}

	IEnumerator ShutdownServerInXTime()
	{
		yield return new WaitForSeconds(300f);
		StartShutdownProcess();
	}

	IEnumerator ReadyForPlayers()
	{
		yield return new WaitForSeconds(.5f);
		PlayFabMultiplayerAgentAPI.ReadyForPlayers();
		MirrorServer.StartServer();
		OnServerStatusChange.Invoke("StandBy");
	}

	private void OnServerActive()
	{
		OnServerStatusChange.Invoke("Active");
		Debug.Log("Server Started From Agent Activation");
	}

	private void OnPlayerRemoved(string playfabId)
	{
		ConnectedPlayer player = _connectedPlayers.Find(x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
		_connectedPlayers.Remove(player);

		PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
		OnServerPlayerChange.Invoke(_connectedPlayers.Count);

		CheckPlayerCountToShutdown();
	}

	private void CheckPlayerCountToShutdown()
	{
		if (_connectedPlayers.Count <= 0)
		{
			StartShutdownProcess();
		}
	}

	private void OnMirrorStartServer(){
		OnServerStatusChange.Invoke("OnStartServer");
	}

	private void OnMirrorStopServer(){
		OnServerStatusChange.Invoke("OnStopServer");
	}
	private void OnPlayerAdded(string playfabId)
	{
		_connectedPlayers.Add(new ConnectedPlayer(playfabId));
		PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
		OnServerPlayerChange.Invoke(_connectedPlayers.Count);
	}

	private void OnAgentError(string error)
	{
		Debug.Log(error);
	}

	private void OnShutdown()
	{
		StartShutdownProcess();
	}

	private void StartShutdownProcess()
	{
		if(!AutoShutdown) return;
		Debug.Log("Server is shutting down");
		// foreach (var conn in MirrorServer.Connections)
		// {
		// 	conn.Connection.Send(CustomGameServerMessageTypes.ShutdownMessage);
		// }
		StartCoroutine(ShutdownServer());
	}

	public void ForceShutdown()
	{
		Debug.Log("Server is Force shutting down");
		// foreach (var conn in MirrorServer.Connections)
		// {
		// 	conn.Connection.Send(CustomGameServerMessageTypes.ShutdownMessage);
		// }
		StartCoroutine(ShutdownServer());
	}

	IEnumerator ShutdownServer()
	{
		Debug.Log("ShutdownServer...");
		yield return new WaitForSeconds(5f);
		OnServerStatusChange.Invoke("Shutdown");
		Debug.Log("Application.Quit");
		Application.Quit();
		
		#if UNITY_EDITOR
      	UnityEditor.EditorApplication.isPlaying = false;
 		#endif
	}

	private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
	{
		Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());
		foreach (var conn in MirrorServer.Connections)
		{
			// todo: update to new Networkmessage from Mirror
			// conn.Connection.Send(new MaintenanceMessage()
			// {
			// 	// ScheduledMaintenanceUTC = (DateTime)NextScheduledMaintenanceUtc
			// },
			// CustomGameServerMessageTypes.ShutdownMessage);
		}
	}


	// ================================
	// Test
	// ================================
	public string FBTestStatus = "Ready";
	[Button]
	void TestSatus(){
		OnServerStatusChange.Invoke(FBTestStatus);
	}
}
