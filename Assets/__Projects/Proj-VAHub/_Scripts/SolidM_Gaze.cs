using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidM_Gaze : MonoBehaviour
{
    public LayerMask TargetMask;
    public float RayDistance = 10;
    private SolidM_UICtrl currentFocus;
    private SolidM_UICtrl lastFocused;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, RayDistance, TargetMask, QueryTriggerInteraction.Collide))
        {
            currentFocus = hit.transform.GetComponent<SolidM_UICtrl>();
            Debug.DrawLine(transform.position, transform.position + transform.forward * RayDistance, Color.green);
        }
        else
        {
            currentFocus = null;
            Debug.DrawLine(transform.position, transform.position + transform.forward * RayDistance, Color.red);
        }


        if (currentFocus)
        {
            Debug.Log("SolidM_Gaze 1");
            currentFocus.OnFocused();
        }

        if (!currentFocus && lastFocused)
        {
            lastFocused.OnFocusStop();
        }

        lastFocused = currentFocus;
    }

}
