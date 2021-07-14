using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public struct FX_Transform{
//     Vector3 Position;
//     Quaternion Rotation;
//     S Rotation;
// }

public class FX_TransformBasic : MonoBehaviour
{
    public Transform Target;
    public Space Space;
    public Vector3 SetPosition;
    // public float FadeTime;
    public bool AutoStart = true;



    void Start()
    {
        if(AutoStart){
            if(Space == Space.Self)
                Target.localPosition = SetPosition;
            else
                Target.position = SetPosition;
        }
    }

    void Update()
    {
        
    }
}
