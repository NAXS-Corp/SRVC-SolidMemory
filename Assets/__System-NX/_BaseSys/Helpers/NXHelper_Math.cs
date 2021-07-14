using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NXHelper_Math
{
    public static float Lerp3(float a, float b, float c, float t)
    {
        if (t <= 0.5f)
        {
            return Mathf.Lerp(a, b, t * 2f);
        }
        else
        {
            return Mathf.Lerp(b, c, t);
        }
    }
 
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
