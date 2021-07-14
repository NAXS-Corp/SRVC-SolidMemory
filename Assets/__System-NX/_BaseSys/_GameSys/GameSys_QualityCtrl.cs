using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using NAXS.NXHelper;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using NAXS.Event;
#if !HDRP
using UnityEngine.Rendering.Universal;
#endif


[System.Serializable]
public class QualityCtrl{
    public string QualityName;
    // public RenderPipelineAsset RenderSetting;
    public Button UIButton;
    public List<GameObject> ExclusiveObjs;
    public bool UsePostProcessing = true;

    
    public void SetObjActivation(bool state){
        foreach(GameObject obj in ExclusiveObjs){
            if(obj)
                obj.SetActive(state);
        }
    }
}
public class GameSys_QualityCtrl : MonoBehaviour
{
#if !HDRP
    public int TargetFPS = 45;
    public int MidQThreshold = 40;
    public int LowQThreshold = 28;
    public List<QualityCtrl> QualityCtrls;

    public bool AutoSwitchQuality;
    // private bool 
    public bool EnableHotKeys = true;


    // Average FPS 
    float avgCount;
    public float AvgFps;
    public int AutoSwitchAfterSecond = 15;
    int autoSwitchCount = 0;
 
    [Header("UI")]
    public Text FPSCounterText;
    public Text NotificationText;
    // public CanvasGroup QualityUIGroup;
    // public bool ShowUIOnEsc;

    [FoldoutGroup("Events")]
    public UnityEvent OnAutoMidQuality;
    [FoldoutGroup("Events")]
    public UnityEvent OnAutoLowQuality;
    // [FoldoutGroup("Events")]
    // public UnityEvent OnShowUI;
    // public UnityEvent OnHideUI;
    // [FoldoutGroup("Events")]
    // public StringEvent DisplayFPS;




    void Start()
    {
        Application.targetFrameRate = TargetFPS;
        SwitchQuality(2);
        StartCoroutine(QualityTest());
        
    }

    void Update()
    {
        AvgFPSUpdate();
        HotKey();
    }

    void SwitchQuality(int qualityIdx){
        // GraphicsSettings.renderPipelineAsset = QualityCtrls[qualityIdx].RenderSetting;
        QualitySettings.SetQualityLevel(qualityIdx, true);

        for(int i = 0; i < QualityCtrls.Count; i++){
            if(i == qualityIdx){
                // Enable/Disabl PP
                var camData = Camera.main.GetUniversalAdditionalCameraData();
                camData.renderPostProcessing = QualityCtrls[i].UsePostProcessing;

                QualityCtrls[i].SetObjActivation(true);
                QualityCtrls[i].UIButton.interactable = false;
            }else{
                QualityCtrls[i].SetObjActivation(false);
                QualityCtrls[i].UIButton.interactable = true;
            }
        }
        Debug.Log("Active render pipeline asset is: " + GraphicsSettings.renderPipelineAsset.name);

        NXEvent.EmitEvent("OnQualityChanged");
        // SetUIState(false);
    }

    void AvgFPSUpdate(){
        avgCount += ((Time.deltaTime/Time.timeScale) - avgCount) * 0.03f; 
        AvgFps = (1F/avgCount); 
        // DisplayFPS.Invoke(AvgFps.ToString());
        FPSCounterText.text = Mathf.RoundToInt(AvgFps).ToString();
    }

    IEnumerator QualityTest(){

        yield return new WaitForSeconds(AutoSwitchAfterSecond);

        if(AutoSwitchQuality){
            if(autoSwitchCount == 0){
                if(AvgFps < MidQThreshold){
                    autoSwitchCount += 1;
                    // SwitchQuality(1);
                    PlayerSetMidQ();
                    ShowNotification(QualityCtrls[1].QualityName);
                    OnAutoMidQuality.Invoke();
                    AvgReset();
                    StartCoroutine(QualityTest());
                }
            }else if(autoSwitchCount == 1){
                if(AvgFps < LowQThreshold){
                    // ShowNotification("MID");
                    // SwitchQuality(0);
                    PlayerSetLowQ();
                    ShowNotification(QualityCtrls[0].QualityName);
                    OnAutoLowQuality.Invoke();
                    autoSwitchCount += 1;
                }
            }
        }
    }

    public Text SystemNoti;
    public void ShowSysNotification(string txt){
        SystemNoti.text = txt;
        SystemNoti.color = Color.white;
        SystemNoti.gameObject.SetActive(true);
        SystemNoti.DOFade(0, 5f).SetDelay(10).OnComplete(()=> {
            SystemNoti.gameObject.SetActive(false);
        });
    }
    public void ShowNotification(string Q){
        NotificationText.text = string.Format("Graphic quality is set to [{0}]. Press [Esc] to change manually.", Q);
        NotificationText.color = Color.white;
        NotificationText.gameObject.SetActive(true);
        NotificationText.DOFade(0, 8f).SetDelay(10).OnComplete(()=> {
            NotificationText.gameObject.SetActive(false);
        });
    }

    void AvgReset(){
        avgCount = 0;
    }

    void HotKey(){

        if(!EnableHotKeys || !Input.GetKey(KeyCode.Q)) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SwitchQuality(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SwitchQuality(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SwitchQuality(0);
        }
    }



    // void SetUIState(bool state){
    //     if(!QualityUIGroup) return;
    //     if(state){
    //         QualityUIGroup.gameObject.SetActive(true);
    //         QualityUIGroup.DOFade(1, 0.5f);
    //         OnShowUI.Invoke();
    //     }else{
            
    //         QualityUIGroup.DOFade(0, 1f).OnComplete(() => {
    //             QualityUIGroup.gameObject.SetActive(false);
    //         });
    //         OnHideUI.Invoke();
    //     }
    // }


    ///////////////
    // API ////////
    ///////////////
    // public void ShowUI(){
    //     SetUIState(true);
    // }

    // public void HideUI(){
    //     SetUIState(false);
    // }

    public void PlayerSetHighQ(){
        SwitchQuality(2);
        AutoSwitchQuality = false;
    }
    public void PlayerSetMidQ(){
        SwitchQuality(1);
        AutoSwitchQuality = false;
    }
    public void PlayerSetLowQ(){
        SwitchQuality(0);
        AutoSwitchQuality = false;
    }
#endif
}
