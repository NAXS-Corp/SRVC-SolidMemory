using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FX_AudioFader : MonoBehaviour
{
    public AudioSource audioSource;
    public bool AutoStartFadeIn;
    public bool AutoStartFadeOut;
    public float FadeInTime;
    public float FadeOutTime;

    
    // Start is called before the first frame update
    void Start()
    {
        if(AutoStartFadeIn)
            FadeIn(FadeInTime);  
            
        if(AutoStartFadeOut)
            FadeIn(FadeOutTime);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeIn(float time){
        audioSource.Play();
        audioSource.volume = 0;
        audioSource.DOFade(1, time);
    }
    public void FadeOut(float time){
        audioSource.DOFade(0, time);
    }
}
