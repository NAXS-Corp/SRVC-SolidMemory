using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity​Engine.Rendering;
using Sirenix.OdinInspector;
using DG.Tweening;

[ExecuteInEditMode]
public class FX_PPChanger : MonoBehaviour
{
    public Volume TargetVolume;
    public VolumeProfile Profile;

    [Header("Resources Search")]
    public string ResourcesPath = "0Selves_PP";

    // Start is called before the first frame update

    [Button("Test")]
    void Start()
    {
        if (Profile)
        {
            TargetVolume.profile = Profile;
        }
        else
        {
            Profile = Resources.Load<VolumeProfile>(ResourcesPath);
            if(Profile)
                TargetVolume.profile = Profile;
        }
    }

}
