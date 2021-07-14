using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NAXS.NXHelper;
using Mirror;
// using si

// [RequireComponent(typeof(NetworkIdentity))]
[System.Serializable]
public class ObjCtrl
{
    public GameObject _GameObject;
    public Behaviour _Behavior;
}

public class MirrorSys_NetPlayer : MonoBehaviour
{
    NetworkIdentity netIdentity;

    public static MirrorSys_NetPlayer Local;
    // public static GameObject PlayerObject;

    [Tooltip("The main movable part of netplayer, e.g. the player controller")]
    public Transform PlayerMovableObject;


// #if UNITY_EDITOR
    [Header("Dev")]
    public bool LocalDevAuto = false;
    bool isLocalDevMode;
// #endif

    [Header("Player States")]
    public bool IsLocalPlayer = false;
    public bool IsRemotePlayer = false;
    public bool IsOfflineMode = false;

    [Tooltip("if false, deactivate gameobject and wait to be activate till other event")]
    public bool ActivateLocalPlayerOnSpawn = true; 

    [Header("Net Role Setting")]
    public List<GameObject> DeactiveOnLocal;
    public Behaviour[] DestroyOnRemotePlayer;
    public GameObject[] DestroyObjOnRemotePlayer;
    public ObjCtrl[] DestroyOnServer;
// #if UNITY_EDITOR
    public Behaviour[] DestroyOnLocalDev;
// #endif
    [Header("Events")]
    public BoolEvent OnSpawned;
    // public CharacterController CC;


    void Start()
    {
        if (isLocalDevMode)
        {
            PlayerSetup(true);
            return;
        }

        netIdentity = GetComponent<NetworkIdentity>();

        if (netIdentity)
        {
            if(netIdentity.isServer)
                ServerSetup();
            else if(netIdentity.isClient)
                PlayerSetup(netIdentity.isLocalPlayer);
        }
        else
        {
            PlayerSetup(true);
        }

        if(MirrorSys_NetPlayerManager.singleton)
            MirrorSys_NetPlayerManager.singleton.AddNetPlayerToList(this);
    }



    void PlayerSetup(bool isLocal)
    {
        if (isLocal)
        {
            IsLocalPlayer = true;
            MirrorSys_NetPlayer.Local = this;
            gameObject.name += "_Local";
            // Debug.Log("=== MirrorSys_NetPlayer isLocal");

            foreach (GameObject deact in DeactiveOnLocal)
            {
                if (deact) deact.SetActive(false);
            }
        }
        else
        {
            IsRemotePlayer = true;
            foreach (Behaviour C in DestroyOnRemotePlayer)
            {
                if (C) C.enabled = false;
            }
            foreach (GameObject G in DestroyObjOnRemotePlayer)
            {
                if (G) Destroy(G);
            }
            // CC.enabled = false;
        }

        

        if (netIdentity)
        {
            IsOfflineMode = false;
            gameObject.name += string.Concat("_", netIdentity.netId.ToString());
        }else{
            IsOfflineMode = true;
        }


        if(!ActivateLocalPlayerOnSpawn && isLocal){
            this.gameObject.SetActive(false);
        }

        
        OnSpawned.Invoke(isLocal);
        DontDestroyOnLoad(gameObject);
    }


    void ServerSetup()
    {
        foreach (ObjCtrl obj in DestroyOnServer)
        {
            if (obj._GameObject)
            {
                Destroy(obj._GameObject);
            }
            if (obj._Behavior)
            {
                Destroy(obj._Behavior);
            }
        }
    }

    
// #if UNITY_EDITOR
    GameObject myGo;
    void Awake()
    {
        Debug.Log("....Awake");
        myGo = this.gameObject;
        if (LocalDevAuto)
            LocalDevSetup();
    }

    void LocalDevSetup()
    {
        // Stop LocalDevSetup if NetworkManager isActiveAndEnabled
        if(NetworkManager.singleton != null && NetworkManager.singleton.isActiveAndEnabled)
            return;
        

        isLocalDevMode = true;
        foreach (Behaviour behaviour in DestroyOnLocalDev)
        {
            Destroy(behaviour);
        }
        
        var nid = GetComponent<NetworkIdentity>();
        if(!nid)
            return;
            
        var newGo = new GameObject(gameObject.name+"_LocalDev");
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach(Transform child in children)
        {
            if(child.parent == this.transform)      
                child.parent = newGo.transform;
        }
        PlayerSetup(true);
        Destroy(gameObject);
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void OnDestroy()
    {
        if(MirrorSys_NetPlayerManager.singleton)
            MirrorSys_NetPlayerManager.singleton.RemoveNetPlayerFromList(this);
    }
    // IEnumerator SetActiveDelay2()
    // {
    //     Debug.Log("....SetActiveDelay2");
    //     yield return new WaitForSeconds(1);
    //     Debug.Log("....SetActiveDelay2 2");
    //     this.gameObject.SetActive(true);
    // }

    // IEnumerator SetActiveDelay(float delay, GameObject GO, bool state)
    // {
    //     Debug.Log("....SetActiveDelay");
    //     yield return new WaitForSeconds(delay);
    //     Debug.Log("....SetActiveDelay 2");
    //     GO.SetActive(state);
    // }
// #endif
}
