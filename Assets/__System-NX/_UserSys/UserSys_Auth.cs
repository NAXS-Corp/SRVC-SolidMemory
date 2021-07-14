using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEngine.Networking;
using NAXS.Event;


namespace NAXS.User
{
    [System.Serializable]
    public class UserProfile
    {
        public string uid;
        public string email;
        public string displayName;
        public string photoURL;
        
        public UserProfile(){
            this.uid = null;
            this.email = null;
            this.displayName = null;
            this.photoURL = null;
        }
        public UserProfile(string uid, string email, string displayName, string photoURL)
        {
            this.uid = uid;
            this.email = email;
            this.displayName = displayName;
            this.photoURL = photoURL;
        }
    }

    public class UserSys_Auth : MonoBehaviour
    {
        public string TestUID;
        public static string LocalUID;
        public static UserProfile LocalUser;
        protected SimpleFirebaseUnity.Firebase dbUserRef;

        
        void GetUserProfile(string uid)
        {
            dbUserRef = Firebase_Manager.instance.firebase.Child("users/" + uid);
            dbUserRef.OnGetSuccess += OnGetProfile;
            dbUserRef.GetValue();
        }

        // Update is called once per frame
        void OnGetProfile(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
        {
            Debug.Log("OnGetProfile " + snapshot.RawJson);
            JSONNode parsed = JSON.Parse(snapshot.RawJson);
            LocalUser = new UserProfile(parsed["uid"], parsed["email"], parsed["displayName"], parsed["photoURL"]);

            //broadcast the user settings. Receiver:  MirrorSys_UserProfile, NXP_MainCtrl
            NXEvent.EmitEventData("OnGetLocalUserProfiile", LocalUser);
        }

        ////////////////////
        // API

        // called from frontend
        public void SetUserUID(string uid)
        {
            Debug.Log("UserSys_Auth SetUserUID " + uid);
            LocalUID = uid;
            GetUserProfile(uid);
        }

        // Dev
        [Button]
        void Test()
        {
            SetUserUID(TestUID);
        }
    }
}