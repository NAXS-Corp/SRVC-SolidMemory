using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using NAXS.Event;

public class AddrSys_SceneManager : MonoBehaviour
{

    public static AddrSys_SceneManager instance;

    void Start()
    {
        AddrSys_SceneManager.instance = this;
        NXEvent.StartListening("OnLoadScCode", OnLoadScCode);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneInitialize();
    }

    void SceneInitialize(){

    }
    
    void OnResourcesRetrieved(AsyncOperationHandle<SceneInstance> obj)
    {
        NXEvent.EmitEvent("CompletedLoadScene");
    }

    // ====================
    // ====================
    // API

    public void OnLoadScCode(){
        string scCode = NXEvent.GetString("OnLoadScCode");
        LoadScene(scCode);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        Debug.Log("AddrSys_SceneManager OnSceneLoaded "+scene.name);
        SceneInitialize();
    }

    public void LoadScene(string AddrSceneCode)
    {   
        NXEvent.SetData("AddrLoadingScene", AddrSceneCode);
        NXEvent.EmitEvent("AddrLoadingScene");
        Debug.Log("Loading scene...");

        //Load directly
        Addressables.LoadSceneAsync(AddrSceneCode, UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += OnResourcesRetrieved;
    }


}
