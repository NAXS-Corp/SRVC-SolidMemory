using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.NXPlayer;
using Mirror;
using Sirenix.OdinInspector;
using NAXS;

namespace NAXS.MirrorSys
{

    // 1. Responsible to spawn, initiate, mange and destroy the NXPlayer, own by this NetPlayer.
    [RequireComponent(typeof(NetworkIdentity))]
    public class MirrorSys_NetPlayer_v2 : NetworkBehaviour
    {

        public static MirrorSys_NetPlayer_v2 LOCAL_NXPLAYER;

        [Header("Objects")]
        public MirrorSys_MinimalAnimator netAnimator;
        public MirrorSys_UserProfile UserProfileCtrl;

        [Header("Player Object")]
        public NXP_MainCtrl CurrentNXPlayer; // current player Objects



        // =====================
        // Setup================
        // =====================
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            // RegisterPlayer to NetPlayerManager
            if (MirrorSys_NetPlayerManager_v2.instance)
                MirrorSys_NetPlayerManager_v2.instance.RegisterPlayer(this);

            if (isLocalPlayer){
                MirrorSys_NetPlayer_v2.LOCAL_NXPLAYER = this;
            }

            #if UNITY_EDITOR
            if (isLocalPlayer)
                this.gameObject.name = string.Concat("LOCAL_", this.gameObject.name );
            #endif


            SpawnNXPOnStart();
        }


        // =====================
        // Methods===============
        // =====================

        public void OnDisconnectPlayers(){

        }

        public void SpawnNXPOnStart()
        {
            if (isServer && !isClient) return; // can run as a server, but can be a host(Server+Client)
            if (!MirrorSys_NetPlayerManager_v2.instance) return;

            // default spawn point
            // Vector3 spawnPoint = Vector3.zero;
            // Quaternion spawnRotation = Quaternion.identity;

            if (isLocalPlayer)
            {
                if (NXP_MainCtrl.LOCAL)
                {
                    // offline to online
                    SetupNXP(NXP_MainCtrl.LOCAL);
                    return;
                    // // offline NXP exists, copy transform and destroy it
                    // spawnPoint = NXP_MainCtrl.LOCAL.transform.position;
                    // spawnRotation = NXP_MainCtrl.LOCAL.transform.rotation;
                    // Destroy(NXP_MainCtrl.LOCAL.gameObject);
                }
                else{
                    // No need to spawn yet
                    // Pre-spawn
                    SpawnNXP(MirrorSys_NetPlayerManager_v2.instance.GetCurrentNXPType(), Vector3.zero, Quaternion.identity);
                }
            }
            else
            {
                // Remote Client 
                SpawnNXP(MirrorSys_NetPlayerManager_v2.instance.GetCurrentNXPType(), this.transform.position, this.transform.rotation);
            }

        }

        public void ChangeNXPType(GameObject NXPPrefab)
        {
            if (isServer && !isClient) return;
            if (!NXPPrefab) return;

            // default spawn point
            Vector3 spawnPoint = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            // copy transform and destroy CurrentPlayer rigestered in this NetPlayer
            if (CurrentNXPlayer)
            {
                spawnPoint = CurrentNXPlayer.transform.position;
                spawnRotation = CurrentNXPlayer.transform.rotation;
                Destroy(CurrentNXPlayer.gameObject);
            }

            SpawnNXP(NXPPrefab, spawnPoint, spawnRotation);
        }


        void SpawnNXP(GameObject NXPPrefab, Vector3 position, Quaternion rotation)
        {
            // NXDebug.Log(" ### NetPlayer:: SpawnPlayer " + isLocalPlayer.ToString());

            GameObject newNXPGO = Instantiate(NXPPrefab, position, rotation); //Set Position and rotation??

            if (isLocalPlayer) { 
                #if UNITY_EDITOR
                newNXPGO.name = string.Concat("LOCAL_", newNXPGO.name); 
                #endif
            }
            else if (isClient) { 
                newNXPGO.transform.parent = this.transform; 
                newNXPGO.transform.localPosition = Vector3.zero;
                newNXPGO.transform.rotation = transform.rotation;
            }

            var newNXP = newNXPGO.GetComponent<NXP_MainCtrl>();
            if (newNXP) SetupNXP(newNXP);
        }

        void SetupNXP(NXP_MainCtrl newNXP)
        {
            CurrentNXPlayer = newNXP;
            CurrentNXPlayer.Initialize(isLocalPlayer, this);

            //Set network animator target
            if (CurrentNXPlayer._3rdAnimator) netAnimator.m_Animator = CurrentNXPlayer._3rdAnimator;
        }


        // =====================
        // Update & Sync========
        // =====================
        void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                if (CurrentNXPlayer)
                {
                    transform.position = CurrentNXPlayer.transform.position;
                    transform.rotation = CurrentNXPlayer.transform.rotation;
                }
            }
        }

        // =====================
        // Destroy===============
        // =====================

        void OnDestroy()
        {
            // Net Player destroyed when client disconnect, or out of NetworkVisibility
            if (MirrorSys_NetPlayerManager_v2.instance)
                MirrorSys_NetPlayerManager_v2.instance.RemovePlayer(this);

            if (isLocalPlayer)
            {
                // do nothing
                // online to offline
            }
            else if (isClient)
            {
                if (CurrentNXPlayer)
                    Destroy(CurrentNXPlayer.gameObject);
            }
        }
    }
}