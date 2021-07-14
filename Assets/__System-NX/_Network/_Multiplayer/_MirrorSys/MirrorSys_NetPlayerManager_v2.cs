using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;
using Mirror;
using NAXS.NXPlayer;
using Sirenix.OdinInspector;

namespace NAXS.MirrorSys{
    public class MirrorSys_NetPlayerManager_v2 : MonoBehaviour
    {
        public static MirrorSys_NetPlayerManager_v2 instance;
        public List<GameObject> PlayerNXPTypes;
        public List<MirrorSys_NetPlayer_v2> NetPlayerList;
        public int CurrentNXPType = 0;

        public GameObject GetCurrentNXPType(){
            if(CurrentNXPType < 0 || CurrentNXPType > PlayerNXPTypes.Count) return null;
            return PlayerNXPTypes[CurrentNXPType];
        }

        void Awake()
        {
            if(!MirrorSys_NetPlayerManager_v2.instance)
                MirrorSys_NetPlayerManager_v2.instance = this;
            else
                Destroy(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            EventRegister();
        }

        //////////////////////////////////////
        //////////////////////////////////////
        //FUNCTIONS

        public void OnDisconnectPlayers(){
            // foreach(MirrorSys_NetPlayer_v2 netPlayer in NetPlayerList){
            //     netPlayer.OnDisconnectPlayers();
            // }
        }

        public void RegisterPlayer(MirrorSys_NetPlayer_v2 netPlayer){
            NetPlayerList.Add(netPlayer);
        }

        public void RemovePlayer(MirrorSys_NetPlayer_v2 netPlayer){
            NetPlayerList.Remove(netPlayer);
        }

        public void ChangePlayerBodyTypeAll(int idx){
            CurrentNXPType = idx;
            if(ClientScene.ready){
                Debug.Log("ChangePlayerBodyTypeAll 1");
                //Change all NetPlayer in List
                if(PlayerNXPTypes[idx]){
                    // ClientScene.play
                    foreach(MirrorSys_NetPlayer_v2 netPlayer in NetPlayerList){
                        netPlayer.ChangeNXPType(PlayerNXPTypes[idx]);
                    }
                }
            }else{
                // client is offline
                if(NXP_MainCtrl.LOCAL){
                    Debug.Log("ChangePlayerBodyTypeAll 2");
                    // send message to NXP_MainCtrl directly
                    NXP_MainCtrl.LOCAL.SwitchLocalPlayerNXP(PlayerNXPTypes[idx]);
                }
                else{
                    // no offline player, create new one
                    var go = Instantiate(PlayerNXPTypes[idx], Vector3.zero, Quaternion.identity);
                    go.GetComponent<NXP_MainCtrl>().Initialize(true);
                }
            }
        }

        // public void ServerChangeVisRange(int range){
        //     foreach(MirrorSys_NetPlayer_v2 netPlayer in NetPlayerList){
        //         var proximityCheck = netPlayer.GetComponent<NetworkProximityChecker>();
        //         if(proximityCheck){
        //             proximityCheck.visRange = range;
        //         }
        //     }
        // }

        // public void ServerSetVisCheck(bool state){
        //     foreach(MirrorSys_NetPlayer_v2 netPlayer in NetPlayerList){
        //         var vis = netPlayer.GetComponent<NetworkVisibility>();
        //         if(vis){
        //             vis.enabled = state;
        //         }
        //     }
        // }

        //////////////////////////////////////
        //////////////////////////////////////
        // Event Listener

        void EventRegister(){
            NXEvent.StartListening("ChangePlayerBodyType", OnChangeBodyType);
        }

        void OnChangeBodyType()
        {
            int idx = NXEvent.GetInt("ChangePlayerBodyType");
            ChangePlayerBodyTypeAll(idx);
            // if(NetworkManager.singleton.isNetworkActive){
            //     ChangePlayerBodyTypeAll(idx);
            // }else{
            //     NXEvent.SetData("ChangePlayerBodyType_Offline", PlayerNXPTypes[idx]);
            //     NXEvent.EmitEvent("ChangePlayerBodyType_Offline");
            // }
        }

    }
}