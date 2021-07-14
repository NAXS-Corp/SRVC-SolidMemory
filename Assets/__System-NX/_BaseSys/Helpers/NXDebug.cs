using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NAXS
{
    public static class NXDebug
    {
        #if UNITY_EDITOR
        public static bool Enable = true;
        #else
        public static bool Enable = false;
        #endif
        
        public static void Log(string log){
            if(Enable)
                Debug.Log(log);
        }
    }
}