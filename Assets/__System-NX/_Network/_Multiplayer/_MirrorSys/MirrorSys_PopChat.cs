using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using NAXS.Event;
using NAXS.Base;
using TMPro;

[ExecuteInEditMode]
public class MirrorSys_PopChat : NetworkBehaviour
{
    public static string LOCAL_USERNAME;
    // public bool ShowInput;
    // public InputField ChatInput;
    // public Transform TextPivot;
    // public float PlayerHeightOffset = 1.2f;
    // public Vector2 BaseOffset = new Vector2(0, 0);
    // public Vector2 ChatTextOffset = new Vector2(0, 10);

    public Transform TextPivot;
    public TextMeshPro UsernameTMP;
    public TextMeshPro ChatTMP;
    public float HideTextDistance = 20f;
    public float LineDistance = 20f;
    public string m_PopString;

    float currentDist;
    Transform MainCamTransform;
    Camera MainCam;
    [SyncVar(hook = nameof(OnGetRemoteUsername))] public string m_Username;

    void Start()
    {
#if UNITY_EDITOR
        Debug.Log("MirrorSys_Popchat Start "+m_Username);
#endif
        // Reset
        if (ChatTMP) ChatTMP.text = "";
        if (UsernameTMP) UsernameTMP.text = "";

#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if(isClient && !string.IsNullOrEmpty(m_Username)){
            SetUsernameDisplay(m_Username);
        }
    }

    public override void OnStartServer()
    {
        UsernameTMP.gameObject.SetActive(false);
        ChatTMP.gameObject.SetActive(false);
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
#if UNITY_EDITOR
        Debug.Log("MirrorSys_Popchat OnStartClient "+m_Username);
#endif
        MainCam = Camera.main;
        MainCamTransform = MainCam.transform;

        if (isLocalPlayer)
        {
            NXEvent.StartListening("OnUsernameInput", OnLocalUsernameInput);
            NXEvent.StartListening("OnChatInput", OnChatInput);

            //Already has an username
            if (!string.IsNullOrEmpty(LOCAL_USERNAME))
            {
                //set username display and sync to all clients
                SetLocalUsername(LOCAL_USERNAME);
            }
        }
        else
        {
            // Is remote client, try to get its name
#if UNITY_EDITOR
            Debug.Log("MirrorSys_Popchat m_Username:: " + m_Username);
#endif
            if (!string.IsNullOrEmpty(m_Username))
            {
                SetUsernameDisplay(m_Username);
            }
        }
    }

    void FixedUpdate()
    {
        if (!isClient) return;

        #if !UNITY_SERVER
            Vector3 forward = MainCamTransform.position - TextPivot.position;
            forward = new Vector3(forward.x, 0, forward.z); // Ignore Y difference
            if (forward != Vector3.zero)
                TextPivot.rotation = Quaternion.LookRotation(forward, Vector3.up);
        #endif
    }

    IEnumerator DistanceCheck()
    {
        while (true)
        {
            currentDist = Vector3.Distance(transform.position, MainCamTransform.position);
            yield return new WaitForSeconds(1f);
        }
    }

    // void OnGUI ()
    // {
    //     if(!isClient) return;

    //     if(currentDist > LineDistance){

    //     }

    //     if(currentDist < HideTextDistance){
    //         Vector2 worldPoint = MainCam.WorldToScreenPoint (TextPivot.position);
    //         worldPoint += BaseOffset;
    //         GUI.Label (new Rect (worldPoint.x - 100, (Screen.height - worldPoint.y) - 50, 200, 100), m_Username);

    //         worldPoint += ChatTextOffset;
    //         GUI.Label (new Rect (worldPoint.x - 100, (Screen.height - worldPoint.y) - 50, 200, 100), m_PopString);
    //     }
    // }

    #region username
    ////////////////////////////
    ////////////////////////////
    // Username Function
    void OnLocalUsernameInput()
    {
        var name = NXEvent.GetString("OnUsernameInput");
        SetLocalUsername(name);
    }

    public void SetLocalUsername(string name)
    {
        if (!isLocalPlayer) return;
        MirrorSys_PopChat.LOCAL_USERNAME = name;
        CmdSendUsername(LOCAL_USERNAME);
        SetUsernameDisplay(LOCAL_USERNAME);
    }


    [Command]
    // Execute on server only
    public void CmdSendUsername(string name)
    {
        if (name.Trim() != "")
            m_Username = name; //set syncvar
    }

    // Syncvar hook, execute on all clients
    void OnGetRemoteUsername(string oldVal, string newVal)
    {
#if UNITY_EDITOR
        Debug.Log("MirrorSys_Popchat OnGetRemoteUsername:: " + newVal);
#endif
        SetUsernameDisplay(newVal);
    }

    void SetUsernameDisplay(string name)
    {
#if UNITY_EDITOR
        Debug.Log("MirrorSys_Popchat SetUsernameDisplay:: " + name);
#endif
        m_Username = name;
        UsernameTMP.text = name;
        UsernameTMP.gameObject.SetActive(true);
    }
    #endregion


    #region chat
    ////////////////////////////
    ////////////////////////////
    // Chat Function
    void OnChatInput()
    {
        var chatMessage = NXEvent.GetString("OnChatInput");
        GetChatInput(chatMessage);
    }

    void GetChatInput(string chatMessage)
    {
        Debug.Log("SendChat " + chatMessage);
        CmdSendChat(chatMessage);
        SetPopText(chatMessage);
    }

    void SetPopText(string chatMessage)
    {
        m_PopString = chatMessage;
        if (ChatTMP)
        {
            StopCoroutine(HideChat());
            ChatTMP.text = chatMessage;
            ChatTMP.gameObject.SetActive(true);
            StartCoroutine(HideChat());
        }

    }

    IEnumerator HideChat()
    {
        yield return new WaitForSeconds(20);
        ChatTMP.gameObject.SetActive(false);
    }


    // Network Function
    [Command]
    public void CmdSendChat(string message)
    {
        if (message.Trim() != "")
            RpcReceiveChat(message.Trim());
    }

    [ClientRpc]
    public void RpcReceiveChat(string message)
    {
        SetPopText(message);
    }


    void OnDisable()
    {
        // StopCoroutine(DistanceCheck());
    }
    #endregion
}
