using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using SimpleJSON;
using NAXS.Event;
using BestHTTP.ServerSentEvents;


[Serializable]
public class ChatMsg{
    // public string ServerId;
    public string MsgId;
    public int Datetime;
    public string Message;
    public string UserId;
    public ChatMsg (string MsgId, int Datetime, string Message, string UserId){
        this.MsgId = MsgId;
        this.Datetime = Datetime;
        this.Message = Message;
        this.UserId = UserId;
    }
    public ChatMsg (int Datetime, string Message, string UserId){
        this.Datetime = Datetime;
        this.Message = Message;
        this.UserId = UserId;
    }
}

public class Firebase_Chatroom : Firebase_CtrlBase
{
    [Header("UI Element")]
    public Text ChatroomText;
    public Text DebugText;
    [Header("UI Setting")]
    public Color IDColor;
    private string colorFormat;
    private string colorFormatEnd ="]</color>  ";

    [Header("Basic Setting")]
    public bool GetOnInitialize = true;
    public int MsgNumOnStart = 30;
    public int MsgNumOnUpdate = 3;
    public int MsgNumMax = 60;
    [Header("SSE")]
    EventSource eventSource;
    public bool AutoInitSSE;
    public string SSEUri = "https://naxs-pf-dev.firebaseio.com/Chatroom/AF-EV20f1.json?orderBy=%22$key%22&limitToLast=20";
    public int ReadBuffer = 0;
    // [Header("Messages")]
    // public List<ChatMsg> ChatMsgList;


    protected override void Initialize(){
        // InGame Event Listener
        NXEvent.StartListening("OnUsernameInput", ()=>{
            OnUsernameInput(NXEvent.GetString("OnUsernameInput"));
        });

        NXEvent.StartListening("OnChatInput", ()=>{
            OnChatInput(NXEvent.GetString("OnChatInput"));
        });

        ChatroomText.text = "";
        colorFormat = "<color=#"+ ColorUtility.ToHtmlStringRGB(IDColor) +">[";

        TargetDB = Firebase_Manager.instance.firebase.Child(DBPath);
        SSE_Initialize();
    }

    void SSE_Initialize(){
        Debug.Log("== FB SSE_Initialize ");
        this.eventSource = new EventSource(new Uri(SSEUri), ReadBuffer);
        this.eventSource.OnOpen +=  SSE_OnOpen;
        this.eventSource.On("put",  SSE_OnPut);
        this.eventSource.On("patch", OnPatch);
        this.eventSource.Open();
    }

    // //////////////////////////////////
    // Server Sent Events

    void  SSE_OnPut(EventSource source, Message msg){
        Debug.Log(string.Format("== FB OnPut: <color=yellow>{0}</color>", msg.Data.ToString()));
        OnFBGetUpdate(msg.Data.ToString());
    }
    void OnPatch(EventSource source, Message msg){
        Debug.Log("== FB OnPatch "+msg.Data);
    }
    
    private void  SSE_OnOpen(EventSource eventSource)
    {
        Debug.Log("== FB OnOpen");
    }

    private void  SSE_OnClosed(EventSource eventSource)
    {
        Debug.Log("== FB OnClose");
    }

    private void OnError(EventSource eventSource, string error)
    {
        Debug.Log(string.Format("== FB Error: <color=red>{0}</color>", error));
    }

    private void OnStateChanged(EventSource eventSource, States oldState, States newState)
    {
        Debug.Log(string.Format("== FB State Changed {0} => {1}", oldState, newState));
    }

    private void OnMessage(EventSource eventSource, Message message)
    {
        Debug.Log(string.Format("== FB Message: <color=yellow>{0}</color>", message));
    }

    // ///////////////////////////////////////////
    // ///////////////////////////////////////////
    // FB Callbacks


    void OnFBGetUpdate(string newMsg){
        JSONNode parsed = JSON.Parse(newMsg);        

        if(parsed["data"]["Message"]){
            // Got only one message
            StringBuilder textBuilder = new StringBuilder();
            // textBuilder.Append(string.Concat("[", parsed["data"]["UserId"], "] ", parsed["data"]["Message"]));
            // textBuilder.Append("\n");
            ChatroomText.text = string.Concat(ChatroomText.text, FormatMessage(parsed["data"]["UserId"], parsed["data"]["Message"]));
        }else{
            //Got Multiple messages, parse to list
            // var newMsgs = new List<ChatMsg>();
            StringBuilder textBuilder = new StringBuilder();
            foreach(JSONNode p in parsed["data"]){
                if(!p["Message"]){
                    //skip node without message content
                    continue;
                }
                Debug.Log("== FB Msg "+p["Message"]+" "+p["UserId"]);
                // textBuilder.Append(string.Concat("[", p["UserId"], "] ", p["Message"]));
                // textBuilder.Append(string.Concat(colorFormat, p["UserId"], colorFormatEnd, p["Message"]));
                // textBuilder.Append("\n");
                textBuilder.Append(FormatMessage(p["UserId"], p["Message"]));
            }
            ChatroomText.text = string.Concat(ChatroomText.text, textBuilder.ToString());
        }
    }

    string FormatMessage(string id, string message){
        return string.Concat(colorFormat, id, colorFormatEnd, message, "\n");
    }



    // //////////////////////
    // //////////////////////
    // JSON Formatting
    List<ChatMsg> DecodedList(string rawJson){
        
        //Json parsing
        JSONNode parsed = JSON.Parse(rawJson);

        //parse to list
        var newMsgs = new List<ChatMsg>();
        foreach(JSONNode p in parsed){
            int datetime;
            int.TryParse(p["Datetime"], out datetime);
            newMsgs.Add(new ChatMsg(datetime, p["Message"], p["UserId"]));
        }
        return newMsgs;
    }
    void ConvertListToText(List<ChatMsg> newList, bool append){
        StringBuilder textBuilder = new StringBuilder();

        foreach(ChatMsg msg in newList){
            textBuilder.Append(string.Concat(colorFormat,"[", msg.UserId, "] ", msg.Message, colorFormatEnd));
            textBuilder.Append("\n");
        }

        if(append){
            ChatroomText.text = string.Concat(ChatroomText.text, textBuilder.ToString());
        }else{
            ChatroomText.text = textBuilder.ToString();
        }
    }

    ////////////////////////////////
    ////////////////////////////////
    // Send Message

    [ShowInInspector]private string m_UserId = "anon";

    void OnUsernameInput(string userId){
        m_UserId = userId;
        Debug.Log("OnLocalUsernameInput " +userId);
    }

    void OnChatInput(string chatMessage){
        SendChatMessage(chatMessage);
    }

    public void SendChatMessage(string Message){
        string jsonStr = "{ \"UserId\": \""+ m_UserId +"\", \"Message\": \""+ Message +"\", \"Datetime\": \""+ System.DateTime.UtcNow.Ticks +"\"}";
        TargetDB.Push(jsonStr, true);
    }

    ////////////////////////////////
    ////////////////////////////////
    // Debug 

    // void Debug.Log(string log){
    //     Debug.Log(log);
    //     if(DebugText){
    //         DebugText.text = string.Concat(DebugText.text, "\n", log);
    //     }
    // }
}