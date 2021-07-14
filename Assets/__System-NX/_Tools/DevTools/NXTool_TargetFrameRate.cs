using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXTool_TargetFrameRate : MonoBehaviour
{
    #if UNITY_EDITOR
    public int EditorFrameRate = -1;
    #endif
    
    void Start()
    {
        #if UNITY_EDITOR
        Application.targetFrameRate = EditorFrameRate;
        #endif
    }
}
