using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer{
    public class NXP_PlayerTransport : MonoBehaviour
    {
        public bool AutoStart;
        public Transform TransportTarget;
        
        void Start()
        {
            if(AutoStart){
                Transport();
            }
        }

        public void Transport(){
            if(NXP_MainCtrl.LOCAL){
                NXP_MainCtrl.LOCAL.TransportTransform(TransportTarget);
            }
        }
    }
}