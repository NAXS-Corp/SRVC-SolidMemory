using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioFX_CtrlBase : MonoBehaviour
{
    [Header("Basics")]
    [Range(0, 1)]
    public float Threshold = 0;
    public Vector2 RangeRemap = Vector2.up;
    // Start is called before the first frame update
    void Start()
    {
        if(!AudioFX_Analyzer.singleton){
            gameObject.AddComponent<AudioFX_Analyzer>();
        }
        StartChild();
    }

    public virtual void StartChild(){

    }

    // Update is called once per frame
    void Update()
    {
        float newVal = AudioFX_Analyzer.singleton.outputValue;
        if(newVal < Threshold)  newVal = 0f;
        newVal = NXHelper_Math.Remap(newVal, 0, 1, RangeRemap.x, RangeRemap.y);
        FXUpdate(newVal);
    }

    public virtual void FXUpdate(float ctrlValue){

    }
}
