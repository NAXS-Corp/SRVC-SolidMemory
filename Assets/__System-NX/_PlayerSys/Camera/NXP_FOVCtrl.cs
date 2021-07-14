using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NAXS.Base;

namespace NAXS.NXPlayer
{
    public class NXP_FOVCtrl : MonoBehaviour
    {
        public static NXP_FOVCtrl singleton;
        public NXP_Movement NXPMovement;
        public CinemachineVirtualCamera VCam;

        public float FOVBase = 90;
        public Vector2 FOVLimiter = new Vector2(30f, 160f);
        [Range(0f, 1f)]public float MasterCtrl = 1f;

        [Header("Move Detection")]
        public bool MoveDetection;
        public float FOVMoveRatio = 0.8f;
        public float ZoomSpeedMove = 10;
        public float ZoomSpeedIdle = 10;
        float movingRatio = 1f;
        
        [Header("Distance Detection")]
        public bool DistDetection = true;
        public float FOVFarRatio = 1.2f;
        public Vector2 DistanceRange = new Vector2(1f, 30f);
        public float ZoomSpeedDistance = 1f;
        float distRatio = 5f;
        float range;
        public LayerMask DetectionMask;


        void Awake()
        {
            NXP_FOVCtrl.singleton = this;
            range = DistanceRange.y - DistanceRange.x;
        }
        void Reset()
        {
            VCam = GetComponent<CinemachineVirtualCamera>();
        }

        void Update()
        {
            float finalRatio = 1f;

            if(MoveDetection)
            {
                if(NXPMovement.IsMoveing)
                    movingRatio = Mathf.Lerp(movingRatio, FOVMoveRatio, Time.deltaTime * ZoomSpeedMove);
                else
                    movingRatio = Mathf.Lerp(movingRatio, 1, Time.deltaTime * ZoomSpeedIdle);

                finalRatio *= movingRatio;
            }

            if(DistDetection){
                
                // Ray ray = NXBase_ObjManager.instance.MainCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, DistanceRange.y, DetectionMask))
                {
                    float hitDistance = Vector3.Distance(hit.point, transform.position);
                    float distanceRatio = Mathf.Clamp((hitDistance - DistanceRange.x) / range ,0, 1);
                    float targetRatio = Mathf.Lerp(distRatio, FOVFarRatio, distanceRatio);
                    distRatio = Mathf.Lerp(distRatio, targetRatio, Time.deltaTime * ZoomSpeedDistance);
                    Debug.DrawLine(ray.origin, hit.point, Color.green);
                }
                else
                {
                    distRatio = Mathf.Lerp(distRatio, 1f, Time.deltaTime * ZoomSpeedDistance);
                    Debug.DrawLine(ray.origin, ray.direction* DistanceRange.y, Color.red);
                }
                finalRatio *= distRatio;
            }

            VCam.m_Lens.FieldOfView = Mathf.Clamp(FOVBase * finalRatio * MasterCtrl, FOVLimiter.x, FOVLimiter.y);
        }

    }
}