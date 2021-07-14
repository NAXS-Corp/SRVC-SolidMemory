using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class NXTool_SceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.S)){
            if(Input.GetKeyDown(KeyCode.Alpha1)){
                SceneManager.LoadScene(0);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)){
                SceneManager.LoadScene(1);
            }
        }
    }
}
