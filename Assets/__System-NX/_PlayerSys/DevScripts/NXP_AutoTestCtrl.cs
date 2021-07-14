using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NAXS.MirrorSys;
namespace NAXS.NXPlayer
{
    public class NXP_AutoTestCtrl : MonoBehaviour
    {
        // public bool DestroyAfterSetup = true;
        public Transform SimuMainCam;
        public bool DestroyCamBrain = true;
        public GameObject[] DestroyOnBuild;
        
        // private MirrorSys_NetPlayer lastLocalPlayer;
        // private bool Settaled = false;
        // private bool NetplayerSettaled = false;

        private NXP_MainCtrl SettaledNXPlayer;
        private MirrorSys_NetPlayer_v2 SettaledNetPlayer;

        void Start()
        {
            // if(NetworkManager.on)
        }

        void FixedUpdate()
        {
            if(!SettaledNXPlayer || SettaledNXPlayer != NXP_MainCtrl.LOCAL){
                Setup();
            }
            if(!SettaledNetPlayer || SettaledNetPlayer != MirrorSys_NetPlayer_v2.LOCAL_NXPLAYER){
                SetupNetplayer();
            }
        }

        public void ResetState(){
            // Settaled = false;
            SettaledNXPlayer = null;
            SettaledNetPlayer = null;
        }

        public void Setup(){
            Debug.Log("NXP_AutoTest: SetupLocalPlayer");
            NXP_MainCtrl.LOCAL.SetSimulationMode(true);
            if(SimuMainCam)
                NXP_MainCtrl.LOCAL.MainCam = SimuMainCam;
            if(DestroyCamBrain)
                Destroy(NXP_MainCtrl.LOCAL._NXPCam.gameObject);

            #if !UNITY_EDITOR
            foreach(GameObject go in DestroyOnBuild){
                if(go)
                    Destroy(go);
            }
            #endif
            
            Debug.Log("NXP_AutoTest: Enable _NXPInput Simulation");
            NXP_MainCtrl.LOCAL._NXPInput.StartSimulation();
            SettaledNXPlayer = NXP_MainCtrl.LOCAL;
            // Settaled = true;
        }

        void SetupNetplayer(){
            Debug.Log("NXP_AutoTest: SetupNetplayer");
            var popChat = MirrorSys_NetPlayer_v2.LOCAL_NXPLAYER.GetComponent<MirrorSys_PopChat>();
            string[] names = {"John", "Tommy", "Yolo"};
            popChat.SetLocalUsername(names[Random.Range(0, names.Length)]);
            SettaledNetPlayer = MirrorSys_NetPlayer_v2.LOCAL_NXPLAYER;
        }
    }
}