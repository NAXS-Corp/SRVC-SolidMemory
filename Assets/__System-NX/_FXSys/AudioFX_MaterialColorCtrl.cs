using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFX_MaterialColorCtrl : AudioFX_CtrlBase
{
    
    [Header("Materials")]
    public Material TargetMaterial;
    public Renderer TargetObject;
    public string ShaderProperty = "_Emmission";

    public Color MinColor;
    public Color MaxColor = Color.white;

    Material _FinalMaterial;

    public override void StartChild()
    {
        if(TargetMaterial){
            _FinalMaterial = TargetMaterial;
        }else if(TargetObject){
            _FinalMaterial = TargetObject.material;
        }
    }

    public override void FXUpdate(float ctrlValue){
        _FinalMaterial.SetColor(ShaderProperty, Color.Lerp(MinColor, MaxColor, ctrlValue));
    }
}
