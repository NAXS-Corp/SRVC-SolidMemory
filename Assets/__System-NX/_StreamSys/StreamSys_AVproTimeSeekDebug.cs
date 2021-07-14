using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class StreamSys_AVproTimeSeekDebug : MonoBehaviour
{

    public MediaPlayer m_MediaPlayer;
    public string MediaURL;
    public InputField Time;
    public InputField Duration;
    public InputField URL;
    private float InputSeekTime;
    private float FileDuration;
    // Start is called before the first frame update
    void Awake()
    {
        m_MediaPlayer.Events.AddListener(OnAVEvent);
    }

    public void OnAVEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Started:
                // getduration in seconds
                FileDuration = m_MediaPlayer.Info.GetDurationMs() / 1000;
                Debug.Log("File duration: " + FileDuration);
                Duration.text = FileDuration.ToString();
                break;
            case MediaPlayerEvent.EventType.FinishedBuffering:
                // getduration in seconds
                // FileDuration = m_MediaPlayer.Info.GetDurationMs() / 1000;
                // Debug.Log("File duration: " + FileDuration);
                // Duration.text = FileDuration.ToString();
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                break;
            case MediaPlayerEvent.EventType.FinishedSeeking:
                break;
        }
    }

    public void OnConfirm()
    {

        if (Time)
        {
            if (Time.text == "")
            {
                InputSeekTime = 0;
            }
            else
            {
                Debug.Log("InputSeekTime: " + Time.text);
                InputSeekTime = float.Parse(Time.text) * 1000f;
            }
        }
        if (m_MediaPlayer)
        {
            // m_MediaPlayer.Control.Pause();
            Debug.Log("SeekFast: " + InputSeekTime / 1000f);
            m_MediaPlayer.Control.SeekFast(InputSeekTime);
            // m_MediaPlayer.Control.Play();
        }
    }

    public void OnChangeFile()
    {
        if (m_MediaPlayer)
        {
            if (URL)
            {
                MediaURL = URL.text;
            }
            m_MediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, MediaURL, true);
            m_MediaPlayer.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("enter"))
        {
            OnConfirm();
        }
    }
}
