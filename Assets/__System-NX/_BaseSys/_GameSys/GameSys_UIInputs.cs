using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAXS.Event;

public class GameSys_UIInputs : MonoBehaviour
{
    public Canvas ChatCanvas;
    public InputField UsernameInput;
    
    public void SendChatInput(string input){
        NXEvent.SetData("OnChatInput", input);
        NXEvent.EmitEvent("OnChatInput");
    }
    
    public void SendUsernameInput(string input){
        NXEvent.SetData("OnUsernameInput", input);
        NXEvent.EmitEvent("OnUsernameInput");
    }


}
