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
[AddComponentMenu("_NXAction/Light/NXLightBasic")]
[ExecuteInEditMode]
public class NXAction_LightBasic : NXAction_LightBase
{   

    [Title("Ctrl Options")][HideLabel]
    [EnumToggleButtons]public NXLightOptions CtrlOptions = NXLightOptions.Intensity | NXLightOptions.Color | NXLightOptions.Fade;
    public bool RevertLightOnStop = true;
    bool useFade => CtrlOptions.HasFlag(NXLightOptions.Fade) ? true : false;

    [Title("Target Values")]
    public LightData TargetSetting = new LightData();
    private LightData LastLightData;

    // private bool stateFading = false;

    //=======================//
    //Override default methods
    //=======================//

    public override void StartAction(){
        if(RevertLightOnStop)
            LastLightData = new LightData(TargetLight, TargetSetting.FadeTime);
        TargetSetting.Rotation = transform.rotation;
        SetLight(TargetSetting);
    }

    public override void StopAction(){
        if(RevertLightOnStop)
            SetLight(LastLightData);
    }


    //=======================//
    //Private methods
    //=======================//
    void DirectSet(LightData lightData){
        if(CtrlOptions.HasFlag(NXLightOptions.Intensity))
            TargetLight.intensity = lightData.Intensity;
        if(CtrlOptions.HasFlag(NXLightOptions.Rotation))
            TargetLight.transform.rotation = lightData.Rotation;
        if(CtrlOptions.HasFlag(NXLightOptions.Color))
            TargetLight.color = lightData.Color;
    }

    void RunFade(LightData lightData){
        DOTween.Kill(TargetLight.intensity);
        DOTween.Kill(TargetLight.color);
        DOTween.Kill(TargetLight.transform.rotation);
        // stateFading = true;

        if(CtrlOptions.HasFlag(NXLightOptions.Intensity))
            TargetLight.DOIntensity(lightData.Intensity, lightData.FadeTime).OnComplete(OnFadeComplete);
        if(CtrlOptions.HasFlag(NXLightOptions.Rotation))
            TargetLight.transform.DORotateQuaternion(lightData.Rotation, lightData.FadeTime).OnComplete(OnFadeComplete);
        if(CtrlOptions.HasFlag(NXLightOptions.Color))
            TargetLight.DOColor(lightData.Color, lightData.FadeTime).OnComplete(OnFadeComplete);
    }

    void OnFadeComplete(){
        // stateFading = false;
    }
    

    void SetLight(LightData lightData){
        if(useFade) 
            RunFade(lightData);
        else
            DirectSet(lightData);
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
        Handles.color = TargetSetting.Color;
        Handles.DrawWireDisc(transform.position, transform.forward, 1f);
        Handles.DrawWireDisc(transform.position, transform.up, 1f);
        Handles.DrawWireDisc(transform.position, transform.right, 1f);
    }
    
    #endif

}
