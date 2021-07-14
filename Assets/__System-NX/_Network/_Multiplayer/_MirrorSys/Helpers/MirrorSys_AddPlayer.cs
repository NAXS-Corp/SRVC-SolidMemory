using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorSys_AddPlayer : MonoBehaviour
{
    public GameObject OfflinePlayer;
    public float DelayTime;
    public bool AddOnStart;
    // Start is called before the first frame update
    void Start()
    {
        if (AddOnStart)
        {
            AddPlayer();
        }
    }

    public void AddPlayer()
    {
        if(MirrorSys_NetPlayerManager.singleton)
        {
            MirrorSys_NetPlayerManager.singleton.AddOrActivePlayer();
            return;
        }
        if (DelayTime > 0)
            StartCoroutine(Delay());
        else
            InstantiatePlayer();
    }

    public void InstantiatePlayer()
    {
        if (ClientScene.ready)
        {
            Debug.Log("MirrorSys_AddPlayer: Add Online Player");
            ClientScene.AddPlayer(ClientScene.readyConnection);
        }
        else
        {
            Debug.Log("MirrorSys_AddPlayer: Add Offline Player");
            Instantiate(OfflinePlayer, transform.position, Quaternion.identity);
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(DelayTime);
        InstantiatePlayer();
    }
}
