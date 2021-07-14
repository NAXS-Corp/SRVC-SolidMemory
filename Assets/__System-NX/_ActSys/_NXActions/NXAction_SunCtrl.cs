using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
using NAXS.Base;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Flags]
public enum NXSunOptions
{
    Rotation = 1 << 1,
    Intensity = 1 << 2,
    Color = 1 << 3,
    Fade = 1 << 4,
}

[System.Serializable]
[AddComponentMenu("_NXAction/Light/NXSunCtrl")]
[ExecuteInEditMode]
public class NXAction_SunCtrl : NXAction_LightBase
{
    public static NXAction_SunCtrl Current;

    [PropertyOrder(999)]
    [FoldoutGroup("Advance")]
    public bool EditorPreview = true;

    [Title("Ctrl Options")]
    [HideLabel]
    [EnumToggleButtons] public NXSunOptions CtrlOptions = NXSunOptions.Rotation | NXSunOptions.Intensity | NXSunOptions.Color | NXSunOptions.Fade;

    [Title("Target Values")]
    public float TargetIntensity = 1;
    public Color TargetColor = Color.white;
    public Vector2 FadeTime = new Vector2(12f, 12f);
    public bool AutoFadeInOut;

    float lastIntensity;
    Color lastColor;
    Quaternion lastRotation;

    // private bool startFade;

    //=======================//
    //Override default methods
    //=======================//


    public override void StartAction()
    {
        if(AutoFadeInOut)
            RecordSetting();
        NXAction_SunCtrl.Current = this;
        if (CtrlOptions.HasFlag(NXSunOptions.Fade))
            RunFade(true);
        else
            DirectSet();
    }

    public override void StopAction()
    {
        NXAction_SunCtrl.Current = null;

    }

    public override void UpdateAction()
    {
    }


    //=======================//
    //Private methods
    //=======================//

    void RecordSetting()
    {
        lastIntensity = TargetLight.intensity;
        lastColor = TargetLight.color;
        lastRotation = TargetLight.transform.rotation;
    }

    void RunFade(bool isFadeIn)
    {
        isFadeInCompleted = false;
        isFadeOutCompleted = false;
        float _fadeTime = FadeTime.x;
        float _targetItensity = TargetIntensity;
        Color _targetColor = TargetColor;
        Quaternion _targetRotation = transform.rotation;

        if(!isFadeIn){
            _fadeTime = FadeTime.y;
            _targetItensity = lastIntensity;
            _targetColor = lastColor;
            _targetRotation = lastRotation;
        }

        // startFade = true;
        DebugLog("AutoFade " + gameObject.name);
        DOTween.Kill(TargetLight.transform);
        DOTween.Kill(TargetLight);
        DOTween.Kill(TargetLight.intensity);
        DOTween.Kill(TargetLight.transform.rotation);
        DOTween.Kill(TargetLight.color);
        if (CtrlOptions.HasFlag(NXSunOptions.Intensity))
            TargetLight.DOIntensity(_targetItensity, _fadeTime).OnComplete(()=>OnFadeComplete(isFadeIn));
        if (CtrlOptions.HasFlag(NXSunOptions.Rotation))
            TargetLight.transform.DORotateQuaternion(_targetRotation, _fadeTime).OnComplete(()=>OnFadeComplete(isFadeIn));
        if (CtrlOptions.HasFlag(NXSunOptions.Color))
            TargetLight.DOColor(_targetColor, _fadeTime).OnComplete(()=>OnFadeComplete(isFadeIn));
    }
    
    bool isFadeInCompleted;
    bool isFadeOutCompleted;
    void OnFadeInComplete()
    {

    }
    void OnFadeComplete(bool isFadeIn)
    {
        if(isFadeIn)
        {
            if(isFadeInCompleted) return;
            if(AutoFadeInOut) RunFade(false);
            isFadeInCompleted = true;
        }
        else
        {
            if(isFadeOutCompleted) return;
            isFadeOutCompleted = true;
        }
    }

    void DirectSet()
    {
        // DebugLog("Direct Mode");
        if (CtrlOptions.HasFlag(NXSunOptions.Intensity))
            TargetLight.intensity = TargetIntensity;
        if (CtrlOptions.HasFlag(NXSunOptions.Rotation))
            TargetLight.transform.rotation = transform.rotation;
        if (CtrlOptions.HasFlag(NXSunOptions.Color))
            TargetLight.color = TargetColor;
    }

    //=======================//
    //====EDITOR Behavior====//
    //=======================//
#if UNITY_EDITOR

    void FindGlobalSunLight()
    {
        if (TargetLight) return;
        Light sunLight;
        if (sunLight = (Light)NXBase_ObjManager.FetchObject("SunLight"))
        {
            TargetLight = sunLight;
        }
    }


    public override void OnActivateEditorChild()
    {
        NXAction_SunCtrl.Current = this;
    }

    public override void OnDeactivateEditorChild()
    {
        NXAction_SunCtrl.Current = null;
    }

    public override void EditorReset()
    {
        FindGlobalSunLight();
    }

    public override void EditorUpdate()
    {
        if (NXAction_SunCtrl.Current != this) return;
        if (!EditorPreview) return;

        // Experimental: Fade in editor
        // if(startFade) return;

        if (TargetLight)
            DirectSet();

    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = TargetColor;
        // Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.DrawLine(transform.position - transform.forward, transform.position + transform.forward * 4f);
        Handles.color = TargetColor;
        Handles.DrawWireDisc(transform.position, transform.forward, 1f);
    }

#endif

}
