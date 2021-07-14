using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddrSys_GameManager : MonoBehaviour
{
    public static AddrSys_GameManager instance;

    // Start is called before the first frame update
    public void Awake()
    {
        if(!this.enabled)
            return;
        if(!gameObject.activeSelf)
            return;
        

        Debug.Log("!!! AddrSys_GameManager Awake");
        if (AddrSys_GameManager.instance == null)
        {
            AddrSys_GameManager.instance = this;
            Debug.Log(instance);
            DontDestroyOnLoad(this);
        }
        else if (this!= AddrSys_GameManager.instance)
        {
            Debug.Log("!instance");
            DestroyImmediate(this.gameObject);
        }
    }

}
