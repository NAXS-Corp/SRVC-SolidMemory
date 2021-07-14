using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FX_Billboard : MonoBehaviour
{

    private Camera m_Camera;

    void Start(){
        m_Camera = Camera.main;
    }
 
    void LateUpdate()
    {

        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }

    // ===================
    // For Editor Preview
    // ===================

    #if UNITY_EDITOR
    void OnEnable(){
        EditorApplication.update += EditorPreviewUpdate;
    }

    void OnDisable(){
        EditorApplication.update -= EditorPreviewUpdate;
    }

    void EditorPreviewUpdate(){
        if(Application.isPlaying)
            return;

        m_Camera = SceneView.lastActiveSceneView.camera;
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
        return;
        
    }
    #endif
}
