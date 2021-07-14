using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;

public class NXBase_GameStarter : MonoBehaviour
{
    public static NXBase_GameStarter instance;
    public bool isGamestart { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isGamestart = false;
        NXEvent.StartListening("VRGamestart", GetListener);
    }

    void GetListener()
    {
        isGamestart = NXEvent.GetBool("VRGamestart");
        // Debug.Log("PlayerInput>> isGamestart: " + isGamestart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
