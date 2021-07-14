using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS.NXPlayer
{
    public class NXP_FollowPlayer : MonoBehaviour
    {
        // public bool FollowPosition;
        // Update is called once per frame
        void Update()
        {
            if(NXP_MainCtrl.LOCAL){
                transform.position = NXP_MainCtrl.LOCAL.transform.position;
            }
        }
    }

}