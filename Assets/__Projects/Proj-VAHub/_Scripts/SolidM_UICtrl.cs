using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WebSocketSharp;
using FurioosSDK.Core;
using NAXS.Event;

public class SolidM_UICtrl : MonoBehaviour
{
    public bool useFurioos;
    public CanvasGroup canvas;
    public AudioSource Audio;
    public string targetUrl;
    public float fadeTime = 1f;
    public float RetriggerTime = 3f;
    bool FSOpened;
    bool triggered;
    // bool allowTrigger;


    // Start is called before the first frame update
    void Start()
    {
        canvas.alpha = 0;
        canvas.gameObject.SetActive(false);
        FSSocket.OnOpen += OnFSOpen;

        // allowTrigger = true;
        triggered = false;
    }

    private void Update()
    {
        if (triggered)
        {
            // OnTriggerFocused();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                FadeOut();
            }
        }
    }


    // Trigger

    public void OnFocused()
    {
        if (!triggered)
        {
            FadeIn();
        }
        triggered = true;
    }

    // void OnTriggerFocused()
    // {
    //     if (Input.GetKeyDown(KeyCode.Return))
    //     {
    //         OpenURL();
    //     }
    // } 

    public void OnTriggerStop()
    {
        FadeOut();
    }

    // API 

    public void FadeIn()
    {
        NXEvent.EmitEvent("DisableCameraRotation");
        NXEvent.EmitEvent("DisablePlayerMovement");
        triggered = true;

        canvas.gameObject.SetActive(true);
        canvas.DOFade(1f, fadeTime);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Audio)
        {
            Audio.Play();
            Audio.DOFade(1f, fadeTime);
        }
    }

    public void FadeOut()
    {
        // triggered = false;
        NXEvent.EmitEvent("EnableCameraRotation");
        NXEvent.EmitEvent("EnablePlayerMovement");

        canvas.DOFade(0f, fadeTime).OnComplete(() => {
            canvas.gameObject.SetActive(false);
        });

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Audio)
            Audio.DOFade(0f, fadeTime);

        StartCoroutine(AllowTriggerAgain());
    }
    IEnumerator AllowTriggerAgain()
    {
        yield return new WaitForSeconds(RetriggerTime);
        // allowTrigger = true;
        triggered = false;
    }

    public void OpenURL()
    {
        if (useFurioos && FSOpened)
            FSSend(targetUrl);
        else
            Application.OpenURL(targetUrl);
    }

    // Furioos
    void OnFSOpen()
    {
        FSOpened = true;
    }

    void FSSend(string data)
    {
        FSSocket.Send(data);
    }
}
