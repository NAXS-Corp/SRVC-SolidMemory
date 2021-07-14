using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;

namespace NAXS.NXPlayer{
    public class NXP_DevSetup : MonoBehaviour
    {
        public static NXP_DevSetup instance;
        public NXP_MainCtrl DevPlayer;
        public Transform SpawnPoint;
        public bool DestroyOnBuild = true;

        // public bool DontDestory;
        
        void Awake()
        {
            #if !UNITY_EDITOR
            if(DestroyOnBuild){
                DestroyImmediate(this.gameObject);
                return;
            }                
            #endif

            if(this.gameObject.activeSelf){
                if(!NXP_DevSetup.instance){
                    NXP_DevSetup.instance = this;
                }else{
                    DestroyImmediate(this.gameObject);
                }
            }
        }

        #if UNITY_EDITOR
        private void Update() {
            if(Input.GetKey(KeyCode.LeftShift)){
                if(Input.GetKeyDown(KeyCode.Plus)){
                    // var go = 

                }
            }
        }
        #endif

        void Start()
        {
            if(DevPlayer){
                DevPlayer.Initialize(true);
                if(SpawnPoint){
                    NXP_MainCtrl.LOCAL.TransportTransform(SpawnPoint);
                }
            }

            // if(DontDestory){
            //     DontDestroyOnLoad(this.gameObject);
            // }
        }
    }
}