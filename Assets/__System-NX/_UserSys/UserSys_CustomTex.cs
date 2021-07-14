using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

namespace NAXS.User
{
    public class UserSys_CustomTex : MonoBehaviour
    {
        public string TestUID;

        Firebase_Manager FirebaseManager;
        protected SimpleFirebaseUnity.Firebase dbUserRef;
        public UserProfile CurrentUser;
        public Renderer targetRenderer;
        public string targetTexProp = "_MainTex";

        [Button("Test")]
        void Start()
        {
            dbUserRef = Firebase_Manager.instance.firebase.Child("users/" + TestUID);

            dbUserRef.OnGetSuccess += OnGetValue;
            dbUserRef.GetValue();
        }

        public void OnGetUID(string uid)
        {
            GetPhotoURL(uid);
        }

        public void GetPhotoURL(string uid)
        {
            dbUserRef = Firebase_Manager.instance.firebase.Child("users/" + TestUID);

            dbUserRef.OnGetSuccess += OnGetValue;
            dbUserRef.GetValue();
        }

        // Update is called once per frame
        void OnGetValue(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
        {

            Debug.Log("GetUser " + snapshot.RawJson);
            JSONNode parsed = JSON.Parse(snapshot.RawJson);
            CurrentUser = new UserProfile(parsed["uid"], parsed["email"], parsed["displayName"], parsed["photoURL"]);

            StartCoroutine(GetTexture());
        }


        IEnumerator GetTexture()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(CurrentUser.photoURL);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Debug.Log(www.error);
            }
            else
            {
                if (targetRenderer)
                {
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    targetRenderer.material.SetTexture(targetTexProp, myTexture);
                }
            }
        }
    }
}