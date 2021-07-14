using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Ludiq;

[IncludeInSettings(true)]
public abstract class NXTrigger_ZoneBase : NXTrigger
{

    //============================//
    //====Inspector & Variable====//
    //============================//

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
        DebugLog("OnTriggerEnter "+other.gameObject.name);
        OnTriggerStartBase();
    }

    
    void OnTriggerExit(Collider other)
    {
        if(!ConditionCheck(other)) return;
        DebugLog("OnTriggerExit "+other.gameObject.name);
        OnTriggerStopBase();
    }


    bool ConditionCheck(Collider other){
        #if UNITY_EDITOR
        if(!Application.isPlaying)
            return false;
        #endif
        if(_LayerMask != (_LayerMask | (1 << other.gameObject.layer)))
            return false;
        if(!string.IsNullOrEmpty(_TagMask) && !other.gameObject.CompareTag(_TagMask))
            return false;

        return true;
    }


    //=======================//
    //====EDITOR Behavior====//
    //=======================//
    #if UNITY_EDITOR
    protected override void OnEditorReset()
    {
        SetCollider();
    }

    protected override void OnEditorValidateBase()
    {
        SetCollider();
    }

    void SetCollider(){
        Collider collider;
        if(collider = GetComponent<Collider>()){
            m_Collider = collider;
            m_Collider.isTrigger = true;
        }
    }

    [Button][HideIf("m_Collider")]
    [GUIColor(0.1f, 0.85f, 0.25f)]
    void AddBoxCollider(){
        gameObject.AddComponent<BoxCollider>();
        SetCollider();
    }
    [Button][HideIf("m_Collider")]
    [GUIColor(0.1f, 0.85f, 0.25f)]
    void AddSphereCollider(){
        gameObject.AddComponent<SphereCollider>();
        SetCollider();
    }
    #endif
}