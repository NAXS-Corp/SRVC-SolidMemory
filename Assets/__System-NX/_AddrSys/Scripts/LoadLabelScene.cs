using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class LoadLabelScene : MonoBehaviour
{
    public AssetLabelReference m_AssetLabel;

    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetsAsync<Scene>(m_AssetLabel, null).Completed += OnResourcesRetrieved;
    }

    private void OnResourcesRetrieved(AsyncOperationHandle<IList<Scene>> objs)
    {
        
        if (objs.Result != null)
        {
            foreach (var obj in objs.Result)
            {
                SceneManager.LoadSceneAsync(obj.name);
            }
        }
    }

}
