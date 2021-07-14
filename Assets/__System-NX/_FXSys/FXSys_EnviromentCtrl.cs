using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class FXSys_EnviromentCtrl : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool editorPreview = true;
    #endif

    public bool AlwaysUpdate = true;
    public bool UseSlider = true;
    [HideIf("UseSlider")]
    public float _FogDensity;
    [ShowIf("UseSlider")]
    [Range(0, 1)]public float _FogDensitySlider;
    [ShowIf("UseSlider")]
    public float MaxFogDensity = 0.01f;
	public float FogDensity {
		get {
			return RenderSettings.fogDensity;
		}
		set{
			RenderSettings.fogDensity = value;
		}
	}
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
		#if UNITY_EDITOR
		if(editorPreview && !Application.isPlaying){
            if(UseSlider) _FogDensity = _FogDensitySlider * MaxFogDensity;
            RenderSettings.fogDensity = _FogDensity;
            return;
		}        
		#endif

        if(AlwaysUpdate){
            if(UseSlider) _FogDensity = _FogDensitySlider * MaxFogDensity;
            RenderSettings.fogDensity = _FogDensity;
        }
    }

    public void ToggleFog(){
        RenderSettings.fog = !RenderSettings.fog;
    }
    
	public void SetFogDensity(float newDensity){
		RenderSettings.fogDensity = newDensity;
	}

    
    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

}

