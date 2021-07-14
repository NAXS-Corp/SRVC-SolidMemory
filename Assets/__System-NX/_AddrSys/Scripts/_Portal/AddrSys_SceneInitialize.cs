using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class AddrSys_SceneInitialize : MonoBehaviour
{
    public string LobbyScene;


    public void LoadInitialScene(){
        AddrSys_SceneManager.instance.LoadScene(LobbyScene);
    }

    void Update()
    {
        
    }
}
