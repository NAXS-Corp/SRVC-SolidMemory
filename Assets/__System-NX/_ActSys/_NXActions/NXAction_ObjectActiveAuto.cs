using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using DG.Tweening;

[AddComponentMenu("_NXAction/Basic/NX Simple GameObject")]
public class NXAction_ObjectActiveAuto : NXAction
{
    
    public List<GameObject> GameObjects = new List<GameObject>(1);

    protected override void Awake()
    {
        SetActiveAll(false);
    }

    protected override void Start(){}
    public override void StartAction(){
        SetActiveAll(true);
    }
    public override void StopAction(){
        SetActiveAll(false);
    }
    public override void UpdateAction(){}

    void SetActiveAll(bool state)
    {
        foreach(GameObject obj in GameObjects)
        {
            if(obj)
                obj.SetActive(state);
        }
    }

    #if UNITY_EDITOR
    // [Button("Find Childern")]
    // void AddChildernGO(){
    //     var foundTransforms = new List<Transform>(GetComponentsInChildren<Transform>(true));
    //     foreach(Transform T in foundTransforms){
    //         if(T.parent == this.transform){
    //             GameObjects.Add(T.gameObject);
    //         }
    //     }
    // }
    #endif
}