using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("_NXAction/Audio/NX AudioStream")]
public class NXAction_AudioStream : NXAction
{
    [Title("References")]
    public AudioSource m_AudioSource;
    #if UNITY_EDITOR
    public Object StreamAudioObject;
    #endif
    [SerializeField, ReadOnly] string filePath;
    [Title("Ctrl Options")]
    [EnumToggleButtons]
    public NXActionOptions CtrlOptions;
    bool useFade =>  (CtrlOptions.HasFlag(NXActionOptions.Fade)) ? true : false;
    bool useRevert =>  (CtrlOptions.HasFlag(NXActionOptions.Revertable)) ? true : false;
    [HorizontalGroup("Preload")]
    public bool PreloadOnAwake = true;
    [HorizontalGroup("Preload"), LabelWidth(50), LabelText("Delay")]
    public float PreloadDelay = 5f;
    public bool AutoDisable = true;

    [ShowIf("useFade")]
    [HorizontalGroup("Fade")]
    public Vector2 FadeTime = new Vector2(5f, 5f);
    [ShowIf("useFade")]
    [HorizontalGroup("Fade")][HideLabel]
    public Ease FadeMode = Ease.Linear;
    [Title("Audio Setting")]
    public bool Loop = true;
    public Vector2 VolumeMinMax = Vector2.up;

    string runtimeFilePath;
    bool isClipLoading;
    bool isClipLoaded;
    bool playOnLoaded;

    //////////////////////////////
    //Setup//////////////////////
    //////////////////////////////
    protected override void Start(){
        m_AudioSource.volume = VolumeMinMax.x;
    }

    //////////////////////////////
    //Action//////////////////////
    //////////////////////////////

    public override void StartAction(){
        if(isClipLoaded)
        {
            PlayAudio();
        }
        else if(isClipLoading)
        {
            playOnLoaded = true;
        }
        else
        {
            PreloadClip(0f, true);
        }
    }
    public override void StopAction(){
        if(useRevert)
        {
            if(useFade)
            {
                //Fadeout
                m_AudioSource.DOKill();
                m_AudioSource.DOFade(VolumeMinMax.x, FadeTime.y).SetEase(FadeMode).OnComplete(OnStop);
            }
            else
            {
                OnStop();
            }
        }
    }

    void OnStop(){
        m_AudioSource.volume = VolumeMinMax.x;
        m_AudioSource.Stop();
        if(AutoDisable){
            m_AudioSource.enabled = false;
        }
    }

    void PlayAudio(){
        m_AudioSource.enabled = true;
        m_AudioSource.Play();    
        if(useFade)
        {   
            //Fadein
            m_AudioSource.DOKill();
            m_AudioSource.DOFade(VolumeMinMax.y, FadeTime.x).SetEase(FadeMode);
        }
        else
        {
            m_AudioSource.volume = VolumeMinMax.y;
        }
    }

    //////////////////////////////
    //Preload//////////////////////
    //////////////////////////////
    protected override void Awake()
    {
        if (PreloadOnAwake)
        {
            PreloadClip(PreloadDelay, false);
        }
        if(AutoDisable){
            m_AudioSource.enabled = false;
        }
    }

    public void PreloadClip(float delayTime, bool playOnLoaded)
    {
        runtimeFilePath = string.Concat(Application.streamingAssetsPath, filePath);
        #if UNITY_EDITOR
        StartCoroutine(Mp3Import(runtimeFilePath));
        #else
        StartCoroutine(GetAudioClip(runtimeFilePath, delayTime, playOnLoaded));
        #endif
    }

    IEnumerator GetAudioClip(string filePath, float delayTime, bool playOnLoaded)
    {
        if(delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        isClipLoading = true;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip;
                myClip = DownloadHandlerAudioClip.GetContent(www);
                m_AudioSource.clip = myClip;
                isClipLoaded = true;
                if(playOnLoaded || playOnLoaded)
                    PlayAudio();
            }
            isClipLoading = false;
        }
    }



    //////////////////////////////
    //EDITOR Functions////////////
    //////////////////////////////

#if UNITY_EDITOR
    AudioImporter importer;
    
    IEnumerator Mp3Import(string path)
    {
        isClipLoading = true;
        if(!importer)
            importer = gameObject.AddComponent<NAudioImporter>();
        importer.Import(path);

        while (!importer.isDone)
            yield return null;

        m_AudioSource.clip = importer.audioClip;
        isClipLoaded = true;
        isClipLoading = false;
        if(playOnLoaded)
            PlayAudio();
    }

    public override void OnValidateChild()
    {
        if (StreamAudioObject)
        {
            string assetPath = AssetDatabase.GetAssetPath(StreamAudioObject);
            const string kAssetPrefix = "Assets/StreamingAssets";

            if (assetPath.StartsWith(kAssetPrefix))
            {
                filePath = assetPath.Substring(kAssetPrefix.Length);
            }

            if (!assetPath.EndsWith(".mp3"))
            {
                StreamAudioObject = null;
                // assetPath = null;
                filePath = null;
                Debug.LogError("MP3 Only");
            }
        }

        if(m_AudioSource){
            m_AudioSource.playOnAwake = false;
            m_AudioSource.loop = Loop;
        }
    }

    public override void EditorReset(){
        PreloadDelay = Random.Range(5f, 10f);
        AudioSource source = GetComponent<AudioSource>();
        if(source)
            m_AudioSource = source;
    }
    
    [Button][HideIf("m_AudioSource")]
    [GUIColor(0.1f, 0.85f, 0.25f)]
    void AddAudioSource(){
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        // EditorReset();
    }
#endif

}
