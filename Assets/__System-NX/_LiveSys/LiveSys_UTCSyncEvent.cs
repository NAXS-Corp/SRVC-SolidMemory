using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

public class LiveSys_UTCSyncEvent : MonoBehaviour
{

    [Header("Time Setting")]
    [ReadOnly][SerializeField]private int EventStartDatetime = 0;
    [Tooltip("Seconds")] public int EventDuration = 1800;

    [Header("Play Setting")]
    public bool StopPlayOnAwake;
    public bool AutoStartSync = true;
    public bool Looped;



    [Header("Submodule")]
    public MediaPlayer m_MediaPlayer;
    bool mediaPlayerBuffered;
    bool mediaPlayerSeeked;
    public PlayableDirector TimelinePlayer;

    void Awake()
    {
        if(StopPlayOnAwake){
            TimelinePlayer.Stop();
            m_MediaPlayer.Stop();
        }
    }

    void Start()
    {
        #if !UNITY_EDITOR
        ShowGUI = false;
        #endif

        if (m_MediaPlayer)
            m_MediaPlayer.Events.AddListener(OnAVEvent);

        if(AutoStartSync)
            StartCoroutine(CheckSchedule());
    }

    public void SetEventStartDatetime(int UTCDatetime){
        Debug.Log("////LiveSys_UTCSyncEvent SetEventStartDatetime "+UTCDatetime);
        EventStartDatetime = UTCDatetime;
    }

    IEnumerator CheckSchedule()
    {

        while (EventStartDatetime <= 0)
        {
            //wait till EventStartDatetime is setted
            Debug.Log("////LiveSys_UTCSyncEvent finding start datetime ");
            yield return null;
        }

        int currentDatetime = NXHelper_Time.Current();
        int timeElapsed = currentDatetime - EventStartDatetime;
        Debug.Log("////LiveSys_UTCSyncEvent CheckSchedule timeElapsed " + timeElapsed);

        if (timeElapsed >= 0)
        {
            // event started
            if (timeElapsed > EventDuration && !Looped)
            {
                //event ended
                Debug.Log("Event Ended");
                StopCoroutine("CheckSchedule");
                yield return null;
            }

            if (timeElapsed > EventDuration && Looped)
            {
                //Loop mode
                StartCoroutine(TryStartEvent());
                StopCoroutine("CheckSchedule");
                yield return null;
            }

            if (timeElapsed < EventDuration)
            {
                //During the event
                StartCoroutine(TryStartEvent());
                StopCoroutine("CheckSchedule");
                yield return null;
            }
        }
        else
        {
            // Event not started yet, checkSchedule in routine
            yield return new WaitForSeconds(1);
        }
    }

    int GetPlayTimeFromStartDatetime()
    {
        int timeElapsed = NXHelper_Time.Current() - EventStartDatetime;
        int currentRound = timeElapsed / EventDuration;
        int startTimeLooped = timeElapsed % EventDuration;

        Debug.Log("////LiveSys_UTCSyncEvent GetPlayTimeFromStartDatetime "+timeElapsed+" / "+currentRound+" / "+startTimeLooped);
        return startTimeLooped;
    }

    IEnumerator TryStartEvent()
    {
        Debug.Log("////LiveSys_UTCSyncEvent TryStartEvent");

        while (!mediaPlayerBuffered)
        {
            //wait till avpro is started
            Debug.Log("////LiveSys_UTCSyncEvent mediaPlayer not buffered");
            yield return null;
        }

        Debug.Log("////LiveSys_UTCSyncEvent StartPlaying:: "+GetPlayTimeFromStartDatetime());

        // AVPro, seek and play
        m_MediaPlayer.Control.Seek(GetPlayTimeFromStartDatetime() * 1000);
        
        // Problem: FinishedSeeking sometimes never triggered 
        // while (!mediaPlayerSeeked)
        // {
        //     //wait till avpro is started
        //     Debug.Log("////LiveSys_UTCSyncEvent mediaPlayer not seeked");
        //     yield return null;
        // }

        m_MediaPlayer.Play();

        // wait AVPro to seek
        yield return new WaitForSeconds(1);

        Debug.Log("////LiveSys_UTCSyncEvent StartPlaying2:: "+GetPlayTimeFromStartDatetime());
        
        // Timeline, seek and play
        TimelinePlayer.initialTime = GetPlayTimeFromStartDatetime();
        TimelinePlayer.Play();

        Debug.Log("////LiveSys_UTCSyncEvent StartEvent finished");
        StopCoroutine("TryStartEvent");

        ResetState();
    }

    void ResetState(){
        mediaPlayerBuffered = false;
        mediaPlayerSeeked = false;
    }


    // AVPro
    public void OnAVEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        
        switch (et)
        {
            case MediaPlayerEvent.EventType.Started:
                mediaPlayerBuffered = true;
                break;
            case MediaPlayerEvent.EventType.FinishedBuffering:
                mediaPlayerBuffered = true;
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                break;
            case MediaPlayerEvent.EventType.FinishedSeeking:
                mediaPlayerSeeked = true;
                break;
        }
        Debug.Log("=== AVPro OnVideoEvent: " + et.ToString());
    }

#region API
    public void SetUTCSyncState(bool state){
        if(state){
            StartUTCSync();
        }else{
            StopUTCSync();
        }
    }
    public void StartUTCSync(){
        StopCoroutine("CheckSchedule");
        StopCoroutine("TryStartEvent");
        StartCoroutine(CheckSchedule());
    }
    public void StopUTCSync(){
        StopCoroutine("CheckSchedule");
        StopCoroutine("TryStartEvent");
        m_MediaPlayer.Stop();
        m_MediaPlayer.Play();

        TimelinePlayer.initialTime = 0;
        TimelinePlayer.Play();
    }
#endregion
#region GUI
    ///////////////////
    // Debug GUI
    private bool ShowGUI = false;
    public void SetGUI(bool state){
        ShowGUI = state;
    }
    void OnGUI()
    {
        
        if(!ShowGUI){
            return;
        }
        int y = 20;
        int Offset = 30;
        GUI.TextArea(new Rect(20,y, 500, 20), "NXHelper_Time.Current: "+ NXHelper_Time.Current());
        y+= Offset;
        GUI.TextArea(new Rect(20,y, 500, 20), "AVPro::::: "+ (m_MediaPlayer.Control.GetCurrentTimeMs()/1000));
        y+= Offset;
        GUI.TextArea(new Rect(20,y, 500, 20), "Timeline:: "+ (TimelinePlayer.time));
    }
#endregion
}
