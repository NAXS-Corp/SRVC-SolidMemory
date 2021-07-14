using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NXTool_DebugManager : MonoBehaviour
{
    [Serializable]
    public struct DebugObj
    {
        public GameObject WatchObj;
        public String LabelName;
        public Text DebugUI;
    }

    public DebugObj[] DebugObjs;

    void Start()
    {
        CheckDebugObj();
    }

    public void DebugEvent(GameObject m_WatchObj)
    {
        if (m_WatchObj)
        {
            foreach (var Objlist in DebugObjs)
            {
                if (Objlist.WatchObj == m_WatchObj)
                {
                    bool statue = Objlist.WatchObj.activeSelf;
                    Objlist.DebugUI.text = Objlist.LabelName + " : " + statue;
                }
            }
        }
    }

    public void CheckDebugObj()
    {
        foreach (var Objlist in DebugObjs)
        {
            bool statue = Objlist.WatchObj.activeSelf;
            Objlist.DebugUI.text = Objlist.LabelName + " : " + statue;
        }
    }
}
