using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;

[System.Serializable]
[AddComponentMenu("_NXAction/Light/NXLightStrobe")]
public class NXAction_LightStrobe : NXAction_LightBase
{    
    [System.Flags]
    public enum StrobeFadeMode
    {
        none = 0 << 1,
        StrobeFade = 1 << 1, 
        IntensityFade = 1 << 2
    }

    [TitleGroup("Settings")]
    [HideLabel][EnumToggleButtons]
    public StrobeFadeMode FadeMode;
	public Vector2 IntensityMinmax = Vector2.up;
    public float StrobeTime = 0.1f;
    [HideIf("FadeMode", 0)]
    public float FadeTime = 3f;
	
    [FoldoutGroup("Runtime Ctrl")]
	[SerializeField, Range(0f, 1f)]private float MaxIntensityCtrl = 1f;
    [FoldoutGroup("Runtime Ctrl")]
	[SerializeField, Range(0f, 1f)]private float StrobeSpeedCtrl = 0.5f;
    [FoldoutGroup("Runtime Ctrl")]
	[SerializeField, Range(0f, 1f)]private float OnOffRatio = 0.5f;


    
	private float intensityOutput;
	private bool strobeDir;
	private float fixCounter;
	private float currentPeriod;
    float _maxIntensity;
    float _OnTime = 0.5f;
    float _OffTime = 0.5f;


    //Override default methods
    public override void StartAction(){
        if(FadeMode.HasFlag(StrobeFadeMode.IntensityFade))
        {
            var target = MaxIntensityCtrl;
            MaxIntensityCtrl = 0;
            DOTween.To(()=> MaxIntensityCtrl, x=> MaxIntensityCtrl = x, target, FadeTime);
        }
        if(FadeMode.HasFlag(StrobeFadeMode.StrobeFade))
        {
            var target = StrobeSpeedCtrl;
            StrobeSpeedCtrl = 1f;
            DOTween.To(()=> StrobeSpeedCtrl, x=> StrobeSpeedCtrl = x, target, FadeTime);
        }
    }
    public override void StopAction(){

    }
	public override void UpdateAction() {
        _maxIntensity = IntensityMinmax.y * MaxIntensityCtrl;

        // Disable Strobe in highest speed
        if(StrobeSpeedCtrl > 0.99f){
            TargetLight.intensity = _maxIntensity;
            return;
        }

        _OnTime = StrobeTime* OnOffRatio;
        _OffTime = StrobeTime* (1 - OnOffRatio);

        fixCounter -= Time.deltaTime;
        if(fixCounter < 0){
            strobeDir = !strobeDir;
            if(strobeDir){
                fixCounter = _OnTime / StrobeSpeedCtrl / 50f;
                currentPeriod = _OnTime / StrobeSpeedCtrl / 50f;
                TargetLight.intensity = _maxIntensity;
                intensityOutput = IntensityMinmax.x;
            }else{
                fixCounter = _OffTime / StrobeSpeedCtrl / 50f;
                currentPeriod = _OffTime / StrobeSpeedCtrl / 50f;
                TargetLight.intensity = IntensityMinmax.x;
                intensityOutput = _maxIntensity;
            }
        }
	}

    
	// Runtime control api for external scripts
	public void SetMaxIntensity (float value){
		IntensityMinmax.y = value;
	}

	public void SetStrobeSpeed (float value) {
		StrobeSpeedCtrl = value;
	}
}
