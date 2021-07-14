using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class FX_SkyboxChanger : MonoBehaviour
{
    public bool EditorPreview;

    [OnValueChanged("ChangeSkybox")]
    public Material Skybox;
    // Start is called before the first frame update
    void OnEnable()
    {
        ChangeSkybox();
    }

    void ChangeSkybox(){

        #if UNITY_EDITOR
        if(!Application.isPlaying && EditorPreview && Skybox)
        {
            RenderSettings.skybox = Skybox;
            return;
        }
        #endif
        if(Skybox)
            RenderSettings.skybox = Skybox;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
