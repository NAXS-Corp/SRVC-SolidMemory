using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AddrSys_SimpleScLoader : MonoBehaviour
{
    public bool LoadOnStart;
    public string TargetSceneCode = "Lv0";
    // Start is called before the first frame update
    void Start()
    {
        if (LoadOnStart)
            LoadTargetScene();
    }
    public void LoadTargetScene()
    {
        if (AddrSys_SceneManager.instance)
        {
            AddrSys_SceneManager.instance.LoadScene(TargetSceneCode);
        }
    }

    public void LoadTargetSceneDummy(string dummyCode)
    {
        AddrSys_SceneManager.instance.LoadScene(TargetSceneCode);
    }


    public void LoadSceneCode(string sceneCode)
    {
        AddrSys_SceneManager.instance.LoadScene(sceneCode);
    }

    public void GameStartInputEnabled(bool enable)
    {
        NXEvent.SetData("VRGamestart", enable);
        NXEvent.EmitEvent("VRGamestart");
    }
}
