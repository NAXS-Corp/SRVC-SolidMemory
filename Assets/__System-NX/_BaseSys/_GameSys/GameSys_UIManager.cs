// Separate Map, LoginUI to module later.

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using NAXS.Event;
using UnityEngine.Events;
using NAXS.NXHelper;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSys_UIManager : MonoBehaviour
{
    public static GameSys_UIManager instance;

    [Header("UI Elements")]
    public CanvasGroup MasterUIGroup;
    public Text GlobalNotiText;
    public Text LiveSceneText;
    private bool isMaterUIShowed;
    private bool LoginUIState;
    public string InfoURL;
    public string LiveStreamingURL;
    public string DiscordURL;
    private bool CheckMapUI = true;
    private bool CheckInstructionsUI;
    public Button MapButton;
    public Button InstructionsButton;
    public Button AboutButton;
    public CanvasGroup MapUI;
    public CanvasGroup InstructionsUI;
    public CanvasGroup AboutUI;
    public CanvasGroup NotificationUI;
    private Button CurrentButton;
    private CanvasGroup CurrentCanvasGroup;

    [Header("LoadingBar")]
    // public Text LoadingText;
    // public RectTransform LoadingBar;
    // private float loadingBarWidth = 800;
    public CanvasGroup LoadingUI;


    [Header("UI Map")]
    public Text MapTitleUI;
    public Text MapArtistUI;
    public Text MapTimeUI;
    public Image MapDividerUI;
    private Button CheckPreMapButton;
    private string CheckPreMapButtonName;
    private string CheckPreMapSelectedButtonName;
    public UIMap[] MapButtonSettings;
    private string ScCode;

    [Serializable]
    public struct UIMap
    {
        public Button MapButton;
        public string MapTitle;
        public string MapArtist;
        public string MapTime;
    }

    public Firebase_ObjList ServerMapObjList;

    [Header("Login UI")]
    public CanvasGroup LoginUI;
    public LoginUIList[] LoginUISettings;
    private int CheckLoginUI = 0;
    [Serializable]
    public struct LoginUIList
    {
        public string Id;
        public Image PlayLog;
    }

    [Header("Event")]
    public StringEvent OnSceneChanged;

    [Header("UX Setting")]
    public bool ShowUIOnTab = true;
    public bool ToggleUI = true;
    public bool HideCursorOnStart = false;

    private string LiveScCode;
    // public bool FreeCursorOnEsc;
    // public bool LockCursorOnClick;
    // bool anyUIShowed;

    [Header("Chat")]
    public InputField ChatInput;
    public Canvas ChatInputCanvas;
    bool ChatUIState = false;

    void Start()
    {
        allowChangeScene = true;
        GameSys_UIManager.instance = this;
        // ServerMapObjList = gameObject.GetComponent<Firebase_ObjList>();

        CurrentButton = MapButton;
        CurrentCanvasGroup = MapUI;

        SetChatUISate(false);
        HideMasterUI();
        // ShowLoginUI();

        // if(HideCursorOnStart)
        // HideCursor();

        PageButtonListener();
        StartEventListeners();
    }

    public void OnGetPortalUpdate()
    {
        Dictionary<string, object> portalStates = ServerMapObjList.ObjStates;
        // List<string> keys = portalStates.Keys;
        // foreach (string key in portalStates.Keys){
        //     Debug.Log(key);
        //     Debug.Log(portalStates[key]);
        // }
        foreach (var maplist in MapButtonSettings)
        {
            var btnName = maplist.MapButton.name;
            if (portalStates.ContainsKey(btnName))
            {
                // maplist
                // portalStates[btnName] : bool
                var state = Convert.ToBoolean(portalStates[btnName]);
                if (state)
                {
                    maplist.MapButton.interactable = true;
                }
                else
                {
                    maplist.MapButton.interactable = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HotKeyUpdate();
    }

    float enterHoldCounter;
    bool allowChangeScene = true;


    //////////////////////////////
    //////////////////////////////
    // Events

    public void PageButtonListener()
    {
        MapButton.onClick.AddListener(() => PageCtrl(MapUI, MapButton));
        InstructionsButton.onClick.AddListener(() => PageCtrl(InstructionsUI, InstructionsButton));
        AboutButton.onClick.AddListener(() => PageCtrl(AboutUI, AboutButton));
    }

    public void ShowCanvasGroup(CanvasGroup GroupUI, float FadeLength)
    {
        GroupUI.gameObject.SetActive(true);
        GroupUI.DOFade(1, FadeLength);
    }

    public void HideCanvasGroup(CanvasGroup GroupUI, float FadeLength)
    {
        GroupUI.DOFade(0, FadeLength).OnComplete(() =>
        {
            GroupUI.gameObject.SetActive(false);
        });
    }

    public void PageCtrl(CanvasGroup PageGroupUI, Button PageButtonUI)
    {
        // Turn off previous GroupUI & ButtonUI
        if (CurrentCanvasGroup != null && CurrentButton != null)
        {
            Debug.Log("PageCtrl>> Hide Current.");
            HideCanvasGroup(CurrentCanvasGroup, 0.5f);
            CurrentButton.interactable = true;
        }
        ShowCanvasGroup(PageGroupUI, 0.5f);
        PageButtonUI.interactable = false;
        CurrentCanvasGroup = PageGroupUI;
        CurrentButton = PageButtonUI;
    }

    void StartEventListeners()
    {
        NXEvent.StartListening("OnScLoaded", OnScLoaded);
        NXEvent.StartListening("LoadingData", () =>
        {
            ShowCanvasGroup(LoadingUI, 4f);
        });
        NXEvent.StartListening("CompletedLoadScene", () =>
        {
            HideCanvasGroup(LoadingUI, 6f);
        });
    }

    void OnScLoaded()
    {
        string scCode = NXEvent.GetString("OnScLoaded");
        OnSceneChanged.Invoke(scCode);
        ResetAllowChangeSc();
    }
    public void OnGetLiveScMessage(string scCode)
    {
        if (scCode == "-") return;
        LiveScCode = scCode;
        var colorNow = LiveSceneText.color;
        LiveSceneText.color = new Color(colorNow.r, colorNow.g, colorNow.b, 1f); //Set Alpha to 1
        LiveSceneText.DOFade(0, 5f).SetDelay(600f); // wait 300s and fade out in 5s

        //MapLiveColor
        OnGetMapLiveNotification(LiveScCode);
    }

    public void OnGetGlobalNotification(string notiText)
    {
        GlobalNotiText.text = notiText;
    }

    public void OnClickInfo()
    {
#if UNITY_EDITOR
        Application.OpenURL(InfoURL);
#endif
        Application.ExternalEval("window.open('" + InfoURL + "');");
    }

    public void OnClickStreaming()
    {
#if UNITY_EDITOR
        Application.OpenURL(LiveStreamingURL);
#endif
        Application.ExternalEval("window.open('" + LiveStreamingURL + "');");
    }

    public void OnClickDiscord()
    {
#if UNITY_EDITOR
        Application.OpenURL(DiscordURL);
#endif
        Application.ExternalEval("window.open('" + DiscordURL + "');");
    }

    //////////////////////////////
    //////////////////////////////
    // UI

    // public image LoadingBar
    float enterHoldPercentage;
    float holdTime = 3f;
    bool isHoldEnter = false;

    void HotKeyUpdate()
    {
        if (ShowUIOnTab)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (ToggleUI)
                {
                    ToggleUIState();
                }
                else
                {
                    ShowMasterUI();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!isMaterUIShowed && !LoginUIState)
            {
                ToggleChatInput();
            }

            if (LoginUIState)
            {
                LoginUINext();
            }

            // isHoldEnter = false;
            // ResetAllowChangeSc();
        }


        // if(Input.GetKey(KeyCode.Return)){
        //     if(allowChangeScene){
        //         isHoldEnter = true;
        //         enterHoldCounter += Time.deltaTime;
        //         Rect rect = LoadingBar.rect;
        //         float width = enterHoldCounter / holdTime;
        //         rect.width = width;
        //         LoadingBar.rect = rect;

        //         if(enterHoldCounter > holdTime){
        //             allowChangeScene = false;
        //             LoadingText.gameObject.SetActive(true);
        //             NXEvent.SetData("OnLoadScCode", LiveScCode);
        //             NXEvent.EmitEvent("OnLoadScCode");
        //             enterHoldCounter = 0;
        //         }
        //     }
        // } 
        // else {
        //     // allowChangeScene = true;
        //     // isHoldEnter = false;
        //     // enterHoldCounter = 0f;
        //     // LoadingBar.width = 0f;
        //     // LoadingText.gameObject.SetActive(false);
        // }
        //if hold enter for 3s
        // SimpleSceneLoader.LoadScCode(LiveScCode);
    }

    void ResetAllowChangeSc()
    {
        // allowChangeScene = true;
        // isHoldEnter = false;
        // enterHoldCounter = 0f;
        // LoadingBar.width = 0f;
        // LoadingText.gameObject.SetActive(false);
        // allowChangeScene = true;
    }

    public void SetChatUISate(bool state)
    {
        ChatUIState = state;
        if (ChatUIState)
        {
            ChatInputCanvas.gameObject.SetActive(true);
            ChatInput.text = "";
            FocusInput(ChatInput);
            ShowCursor();
        }
        else
        {
            ChatInput.DeactivateInputField();
            ChatInputCanvas.gameObject.SetActive(false);
            HideCursor();
        }
    }
    public void ToggleChatInput()
    {
        var newState = !ChatUIState;
        SetChatUISate(newState);
    }


    public void ToggleUIState()
    {
        if (isMaterUIShowed) HideMasterUI();
        else ShowMasterUI();
    }

    public void ShowMasterUI()
    {
        MasterUIGroup.gameObject.SetActive(true);
        MasterUIGroup.DOFade(1, 0.5f);
        isMaterUIShowed = true;
        ShowCursor();
        SetChatUISate(false);
    }

    public void HideMasterUI()
    {
        MasterUIGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            MasterUIGroup.gameObject.SetActive(false);
        });
        isMaterUIShowed = false;
        HideCursor();
        SetChatUISate(true);
    }

    // public void ShowMapUI(){
    //     if(CheckInstructionsUI){
    //         InstructionsUI.DOFade(0,0.5f).OnComplete(() => {
    //             InstructionsUI.gameObject.SetActive(false);
    //         });
    //         CheckInstructionsUI = false;
    //         InstructionsButton.interactable = true;
    //     }
    //     if(!CheckInstructionsUI){
    //         MapUI.gameObject.SetActive(true);
    //         MapUI.DOFade(1, 0.5f);
    //         CheckMapUI = true;
    //         MapButton.interactable = false;
    //     }        
    // }

    // public void ShowInstructionsUI() {
    //     if(CheckMapUI){
    //         MapUI.DOFade(0,0.5f).OnComplete(() => {
    //             MapUI.gameObject.SetActive(false);
    //         });
    //         CheckMapUI = false;
    //         MapButton.interactable = true;
    //     }
    //     if(!CheckMapUI){
    //         InstructionsUI.gameObject.SetActive(true);
    //         InstructionsUI.DOFade(1, 0.5f);
    //         CheckInstructionsUI = true;
    //         InstructionsButton.interactable = false;
    //     }  
    // }

    // Map Function (Separate later)
    public void ShowMapDetails(Button m_MapButton)
    {
        foreach (var maplist in MapButtonSettings)
        {
            if (m_MapButton == maplist.MapButton)
            {
                // OnEnter() event:
                MapTitleUI.text = maplist.MapTitle;
                MapTitleUI.DOFade(1, 0.3f);
                MapArtistUI.text = maplist.MapArtist;
                MapArtistUI.DOFade(1, 0.3f);
                MapTimeUI.text = maplist.MapTime;
                MapTimeUI.DOFade(1, 0.3f);
                MapDividerUI.DOFade(1, 0.3f);

                // OnClick() event:
                // MapTitleUI.DOFade(0,0.3f).OnComplete(() => {
                //     MapTitleUI.text = maplist.MapTitle;
                //     MapTitleUI.DOFade(1,0.3f);
                // });
                // MapArtistUI.DOFade(0,0.3f).OnComplete(() => {
                //     MapArtistUI.text = maplist.MapArtist;
                //     MapArtistUI.DOFade(1,0.3f);
                // });
                // MapTimeUI.DOFade(0,0.3f).OnComplete(() => {
                //     MapTimeUI.text = maplist.MapTime;
                //     MapTimeUI.DOFade(1,0.3f);
                // });
                // MapDividerUI.DOFade(0,0.3f).OnComplete(() => {
                //     MapDividerUI.DOFade(1,0.3f);
                // });

                // if (CheckPreMapButton = null) {
                //     CheckPreMapButton = m_MapButton;
                // }else{
                //     CheckPreMapButton.interactable = true;
                //     CheckPreMapButton = m_MapButton;
                // }

                // m_MapButton.interactable = false;
            }
        }
    }

    public void HideMapDetails(Button m_MapButton)
    {
        foreach (var maplist in MapButtonSettings)
        {
            if (m_MapButton == maplist.MapButton)
            {
                // OnEnter() event:
                MapTitleUI.DOFade(0, 0.3f);
                MapArtistUI.DOFade(0, 0.3f);
                MapTimeUI.DOFade(0, 0.3f);
                MapDividerUI.DOFade(0, 0.3f);
            }
        }
    }

    public void OnGetMapLiveNotification(string MapName)
    {
        NormalColorChange("#CC0033", MapName);
    }

    public void OnLoadMapScene(Button m_MapButton)
    {
        ScCode = m_MapButton.name;
        Debug.Log(ScCode);
        AddrSys_SceneManager.instance.LoadScene(ScCode);
    }

    public void CurrentMapColor(string ScCode)
    {
        DisabledColorChange("#0E1E6D", ScCode);
    }

    public void NormalColorChange(string ColorCode, string ScCode)
    {
        Color ChoosedMapColor = new Color();
        ColorUtility.TryParseHtmlString(ColorCode, out ChoosedMapColor);
        Color NormalColor = new Color();
        ColorUtility.TryParseHtmlString("#3F53F8", out NormalColor);
        foreach (var maplist in MapButtonSettings)
        {
            if (ScCode == maplist.MapButton.name)
            {
                ColorBlock MapColor = maplist.MapButton.colors;
                MapColor.normalColor = ChoosedMapColor;
                maplist.MapButton.colors = MapColor;
                // maplist.MapButton.colors.normalColor = ChoosedMapColor;

                if (CheckPreMapButtonName == null)
                {
                    CheckPreMapButtonName = ScCode;
                    Debug.Log("CheckPreMapColorButton_> " + CheckPreMapButtonName);
                }
                else
                {
                    Debug.Log("CheckPreMapColorButton_> 2call: " + CheckPreMapButtonName);
                    foreach (var premap in MapButtonSettings)
                    {
                        if (CheckPreMapButtonName == premap.MapButton.name)
                        {
                            ColorBlock PreMapButtonColor = premap.MapButton.colors;
                            PreMapButtonColor.normalColor = NormalColor;
                            premap.MapButton.colors = PreMapButtonColor;
                        }
                    }
                    CheckPreMapButtonName = ScCode;
                }
            }
        }
    }

    public void DisabledColorChange(string ColorCode, string ScCode)
    {
        Color ChoosedMapColor = new Color();
        ColorUtility.TryParseHtmlString(ColorCode, out ChoosedMapColor);
        Color NormalColor = new Color();
        ColorUtility.TryParseHtmlString("#3F53F8", out NormalColor);
        foreach (var maplist in MapButtonSettings)
        {
            if (ScCode == maplist.MapButton.name)
            {
                ColorBlock MapColor = maplist.MapButton.colors;
                MapColor.disabledColor = ChoosedMapColor;
                maplist.MapButton.colors = MapColor;
                // maplist.MapButton.colors.normalColor = ChoosedMapColor;

                if (CheckPreMapSelectedButtonName == null)
                {
                    CheckPreMapSelectedButtonName = ScCode;
                    Debug.Log("CheckPreMapColorButton_> " + CheckPreMapSelectedButtonName);
                }
                else
                {
                    Debug.Log("CheckPreMapColorButton_> 2call: " + CheckPreMapSelectedButtonName);
                    foreach (var premap in MapButtonSettings)
                    {
                        if (CheckPreMapSelectedButtonName == premap.MapButton.name)
                        {
                            ColorBlock PreMapButtonColor = premap.MapButton.colors;
                            PreMapButtonColor.disabledColor = NormalColor;
                            premap.MapButton.colors = PreMapButtonColor;
                            premap.MapButton.interactable = true;
                        }
                    }
                    CheckPreMapSelectedButtonName = ScCode;
                }
                maplist.MapButton.interactable = false;
            }
        }
    }

    // Login UI
    public void ShowLoginUI()
    {
        ShowCursor();
        LoginUIState = true;
        LoginUI.DOFade(1, 0.5f);
        LoginUI.gameObject.SetActive(true);
    }
    public InputField IDInput;
    public void LoginUINext()
    {
        if (LoginUI.isActiveAndEnabled == true)
        {
            string ToStringCheckLoginUI;
            ToStringCheckLoginUI = CheckLoginUI.ToString();
            foreach (var LoginList in LoginUISettings)
            {
                if (ToStringCheckLoginUI == LoginList.Id)
                {
                    LoginList.PlayLog.DOFade(0, 0.3f).OnComplete(() =>
                    {
                        LoginList.PlayLog.gameObject.SetActive(false);
                    });
                }
            }

            CheckLoginUI++;
            // Debug.Log(CheckLoginUI);
            ToStringCheckLoginUI = CheckLoginUI.ToString();
            Debug.Log(ToStringCheckLoginUI);
            foreach (var LoginList in LoginUISettings)
            {
                if (ToStringCheckLoginUI == LoginList.Id)
                {
                    Debug.Log("Changed.");
                    LoginList.PlayLog.DOFade(0, 0.3f).OnComplete(() =>
                    {
                        LoginList.PlayLog.DOFade(1, 0.3f);
                        LoginList.PlayLog.gameObject.SetActive(true);
                        if (CheckLoginUI == 3)
                        {
                            FocusInput(IDInput);
                        }
                    });
                }
            }

            if (CheckLoginUI == 3)
            {
                FocusInput(IDInput);
            }

        }
        else
        {
            return;
        }
    }

    public void HideLoginUI()
    {
#if UNITY_EDITOR
        Debug.Log("HideLoginUI");
#endif
        LoginUIState = false;
        LoginUI.DOFade(0, 0.5f).OnComplete(() =>
        {
            LoginUI.gameObject.SetActive(false);
        });
        NotificationUI.DOFade(1, 0.5f);
        NotificationUI.gameObject.SetActive(true);

        NXEvent.SetData("OnUsernameInput", IDInput.text);
        NXEvent.EmitEvent("OnUsernameInput");

        HideCursor();
    }

// =========================================================
// Static Methods
    public static void ShowCursor()
    {
        // Debug.Log("GameSys_WebglCursor:: FreeCursor");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // cursorLocked = false;
    }

    public static void HideCursor()
    {
        // Debug.Log("GameSys_WebglCursor:: LockAndHideCursor");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // cursorLocked = true;
    }

    public static void FocusInput(InputField inputField)
    {
        inputField.ActivateInputField();
        inputField.interactable = true;
        inputField.Select();
        EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        // inputField.OnPointerClick (null);
    }
}
