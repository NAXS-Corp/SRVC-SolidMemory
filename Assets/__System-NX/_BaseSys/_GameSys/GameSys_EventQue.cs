using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventQue{
    public float Delay;
    public UnityEvent Event;
}

public class GameSys_EventQue : MonoBehaviour
{
    public bool AutoStart;
    public List<EventQue> EventQues;
    private int currentQueIdx = 0;
    
    void OnEnable()
    {
        if(AutoStart)
            StartEventQue();
    }

    public void StartEventQue(){
        currentQueIdx = 0;
        StartCoroutine("Que");
    }

    IEnumerator Que(){
        Debug.Log("Start Event Que");
        if(currentQueIdx >= EventQues.Count){
            // StopCoroutine("Que");
            Debug.Log("Event Que Stopped");
            yield break;
        }
        if(EventQues[currentQueIdx] != null){
            Debug.Log("Event Que wait"+EventQues[currentQueIdx].Delay);
            if(EventQues[currentQueIdx].Delay > 0)
                yield return new WaitForSeconds(EventQues[currentQueIdx].Delay);
            Debug.Log("Event Que "+currentQueIdx);
            EventQues[currentQueIdx].Event.Invoke();
            
            currentQueIdx += 1;
            StartCoroutine("Que");
        }
        else{
            currentQueIdx += 1; // skip current que
        }
    }
}
