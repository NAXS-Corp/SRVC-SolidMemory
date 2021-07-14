using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class NXTool_FollowTransform : MonoBehaviour
{
    [Header("OBJ")]
    public Transform follower;

    [HideIf("followMainCam")]
    public Transform target;
    public bool followMainCam;
    public bool ExecuteOnceOnStart;

    [FoldoutGroup("Position")] public bool followPos = true;
    [FoldoutGroup("Position")] public bool followYOnly = false;
    [FoldoutGroup("Position")] public bool followXZOnly = false;
    [FoldoutGroup("Position")] public Vector3 posOffset;
    // public Vector3 followPosAxis = Vector3.one;
    [FoldoutGroup("Position")] public float lerpPosSpeed = 0f;

    [FoldoutGroup("Rotation")] public bool followRot = false;
    [FoldoutGroup("Rotation")] public bool followYRotOnly = false;
    // public bool localRot = false;
    // public Vector3 rotOffset;

    [FoldoutGroup("Rotation"), HideIf("useRotLerpSpeedVec"), LabelText("LerpSpeed")] public float lerpRotSpeed = 0f;
    [FoldoutGroup("Rotation")] public bool useRotLerpSpeedVec;
    [FoldoutGroup("Rotation"), ShowIf("useRotLerpSpeedVec"), LabelText("LerpSpeed")] public Vector3 RotLerpSpeedVec;

    // Vector3 directionVec;
    // Vector3 upVec;
    // Vector3 tiltVec;


    void Reset()
    {
        if (!follower)
        {
            follower = transform;
        }
    }

    void OnEnable()
    {
        if (!follower)
        {
            follower = transform;
        }
        if (followMainCam)
        {
            target = Camera.main.transform;
        }
        if (ExecuteOnceOnStart)
        {
            SetTransform();
        }
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SyncTransform();
            return;
        }
#endif

        if (!ExecuteOnceOnStart)
            SetTransform();
    }

    void SyncTransform()
    {
        if (followPos && follower && target)
            SyncPos();
        if (followRot && follower && target)
            SyncRot();
    }

    void SyncPos()
    {
        //normal follow
        // follower.position = target.position + posOffset;
        Vector3 targetPos;
        
        if (followYOnly)
        {
            targetPos = new Vector3(follower.position.x, target.position.y + posOffset.y, follower.position.z);
        }
        else if (followXZOnly)
        {
            targetPos = new Vector3(target.position.x + posOffset.x, follower.position.y, target.position.z + posOffset.z);
        }else{
            targetPos = target.position + posOffset;
        }

        
        if (lerpPosSpeed > 0){
            follower.position = Vector3.Lerp(follower.position, targetPos, Time.deltaTime * lerpPosSpeed);
        }else{
            follower.position = targetPos;
        }
    }

    void SyncRot()
    {
        if (followYRotOnly)
        {
            follower.eulerAngles = new Vector3(follower.eulerAngles.x, target.transform.eulerAngles.y, follower.eulerAngles.z);
        }
        else
        {
            follower.rotation = target.rotation;
        }
    }

    void SetTransform()
    {

        SyncPos();

        if (followRot)
        {
            Quaternion finalRot;
            if (!useRotLerpSpeedVec && lerpRotSpeed > 0)
            {
                finalRot = Quaternion.Lerp(follower.rotation, target.rotation, Time.deltaTime * lerpRotSpeed);
            }
            // else if (useRotLerpSpeedVec)
            // {


            //     // Using Vector
            //     upVec = Vector3.Lerp(upVec, target.up, RotLerpSpeedVec.x * Time.deltaTime);
            //     directionVec = Vector3.Lerp(directionVec, target.right, RotLerpSpeedVec.y * Time.deltaTime);
            //     tiltVec = Vector3.Lerp(tiltVec, target.forward, RotLerpSpeedVec.z * Time.deltaTime);
            //     // finalRot = Quaternion.FromToRotation(target.right, target.forward) * Quaternion.LookRotation(directionVec, upVec);
            //     finalRot = Quaternion.LookRotation(directionVec, upVec);


            //     // finalRot = follower.rotation;
            //     // Quaternion XRot = Quaternion.Euler(target.eulerAngles.x, 0, 0);
            //     // Quaternion YRot = Quaternion.Euler(0, target.eulerAngles.y, 0);
            //     // Quaternion ZRot = Quaternion.Euler(0, 0, target.eulerAngles.z);

            //     // XRot = Quaternion.Lerp();
            //     // finalRot.SetAxisAngle()
            //     // follower.Rotate()



            //     // Problem when euler axis went across zero
            //     // var x = Mathf.Lerp(follower.eulerAngles.x, target.eulerAngles.x, Time.deltaTime / RotLerpSpeedVec.x);
            //     // var y = Mathf.Lerp(follower.eulerAngles.y, target.eulerAngles.y, Time.deltaTime / RotLerpSpeedVec.y);
            //     // var z = Mathf.Lerp(follower.eulerAngles.z, target.eulerAngles.z, Time.deltaTime / RotLerpSpeedVec.z);
            //     // finalRot = Quaternion.Euler(x,y,z);
            // }
            else
            {
                finalRot = target.rotation;
            }

            // if(rotOffset != Vector3.zero){
            //     finalRot = finalRot * Quaternion.Euler(rotOffset);
            // }

            follower.rotation = finalRot;
        }
    }


#if UNITY_EDITOR

    Vector3 originPos;
    Quaternion originRot;
    bool isPreviewing;

    [Button("Preview"), HideIf("isPreviewing")]
    void EditorPreview()
    {
        originPos = follower.position;
        originRot = follower.rotation;
        isPreviewing = true;
        SetTransform();
    }


    [Button("Reset"), ShowIf("isPreviewing")]
    void EditorReset()
    {
        if (!isPreviewing) return;
        // SetTransform();
        follower.position = originPos;
        follower.rotation = originRot;
        isPreviewing = false;
    }

    void OnDisable()
    {
        EditorReset();
    }
#endif
}
