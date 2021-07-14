using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FX_LightFader : MonoBehaviour
{
    public Light _Light;
    public bool AutoStart = true;
    public bool Loop = true;
    public Vector2 IntensityRange = Vector2.up;
    public float FadingTime = 5f;
    public Ease easeMode = Ease.InOutSine;


    bool fadeDir;

    #if UNITY_EDITOR
    void Reset()
    {
        _Light = GetComponent<Light>();
    }
    #endif

    void Start()
    {
        if(AutoStart) FadeIn();
        
    }

    public void FadeIn(){
        fadeDir = true;
        _Light.intensity = IntensityRange.x;
        _Light.DOIntensity(IntensityRange.y, FadingTime).SetEase(easeMode).OnComplete(CheckLoop);
    }

    public void FadeOut(){
        fadeDir = false;
        _Light.intensity = IntensityRange.y;
        _Light.DOIntensity(IntensityRange.x, FadingTime).SetEase(easeMode).OnComplete(CheckLoop);
    }

    void CheckLoop()
    {
        if(Loop)
        {
            fadeDir = !fadeDir;
            if(fadeDir)
                FadeIn();
            else
                FadeOut();
        }
    }

    public void FadeLoop(bool dir)
    {
        
    }

    public void FadeTo(float from, float to, float time){
        _Light.intensity = from;
        _Light.DOIntensity(to, time).SetEase(easeMode);
    }
}
