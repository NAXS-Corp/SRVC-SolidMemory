using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXP_SimpleCameraLook : MonoBehaviour
{

    public float mouseSensitivityOnDrag = 100f;
    public float mouseSensitivityOnStatic = 10f;
    public Transform NXP_Player;
    float xRotation = 0f;
    float yRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CameraDrag(mouseSensitivityOnDrag);
        }
    }

    void CameraDrag(float MouseSpeed)
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSpeed * Time.deltaTime;
        float mosueY = Input.GetAxis("Mouse Y") * MouseSpeed * Time.deltaTime;

        xRotation -= mosueY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        if(NXP_Player)
            NXP_Player.Rotate(Vector3.up * -mouseX);
    }
}
