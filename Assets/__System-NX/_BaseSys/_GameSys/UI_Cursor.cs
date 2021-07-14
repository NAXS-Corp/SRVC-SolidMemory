using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Cursor : MonoBehaviour
{
    public Texture2D CursorTex;
    public Vector2 Offset;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(CursorTex, Offset, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
