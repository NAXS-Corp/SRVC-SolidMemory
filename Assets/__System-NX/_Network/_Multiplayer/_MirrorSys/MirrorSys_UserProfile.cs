using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NAXS.Event;
using NAXS.User;

namespace NAXS.MirrorSys
{
    public class MirrorSys_UserProfile : NetworkBehaviour
    {
        private MirrorSys_NetPlayer_v2 myNetPlayer;
        [SyncVar(hook = nameof(OnRemoteGetUserProfile))] public UserProfile MyUserProfile; //the userProfile owned by this local or remote player


        private void Start()
        {

            myNetPlayer = GetComponent<MirrorSys_NetPlayer_v2>();
            WathLocalUserProfile();
        }

        void WathLocalUserProfile()
        {
            if (!isLocalPlayer) return;
            NXEvent.StartListening("OnGetLocalUserProfiile", () =>
            {

                MyUserProfile = NXEvent.GetData("OnGetLocalUserProfiile") as UserProfile;
                Debug.Log("WathLocalUserProfile" + MyUserProfile.displayName);

                SetupUserProfile(MyUserProfile);
                CmdSetUserProfile(MyUserProfile);

            });
        }


        // from local to server
        [Command]
        void CmdSetUserProfile(UserProfile userProfile)
        {
            MyUserProfile = userProfile;
        }

        // Remote client got update from the server
        void OnRemoteGetUserProfile(UserProfile oldUserProfile, UserProfile newUserProfile)
        {
            SetupUserProfile(newUserProfile);
        }

        public void SetupUserProfile(UserProfile userProfile)
        {
            if(myNetPlayer.CurrentNXPlayer)
                myNetPlayer.CurrentNXPlayer.MyUserProfile = userProfile;
        }
    }
}