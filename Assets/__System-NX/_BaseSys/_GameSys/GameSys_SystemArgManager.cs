using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class GameSys_SystemArgManager : MonoBehaviour
{
    [System.Serializable]
    public struct SysArg{
        public string Arg;
        public UnityEvent Callback;
    }
    public List<SysArg> SysArgs;

    void Awake()
    {
        foreach(SysArg thisArg in SysArgs){
            if(GetCommandArg(thisArg.Arg) != null){
                thisArg.Callback.Invoke();
            }
        }
    }
    
    private string GetCommandArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    public void TestEvent(int idx){
        SysArgs[idx].Callback.Invoke();
    }
        
}
