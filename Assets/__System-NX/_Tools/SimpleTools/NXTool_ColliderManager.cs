#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;


public enum ColliderTypes
{
    BoxCollider,
    SphereCollider,
    MeshCollider
}

[ExecuteInEditMode]
public class NXTool_ColliderManager : MonoBehaviour
{
    

    public ColliderTypes TypeToDelete;
    public ColliderTypes TypeToReplace;
    public ColliderTypes TypeToAdd;
    // public Behaviour TypeToA
    [Button]
    void Delete(){
        Debug.Log("Searching :: "+TypeToDelete.ToString());
        Type t = FindColliderType(TypeToDelete.ToString());
        var newArray = GetComponentsInChildren(t, true);
        Debug.Log("Found newArray:: "+newArray.Length);
        if(newArray.Length == 0 || !EditorUtility.DisplayDialog("?",
                "Replace " + newArray.Length
                + TypeToDelete.ToString() + "?", "Replace", "No"))
            return;
        for(int i = 0; i < newArray.Length; i++){
            DestroyImmediate(newArray[i]);
        }
    }
    [Button]
    void Replace(){
        Debug.Log("Searching :: "+TypeToDelete.ToString());
        Type t = FindColliderType(TypeToDelete.ToString());
        Type addType = FindColliderType(TypeToReplace.ToString());
        var newArray = GetComponentsInChildren(t, true);
        Debug.Log("Found newArray:: "+newArray.Length);
        if(newArray.Length == 0 || !EditorUtility.DisplayDialog("?",
                "Replace " + newArray.Length
                + TypeToDelete.ToString() + "?", "Replace", "No"))
            return;
        for(int i = 0; i < newArray.Length; i++){
            newArray[i].gameObject.AddComponent(addType);
            DestroyImmediate(newArray[i]);
        }
    }

    [Button]
    void Add(){
        Debug.Log("Searching :: MeshRenderer");
        // Type t = FindColliderType(TypeToAdd.ToString());
        Type addType = FindColliderType(TypeToAdd.ToString());
        var newArray = GetComponentsInChildren(typeof(MeshRenderer), true);
        Debug.Log("Found newArray:: "+newArray.Length);
        if(newArray.Length == 0 || !EditorUtility.DisplayDialog("?",
                "Add Collider to " + newArray.Length
                + "MeshRenderer" + "?", "Add", "No"))
            return;
        for(int i = 0; i < newArray.Length; i++){
            newArray[i].gameObject.AddComponent(addType);
            // DestroyImmediate(newArray[i]);
        }
    }


    Type FindColliderType(string typeString){
        switch (typeString)
        {
            case "SphereCollider":
                return typeof(SphereCollider);
            case "MeshCollider":
                return typeof(MeshCollider);
            default:
                return typeof(BoxCollider);
        }
    }

}
#endif