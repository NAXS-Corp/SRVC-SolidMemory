using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


[System.Serializable]
[System.Flags]
public enum NXLightOptions
{
    Intensity = 1 << 1,
    Color = 1 << 2,
    Rotation = 1 << 3,
    Fade = 1 << 4,
}

[System.Serializable]
public class LightData{
    public float Intensity;
    public Color Color;
    [HideInInspector]public Quaternion Rotation;
    public float FadeTime;

    //Constructor
    public LightData()
    {
        Intensity = 1;
        Color = Color.white;
        Rotation = Quaternion.identity;
        FadeTime = 3f;
    }
    
    //Construct LightData from existing light
    public LightData(Light light, float fadeTime)
    {
        Intensity = light.intensity;
        Color = light.color;
        Rotation = light.transform.rotation;
        FadeTime = fadeTime;
    }
}

public abstract class NXAction_LightBase : NXAction
{
    // [Header("Light Base")]
    // [PropertyOrder(3)]
    [Required]
    public Light TargetLight;
	// public Vector2 IntensityMinmax = Vector2.up;
    // public Vector2 FadeTime = new Vector2(0, 0);

    #if UNITY_EDITOR
	public override void EditorReset()
    {
        if(TargetLight) return;
        Light light;
        if(light = GetComponent<Light>()){
		    TargetLight = light;
        }
        EditorLightSetup();
    }
    
	public virtual void EditorLightSetup()
    {
    }
    
    #endif
}
