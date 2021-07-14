using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXTool_OpenUrl : MonoBehaviour
{
    public void OpenUrl(string url){
        #if !UNITY_WEBGL
            Application.OpenURL(url);
        #else
            Application.ExternalEval("window.open('"+url+"');");
        #endif
    }
}
