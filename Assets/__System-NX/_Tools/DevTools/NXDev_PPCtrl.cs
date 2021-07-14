using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if !HDRP
using UnityEngine.Rendering.Universal;
#endif

public class NXDev_PPCtrl : MonoBehaviour
{
#if !HDRP
    private Volume[] volumes;
    bool volumeState = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.P)){
                FindVolulmes();
            }
        }
        if(Input.GetKeyDown(KeyCode.P)){
            ToggleVolumes();
        }
    }

    void FindVolulmes(){
        volumes = Resources.FindObjectsOfTypeAll<Volume>();
    }

    void ToggleVolumes(){
        volumeState = !volumeState;
        foreach(Volume v in volumes){
            v.enabled = volumeState;
        }
    }
#endif
}
