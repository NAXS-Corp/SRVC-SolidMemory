using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NAXS.NXPlayer
{
    public class NXP_PlayerLight : MonoBehaviour
    {
        public Light m_Light;
        public NXP_Movement m_Move;

        
        [Header("Fade")]
        [Tooltip("Base Intensity")]
        public Vector2 FadeIntensity = Vector2.up;
        public Ease FadeMode;
        public float FadeTime = 2f;
        
        [Header("Strobe")]
        [Tooltip("Multipliy")]
        public Vector2 StrobeMulti = new Vector2(1f,2f);
        public float StrobeTime = 0.03f;

        
        [Header("Runtime Ctrl")]
        [SerializeField, Range(0f, 1f)]private float MasterCtrl = 1f;
        [SerializeField, Range(0.1f, 1f)]private float StrobeSpeedCtrl = 0.5f;
        [SerializeField, Range(0.1f, 0.9f)]private float OnOffRatio = 0.5f;

        Vector3 lastPosition;

        bool strobeDir;
        float strobeTimer;
        float _OnTime = 0.5f;
        float _OffTime = 0.5f;
        float strobeOutput;
        float strobeMin;

        
        // bool fadeDir;
        float fadeOutput;

        void Start()
        {
            
            DoFadeOn();
        }

        void DoFadeOn(){
            DOTween.To(()=> fadeOutput, x=> fadeOutput = x, FadeIntensity.y, FadeTime).SetEase(FadeMode).OnComplete(DoFadeOff);
        }

        void DoFadeOff(){
            DOTween.To(()=> fadeOutput, x=> fadeOutput = x, FadeIntensity.x, FadeTime).SetEase(FadeMode).OnComplete(DoFadeOn);
        }

        void Update()
        {
            StrobeUpdate();
            m_Light.intensity = MasterCtrl * strobeOutput * fadeOutput;
        }

        void StrobeUpdate()
        {
            if(m_Move.IsIdle)
            {
                strobeMin = Mathf.Lerp(strobeMin, StrobeMulti.y, Time.deltaTime);
            }else{
                strobeMin = StrobeMulti.x;
            }

            if(StrobeSpeedCtrl > 0.99f){
                strobeOutput = StrobeMulti.y;
                return;
            }

            strobeTimer -= Time.deltaTime;
            if(strobeTimer < 0){

                strobeDir = !strobeDir;
                _OnTime = StrobeTime* OnOffRatio;
                _OffTime = StrobeTime* (1 - OnOffRatio);

                if(strobeDir){
                    strobeTimer = _OnTime / StrobeSpeedCtrl / 50f;
                    // currentPeriod = _OnTime / StrobeSpeedCtrl / 50f;
                    strobeOutput = StrobeMulti.y;
                    // intensityOutput = IntensityMinmax.x;
                }else{
                    strobeTimer = _OffTime / StrobeSpeedCtrl / 50f;
                    // currentPeriod = _OffTime / StrobeSpeedCtrl / 50f;
                    strobeOutput = strobeMin;
                    // intensityOutput = _maxIntensity;
                }
            }
        }
    }
}