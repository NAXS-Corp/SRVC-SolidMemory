#define isDebug
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using Ludiq;
using Bolt;

[System.Serializable]
public enum NXTriggerMode{
    ActionMode,
    FlowMode
}

[IncludeInSettings(true)]
public abstract class NXTrigger : MonoBehaviour
{
    //====================
    //Inspector Paramaters
    //====================
    

    //TriggerMode
    public NXTriggerMode TriggerMode;

    [BoxGroup("Actions"), ShowIf("TriggerMode", NXTriggerMode.ActionMode)]
    [ListDrawerSettings(Expanded = true)]
    public List<NXAction> Actions;

    
    [BoxGroup("Actions"), ShowIf("TriggerMode", NXTriggerMode.FlowMode)]
    public FlowMachine ActionFlow;

    // [FoldoutGroup("Threshold")]
    // [PropertyOrder(999)]
    // public float ThresholdTime = 0f;
    // bool UseThreshold => ThresholdTime > 0f ? true : false;

    [FoldoutGroup("Advanced")]
    public bool PreventRetriggering = false;
    [FoldoutGroup("Advanced/Events")]
    [PropertyOrder(999)]
    public UnityEvent OnTriggerStartEvent;
    [FoldoutGroup("Advanced/Events")]
    [PropertyOrder(999)]
    public UnityEvent OnTriggerStopEvent;

    //====================
    //Protected Variables
    //====================
    protected bool StateTriggered = false;
    protected bool hasTriggeredOnce;


    //====================
    //Base Methods
    //====================
    void Awake()
    {
        #if isDebug
        DebugLog("Awake");
        #endif
        
        TryFindFlowMachine();
    }

    protected virtual void Start()
    {
    }


    //====================
    //Protected Methods
    //====================
    
    protected void SetTriggerState(bool state){
        StateTriggered = state;
        #if UNITY_EDITOR
        EditorDebug = "Triggered";
        #endif
    }


    protected void OnTriggeringBase(){

    }

    protected void OnTriggerStartBase(){
        #if UNITY_EDITOR
        EditorDebug = "OnTriggerStartBase";
        #endif
        if(PreventRetriggering && hasTriggeredOnce) return;
        SetTriggerState(true);

        switch (TriggerMode)
        {
            case NXTriggerMode.FlowMode:
                CustomEvent.Trigger(gameObject, "OnTriggerStart");
                break;
            
            default:
                foreach(NXAction action in Actions){
                    if(!action) continue;
                    action.StartActionBase();
                }
                break;
        }

        OnTriggerStartEvent.Invoke();
        // OnTriggerStartChild();
        hasTriggeredOnce = true;
    }
    
    protected void OnTriggerUpdateBase(){

    }


    protected void OnTriggerStopBase(){

        #if UNITY_EDITOR
        EditorDebug = "OnTriggerStopBase";
        #endif
        SetTriggerState(false);
        
        switch (TriggerMode)
        {
            case NXTriggerMode.FlowMode:
                CustomEvent.Trigger(gameObject, "OnTriggerStop");

                break;
            
            default:
                foreach(NXAction action in Actions){
                    if(!action) continue;
                    action.StopActionBase();
                }
                break;
        }

        OnTriggerStopEvent.Invoke();
        
    }

    //====================
    //Virtual Methods
    //====================
    
    protected virtual void Update(){}
    // public virtual void OnTriggerStartChild(){}
    // public virtual void OnTriggerStopChild(){}
        

    void TryFindFlowMachine(){
        FlowMachine flow = GetComponent<FlowMachine>();
        if(flow != null){
            TriggerMode = NXTriggerMode.FlowMode;
            ActionFlow = flow;
        }
    }

    //=======================//
    //====EDITOR Behavior====//
    //=======================//
    
    #if UNITY_EDITOR
        [FoldoutGroup("Advanced")]
        [ReadOnly][PropertyOrder(999)]
        public string EditorDebug;

        void Reset()
        {
            #if isDebug
            DebugLog("OnReset");
            #endif
            InitializeEditor();
            OnEditorReset();
        }
        
        void OnValidate()
        {
            #if isDebug
            DebugLog("OnValidate");
            #endif
            InitializeEditor();
            OnEditorValidateBase();
            OnEditorValidate();
        }
        
        [OnInspectorGUI]
        private void OnInspectorUpdate()
        {
            if(Application.isPlaying)
                return;
            if(ActionFlow != null){
                TriggerMode = NXTriggerMode.FlowMode;
                return;
            }

            #if isDebug
            // DebugLog("OnInspectorUpdate");
            #endif

            //todo : find better way to detect if sibling component is added
            //try find Bolt FlowMachine
            TryFindFlowMachine();
        }

        void InitializeEditor(){
            FindAllActions();
        }
        
        [BoxGroup("Actions")]
        [Button("Refresh Action List")]
        void FindAllActions(){
            ClearActions();
            var FoundActions = new List<NXAction>(GetComponentsInChildren<NXAction>(true));
            foreach(NXAction action in FoundActions){
                if(action.transform == this.transform || action.transform.parent == this.transform){
                    AddAction(action);
                }
            }
        }

        void ClearActions(){
            foreach(NXAction action in Actions){
                if(action.transform != this.transform && action.transform.parent != this.transform){
                    Actions.Remove(action);
                }
            }
        }


        public void AddAction(NXAction _action)
        {
            if(!Actions.Contains(_action))
                Actions.Add(_action);
            _action.SetToTriggerMode(this);
        }
        

        protected virtual void OnEditorReset(){}
        protected virtual void OnEditorValidate(){}
        protected virtual void OnEditorValidateBase(){}
    #endif

    protected void DebugLog(string log){
        #if UNITY_EDITOR
        Debug.Log(string.Concat("===NXTrigger=== ", gameObject.name, ": ", log));
        #endif
    }
}
