using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NAXS;
using Sirenix.OdinInspector;

// public class AnimParam{
//     public string name;
//     private int _nameHash = -1;
//     public bool boolState;
//     public float floatState;
//     public AnimatorControllerParameterType type;
//     public int nameHash{
//         get{
//             if(_nameHash != -1) return _nameHash;
//             _nameHash = Animator.StringToHash(this.name);
//             return _nameHash;
//         }
//     }
// }

[System.Serializable]
public class MinimalParam
{
    public string name;
    private int _nameHash = -1;
    public bool boolState = false;
    public int nameHash{
        get{
            if(_nameHash != -1) return _nameHash;
            _nameHash = Animator.StringToHash(this.name);
            return _nameHash;
        }
    }

    public MinimalParam (string _name){
        name = _name;
    }

    public bool isEqual(MinimalParam param){
        return param.boolState == boolState;
    }
}

public class MirrorSys_MinimalAnimator : NetworkBehaviour
{
    // ==================================
    public Animator m_Animator;
    public MinimalParam[] MinimalParams = new MinimalParam[]{
        new MinimalParam("IsGrounded"), new MinimalParam("IsMoving"), new MinimalParam("IsRunning"), new MinimalParam("CustomAnim0"), new MinimalParam("CustomAnim1"), new MinimalParam("CustomAnim2"), new MinimalParam("CustomAnim3"), new MinimalParam("CustomAnim4")
    };

    // MinimalParam[] LastMinimalParams;
    bool[] LastBoolParams;

    private bool IsParamChanged(MinimalParam[] paramA, MinimalParam[] paramB){
        for(int i = 0; i < paramA.Length; i++){
            if(paramA[i].boolState != paramB[i].boolState)
                return true;
        }
        return false;
    }

    // ==================================
    [SyncVar(hook = nameof(SyncParam0))] public bool BoolParam0;
    [SyncVar(hook = nameof(SyncParam1))] public bool BoolParam1;
    [SyncVar(hook = nameof(SyncParam2))] public bool BoolParam2;
    [SyncVar(hook = nameof(SyncParam3))] public bool BoolParam3;
    [SyncVar(hook = nameof(SyncParam4))] public bool BoolParam4;
    [SyncVar(hook = nameof(SyncParam5))] public bool BoolParam5;
    [SyncVar(hook = nameof(SyncParam6))] public bool BoolParam6;
    [SyncVar(hook = nameof(SyncParam7))] public bool BoolParam7;
    
    float nextSendTime;

    // ==================================

    void Start()
    {
        // LastMinimalParams = MinimalParams;
        LastBoolParams = new bool[MinimalParams.Length];
        SyncLastBoolParams();
    }

    void FixedUpdate()
    {
        if(!m_Animator) return;
        

        if(isLocalPlayer){
            UpdateLocalPlayer();
        }
    }
    
    void UpdateLocalPlayer()
    {
        float now = Time.time;
        if (syncInterval >= 0 && now > nextSendTime){
            nextSendTime = now + syncInterval;
        }else{
            return;
        }

        foreach(MinimalParam param in MinimalParams){
            param.boolState = m_Animator.GetBool(param.nameHash);
        }

        if(IsParamsChanged()){
            CmdUpdateSyncVarOnServer(MinimalParams[0].boolState, MinimalParams[1].boolState, MinimalParams[2].boolState, MinimalParams[3].boolState, MinimalParams[4].boolState, MinimalParams[5].boolState, MinimalParams[6].boolState, MinimalParams[7].boolState);
        }
        SyncLastBoolParams();
    }

    bool IsParamsChanged(){
        for(int i = 0; i < MinimalParams.Length; i++){
            if(MinimalParams[i].boolState != LastBoolParams[i])
                return true;
        }
        return false;
    }

    void SyncLastBoolParams(){
        for(int i = 0; i < MinimalParams.Length; i++){
            LastBoolParams[i] = MinimalParams[i].boolState;
        }
    }
    
    [Command]
    void CmdUpdateSyncVarOnServer(bool _BoolParam0, bool _BoolParam1, bool _BoolParam2, bool _BoolParam3, bool _BoolParam4, bool _BoolParam5, bool _BoolParam6, bool _BoolParam7){
        // NXDebug.Log("CmdUpdateSyncVarOnServer "+_BoolParam0.ToString());
        BoolParam0 = _BoolParam0;
        BoolParam1 = _BoolParam1;
        BoolParam2 = _BoolParam2;
        BoolParam3 = _BoolParam3;
        BoolParam4 = _BoolParam4;
        BoolParam5 = _BoolParam5;
        BoolParam6 = _BoolParam6;
        BoolParam7 = _BoolParam7;
    }
    

    void SyncParam0(bool oldBool, bool newBool){
        // NXDebug.Log("SyncParam0 "+newBool.ToString());
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[0].nameHash, newBool);
    }
    void SyncParam1(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[1].nameHash, newBool);
    }
    void SyncParam2(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[2].nameHash, newBool);
    }
    void SyncParam3(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[3].nameHash, newBool);
    }
    void SyncParam4(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[4].nameHash, newBool);
    }
    void SyncParam5(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[5].nameHash, newBool);
    }
    void SyncParam6(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[6].nameHash, newBool);
    }
    void SyncParam7(bool oldBool, bool newBool){
        if(m_Animator)
            m_Animator.SetBool(MinimalParams[7].nameHash, newBool);
    }

    // Animator Events
    public void SendAnimatorEvent(int idx){
        if(!isLocalPlayer) return;
        CmdSendAnimatorEvent(idx);
    }

    [Command]
    void CmdSendAnimatorEvent(int idx){
        RpcAnimatorEvent(idx);
    }


    [ClientRpc]
    public void RpcAnimatorEvent(int idx)
    {
        OnReceiveAnimatorEvent(idx);
    }
    
    public void OnReceiveAnimatorEvent(int idx){
        string evtName = string.Concat("Event", idx.ToString());
        m_Animator.SetTrigger(evtName);
    }



    //DEBUG
    void OnDisable()
    {
        // NXDebug.Log("OnDisable");
    }
}
