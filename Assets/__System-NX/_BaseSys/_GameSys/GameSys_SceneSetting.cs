using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.Event;
public class GameSys_SceneSetting : MonoBehaviour
{
    public float DelayActivation = 0.5f;
    public string ScCode= "Lv0";
    public int PlayerBodyType;
    public int ServerZoneID;

    void Start()
    {
        if(DelayActivation > 0)
            StartCoroutine(ActivateDelay());
        else{
            Activate();
        }
    }


    IEnumerator ActivateDelay(){
        yield return new WaitForSeconds(DelayActivation);
        Activate();
    }

    void Activate(){
        Debug.Log("GameSys_SceneSetting START:: "+ScCode+" / "+PlayerBodyType+" / zoneId "+ServerZoneID);
        
        if(Firebase_ClientCtrl.instance){
            Debug.Log("ZoneChange Firebase_ClientCtrl");
            Firebase_ClientCtrl.instance.ExecuteChangeZone(ServerZoneID);
        }

        Debug.Log("ZoneChange NXEvent ChangeServerZoneID");
        NXEvent.SetData("ChangeServerZoneID", ServerZoneID);
        NXEvent.EmitEvent("ChangeServerZoneID");


        
        NXEvent.SetData("ChangeServerZoneID", ServerZoneID);
        NXEvent.EmitEvent("ChangeServerZoneID");


        // Debug.Log("GameSys_SceneSetting START2:: "+ScCode+" / "+PlayerBodyType+" / "+ServerZoneID);

        NXEvent.SetData("OnScLoaded", ScCode);
        NXEvent.EmitEvent("OnScLoaded");

        NXEvent.SetData("ChangePlayerBodyType", PlayerBodyType);
        NXEvent.EmitEvent("ChangePlayerBodyType");

    }

}
