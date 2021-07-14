using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;

[AddComponentMenu("_NXAction/General/NX Timeline Ctrl")]
public class NXAction_Timeline : NXAction
{
    public PlayableDirector TimelineDirector;
    public bool DisableChildObjectsOnAwake = true;
    [ReadOnly]public List<GameObject> ChildList;
    protected override void Awake() { 
        TimelineDirector.Stop();
        TimelineDirector.Evaluate();
        TimelineDirector.playOnAwake = false;
        if(DisableChildObjectsOnAwake)
        {
            foreach(Transform child in transform.GetComponentsInChildren<Transform>(true))
            {
                if(child.gameObject && child != this.transform){
                    ChildList.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public override void StartAction() {
        if(DisableChildObjectsOnAwake){
            foreach(GameObject child in ChildList)
            {
                if(child && child != this.gameObject)
                    child.SetActive(true);
            }
        }
        TimelineDirector.Play();
    }
    public override void StopAction() { }


    #if UNITY_EDITOR
    public override void EditorReset() {
        if(!TimelineDirector)
            TimelineDirector = GetComponent<PlayableDirector>();
        if(TimelineDirector)
            TimelineDirector.playOnAwake = false;
    }

    [Button]
    [HideIf("TimelineDirector")]
    [GUIColor(0.1f, 0.85f, 0.25f)]
    void AddTimelineDirector()
    {
        gameObject.AddComponent<PlayableDirector>();
        EditorReset();
    }
    #endif

}
