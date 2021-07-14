using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

// [ExecuteInEditMode]
public class FX_TextureOffset : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool EditorPreview;
    #endif

    public bool ControlSharedMateril;
    [ShowIf("ControlSharedMateril")]
    public Material SharedMateril;

    private Renderer rend;

    [System.Serializable]
    public struct ScrollProp
    {
        public Vector2 scrollSpeed;
        public string ShaderProp;
        public ScrollProp(Vector2 speed, string name){
            scrollSpeed = speed;
            ShaderProp = name;
        }
    }
    
    public ScrollProp[] TextureSettings = new ScrollProp[]{new ScrollProp(new Vector2(0, -0.02f), "_BaseMap")};
    void Start()
    {
        rend = GetComponent<Renderer>();
        //rend.material.
    }
    void Update()
    {
        #if UNITY_EDITOR
        if(!Application.isPlaying && !EditorPreview) return;
        #endif

        foreach(ScrollProp setting in TextureSettings){
            Vector2 offset = Time.time * setting.scrollSpeed;
            if(ControlSharedMateril)
                SharedMateril.SetTextureOffset(setting.ShaderProp, offset);
            else
                rend.material.SetTextureOffset(setting.ShaderProp, offset);
        }

    }
}