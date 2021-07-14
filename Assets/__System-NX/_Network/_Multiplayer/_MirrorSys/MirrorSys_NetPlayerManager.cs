using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NAXS.NXHelper;
using Mirror;
using UnityEngine.Events;


public class MirrorSys_NetPlayerManager : MonoBehaviour
{

    public static MirrorSys_NetPlayerManager singleton;
    public GameObject OfflinePlayer;

    private MirrorSys_NetPlayer _ActiveLocaPlayer;
    public MirrorSys_NetPlayer ActiveLocalPlayer{
        set{
            _ActiveLocaPlayer = value;
        }
        get{
            return _ActiveLocaPlayer;
        }
    }

    void Awake()
    {
        if(!MirrorSys_NetPlayerManager.singleton)
            MirrorSys_NetPlayerManager.singleton = this;
        else
            Destroy(this);
    }

    public void AddOrActivePlayer()
    {
        if (MirrorSys_NetPlayer.Local)
        {
            // 
            MirrorSys_NetPlayer.Local.gameObject.SetActive(true);
            Debug.Log("MirrorSys_NetPlayerManager: Active Online Player");
        }
        else
        {
            if (ClientScene.ready)
            {
                if (!ClientScene.localPlayer)
                {
                    Debug.Log("MirrorSys_NetPlayerManager: Add Online Player");
                    ClientScene.AddPlayer(ClientScene.readyConnection);
                }
                else
                {
                    Debug.Log("MirrorSys_NetPlayerManager: Active Online Player");
                    ClientScene.localPlayer.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log("MirrorSys_NetPlayerManager: Add Offline Player");
                Instantiate(OfflinePlayer, transform.position, Quaternion.identity);
            }
        }
    }

    public void ActivateLocalPlayer()
    {
        if (MirrorSys_NetPlayer.Local)
        {
            MirrorSys_NetPlayer.Local.gameObject.SetActive(true);
        }
    }

    public void SwitchToOfflinePlayer()
    {
        if(MirrorSys_NetPlayer.Local && MirrorSys_NetPlayer.Local.IsOfflineMode){
            // offline player already exist
            return;
        }
        
        if(MirrorSys_NetPlayer.Local && OfflinePlayer){
            Transform target = MirrorSys_NetPlayer.Local.PlayerMovableObject;
            Instantiate(OfflinePlayer, target.position, target.rotation);
        }
    }


    // //////////////
    // Events
    // ///////////////
    public UnityEvent OnAddPlayer;
    public UnityEvent OnAddLocalPlayer;

    // //////////////
    // NetPlayerList
    // ///////////////
    
    public List<MirrorSys_NetPlayer> NetPlayerList;
    public void AddNetPlayerToList(MirrorSys_NetPlayer netPlayer){
        NetPlayerList.Add(netPlayer);

        OnAddPlayer.Invoke();
        if(netPlayer.IsLocalPlayer) OnAddLocalPlayer.Invoke();

        Dev_OnAddNewPlayer(netPlayer);
    }
    public void RemoveNetPlayerFromList(MirrorSys_NetPlayer netPlayer){
        NetPlayerList.Remove(netPlayer);
    }

    // Dev Usage


    bool dev_Setted_1 = false;
    bool dev_Setted_2 = false;
    bool dev_AnimatorAllEnabled;
    bool dev_AnimatorLocalEnabled;

    void Dev_OnAddNewPlayer(MirrorSys_NetPlayer netPlayer){
        if(dev_Setted_1)
            netPlayer.GetComponent<MirrorSys_MinimalAnimator>().enabled = dev_AnimatorAllEnabled;
        if(netPlayer.IsLocalPlayer && dev_Setted_2){
            netPlayer.GetComponent<MirrorSys_MinimalAnimator>().enabled = dev_AnimatorLocalEnabled;
        }
    }

    public void Dev_SetAnimatorSyncAll(bool state){
        dev_Setted_1 = true;
        foreach(MirrorSys_NetPlayer netPlayer in NetPlayerList){
            netPlayer.GetComponent<MirrorSys_MinimalAnimator>().enabled = state;
            dev_AnimatorAllEnabled = state;
        }
    }
    
    public void Dev_SetAnimatorSyncLocal(bool state){
        dev_Setted_2 = true;
        if(MirrorSys_NetPlayer.Local)
            MirrorSys_NetPlayer.Local.GetComponent<MirrorSys_MinimalAnimator>().enabled = state;
        dev_AnimatorLocalEnabled = state;
    }
}
