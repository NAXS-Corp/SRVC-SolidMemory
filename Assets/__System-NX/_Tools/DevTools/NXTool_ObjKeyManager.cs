using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NAXS.NXHelper;

[ExecuteInEditMode]
public class NXTool_ObjKeyManager : MonoBehaviour
{
    [SerializeField]GameObject[] NumObjs;
    [SerializeField]BoolEvent[] ShiftToggleEvents;
    bool[] toggleStates;
    [SerializeField]UnityEvent[] AltEvents;

    [Header("GUI")]
    public bool ShowGui = false;
    public float w = 300;
    public float windowBottom = 30;
    public float lineHeight = 30;
    

    KeyCode[] AlphaKeys = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.Alpha0};
    
    public bool AutoDisableOnBuild = true;
    private bool hotkeyEnabled = true;
    private int hotkeyCount = 0;
    // public bool Prevent

    void Start()
    {
        #if !UNITY_EDITOR
        if(AutoDisableOnBuild)
            hotkeyEnabled = false;
        else
            hotkeyEnabled = true;
        #endif
        toggleStates = new bool[ShiftToggleEvents.Length];
    }

    void Update()
    {   
        if(!Application.isPlaying) return;
        
        if(Input.anyKeyDown){
            if(Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Backslash)){
                hotkeyCount += 1;
                if(hotkeyCount >= 3){
                    hotkeyEnabled = true;
                }
            }else{
                hotkeyCount = 0;
            }
        }
        
        if(!hotkeyEnabled){
            return;
        }
        


        if(Input.GetKey(KeyCode.LeftAlt)){
            for(int i = 0; i < AltEvents.Length; i++){
                if(Input.GetKeyDown(AlphaKeys[i])){
                    Debug.Log("NXTool_ObjKeyManager  Call Evetn "+i);
                    // if(ShiftToggleEvents[i] != null){
                        AltEvents[i].Invoke();
                    // }
                    return;
                }
            }
        }
        if(Input.GetKey(KeyCode.LeftShift)){
            for(int i = 0; i < ShiftToggleEvents.Length; i++){
                if(Input.GetKeyDown(AlphaKeys[i])){
                    InvokeShiftEvent(i);
                    // ShiftToggleEvents[i].GetPersistentTarget(0).
                    return;
                }
            }
        }else{
            for(int i = 0; i < NumObjs.Length; i++){
                if(Input.GetKeyDown(AlphaKeys[i])){
                    if(NumObjs[i]){
                        NumObjs[i].SetActive(!NumObjs[i].activeSelf);
                    }
                }
            }
        }
    }

    void InvokeShiftEvent(int i){
                    toggleStates[i] = !toggleStates[i];
                    ShiftToggleEvents[i].Invoke(toggleStates[i]);
    }

    void OnGUI()
    {
        if(!ShowGui) return;

        float h = 30;
        float x = Screen.width - w;
        float y = Screen.height - windowBottom;

        for(int i = 0; i < AlphaKeys.Length; i++){
            GUI.Label(new Rect(x - w - 30, y + i * lineHeight, 30, h), (i+1).ToString());
        }

        GUI.Label(new Rect(x - w * 2, y - 30 * lineHeight, w, h), "Events-LeftAlt");
        for(int i = 0; i < ShiftToggleEvents.Length; i++){
            if(NumObjs[i]){
                GUI.Label(new Rect(x - w * 2, y + i * lineHeight, w, h), string.Concat("Event 0", i.ToString()));
            }
        }

        GUI.Label(new Rect(x - w, y - 30 * lineHeight, w, h), "Alphas");
        for(int i = 0; i < NumObjs.Length; i++){
            if(NumObjs[i]){
                GUI.Toggle(new Rect(x - w, y + i * lineHeight, w, h), NumObjs[i].activeSelf, NumObjs[i].gameObject.name);
            }
        }
    }
}
