using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class ActionSetting
{
    // MasterFader
}

[System.Serializable]
[System.Flags]
public enum NXActionOptions
{
    Fade = 1 << 1,
    Revertable = 1 << 2,
}

public enum TriggerMode
{
    Self, Trigger
}

// [ExecuteInEditMode]
public abstract class NXAction : MonoBehaviour
{
    //=======================//
    //Inspector
    //=======================//
    // [PropertyOrder(999)][FoldoutGroup("Advance")][DisableIf("m_NXTrigger")]
    // public bool AutoStart = true;

    [PropertyOrder(999)]
    [FoldoutGroup("Advance")]
    public bool StopOnFadeIn;
    [PropertyOrder(999)]
    [FoldoutGroup("Advance")]
    [ReadOnly]
    public TriggerMode TriggerMode = TriggerMode.Self;

    [PropertyOrder(999)]
    [FoldoutGroup("Advance")]
    [ReadOnly]
    public NXTrigger m_NXTrigger;

    //=======================//
    //Editor Inspector
    //=======================//
#if UNITY_EDITOR
    [PropertyOrder(999)]
    [FoldoutGroup("Advance")]
    [ShowIf("TriggerMode", TriggerMode.Trigger)]
    [Button("Trigger Mode"), GUIColor(0, 0.7f, 0.1f)]
    void controledByTrigger() { TryFindTrigger(); }
#endif

#if UNITY_EDITOR
    [PropertyOrder(999)]
    [FoldoutGroup("Advance")]
    [ShowIf("TriggerMode", TriggerMode.Self)]
    [Button("Auto Mode"), GUIColor(1, 0.5f, 0)]
    void isAutoMode() { TryFindTrigger(); }
#endif

    //=======================//
    //Virtual
    //=======================//
    protected bool ActionStateStarted = false;
    protected virtual void Awake() { }
    protected virtual void Start() { }
    public virtual void StartAction() { }
    public virtual void StopAction() { }
    public virtual void UpdateAction() { }

    //=======================//
    //Protected
    //=======================//
    public void StartActionBase()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (!this.enabled) return;
        if (!this.gameObject.activeSelf) return;
        ActionStateStarted = true;
        StartAction();
    }

    public void StopActionBase()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        //Return if Action never started, or already stopped
        if (!ActionStateStarted) return;
        if (!this.gameObject.activeSelf) return;

        StopAction();
        ActionStateStarted = false;
    }

    //=======================//
    //Standalone / Auto Mode
    //=======================//

    void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            OnActivateEditor();
#endif

        if (gameObject.activeSelf && TriggerMode == TriggerMode.Self)
            StartActionBase();
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            OnDeactivateEditor();
#endif

        if (TriggerMode == TriggerMode.Self)
            StopActionBase();
    }


    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorUpdate();
#endif
        if (!ActionStateStarted) return;
        UpdateAction();
    }


    //=======================//
    //====EDITOR Behavior====//
    //=======================//
#if UNITY_EDITOR
    // [ReadOnly][PropertyOrder(999)]
    // protected string ActionModeDebug = "Auto Start Mode";
    // [FoldoutGroup("Advance")]
    // [ReadOnly][PropertyOrder(999)]
    // protected string EditorDebug;


    protected void Reset()
    {
        if (Application.isPlaying) return;
        // EditorDebug = "EditorResetBase";
        TryFindTrigger();
        EditorReset();
    }

    protected void OnValidate()
    {
        OnValidateChild();
    }

    protected void OnDestroy()
    {
        if (m_NXTrigger)
        {
            if (!m_NXTrigger.Actions.Contains(this))
                m_NXTrigger.Actions.Remove(this);
        }
    }

    // Cusom Editor Logic=============================

    void TryFindTrigger()
    {
        NXTrigger _foundTrigger = m_NXTrigger;
        if (!_foundTrigger)
        {
            _foundTrigger = GetComponent<NXTrigger>();
            if (!_foundTrigger)
                _foundTrigger = gameObject.GetComponentInParent<NXTrigger>();
        }

        if (_foundTrigger)
        {
            if (_foundTrigger.transform != this.transform.parent)
                _foundTrigger = null;
        }

        if (_foundTrigger)
        {
            Debug.Log("TryFindTrigger:: Found");
            _foundTrigger.AddAction(this);
        }
        else
        {
            this.TriggerMode = TriggerMode.Self;
            m_NXTrigger = null;
        }
    }

    public void SetToTriggerMode(NXTrigger _trigger)
    {
        this.m_NXTrigger = _trigger;
        this.TriggerMode = TriggerMode.Trigger;
    }


    //Called from CustomEditor========================
    public void OnActivateEditor()
    {
        // TryFindTrigger();
        OnActivateEditorChild();
    }

    public void OnDeactivateEditor()
    {
        // TryFindTrigger();
        OnDeactivateEditorChild();
    }

    public void OnEnableEditor()
    {
        // Debug.Log("OnEnableEditor "+ gameObject.name);
        // TryFindTrigger();
        OnEnableEditorChild();
    }

    public void OnDisableEditor()
    {
        // TryFindTrigger();
        // Debug.Log("OnDisableEditor "+ gameObject.name);
        OnDisableEditorChild();
    }


    public void OnInspectorGUIEditor()
    {
        TryFindTrigger();
        OnInspectorGUIEditorChild();
    }

    //Virtual========================
    public virtual void EditorUpdate() { }
    public virtual void EditorReset() { }

    public virtual void OnValidateChild() { }
    public virtual void OnEnableEditorChild() { }
    public virtual void OnDisableEditorChild() { }

    public virtual void OnActivateEditorChild() { }
    public virtual void OnDeactivateEditorChild() { }

    public virtual void OnInspectorGUIEditorChild() { }
#endif


    protected void DebugLog(string log)
    {
#if UNITY_EDITOR
        Debug.Log(string.Concat("===NXAction=== ", gameObject.name, log));
#endif
    }
}
