using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Mirror;

public class NXP_RemoteBodyCtrl : MonoBehaviour
{

    // public float VisibleDistance = 10f;
    // public float LightDist = 2f;
    public Light PlayerLight;
    public TextMesh _TextMesh;
    public Transform _TextContainer;
    Transform MainCam;
    // public NX
    // Start is called before the first frame update
    void Start()
    {
        MainCam = Camera.main.transform;
    }

    void OnEnable()
    {
        // FX_GLPlayers.instance.AddPlayer(transform);
    }

    void OnDisable()
    {
        
        // FX_GLPlayers.instance.RemovePlayer(transform);
    }

    void Update()
    {
        UpdateText();
    }

    void UpdateText(){
        //Update Text Rotation
        // if()
        Vector3 forward = MainCam.position - _TextContainer.position;
        forward = new Vector3(forward.x, 0, forward.z); // Ignore Y difference
        if(forward != Vector3.zero)
            _TextContainer.rotation = Quaternion.LookRotation(forward, Vector3.up);

        _TextMesh.text = string.Concat(" ░ -/  ", (int)(transform.position.x * 1000000), ",   ", (int)(transform.position.y * 1000000),",   ", (int)(transform.position.z * 1000000));
        // _TextMesh.text = string.Concat(transform.position.x*transform.position.y*transform.position.z);
    }
}
