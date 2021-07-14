using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

namespace NAXS.NXPlayer
{
    public class NXP_PlayerLook : MonoBehaviour
    {
        [Header("Head")]
        public Renderer HeadRenderer;
        public string HeadShaderProp = "_EmissionMap";

        public void GetPhotoUrl(string photoUrl){
            
            StartCoroutine(GetAndSetTexture(photoUrl, HeadRenderer, HeadShaderProp));
        }


        IEnumerator GetAndSetTexture(string url, Renderer renderer, string shaderProp)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Debug.Log(www.error);
            }
            else
            {
                if (renderer)
                {
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    renderer.material.SetTexture(shaderProp, myTexture);
                }
            }
        }
    }
}