using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class NXTool_AutoRotate : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool EditorPreview;
    #endif
    private float _rotSpeed = 1f;
    
    public float RotationSpeed{
        get{
            return _rotSpeed;
        }
        set{
            _rotSpeed = value;
        }
    }
    public Vector3andSpace rotateDegreesPerSecond;
    
    // Update is called once per frame
    private void Update()
    {
        
        #if UNITY_EDITOR
        if(!Application.isPlaying){
            if(!EditorPreview)  return;
        }
        #endif
        float deltaTime = Time.deltaTime;
        transform.Rotate(rotateDegreesPerSecond.value*deltaTime*_rotSpeed, rotateDegreesPerSecond.space);
    }


    [Serializable]
    public class Vector3andSpace
    {
        public Vector3 value;
        public Space space = Space.Self;
    }
}
