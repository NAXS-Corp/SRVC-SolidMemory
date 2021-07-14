using UnityEngine;

// [ExecuteInEditMode]
public class NXDev_QualitySelector : MonoBehaviour
{
    public bool PauseGameOnStart;
    KeyCode[] AlphaKeys = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.Alpha0};
    public Vector4 RectSetting;
    bool GamePaused = false;
    public bool ShowGUI;
    public int TargetFrameRate = 45;

    void Start()
    {
        if(TargetFrameRate > 0)
            Application.targetFrameRate = TargetFrameRate;
        if(PauseGameOnStart){
            PauseGame();
        }
    }

    public void ToggleGUI()
    {
        ShowGUI = !ShowGUI;
    }

    void Update()
    {
        
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q)){
            ToggleGUI();
        }
    }
    void OnGUI()
    {
        if(!ShowGUI)
            return;
        string[] names = QualitySettings.names;
        GUILayout.BeginArea (new Rect(RectSetting.x,RectSetting.y,RectSetting.z,RectSetting.w));
        GUILayout.BeginVertical();
        for (int i = 0; i < names.Length; i++)
        {
            if (GUILayout.Button(names[i]))
            {
                SetQuality(i);
            }
            if(Input.GetKey(KeyCode.Q) && Input.GetKeyDown(AlphaKeys[i]))
            {
                SetQuality(i);
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void SetQuality(int i){
        QualitySettings.SetQualityLevel(i, true);
        if(GamePaused)
        {
            ResumeGame();
        }
    }

    void PauseGame ()
    {
        Time.timeScale = 0;
        GamePaused = true;
    }

    void ResumeGame ()
    {
        Time.timeScale = 1;
        GamePaused = false;
    }
}