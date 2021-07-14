using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class NXTool_GetUTCTime : MonoBehaviour
{
    #if UNITY_EDITOR
    public int Hour;
    public int Minutes;
    public int SecondsOfDay;
    public bool ShowGUI;


    // Start is called before the first frame update
    void Start()
    {
        GetUTCTime();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.T)){
                ShowGUI = !ShowGUI;
            }
        }
    }

    void OnGUI()
    {
        if(!ShowGUI){
            return;
        }
        GetUTCTime();
        int y = 20;
        int Offset = 30;
        // GUI.TextArea(new Rect(20,y, 500, 20), "SecondsOfDay: "+SecondsOfDay);
        // y += Offset;
        // GUI.TextArea(new Rect(20,y, 500, 20), "UTCNow.Second: "+System.DateTime.UtcNow.Second);
        // y += Offset;
        GUI.TextArea(new Rect(20,y, 500, 20), "NXHelper_Time.Current: "+ NXHelper_Time.Current());

    }


    [Button]
    void GetUTCTime()
    {
        Hour = (int)System.DateTime.UtcNow.TimeOfDay.Hours;
        Minutes = (int)System.DateTime.Now.TimeOfDay.Minutes;
        SecondsOfDay = (int)System.DateTime.UtcNow.TimeOfDay.Hours * 3600 + (int)System.DateTime.Now.TimeOfDay.Minutes* 60 + (int)System.DateTime.Now.TimeOfDay.Seconds;
    }
    #endif
}
