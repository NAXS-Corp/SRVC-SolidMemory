using UnityEngine;

[ExecuteInEditMode]
public class VertexCount : MonoBehaviour {
    
    void Start()
    {
        // Mesh mesh = GetComponent<MeshFilter>().mesh;
        Mesh mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        Debug.Log("VertexCount "+mesh.vertexCount);
    }
}