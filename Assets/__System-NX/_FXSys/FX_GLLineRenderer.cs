using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_GLLineRenderer : MonoBehaviour
{
    public Material LineMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRenderObject() {
        LineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        {
            GL.Vertex(Vector3.zero);
            GL.Vertex(Vector3.up * 100);
        }
        GL.End();
 
        return;
    }
}
