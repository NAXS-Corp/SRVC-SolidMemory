using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAXS.Event;

namespace NAXS.UI
{
    public class UI_EmitInputEvent : MonoBehaviour
    {

        public void SendChatInput(string input)
        {
            if(string.IsNullOrEmpty(input)) return;
            NXEvent.SetData("OnChatInput", input);
            NXEvent.EmitEvent("OnChatInput");
        }

        public void SendUsernameInput(string input)
        {
            if(string.IsNullOrEmpty(input)) return;
            NXEvent.SetData("OnUsernameInput", input);
            NXEvent.EmitEvent("OnUsernameInput");
        }


    }
}