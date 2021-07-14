using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;

public class NXVRPos_Follow : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        NXEvent.StartListening("VRPosFollow", SetupVRPlayerCamPos);
    }

    void SetupVRPlayerCamPos()
    {
        // this.GetComponent<NXTool_FollowTransform>().target = GameObject.Find("VRPos").transform;
        this.GetComponent<NXTool_FollowTransform>().target = (Transform)NXEvent.GetData("VRPosFollow");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
