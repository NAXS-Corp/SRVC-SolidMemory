// Change Log: 
// 12-09: Seperate Login to new class

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
using Sirenix.OdinInspector;



namespace NAXS.UI
{
    public class UI_MasterManager : MonoBehaviour
    {
        #region Variables
        public static UI_MasterManager instance;

        [Header("Sub Modules")]
        public UI_Chatroom SubModule_Chatroom;

        [Header("UI Elements")]
        public List<UI_Button> LeftPanelButtons; 
        public CanvasGroup MasterUIGroup;
        public Text GlobalNotiText;
        public Text LiveSceneText;
        private bool isMaterUIShowed;
        // public Button MapButton;
        // public Button InstructionsButton;
        // public Button AboutButton;
        // public CanvasGroup MapUI;
        // public CanvasGroup InstructionsUI;
        // public CanvasGroup AboutUI;
        public CanvasGroup NotificationUI;
        public CanvasGroup defaultCanvasGroup;


        // public Text LoadingText;
        // public RectTransform LoadingBar;
        // private float loadingBarWidth = 800;

        [Header("VR Platform")]
        public bool isVR;
        [EnableIf("@this.isVR == true")]
        public GameObject VRLoadingShade;
        [EnableIf("@this.isVR == true")]
        public float Loadingfadein;
        [EnableIf("@this.isVR == true")]
        public float Loadingfadeout;

        [Header("LoadingBar")]
        public CanvasGroup LoadingUI;

        [Header("UI Map")]
        public string MapNormalColor = "#007AFF";
        public string MapLiveColor = "#CC0033";
        public string MapDisableColor = "#0E1D6C";
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

        [Header("Event")]
        public StringEvent OnSceneChanged;

        [Header("UX Setting")]
        public bool MouseInvisibleOnGame;
        public bool ShowUIOnTab = true;
        public bool ShowUIOnVR;
        public bool ToggleUI = true;
        private string LiveScCode;
        #endregion

        
        void Awake()
        {
            Debug.Log("UI_MasterManager Awake");
            if (!UI_MasterManager.instance)
                UI_MasterManager.instance = this;
            else
                Destroy(this);
        }
        void Start()
        {

            SetToDefualtState();

            //Add Listeners
            //PageButtonListener();
            StartMapButtonListeners();
            StartEventListeners();
        }

        void SetToDefualtState()
        {
            // defaultButton = MapButton;
            // defaultCanvasGroup = MapUI;
            allowChangeScene = true;
        }

        public void OnGetPortalUpdate()
        {
            Dictionary<string, object> portalStates = ServerMapObjList.ObjStates;
            foreach (var maplist in MapButtonSettings)
            {
                var btnName = maplist.MapButton.name;
                if (portalStates.ContainsKey(btnName))
                {
                    // maplist
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
            // MapButton.onClick.AddListener(() => PageCtrl(MapUI, MapButton));
            // InstructionsButton.onClick.AddListener(() => PageCtrl(InstructionsUI, InstructionsButton));
            // AboutButton.onClick.AddListener(() => PageCtrl(AboutUI, AboutButton));
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

        public void PageCtrl(CanvasGroup PageGroupUI)
        {   
            foreach(UI_Button b in LeftPanelButtons)
            {
                b.GetComponent<Button>().interactable = true;
            }
            // Turn off previous GroupUI & ButtonUI
            if (defaultCanvasGroup)
            {
                Debug.Log("PageCtrl>> Hide Current.");
                HideCanvasGroup(defaultCanvasGroup, 0.5f);
            }
            ShowCanvasGroup(PageGroupUI, 0.5f);
            defaultCanvasGroup = PageGroupUI;
        }

        void StartEventListeners()
        {
            NXEvent.StartListening("ChangeArtistTitle", () =>
            {
                GlobalNotiText.text = NXEvent.GetString("ChangeArtistTitle");
            });

            NXEvent.StartListening("OnScLoaded", () =>
            {
                string scCode = NXEvent.GetString("OnScLoaded");
                OnSceneChanged.Invoke(scCode);
            });

            NXEvent.StartListening("AddrLoadingScene", () =>
            {
                if (VRLoadingShade)
                {
                    VRLoadingShade.GetComponent<Renderer>().material.DOKill();
                    DOTween.Sequence().Append(VRLoadingShade.GetComponent<Renderer>().material.DOFade(1, Loadingfadein));
                }
                ShowCanvasGroup(LoadingUI, 1f);
                CurrentMapColor(NXEvent.GetString("AddrLoadingScene"));
            });

            NXEvent.StartListening("CompletedLoadScene", () =>
            {
                if (VRLoadingShade)
                {
                    VRLoadingShade.GetComponent<Renderer>().material.DOKill();
                    DOTween.Sequence().Append(VRLoadingShade.GetComponent<Renderer>().material.DOFade(0, Loadingfadeout));
                }
                HideCanvasGroup(LoadingUI, 1f);
            });

            NXEvent.StartListening("OnLoginComplete", () =>
            {
                ShowChatroomUI();
            });

            // VR Menu
            NXEvent.StartListening("CallVRMenu", VRHotKeyUpdate);
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
                        ToggleMasterUI();
                    }
                    else
                    {
                        ShowMasterUI();
                    }
                }
            }

        }

        // VR Menu Callout
        void VRHotKeyUpdate()
        {
            if (ShowUIOnVR)
            {
                if (ToggleUI)
                {
                    ToggleMasterUI();
                }
                else
                {
                    ShowMasterUI();
                }
            }
        }

        #region MasterUI Functions
        public void ToggleMasterUI()
        {
            if (isMaterUIShowed) HideMasterUI();
            else ShowMasterUI();
        }

        public void ShowMasterUI()
        {
            #region VR
            // VR
            NXEvent.SetData("VRRaycast", true);
            NXEvent.EmitEvent("VRRaycast");
            NXEvent.SetData("SetNXPInput", false);
            NXEvent.EmitEvent("SetNXPInput");
            #endregion

            MasterUIGroup.gameObject.SetActive(true);
            MasterUIGroup.DOFade(1, 0.5f);
            isMaterUIShowed = true;
            if (MouseInvisibleOnGame)
            {
                UI_Methods.ShowCursor();
            }
            if (SubModule_Chatroom)
                SubModule_Chatroom.HideAndDisable();
        }

        public void HideMasterUI()
        {
            #region VR
            // VR
            NXEvent.SetData("VRRaycast", false);
            NXEvent.EmitEvent("VRRaycast");
            NXEvent.SetData("SetNXPInput", true);
            NXEvent.EmitEvent("SetNXPInput");
            #endregion

            MasterUIGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                MasterUIGroup.gameObject.SetActive(false);
            });
            isMaterUIShowed = false;
            if (MouseInvisibleOnGame)
            {
                UI_Methods.HideCursor();
            }
            if (SubModule_Chatroom)
                SubModule_Chatroom.EnableAndShow();
        }
        #endregion
        #region Map Functions
        //================================================
        // Map Function (Separate later)

        public void StartMapButtonListeners()
        {

            foreach (var mapBtn in MapButtonSettings)
            {
                //Listener for Mouse Click
                mapBtn.MapButton.onClick.AddListener(() => OnLoadMapScene(mapBtn.MapButton));

                //Listeners for Pointer Enter & Exit
                var m_Trigger = mapBtn.MapButton.GetComponent<EventTrigger>();
                if (!m_Trigger)
                {
                    mapBtn.MapButton.gameObject.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry PointerEnter = new EventTrigger.Entry();
                PointerEnter.eventID = EventTriggerType.PointerEnter;
                PointerEnter.callback.AddListener((data) =>
                {
                    ShowMapDetails(mapBtn.MapButton);
                });
                EventTrigger.Entry PointerExit = new EventTrigger.Entry();
                PointerExit.eventID = EventTriggerType.PointerExit;
                PointerExit.callback.AddListener((data) =>
                {
                    HideMapDetails(mapBtn.MapButton);
                });
                m_Trigger.triggers.Add(PointerEnter);
                m_Trigger.triggers.Add(PointerExit);
            }
        }



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
            NormalColorChange(MapLiveColor, MapName);
        }

        public void OnLoadMapScene(Button m_MapButton)
        {
            ScCode = m_MapButton.name;
            Debug.Log(ScCode);
            AddrSys_SceneManager.instance.LoadScene(ScCode);
            HideMasterUI();
        }

        public void CurrentMapColor(string ScCode)
        {
            DisabledColorChange2(MapDisableColor, ScCode);
        }

        public void NormalColorChange(string ColorCode, string ScCode)
        {
            Color ChoosedMapColor = new Color();
            ColorUtility.TryParseHtmlString(ColorCode, out ChoosedMapColor);
            Color NormalColor = new Color();
            ColorUtility.TryParseHtmlString(MapNormalColor, out NormalColor);
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

        // public void DisabledColorChange(string ColorCode, string ScCode)
        // {
        //     Color ChoosedMapColor = new Color();
        //     ColorUtility.TryParseHtmlString(ColorCode, out ChoosedMapColor);
        //     Color NormalColor = new Color();
        //     ColorUtility.TryParseHtmlString(MapNormalColor, out NormalColor);
        //     foreach (var maplist in MapButtonSettings)
        //     {
        //         if (ScCode == maplist.MapButton.name)
        //         {
        //             ColorBlock MapColor = maplist.MapButton.colors;
        //             MapColor.disabledColor = ChoosedMapColor;
        //             maplist.MapButton.colors = MapColor;
        //             // maplist.MapButton.colors.normalColor = ChoosedMapColor;

        //             if (CheckPreMapSelectedButtonName == null)
        //             {
        //                 CheckPreMapSelectedButtonName = ScCode;
        //                 Debug.Log("CheckPreMapColorButton_> " + CheckPreMapSelectedButtonName);
        //             }
        //             else
        //             {
        //                 Debug.Log("CheckPreMapColorButton_> 2call: " + CheckPreMapSelectedButtonName);
        //                 foreach (var premap in MapButtonSettings)
        //                 {
        //                     if (CheckPreMapSelectedButtonName == premap.MapButton.name)
        //                     {
        //                         ColorBlock PreMapButtonColor = premap.MapButton.colors;
        //                         PreMapButtonColor.disabledColor = NormalColor;
        //                         premap.MapButton.colors = PreMapButtonColor;
        //                         premap.MapButton.interactable = true;
        //                     }
        //                 }
        //                 CheckPreMapSelectedButtonName = ScCode;
        //             }
        //             maplist.MapButton.interactable = false;
        //         }
        //     }
        // }
        
        public void DisabledColorChange2(string ColorCode, string ScCode)
        {
            // Color ChoosedMapColor = new Color();
            // ColorUtility.TryParseHtmlString(ColorCode, out ChoosedMapColor);
            // Color NormalColor = new Color();
            // ColorUtility.TryParseHtmlString(MapNormalColor, out NormalColor);
            foreach (var maplist in MapButtonSettings)
            {
                if (ScCode == maplist.MapButton.name)
                {
                    // ColorBlock MapColor = maplist.MapButton.colors;
                    // MapColor.disabledColor = ChoosedMapColor;
                    // maplist.MapButton.colors = MapColor;
                    // maplist.MapButton.colors.normalColor = ChoosedMapColor;
                    maplist.MapButton.interactable = false;
                }else{

                    
                    maplist.MapButton.interactable = true;
                }
            }
        }
        #endregion

        #region API

        public void ShowChatroomUI(){
            if (SubModule_Chatroom) {
                Debug.Log("UI_MasterManager ShowChatroomUI");
                SubModule_Chatroom.EnableAndShow();
            }
        }

        #endregion


        #region DEBUG
        // DEBUG
#if UNITY_EDITOR
        void OnEnable()
        {
            // Debug.Log("UI_MasterManager OnEnable");
        }

        void OnDisable()
        {
            // Debug.Log("UI_MasterManager OnDisable");
        }

#endif
    }
    #endregion
}