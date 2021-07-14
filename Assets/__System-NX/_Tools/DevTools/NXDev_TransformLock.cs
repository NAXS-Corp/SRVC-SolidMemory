using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class NXDev_TransformLock : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool LockPosition = true;
    public bool LockRotation = true;
    public bool LockScale = true;
    Vector3 _lockPos;
    Quaternion _lockRot;
    Vector3 _lockScale;

    void OnEnable()
    {
        SetLock();
    }
    

    void SetLock(){
        _lockPos = transform.position;
        _lockRot = transform.rotation;
        _lockScale = transform.localScale;
    }

    void Update(){
        transform.position = _lockPos;
        transform.rotation = _lockRot;
        transform.localScale = _lockScale;
    }
    #endif
}
