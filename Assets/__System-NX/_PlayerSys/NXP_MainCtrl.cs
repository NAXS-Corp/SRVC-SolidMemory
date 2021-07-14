using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NAXS;
using NAXS.NXHelper;
using NAXS.Base;
using NAXS.Event;
using NAXS.User;
using Sirenix.OdinInspector;
using NAXS.MirrorSys;

namespace NAXS.NXPlayer
{

    // Manage all sub-modules of NXP system
    public class NXP_MainCtrl : MonoBehaviour
    {
        public static NXP_MainCtrl LOCAL;
        #if UNITY_WEBGL || UNITY_EDITOR
        public bool DEVInitializeOnStart;
        #endif

        [Header("NXM NetPlayer")]
        [HideInInspector]public MirrorSys_NetPlayer_v2 MyNetPlayer;

        [Header("NXP Objects")]
        public NXP_PlayerInput _NXPInput;
        public NXP_Movement _NXPMovement;
        public NXP_AnimatorCtrl _NXPAnimator;
        public NXP_PlayerLook PlayerLook;
        public NXP_CameraManager _NXPCam;
        private bool enablePlayerSimulation = false;


        [Header("Buildin Objects")]
		public CharacterController _characterController; // The Unity's CharacterController
        public Animator _3rdAnimator;
        [ReadOnly]public Transform MainCam = null;


        public List<Object> DestroyOnRemotePlayer;
        public List<Object> DestroyOnLocalPlayer;

        
        public bool IsLocalPlayer{get; set;}

        #if UNITY_WEBGL || UNITY_EDITOR
        [Button]
        public void DevInitialize(){
            Initialize(true);
        }
        #endif

        public BoolEvent OnInitialize;
        void Awake()
        {
            //Disable all controllers before initialize
            SetAllControllersEnabled(false);
        }

        void Start()
        {
            // Debug.Log(" ### NXP_MainCtrl Started");
            DontDestroyOnLoad(this.gameObject);
            StartListeners();

            #if UNITY_WEBGL || UNITY_EDITOR
            if(DEVInitializeOnStart){
                DevInitialize();
            }
            #endif
        }
        
        void SetAllControllersEnabled(bool state){
            _NXPMovement.enabled = state;
            _NXPInput.enabled = state;
            if(_NXPAnimator)
                _NXPAnimator.enabled = state;
            if(_NXPCam)
                _NXPCam.enabled = state;
        }


        // Set all reference for supporting componentes
        public void Initialize(bool isLocalPlayer, MirrorSys_NetPlayer_v2 netPlayer = null)
        {
            this.IsLocalPlayer = isLocalPlayer;
            MyNetPlayer = netPlayer;

            // Listen to NXEvent
            ListenToUserProfile();
            // Fetch User profile, if NetPlayer already exists
            if(MyNetPlayer != null && MyNetPlayer.UserProfileCtrl.MyUserProfile != null){
                MyUserProfile = MyNetPlayer.UserProfileCtrl.MyUserProfile;
            }
            
            if(isLocalPlayer)
            {
                Debug.Log("Setup Local Player");
                SetupLocalPlayer();
            }else{
                SetupRemotePlayer();
            }

            // _NXPAnimator Initialize, should be moved into itself
            if(_NXPAnimator){
                _NXPAnimator._NXPMovement = _NXPMovement;
                if(_3rdAnimator)
                    _NXPAnimator._animator = _3rdAnimator;
            }
            OnInitialize.Invoke(this.IsLocalPlayer);
        }

        void SetupLocalPlayer(){
            
            Debug.Log("NXP_MainCtrl: SetupLocalPlayer");
            NXP_MainCtrl.LOCAL = this;
            gameObject.tag = "LocalPlayer";
            FindMainCam();
            SetAllControllersEnabled(true);
            _NXPMovement.Initialize(this);
            // _NXPInput.Initialize(this);
            if(_NXPCam)
                _NXPCam.Initialize(this);
            NXEvent.EmitEvent("OnLocalPlayerSpawned");
            // VR Player Follow
            NXEvent.SetData("VRPosFollow", this.transform);
            NXEvent.EmitEvent("VRPosFollow");
            foreach(GameObject obj in DestroyOnLocalPlayer){
                if(obj)
                    Destroy(obj);
            }

            // Transport local player to registered spawnpoint
            if(LOCAL_SPAWNPOINT != null){
                TransportTransform(LOCAL_SPAWNPOINT);
                // clear after transport, to prevent re-transporting
                LOCAL_SPAWNPOINT = null;
            }
        }

        void SetupRemotePlayer(){
            gameObject.tag = "Player";
            //Destroy Objects on Remote Player
            Destroy(_NXPMovement);
            Destroy(_NXPInput);
            foreach(GameObject obj in DestroyOnRemotePlayer){
                if(obj)
                    Destroy(obj);
            }
            if(_NXPAnimator)
                Destroy(_NXPAnimator);
            if(_NXPCam)
                Destroy(_NXPCam.gameObject);
        }

        void FindMainCam(){
            if(enablePlayerSimulation) return;
            MainCam = Camera.main.transform;
        }

        
        // /////////////////
        // User Profile

        [Header("User Profile")]
        private UserProfile myUserProfile;
        public UserProfile MyUserProfile{
            get{
                return myUserProfile;
            }
            set{
                // if user profile is changed
                if(myUserProfile != value){
                    myUserProfile = value;
                    OnUpdateUserProfile();
                }
            }
        }
        void ListenToUserProfile(){
            if(IsLocalPlayer){
                NXEvent.StartListening("OnGetLocalUserProfiile", () =>
                {
                    MyUserProfile = NXEvent.GetData("OnGetLocalUserProfiile") as UserProfile;
                    Debug.Log("NXP LocalUserProfile" + MyUserProfile.displayName);
                });
            }
        }

        void OnUpdateUserProfile(){
            Debug.Log("OnUpdateUserProfile"+ MyUserProfile.displayName);
            if(PlayerLook)
                PlayerLook.GetPhotoUrl(MyUserProfile.photoURL);
        }

        // /////////////////
        // API

        public void SwitchLocalPlayerNXP(GameObject newNXP){
            if(NXP_MainCtrl.LOCAL != this) return;
            if(!newNXP) return;
            // NXDebug.Log("### SwitchLocalPlayerNXP "+newNXP.name);
            GameObject newPlayer = Instantiate(newNXP, transform.position, transform.rotation);
            NXP_MainCtrl newPlayerCtrl = newPlayer.GetComponent<NXP_MainCtrl>();
            newPlayerCtrl.Initialize(true);
            Destroy(this.gameObject);
        }

        public void TransportTransform(Transform target){
            Debug.Log("NXP Transport Transform "+target.name+" "+_NXPMovement.transform.name);
            _NXPMovement.TransportTransform(target);
            Debug.Log("NXP Transport Transform "+target.name+" "+_NXPMovement.transform.name+" end");
        }
        
        public void TransportV3(Vector3 targetPos){
            Debug.Log("NXP Transport V3 "+targetPos +" "+_NXPMovement.transform.name);
            _NXPMovement.TransportV3(targetPos);
            Debug.Log("NXP Transport V3 "+targetPos +" "+_NXPMovement.transform.name+" end");
        }

        protected static Transform LOCAL_SPAWNPOINT;
        public static void SET_SPAWNPOINT(Transform target){
            if(NXP_MainCtrl.LOCAL)
                NXP_MainCtrl.LOCAL.TransportTransform(target); // transport immediate
            else
                LOCAL_SPAWNPOINT = target; // register and transport later
        }

        // /////////////////
        // API - Simulation
        public void SetSimulationMode(bool state){
            enablePlayerSimulation = state;
        }


        // /////////////////
        // Listeners
        void StartListeners(){
            NXEvent.StartListening("LocalPlayerTransport_Vector3", () => {
                Vector3 target = (Vector3)NXEvent.GetData("LocalPlayerTransport_Vector3");
                TransportV3(target);
            });
        }
    }
}