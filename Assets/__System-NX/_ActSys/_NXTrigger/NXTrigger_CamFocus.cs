using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using NAXS.Base;
using NAXS.NXHelper;

[System.Serializable]
[AddComponentMenu("_NXTrigger/NX Player Focus")]
public class NXTrigger_CamFocus : NXTrigger_RangeBase
{
    
    [FoldoutGroup("Focus Setting")]
    [Required]public Collider m_Collider;
    [FoldoutGroup("Focus Setting")]
    public float FocusedTime = 1f;
    private float focusTimer;
    private bool isFocusing;
    private bool isFocused;
    [FoldoutGroup("Focus Setting")]
    [Range(0f, 1f), ReadOnly]
    public float FocusingRatio;

    [FoldoutGroup("Extra Event")]
    public FloatEvent OnFocusing;

    protected override void InRangeUpdate() {
        bool lastIsFocused =  isFocused;
        isFocusing = false;
        
        Ray ray = NXBase_ObjManager.instance.MainCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        // Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red);
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform == m_Collider.transform)
                isFocusing = true;
        }


        if(isFocusing)
            focusTimer += Time.deltaTime;
        else
            focusTimer -= Time.deltaTime;

        focusTimer = Mathf.Clamp(focusTimer, 0f, FocusedTime);
        FocusingRatio = focusTimer / FocusedTime;

        if(FocusingRatio != 0)
            OnFocusing.Invoke(FocusingRatio);

        if(focusTimer >= FocusedTime)
            isFocused = true;

        if(focusTimer <= 0f)
            isFocused = false;
        
        if(isFocused && !lastIsFocused)
            OnFocused();
            
        if(!isFocused && lastIsFocused)
            OnStopFocus();
            
    }
    protected override void OnEnterRange() {}
    protected override void OnExitRange() {}

    void OnFocused()
    {
        OnTriggerStartBase();
    }
    void OnStopFocus()
    {
        OnTriggerStopBase();
    }


    //=======================//
    //====EDITOR Behavior====//
    //=======================//
    #if UNITY_EDITOR
    protected override void OnEditorReset()
    {
        SetCollider();
    }

    protected override void OnEditorValidateBase()
    {
        SetCollider();
    }

    void SetCollider(){
        Collider collider;
        if(collider = GetComponent<Collider>()){
            m_Collider = collider;
            // m_Collider.isTrigger = true;
        }
    }

    [Button][HideIf("m_Collider")]
    [GUIColor(0.1f, 0.85f, 0.25f)]
    void AddBoxCollider(){
        gameObject.AddComponent<BoxCollider>();
        SetCollider();
    }
    [Button][HideIf("m_Collider")]
    [GUIColor(0.1f, 0.85f, 0.25f)]
    void AddSphereCollider(){
        gameObject.AddComponent<SphereCollider>();
        SetCollider();
    }
    #endif
}
