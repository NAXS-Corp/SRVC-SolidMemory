using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
//using UnityEngine.SceneManagement;
using NAXS.NXPlayer;

public class AddrSys_PortalManager : MonoBehaviour
{
    public static AddrSys_PortalManager instance;
    // public static List<AddrSys_PortalTrigger> PortalList;
    public string m_SceneAddressToLoad;
    //public AssetLabelReference m_SceneAddressToLoad;
    //public IList<GameObject> m_Assets;

    private IEnumerator PreLoadManager;

    public int m_TargetPortalIdx { get; set; }
    public AddrSys_PortalExitTrigger ExitTrigger;

    void Start()
    {
        DontDestroyOnLoad(this);
        AddrSys_PortalManager.instance = this;
        Debug.Log("AddrSys_PortalManager:: Instance created.");
        PortalCallScene();        
    }


    public void PortalCallScene()
    {   
        Debug.Log("AddrSys_PortalManager:: SceneAddressToLoad: " + m_SceneAddressToLoad);
        AddrSys_SceneManager.instance.LoadScene(m_SceneAddressToLoad);
    }

    public void AddPortal(AddrSys_PortalTrigger newTrigger){
        if(newTrigger.PortalIdx == m_TargetPortalIdx)
        {
            Debug.Log("AddrSys_PortalManager:: Set Enterpoint.");
            NXP_MainCtrl.SET_SPAWNPOINT(newTrigger.transform);
            Destroy(this.gameObject);
        }
        else 
        {
            Debug.Log("No add portal.");
        }
    }
}
