using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTool_PivotSmooth : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 lastRot;
    public Vector3 PosLerp = Vector3.zero;
    public Vector3 RotLerp;

    void Start()
    {
        SetLastData();
    }


    void LateUpdate()
    {
        Vector3 deltaPos = transform.parent.position - lastPos;
        if(PosLerp != Vector3.zero){
            var x = Mathf.Lerp(deltaPos.x, 0, PosLerp.x * Time.deltaTime);
            var y = Mathf.Lerp(deltaPos.y, 0, PosLerp.y * Time.deltaTime);
            var z = Mathf.Lerp(deltaPos.z, 0, PosLerp.z * Time.deltaTime);
            transform.localPosition = new Vector3(x,y,z);
        }
    }

    void SetLastData(){
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
    }
}
