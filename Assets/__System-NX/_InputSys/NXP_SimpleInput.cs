using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using NAXS.NXPlayer;
using Cinemachine;

public class NXP_SimpleInput : MonoBehaviour
{
    [ReadOnly]public Camera MainCam;
    public GameObject RaycastTarget;
    private Vector3 m_RaycastPos;
    public LayerMask GroundMask;
    public GameObject NXPlayer;
    public NXP_Movement m_NXPMovement;
    private float pressCounter;
    // public float mouseSensitivity = 100f;
    // Start is called before the first frame update

    [Button]
    public void Initialize(){
        FindMainCam();
    }

    void Start()
    {
        Initialize();
    }

    void FindMainCam()
    {
        MainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseRaycast();
        UpdateMouseMoveInput();
    }

    void UpdateMouseRaycast()
    {
        Ray raycast = MainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(raycast, out RaycastHit raycastHit, float.MaxValue, GroundMask, QueryTriggerInteraction.Ignore)){
            if (!RaycastTarget.activeSelf)
            {
                RaycastTarget.SetActive(true);
            }
            RaycastTarget.transform.position = raycastHit.point;
            m_RaycastPos = RaycastTarget.transform.position;
        }
        else
        {
            RaycastTarget.SetActive(false);
        }

        RaycastTarget.transform.Rotate(0, GetAnglev3(), 0);
    }

    float GetAnglev3()
    {
        Vector3 relative = RaycastTarget.transform.InverseTransformPoint(NXPlayer.transform.position);
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        return angle;
    }

    void UpdateMouseMoveInput() {

        if (Input.GetMouseButton(0))
        {
            pressCounter += Time.deltaTime;
            // Debug.Log("pressCounter: "+pressCounter);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pressCounter < .4f)
            {
                if (RaycastTarget.activeSelf)
                {
                    if (m_RaycastPos != null)
                    {
                        // Debug.Log("SimpleInput>> MoveTo.");
                        m_NXPMovement.MoveTo(m_RaycastPos, MainCam.transform);
                    }
                }
            }
            pressCounter = 0;
        }
    }

}
