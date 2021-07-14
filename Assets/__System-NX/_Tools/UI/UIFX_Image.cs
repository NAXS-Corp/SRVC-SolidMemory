using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFX_Image : MonoBehaviour
{
    public Image TargetImage;
    public Color ColorFadein;
    public Color ColorNormal;
    public float FadeTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        TargetImage.color = ColorFadein;
        TargetImage.CrossFadeColor(ColorNormal, FadeTime, true, true);
        // TargetImage.DOColor(ColorNormal, FadeTime);
        // DOTween.To(()=> TargetImage.color, x=> TargetImage.color = x, ColorNormal, FadeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
