using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class NXPDev_LoadAddrScene : MonoBehaviour
{

    public string m_SceneAddressToLoad;
    public string m_tag = "LocalPlayer";

    private void OnTriggerEnter(Collider other) {
        if (other.tag == m_tag)
        {
            Addressables.LoadSceneAsync(m_SceneAddressToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += OnSceneLoaded;
        }
    }

    void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {

    }
}
