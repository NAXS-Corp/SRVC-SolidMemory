using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
//using Sirenix.OdinInspector;
using NAXS.NXPlayer;
using NAXS.Event;

public class AddrSys_PortalTrigger : MonoBehaviour
{   
    // public static List<AddrSys_PortalTrigger> PortalList;
    
    [Header("Gate Manager")]
    public int PortalIdx;
    public int TargetPortalIdx = 1;
    //public GameObject PortalComponent;
    public string PortalResource = "Portal_Tunnel";
    public GameObject[] DestroyOnTrigger;
    public Transform EnterPoint;
    public float RandomPositionRange = 1;


    [Header("Addr Setting")]
    // Actually SceneCode
    public string TargetScene; 

    //private string AddrTargetSceneName;

    //public Transform TransportTarget;
    //[FoldoutGroup("Zone Setting")]
    //[Required]public Collider m_Collider;
    [Header("Zone Setting")]
    public Collider m_Collider;
    //[FoldoutGroup("Zone Setting")]
    public LayerMask _LayerMask = 1 << 12;
    //[FoldoutGroup("Zone Setting")]
    public string _TagMask = "LocalPlayer";

    void Awake()
    {

    }

    void Start()
    {

        // AddrSys_PortalTrigger.PortalList.Add(this);
        if (AddrSys_PortalManager.instance != null){
            AddrSys_PortalManager.instance.AddPortal(this);
            Debug.Log("Portal Added.");
        }
        else
        {
            if(AddrSys_SceneManager.instance){
                if(PortalIdx == 0){ //is default entrance
                    // transport Player
                    float range = RandomPositionRange;
                    Vector3 targetPoint = this.EnterPoint.position;
                    Debug.Log("AddrSys_PortalTrigger SetSpawnPoint "+EnterPoint.gameObject.name);

                    // NXEvent.EmitEventData("SetLocalSpawnPoint", this.EnterPoint);
                    NXP_MainCtrl.SET_SPAWNPOINT(this.EnterPoint);
                }
            }else{
                Debug.Log("Instance not found.");
            }
        }

    }

    void OnDisable()
    {
        // AddrSys_PortalManager.PortalList.Remove(this);
    }


    //=======================//
    //====Method==========//
    //=======================//

    void OnTriggerEnter(Collider other)
    {
        if(!isLocalPlayer(other)) return;
        Debug.Log("OnTriggerEnter "+other.gameObject.name);

        if(DestroyOnTrigger != null) {
            foreach (var obj in DestroyOnTrigger){
                DestroyScene(obj);
            }            
        }

        CreatePortal();
    }


    private void DestroyScene(GameObject _DesObj) 
    {
        Destroy(_DesObj);
    }


    private void CreatePortal()
    {
        if (TargetScene != null){
            GameObject newPortal = Resources.Load<GameObject>(PortalResource);

            var newPortalManager = newPortal.GetComponent<AddrSys_PortalManager>();
            newPortalManager.m_SceneAddressToLoad = TargetScene;
            newPortalManager.m_TargetPortalIdx = TargetPortalIdx;

            Debug.Log("PotalTrigger TargetScene: " + TargetScene);
            Debug.Log("TargetPortal Scene Address: " + newPortalManager.m_SceneAddressToLoad);

            // Instantiate the portal prefab
            Instantiate(newPortal, NXP_MainCtrl.LOCAL.transform.position, NXP_MainCtrl.LOCAL.transform.rotation);

            // Disable self to prevent triggering again
            this.gameObject.SetActive(false);
        }
        else {
            Debug.Log("No TargetScene exist.");
            return;
        }
    }


    bool isLocalPlayer(Collider other){
        if(_LayerMask != (_LayerMask | (1 << other.gameObject.layer)))
           return false;
        if(!string.IsNullOrEmpty(_TagMask) && !other.gameObject.CompareTag(_TagMask))
            return false;
        return true;
    }
}