using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using NAXS.Base;

public abstract class NXTrigger_RangeBase : NXTrigger
{
    [FoldoutGroup("Range Detection")]
    public float TriggerRange = 5f;
    protected bool isInRange;

    [FoldoutGroup("Range Detection")]
    [ReadOnly]
    [Range(0, 1)]
    public float ApprochRatio;
    Transform localPlayer;

    protected override void Update()
    {
        bool lastIsInRange = isInRange;

        if(TriggerRange == 0){
            InRangeUpdate();
            return;
        }

        if(!localPlayer){
            var player = GameObject.FindGameObjectWithTag("LocalPlayer");
            if(player)
                localPlayer = player.transform;
            else
                return;
        }

        float PlayerDist = Vector3.Distance(transform.position, localPlayer.position);
        if (PlayerDist < TriggerRange)
        {
            isInRange = true;
            ApprochRatio = 1f - PlayerDist / TriggerRange;
            if (isInRange != lastIsInRange)
                OnEnterRange();
            InRangeUpdate();
        }
        else
        {
            isInRange = false;
            ApprochRatio = 0f;
            if (isInRange != lastIsInRange)
                OnExitRange();
        }
    }

    protected virtual void InRangeUpdate() {}
    protected virtual void OnEnterRange() {}
    protected virtual void OnExitRange() {}

    //=======================//
    //====EDITOR Behavior====//
    //=======================//
#if UNITY_EDITOR


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, TriggerRange);
    }

#endif
}
