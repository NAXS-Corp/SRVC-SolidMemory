using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFX_MaterialCtrl : AudioFX_CtrlBase
{
    public Material TargetMaterial;
    public Renderer TargetObject;
    public string ShaderProperty = "_Emmission";

    Material _FinalMaterial;
    // Start is called before the first frame update
    public override void StartChild()
    {
        if(TargetMaterial){
            _FinalMaterial = TargetMaterial;
        }else if(TargetObject){
            _FinalMaterial = TargetObject.material;
        }
    }

    public override void FXUpdate(float ctrlValue){
        _FinalMaterial.SetFloat(ShaderProperty, ctrlValue);
    }
}
