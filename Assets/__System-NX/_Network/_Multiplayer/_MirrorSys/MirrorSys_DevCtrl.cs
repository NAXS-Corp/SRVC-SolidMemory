using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorSys_DevCtrl : MonoBehaviour
{
    public NetworkManager NetManager;

    public bool AutoStartAsServer;
    public bool AutoClientOnWebgl;
    bool isServer = false;
    // Start is called before the first frame update
    void Start()
    {
        if(AutoStartAsServer){
            NetManager.StartServer();
            isServer = true;
        }
        #if UNITY_WEBGL && !UNITY_EDITOR
        if(AutoClientOnWebgl){
            NetManager.StartClient();
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if(isServer){
            return;
        }

        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.Alpha1)){
                NetManager.StartClient();
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)){
                NetManager.StartHost();
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)){
                NetManager.StartServer();
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)){
                NetManager.StopClient();
            }
        }    
    }

    // void OnStartClient(){
    //     Debug.Log("NetDev:: OnStartClient");
    // }

    // void OnStartServer(){
    //     Debug.Log("NetDev:: OnStartServer");
    //     isServer = true;
    // }

    // void OnStopClient(){
    //     Debug.Log("NetDev:: OnStopClient");
    // }

    // void OnClientj
}
