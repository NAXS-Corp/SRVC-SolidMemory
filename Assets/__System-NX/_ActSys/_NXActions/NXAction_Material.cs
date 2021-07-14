using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

[System.Serializable]
public enum MatPropertyType
{
    Float,
    Color
}

[System.Serializable]
public class MaterialParamater
{
    public string Property = "_BaseColor";
    [EnumToggleButtons] public MatPropertyType PropType = MatPropertyType.Color;
    [ShowIf("PropType", MatPropertyType.Float)] public float Value;
    [HideIf("PropType", MatPropertyType.Float)] public Color Color;
}

[System.Serializable]
[AddComponentMenu("_NXAction/Material/NXMaterial")]
public class NXAction_Material : NXAction
{
    [Title("Ctrl Options")]
    [EnumToggleButtons] 
    public NXActionOptions CtrlOptions = NXActionOptions.Fade;

    [HideIf("SharedMaterial")]
    [LabelText("Renderer")]
    public Renderer SingleRenderer;

    [HideIf("SingleRenderer")]
    [LabelText("Global Material")]
    public Material SharedMaterial;
    public MaterialParamater TargetParamater;

    public Vector2 FadeTime = new Vector2(3, 3);
    MaterialParamater lastMatParam;
    Material targetMaterial
    {
        get
        {
            if (SingleRenderer)
                return SingleRenderer.material;
            else
                return SharedMaterial;
        }
    }

    public bool AutoFadeInOut;
    // bool actionExeDir;
    // bool isFadeInCompleted = false;
    // bool isFadeOutCompleted = false;

    //=======================//
    //Override default methods
    //=======================//

    public override void StartAction()
    {
        Debug.Log("StartAction");
        if (CtrlOptions.HasFlag(NXActionOptions.Revertable) || AutoFadeInOut)
            RecordParam();
        SetMaterial(TargetParamater, true);
    }

    public override void StopAction()
    {
        Debug.Log("StopAction");
        // StopAction
        if (CtrlOptions.HasFlag(NXActionOptions.Revertable) || AutoFadeInOut){
            SetMaterial(lastMatParam, false);
        }
    }


    //=======================//
    //Private methods
    //=======================//
    void RecordParam()
    {
        lastMatParam = TargetParamater;
        if (lastMatParam.PropType == MatPropertyType.Float)
            lastMatParam.Value = targetMaterial.GetFloat(lastMatParam.Property);
        if (lastMatParam.PropType == MatPropertyType.Color)
            lastMatParam.Color = targetMaterial.GetColor(lastMatParam.Property);
    }

    void DirectSet(MaterialParamater _param)
    {
        if (TargetParamater.PropType == MatPropertyType.Float)
            targetMaterial.SetFloat(TargetParamater.Property, TargetParamater.Value);
        if (TargetParamater.PropType == MatPropertyType.Color)
            targetMaterial.SetColor(TargetParamater.Property, TargetParamater.Color);
    }

    void StartFading(MaterialParamater _param, bool isFadeIn)
    {
        float fadeTime = FadeTime.x;
        if(!isFadeIn)
            fadeTime = FadeTime.y;
            
        DOTween.Kill(targetMaterial);
        if (TargetParamater.PropType == MatPropertyType.Float)
            targetMaterial.DOFloat(TargetParamater.Value, TargetParamater.Property, fadeTime).OnComplete(()=>OnFadeComplete(isFadeIn));
        if (TargetParamater.PropType == MatPropertyType.Color)
            targetMaterial.DOColor(TargetParamater.Color, TargetParamater.Property, fadeTime).OnComplete(()=>OnFadeComplete(isFadeIn));
    }

    void OnFadeComplete(bool isFadeIn)
    {
        if(AutoFadeInOut && isFadeIn){
            StopActionBase();
        }
    }


    void SetMaterial(MaterialParamater _param, bool _actionDir)
    {
        if (CtrlOptions.HasFlag(NXActionOptions.Fade))
            StartFading(_param, _actionDir);
        else
            DirectSet(_param);
    }

}
