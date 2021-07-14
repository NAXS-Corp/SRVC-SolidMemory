using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.FX
{

    // Tool for controlling Blendshape via Timeline

    [ExecuteInEditMode]
    public class FX_BlendshapeCtrl : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool PreviewInEditor = true;
#endif
        public SkinnedMeshRenderer TargetSkinnedMesh;

        [Tooltip("The index of the blendshape, listed in the SkinnedMeshRenderer")]
        public int BlendshapeIndex;
        [ReadOnly]
        public string BlendshapeName;
        Mesh mesh;
        public float BlendWeight;

        void Reset()
        {
            if(GetComponent<SkinnedMeshRenderer>()){
                TargetSkinnedMesh = GetComponent<SkinnedMeshRenderer>();
            }
        }

        void Start()
        {
            if(TargetSkinnedMesh){
                mesh = TargetSkinnedMesh.sharedMesh;
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !PreviewInEditor)
            {
                return;
            }
#endif
            if(TargetSkinnedMesh){
                TargetSkinnedMesh.SetBlendShapeWeight(BlendshapeIndex, BlendWeight);
                // Debug.Log("SetBlendshade "+BlendshapeIndex+" "+value);
            }
        }

        void OnValidate()
        {
            Debug.Log("1");
            if(!mesh){
                Debug.Log("2");
                mesh = TargetSkinnedMesh.sharedMesh;
            }
            if(mesh){
                Debug.Log("3");
                if(mesh.blendShapeCount > BlendshapeIndex){
                    BlendshapeName = mesh.GetBlendShapeName(BlendshapeIndex);
                }else{
                    BlendshapeName = "";
                }
            }
        }
    }
}
