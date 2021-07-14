using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
[AddComponentMenu("_NXAction/Enviroment/NXLightSkybox")]
[ExecuteInEditMode]
public class NXAction_Skybox : NXAction
{   

    [Title("Ctrl Options")][HideLabel]
    [EnumToggleButtons]public NXActionOptions CtrlOptions = NXActionOptions.Fade;
    // public bool RevertLightOnStop = true;

    public Material SkyMaterial;
    public float FadeTime = 3;
    Material lastMat;
    public static float FadeValue;

    //=======================//
    //Override default methods
    //=======================//

    public override void StartAction(){
        if(CtrlOptions.HasFlag(NXActionOptions.Revertable))
            lastMat = RenderSettings.skybox;
        SetSky(SkyMaterial, CtrlOptions.HasFlag(NXActionOptions.Fade));
    }

    public override void StopAction(){
        if(CtrlOptions.HasFlag(NXActionOptions.Revertable))
            SetSky(lastMat, CtrlOptions.HasFlag(NXActionOptions.Fade));
    }


    //=======================//
    //Private methods
    //=======================//
    void DirectSet(Material _mat){
        RenderSettings.skybox = _mat;
    }

    void RunFade(Material _mat){
        DOTween.Kill(NXAction_Skybox.FadeValue);
        // DOTween.To(()=> NXAction_Skybox.FadeValue, x=> NXAction_Skybox.FadeValue = x, 1, FadeTime);
    }

    void OnFadeComplete(){
        // stateFading = false;
    }
    

    void SetSky(Material _mat, bool useFade){
        if(useFade) 
            RunFade(_mat);
        else
            DirectSet(_mat);
    }


    //=======================//
    //====EDITOR Behavior====//
    //=======================//
    #if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        // Gizmos.color = TargetSetting.Color;
        // // Gizmos.DrawWireSphere(transform.position, 1);
        // Gizmos.DrawLine(transform.position - transform.forward, transform.position + transform.forward * 4f);
        // Handles.color = TargetSetting.Color;
        // Handles.DrawWireDisc(transform.position, transform.forward, 1f);
        // Handles.DrawWireDisc(transform.position, transform.up, 1f);
        // Handles.DrawWireDisc(transform.position, transform.right, 1f);
    }
    
    #endif

}
