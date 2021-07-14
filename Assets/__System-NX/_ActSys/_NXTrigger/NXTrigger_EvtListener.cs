using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Ludiq;
using NAXS.Event;

[IncludeInSettings(true)]
[AddComponentMenu("_NXTrigger/NXTrigger_EvtListener")]
public class NXTrigger_EvtListener : NXTrigger
{
    //============================//
    //====Inspector & Variable====//
    //============================//
    public string EventName;

    //=======================//
    //====Method==========//
    //=======================//

    protected override void Start(){
        NXEvent.StartListening(EventName, OnTriggerStartBase);
    }

    //=======================//
    //====EDITOR Behavior====//
    //=======================//
}