using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Events;
using NAXS.Event;


public class StreamSys_DatetimePlaylist : MonoBehaviour
{
    [System.Serializable]
    public class MediaItem
    {
        public string MediaUrl;
        public string MediaTitle;
        public string MediaDesc;
        [Tooltip("Seconds")] public float CustomLength = 1800;
    }

    public string BaseUrl;
    [Tooltip("Seconds")] public int FixedMediaLength = 1800;
    public MediaPlayer m_MediaPlayer;
    public string TitlePrefix = "Current Playing  |  ";
    public List<MediaItem> Playlist;

    public int CheckScheduleEverySec = 30;

    private int CurrentMediaIdx = -1;
    private float targetSeekTime;
    private bool isLoadingNewFile = false;


    void Start()
    {
        m_MediaPlayer.Events.AddListener(OnAVEvent);
        CheckSchedule();
        StartCoroutine(CheckScheduleRoutine());
    }


    // Callback function to handle events
    public void OnAVEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Started:
                break;
            case MediaPlayerEvent.EventType.FinishedBuffering:
                Debug.Log("== FinishedBuffering, Seeking: "+targetSeekTime);
                if(isLoadingNewFile){
                    m_MediaPlayer.Control.SeekFast(targetSeekTime);
                    // m_MediaPlayer.Control.Seek(targetSeekTime);
                    m_MediaPlayer.Control.Play();
                    isLoadingNewFile = false;
                }
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                break;
            case MediaPlayerEvent.EventType.FinishedSeeking:
                break;
        }
        Debug.Log("== AVPRo OnVideoEvent: "+et.ToString());
    }

    IEnumerator CheckScheduleRoutine(){
        yield return new WaitForSeconds(CheckScheduleEverySec);
        CheckSchedule();
        StartCoroutine(CheckScheduleRoutine());
    }

    [Button]
    void CheckSchedule()
    {
        int secondsOfDay;
        int totalLength, currentRound, mediaIdx, mediaStartTime;

        secondsOfDay = (int)System.DateTime.UtcNow.TimeOfDay.Hours * 3600 + (int)System.DateTime.Now.TimeOfDay.Minutes* 60 + (int)System.DateTime.Now.TimeOfDay.Seconds;

        if (FixedMediaLength > 0) // using fixed length for every media in playlist
        {
            totalLength = FixedMediaLength * Playlist.Count;
            currentRound = secondsOfDay / totalLength;
            mediaIdx = (secondsOfDay - (currentRound * totalLength)) / FixedMediaLength; // Is equal to: (secondsOfDay % FixedMediaLength)
            mediaStartTime = secondsOfDay - (currentRound * totalLength) - (mediaIdx * FixedMediaLength);

            #if UNITY_EDITOR
            Debug.Log("currentMediaIdx " + mediaIdx);
            Debug.Log("currentMediaPlaytime " + mediaStartTime);
            #endif
            
            if (CurrentMediaIdx != mediaIdx)
            {
                PlayNewMediaFile(mediaIdx, mediaStartTime);
            }else{
                
            }
            CurrentMediaIdx = mediaIdx;
        }
    }


    void PlayNewMediaFile(int mediaIdx, int startTime)
    {
        isLoadingNewFile = true;
        m_MediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, BaseUrl + Playlist[mediaIdx].MediaUrl, true);
        targetSeekTime = startTime * 1000f;
        ChangeInfoDisplay(mediaIdx);
    }

    void ChangeInfoDisplay(int mediaIdx)
    {
        // UI Event
        Debug.Log("===Playing=== " + Playlist[mediaIdx].MediaTitle);
        NXEvent.SetData("ChangeArtistTitle", string.Concat(TitlePrefix, Playlist[mediaIdx].MediaTitle));
        NXEvent.EmitEvent("ChangeArtistTitle");
    }
}
