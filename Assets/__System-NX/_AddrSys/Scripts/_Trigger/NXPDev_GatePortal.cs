using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.NXPlayer
{
    public class NXPDev_GatePortal : MonoBehaviour
    {   
        
        [Header("Gate Manager")]
        public GameObject DestroyObject;
        public GameObject PortalComponent;
        public Vector3 PortalPosition;
        public string TargetScene;

        //public Transform TransportTarget;
        [FoldoutGroup("Zone Setting")]
        [Required]public Collider m_Collider;
        [FoldoutGroup("Zone Setting")]
        public LayerMask _LayerMask = 1 << 12;
        [FoldoutGroup("Zone Setting")]
        public string _TagMask = "LocalPlayer";

        //=======================//
        //====Method==========//
        //=======================//

        void OnTriggerEnter(Collider other)
        {
            if(!ConditionCheck(other)) return;
            Debug.Log("OnTriggerEnter "+other.gameObject.name);
            //var m_NXPMain = other.gameObject.GetComponent<NXP_MainCtrl>();
            //m_NXPMain.Transport(TransportTarget);
            DestroyScene(DestroyObject);
            CreatePortal();
        }


        private void DestroyScene(GameObject _DesObj) 
        {
            Destroy(_DesObj);
        }


        private void CreatePortal()
        {
            GameObject targetgate = Instantiate(PortalComponent, PortalPosition, Quaternion.identity);
            targetgate.GetComponent<NXPDev_LoadAddrScene>().m_SceneAddressToLoad=TargetScene;
        }


        bool ConditionCheck(Collider other){
            #if UNITY_EDITOR
            if(!Application.isPlaying)
                return false;
            #endif
            //if(_LayerMask != (_LayerMask | (1 << other.gameObject.layer)))
            //    return false;
            if(!string.IsNullOrEmpty(_TagMask) && !other.gameObject.CompareTag(_TagMask))
                return false;

            return true;
        }
    }
}