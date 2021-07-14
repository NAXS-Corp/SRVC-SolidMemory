using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FX_TextureABFade : MonoBehaviour
{

    [HideIf("TargetMaterial")]
    public MeshRenderer TargetRenderer;
    [HideIf("TargetRenderer")]
    public Material TargetMaterial;
    public List<Texture2D> TextureList;
    public float FadeTime = 5f;

    private int nowIdx;
    private int nextIdx;
    private float currentTime;

    [FoldoutGroup("Shader Properties")]
    public string TextureA = "_TextureA";
    [FoldoutGroup("Shader Properties")]
    public string TextureB = "_TextureB";
    [FoldoutGroup("Shader Properties")]
    public string ABFader = "_ABFader";
    
    
    void Start()
    {
        if(!TargetMaterial)
            TargetMaterial = TargetRenderer.material;

        nowIdx = 0;
        nextIdx = nowIdx + 1;
        currentTime = 0;

        NextTexture();
    }


    void Update()
    {
        currentTime += Time.deltaTime / FadeTime;
        TargetMaterial.SetFloat(ABFader, currentTime);
        
        if(currentTime > 1f){
            nowIdx = nextIdx;
            nextIdx += 1;
            if( nextIdx >= TextureList.Count){
                nextIdx = 0;
            }
            NextTexture();
        }
    }


    void NextTexture()
    {
        currentTime = 0f;
        TargetMaterial.SetFloat(ABFader, currentTime);
        TargetMaterial.SetTexture(TextureA, TextureList[nowIdx]);
        TargetMaterial.SetTexture(TextureB, TextureList[nextIdx]);
    }
}
