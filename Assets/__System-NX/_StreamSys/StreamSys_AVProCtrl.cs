using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class StreamSys_AVProCtrl : MonoBehaviour
{
    public MediaPlayer m_MediaPlayer;
    public bool isDebug;
    public InputField AVProPathUI;

    void Start()
    {
        if (AVProPathUI.text == "")
        {
            InputFieldDebugUpdate();
        }
    }

    public void OnGetStreamUrl(string url)
    {

        m_MediaPlayer.Stop();
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            m_MediaPlayer.m_VideoPath = url;
            string ext = System.IO.Path.GetExtension(url);
            CheckFileExtension(ext);
            m_MediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, url, true);
            InputFieldDebugUpdate();
        }
        else
        {
            m_MediaPlayer.Stop();
        }
    }

    public void GetStreamUrlFromInputUI(InputField ui)
    {

        m_MediaPlayer.Stop();
        if (Uri.IsWellFormedUriString(ui.text, UriKind.Absolute))
        {
            m_MediaPlayer.m_VideoPath = ui.text;
            string ext = System.IO.Path.GetExtension(ui.text);
            CheckFileExtension(ext);
            m_MediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, ui.text, true);
            InputFieldDebugUpdate();
        }
        else
        {
            m_MediaPlayer.Stop();
        }
    }

    public void CheckFileExtension(string ext)
    {
#if UNITY_WEBGL
        if (ext == ".m3u8")
        {
            if(m_MediaPlayer.PlatformOptionsWebGL.externalLibrary != WebGL.ExternalLibrary.HlsJs){
                m_MediaPlayer.PlatformOptionsWebGL.externalLibrary = WebGL.ExternalLibrary.HlsJs;
                m_MediaPlayer.Reinitialize();
            }
        }
        else
        {
            if(m_MediaPlayer.PlatformOptionsWebGL.externalLibrary != WebGL.ExternalLibrary.None){
                m_MediaPlayer.PlatformOptionsWebGL.externalLibrary = WebGL.ExternalLibrary.None;
                m_MediaPlayer.Reinitialize();
            }
        }
#endif
    }

    public void InputFieldDebugUpdate()
    {
        if (isDebug)
        {
            AVProPathUI.text = m_MediaPlayer.m_VideoPath;
        }
    }
}
