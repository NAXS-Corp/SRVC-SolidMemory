using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;
using NAXS.NXPlayer;

public class AddrSys_PortalExitTrigger : MonoBehaviour
{

    //public Transform TransportTarget;
    //[FoldoutGroup("Zone Setting")]
    //[Required]public Collider m_Collider;
    [Header("Zone Setting")]
    public Collider m_Collider;
    //[FoldoutGroup("Zone Setting")]
    public LayerMask _LayerMask = 1 << 12;
    //[FoldoutGroup("Zone Setting")]
    public string _TagMask = "LocalPlayer";

    //public Transform TransportTarget { get; set; }
    private NXP_MainCtrl playerCtrl;

    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer(other)) return;
        Debug.Log("OnTriggerEnter " + other.gameObject.name);
        
        //AddrSys_PortalManager.instance.PortalCallScene();

        // Debug.Log("TransportTarget value: " + TransportTarget);

        playerCtrl = other.gameObject.GetComponent<NXP_MainCtrl>();
        
        // if (TransportTarget)
        // {
        //     Debug.Log("Target Transport.");
        //     playerCtrl.Transport(TransportTarget);
        // }
        // else
        // {
        // }


    }

    // public void SetTargetAndTransport(Transform target){
    //     playerCtrl.Transport(target);
    // }

    bool isLocalPlayer(Collider other)
    {
        if (_LayerMask != (_LayerMask | (1 << other.gameObject.layer)))
            return false;
        if (!string.IsNullOrEmpty(_TagMask) && !other.gameObject.CompareTag(_TagMask))
            return false;
        return true;
    }
}
