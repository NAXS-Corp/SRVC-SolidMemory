using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;
using UnityEngine.Events;

public class NXEventListener : MonoBehaviour
{
    public string EventName = "ChangeServerZoneID";
    public int EventDataInt;
    public UnityEvent OnGetMessage;
    // Start is called before the first frame update
    void Start()
    {
        NXEvent.StartListening(EventName, OnInvoked);
    }

    void OnInvoked()
    {
        EventDataInt = NXEvent.GetInt(EventName);
        OnGetMessage.Invoke();
    }

    void OnDisable()
    {
        NXEvent.StopListening(EventName, OnInvoked);
    }
}