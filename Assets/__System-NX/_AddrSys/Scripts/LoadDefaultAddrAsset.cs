using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class LoadDefaultAddrAsset : MonoBehaviour
{

    public AssetReference m_Asset;
    bool m_AssetReady = false;
    //int m_ToLoadCount;

    // Start is called before the first frame update
    void Start()
    {
        //m_ToLoadCount = m_Assets.Count;

        //foreach (var Assets in m_Assets)
        //{
            m_Asset.LoadAssetAsync<GameObject>().Completed += OnAssetLoaded;
        //}
    }

    private void OnAssetLoaded(AsyncOperationHandle<GameObject> obj)
    {
        //m_ToLoadCount--;

        //if (m_ToLoadCount <= 0)
            m_AssetReady = true;
            Spawn();
    }

    void Spawn()
    {
        if (m_AssetReady)
        {
            m_Asset.InstantiateAsync(new Vector3(0f, 0f, 5f), Quaternion.identity);
            //var scene = m_Asset;
            //SceneManager.LoadScene(scene.name);
        }
    }
}
