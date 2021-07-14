using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using NAXS.NXPlayer;

[AddComponentMenu("_NXAction/Camera/VCamFOV")]
public class NXAction_CinemachineFov : NXAction_CinemachineBase
{
    public static NXAction_CinemachineFov CurrentAction;

    [EnumToggleButtons]
    public NXActionOptions CtrlOptions;
    public float FadeTime = 5;
    [Range(5, 180)]public float FOV = 80;
    public static float lastFOV;
    public bool UseNXPFov = true;


    public override void StartAction(){
        if(UseNXPFov && NXP_FOVCtrl.singleton)
        {
            Start_NXP();
            return;
        }
        NXAction_CinemachineFov.lastFOV = ActiveVirtualCamera.m_Lens.FieldOfView;
        if(CtrlOptions.HasFlag(NXActionOptions.Fade))
            DOTween.To(()=> ActiveVirtualCamera.m_Lens.FieldOfView, x=> ActiveVirtualCamera.m_Lens.FieldOfView = x, FOV, FadeTime);
        else
            ActiveVirtualCamera.m_Lens.FieldOfView = FOV;

    }

    public override void StopAction(){
        if(UseNXPFov && NXP_FOVCtrl.singleton)
        {
            Stop_NXP();
            return;
        }
        if(CtrlOptions.HasFlag(NXActionOptions.Revertable))
        {
            if(CtrlOptions.HasFlag(NXActionOptions.Fade))
                DOTween.To(()=> ActiveVirtualCamera.m_Lens.FieldOfView, x=> ActiveVirtualCamera.m_Lens.FieldOfView = x, NXAction_CinemachineFov.lastFOV, FadeTime);
            else
                ActiveVirtualCamera.m_Lens.FieldOfView = NXAction_CinemachineFov.lastFOV;
        }
    }
    void Start_NXP(){
        NXAction_CinemachineFov.lastFOV = NXP_FOVCtrl.singleton.FOVBase;
        if(CtrlOptions.HasFlag(NXActionOptions.Fade))
            DOTween.To(()=> NXP_FOVCtrl.singleton.FOVBase, x=> NXP_FOVCtrl.singleton.FOVBase = x, FOV, FadeTime);
        else
            NXP_FOVCtrl.singleton.FOVBase= FOV;

    }

    void Stop_NXP(){
        if(CtrlOptions.HasFlag(NXActionOptions.Revertable))
        {
            if(CtrlOptions.HasFlag(NXActionOptions.Fade))
                DOTween.To(()=> NXP_FOVCtrl.singleton.FOVBase, x=> NXP_FOVCtrl.singleton.FOVBase = x, NXAction_CinemachineFov.lastFOV, FadeTime);
            else
                NXP_FOVCtrl.singleton.FOVBase = NXAction_CinemachineFov.lastFOV;
        }
    }
    //=======================//
    //====EDITOR Behavior====//
    //=======================//
    #if UNITY_EDITOR

    public override void OnActivateEditorChild()
    {
        NXAction_CinemachineFov.CurrentAction = this;
    }
    
    public override void OnDeactivateEditorChild()
    {
        NXAction_CinemachineFov.CurrentAction = null;
    }

    public override void EditorReset()
    {
        // FindGlobalSunLight();
    }

    public override void EditorUpdate()
    {
        // if(NXAction_SunCtrl.Current != this) return;
        // if(!EditorPreview) return;

        // // Experimental: Fade in editor
        // // if(startFade) return;

        // if(TargetLight)
        //     DirectSet();
        
    }
    
    void OnDrawGizmosSelected()
    {
        // // Draw a yellow sphere at the transform's position
        // Gizmos.color = TargetColor;
        // // Gizmos.DrawWireSphere(transform.position, 1);
        // Gizmos.DrawLine(transform.position - transform.forward, transform.position + transform.forward * 4f);
        // Handles.color = TargetColor;
        // Handles.DrawWireDisc(transform.position, transform.forward, 1f);
    }
    
    #endif
}
