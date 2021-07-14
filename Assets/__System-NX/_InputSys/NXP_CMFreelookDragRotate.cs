using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NXP_CMFreelookDragRotate : MonoBehaviour
{

    public Vector2 MouseDragStaticSpeed;
    public Vector2 LerpSpeed;
    private float lastMouseDragX;
    private float lastMouseDragY;

    // Start is called before the first frame update
    void Start()
    {
        CinemachineCore.GetInputAxis = GetMouseAxis;
    }

    public float GetMouseAxis(string m_axis)
    {
        if(m_axis == "Mouse Drag X"){
            if (Input.GetMouseButton(0)){
                return UnityEngine.Input.GetAxis("Mouse Drag X");
            }
            else
            {
                float tempDragx;
                if (UnityEngine.Input.mousePosition.x == 0 || UnityEngine.Input.mousePosition.x == Screen.width)
                {
                    tempDragx = 0;
                }
                else
                {
                    tempDragx = UnityEngine.Input.GetAxis("Mouse Drag X") * MouseDragStaticSpeed.x;
                }

                lastMouseDragX = tempDragx;
                return Mathf.Lerp(lastMouseDragX, tempDragx, Time.deltaTime* LerpSpeed.x);

            }
        }
        else if(m_axis == "Mouse Drag Y")
        {
            if (Input.GetMouseButton(0)){
                return UnityEngine.Input.GetAxis("Mouse Drag Y");
            }
            else
            {
                float tempDragy;
                if (UnityEngine.Input.mousePosition.y == 0 || UnityEngine.Input.mousePosition.y == Screen.height)
                {
                    tempDragy = 0;
                }
                else
                {
                    tempDragy = UnityEngine.Input.GetAxis("Mouse Drag Y") * MouseDragStaticSpeed.y;
                }
                
                lastMouseDragY = tempDragy;
                return Mathf.Lerp(lastMouseDragY, tempDragy, Time.deltaTime* LerpSpeed.y);
            }
        }
        
        return UnityEngine.Input.GetAxis(m_axis);
    }

}
