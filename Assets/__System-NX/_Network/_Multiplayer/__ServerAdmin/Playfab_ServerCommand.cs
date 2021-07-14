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



public class Playfab_ServerCommand : MonoBehaviour
{
    public Firebase_ServerAdmin FBAdmin;
	public string buildId;
    public bool AutoAddServerToFirebase;
    public bool AuthOnStart;

    void Start()
    {
        if(AuthOnStart)
            GetToken();
    }



    [FoldoutGroup("RequestServer"), Button]
	public void LoginRemoteUser()
	{
		Debug.Log("[ClientStartUp].LoginRemoteUser");
		
		//We need to login a user to get at PlayFab API's. 
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		{
			TitleId = PlayFabSettings.TitleId,
			CreateAccount = true,
			CustomId = GUIDUtility.getUniqueID()
		};
		PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnPlayFabError);
	}

	private void OnPlayFabLoginSuccess(LoginResult response)
	{
		Debug.Log("------OnPlayFabLoginSuccess");
	}

    [FoldoutGroup("RequestServer"), Button]
	private void RequestMultiplayerServer()
	{
		Debug.Log("[ClientStartUp].RequestMultiplayerServer");
		RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
		requestData.BuildId = buildId;
		requestData.SessionId = System.Guid.NewGuid().ToString();
		// requestData.PreferredRegions = new List<AzureRegion>() { AzureRegion.EastUs };
		requestData.PreferredRegions = new List<string>() { "EastUs" };
		PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnPlayFabError);
	}

    [FoldoutGroup("SessionCommand")]
    public string SessionID;

    [FoldoutGroup("SessionCommand"), Button]
    void GetToken(){
        PlayFabAuthenticationAPI.GetEntityToken(new PlayFab.AuthenticationModels.GetEntityTokenRequest(),
            result =>
            {
                Debug.Log("You've got title entity token: " + result.EntityToken);
            },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    [FoldoutGroup("SessionCommand"), Button]
	private void ShutDownServer()
	{
		Debug.Log("[ClientStartUp].RequestMultiplayerServer");
		ShutdownMultiplayerServerRequest requestData = new ShutdownMultiplayerServerRequest();
		requestData.BuildId = buildId;
        requestData.Region = "EastUs";
		requestData.SessionId = SessionID;
        // PlayFabMultiplayerAPI.GetTitleEnabledForMultiplayerServersStatus
		PlayFabMultiplayerAPI.ShutdownMultiplayerServer(requestData, OnShutdown, OnPlayFabError);
	}

    
    [FoldoutGroup("SessionCommand"), Button]
	private void GetServerDetails()
	{
		Debug.Log("[ClientStartUp].RequestMultiplayerServer");
		GetMultiplayerServerDetailsRequest requestData = new GetMultiplayerServerDetailsRequest();
		requestData.BuildId = buildId;
		requestData.SessionId = SessionID;
        requestData.Region = "EastUs";
		PlayFabMultiplayerAPI.GetMultiplayerServerDetails(requestData, OnGetServerDetail, OnPlayFabError);
	}

    void OnGetServerDetail(GetMultiplayerServerDetailsResponse response){
        if(response != null){
			Debug.Log("**** Got Server Info **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num +" SessionId :"+response.SessionId + " ServerId:"+response.ServerId);
            if(AutoAddServerToFirebase)
                FBAdmin.FB_AddToServerList(response.IPV4Address, response.Ports[0].Num, response.ServerId, response.SessionId);
        }else{
			Debug.Log("ConnectRemoteClient null");
        }
    }

    private void OnShutdown(PlayFab.MultiplayerModels.EmptyResponse response){
		Debug.Log("OnShutdown response:"+response.ToString());
    }

	private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
	{
		Debug.Log("OnRequestMultiplayerServer response:"+response.ToString());
        if(response != null){
			Debug.Log("**** Got Server Info **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
            if(AutoAddServerToFirebase)
                FBAdmin.FB_AddToServerList(response.IPV4Address, response.Ports[0].Num, response.ServerId, response.SessionId);
        }else{
			Debug.Log("ConnectRemoteClient null");
        }
	}

    void OnPlayFabError(PlayFabError error)
	{
		Debug.Log("OnPlayFabError ERROR "+error.ErrorMessage);
    }

    
}
