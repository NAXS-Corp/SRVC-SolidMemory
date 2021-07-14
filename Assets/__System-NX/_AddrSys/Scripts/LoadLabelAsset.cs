using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class LoadLabelAsset : MonoBehaviour
{
    public IList<GameObject> m_Assets;

    public AssetLabelReference m_AssetLabel;

    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetsAsync<GameObject>(m_AssetLabel, null).Completed += OnResourcesRetrieved;
    }

    private void OnResourcesRetrieved(AsyncOperationHandle<IList<GameObject>> objs)
    {
         m_Assets = objs.Result;
         Spawn();
        //if (objs.Result != null){
            //foreach (var obj in objs.Result){
                // Debug.Log("OnResourcesRetrieved "+it.name);
                //Debug.Log(obj.name);
                //SceneManager.LoadSceneAsync(obj.name);
            //}
        //}
        // Debug.Log("OnResourcesRetrieved "+obj[0].name+" / "+ typeof(obj[0]).ToString());
    }

    // Update is called once per frame
    void Spawn()
    {
        float x_pos = 0;
        
        if (m_Assets != null)
        {
            foreach (var Asset in m_Assets)
            {
                Instantiate(Asset, new Vector3(x_pos, 3, 7), Quaternion.identity, null);
                x_pos += 3f;
                //SceneManager.LoadScene(Asset.name);
            }
        }
    }
}
