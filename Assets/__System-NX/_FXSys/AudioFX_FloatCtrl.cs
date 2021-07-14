using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.NXHelper;

public class AudioFX_FloatCtrl : MonoBehaviour
{
    public Vector2 RangeRemap = Vector2.up;
    [Range(0, 1)]
    public float Threshold = 0;
    public FloatEvent FloatCtrl;

    // Start is called before the first frame update
    void Start()
    {
        if(!AudioFX_Analyzer.singleton){
            gameObject.AddComponent<AudioFX_Analyzer>();
        }    
    }

    // Update is called once per frame
    void Update()
    {
        float newVal = AudioFX_Analyzer.singleton.outputValue;
        if(newVal < Threshold)  newVal = 0f;
        newVal = NXHelper_Math.Remap(newVal, 0, 1, RangeRemap.x, RangeRemap.y);
        FloatCtrl.Invoke(newVal);
    }
}
