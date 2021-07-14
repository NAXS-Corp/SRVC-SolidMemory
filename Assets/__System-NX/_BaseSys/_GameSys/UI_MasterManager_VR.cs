using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;

public class UI_MasterManager_VR : MonoBehaviour
{
    private GameObject VRUIoffset;
    public Vector3 posOffset;
    Transform MainCam;
    // Start is called before the first frame update
    void Start()
    {
        VRUIoffset = transform.parent.gameObject;
        MainCam = Camera.main.transform;

        RelocateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable() {
        RelocateUI();
    }

    void RelocateUI(){

        //Sync Y
        VRUIoffset.transform.position = new Vector3(MainCam.position.x + posOffset.x, MainCam.position.y + posOffset.y, MainCam.position.z + posOffset.z);

        VRUIoffset.transform.rotation = Quaternion.Euler(VRUIoffset.transform.rotation.eulerAngles.x, MainCam.rotation.eulerAngles.y, VRUIoffset.transform.rotation.eulerAngles.z);
    }
}
