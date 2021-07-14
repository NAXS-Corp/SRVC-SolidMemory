using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class FX_LightStrobe : MonoBehaviour {

	[Header("Settings")]
    #if UNITY_EDITOR
    public bool EditorPreview;
    #endif
    public bool RandomIntensity;
	public Vector2 IntensityMinmax = Vector2.up;
    public float StrobeTime = 0.1f;

	
    [Header("Runtime Ctrl")]
	[SerializeField, Range(0f, 1f)]private float MaxIntensityCtrl = 1f;
	[SerializeField, Range(0f, 1f)]private float StrobeSpeedCtrl = 0.5f;
	[SerializeField, Range(0f, 1f)]private float OnOffRatio = 0.5f;

    
	private float intensityOutput;
	private bool strobeDir;
	private float fixCounter;
	private float currentPeriod;
    float _maxIntensity;
    float _OnTime = 0.5f;
    float _OffTime = 0.5f;
	private Light target;


	// Use this for initialization
	void Start () {
		target = GetComponent<Light>();
	}
	
	
	// Update is called once per frame
	void Update () {
        #if UNITY_EDITOR
        if(!Application.isPlaying){
            if(!EditorPreview) return;
        }
        #endif
        if(RandomIntensity)
            _maxIntensity = Random.Range(IntensityMinmax.x, IntensityMinmax.y)* MaxIntensityCtrl;
        else
            _maxIntensity = IntensityMinmax.y * MaxIntensityCtrl;
			
        // Disable Strobe in highest speed
        if(StrobeSpeedCtrl > 0.99f){
            target.intensity = _maxIntensity;
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
                target.intensity = _maxIntensity;
                intensityOutput = IntensityMinmax.x;
            }else{
                fixCounter = _OffTime / StrobeSpeedCtrl / 50f;
                currentPeriod = _OffTime / StrobeSpeedCtrl / 50f;
                target.intensity = IntensityMinmax.x;
                intensityOutput = _maxIntensity;
            }
        }



	}

    
	// Control api for external scripts
	public void SetMaxIntensity (float value){
		IntensityMinmax.y = value;
	}

	public void SetStrobeSpeed (float value) {
		StrobeSpeedCtrl = value;
	}

}
