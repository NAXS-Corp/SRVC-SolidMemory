using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidM_Gaze : MonoBehaviour
{
    public LayerMask TargetMask;
    public float RayDistance = 10;
    private SolidM_UICtrl lastFocused;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + transform.forward * RayDistance, Color.green);
        if (Physics.Raycast(transform.position, transform.forward, out hit, RayDistance, TargetMask, QueryTriggerInteraction.Collide))
        {
            SolidM_UICtrl currentFocus;
            Debug.Log("SolidM_Gaze gazing " + hit.transform.gameObject.name);
            Debug.Log("SolidM_Gaze gazing2 " + hit.collider.gameObject.name);
            if (currentFocus = hit.transform.GetComponent<SolidM_UICtrl>())
            {
                Debug.Log("SolidM_Gaze OnTriggered");
                currentFocus.OnFocused();
                if (lastFocused && lastFocused != currentFocus)
                {
                    lastFocused.OnTriggerStop();
                }
                lastFocused = currentFocus;
            }
        }


    }
}
