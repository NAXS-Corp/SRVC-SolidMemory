using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXDev_ObjectState : MonoBehaviour
{
    #if UNITY_EDITOR
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        Debug.Log("OnEnable "+gameObject.name);
    }
    void OnDisable()
    {
        Debug.Log("OnDisable "+gameObject.name);
    }
    #endif
}
