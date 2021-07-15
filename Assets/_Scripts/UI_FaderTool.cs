using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FaderTool : MonoBehaviour
{
    public Graphic UITarget;

    public void FadeIn(float duration){
        UITarget.DOFade(1f, duration);
    }
    public void FadeOut(float duration){
        UITarget.DOFade(0f, duration);
    }
}
