
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(NXAction), true)]
public class NXActionEditor : OdinEditor
{
    public NXAction targetAction;

    void Awake()
    {
        this.targetAction = (NXAction)target;
    }


    void OnDestroy()
    {
        
    }

    override protected void OnEnable()
    {
        this.targetAction = (NXAction)target;
        this.targetAction.OnEnableEditor();
    }

    override protected void OnDisable()
    {
        this.targetAction.OnDisableEditor();
    }


    public override void OnInspectorGUI ()
    {
        if (this.targetAction != null)
        {
            this.targetAction.OnInspectorGUIEditor();
        }
        DrawDefaultInspector ();
    }
}