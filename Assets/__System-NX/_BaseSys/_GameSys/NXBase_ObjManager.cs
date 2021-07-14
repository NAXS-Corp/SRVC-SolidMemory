using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


namespace NAXS.Base{

    [System.Serializable]
    public struct NXObject{
        public string ID;
        public Object obj;
    }

    [ExecuteInEditMode]
    public class NXBase_ObjManager : SerializedMonoBehaviour
    {
        private static NXBase_ObjManager _instance;
        public static NXBase_ObjManager instance{
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType (typeof (NXBase_ObjManager)) as NXBase_ObjManager;

                    if (!_instance)
                    {
                        Debug.LogError ("There needs to be one active NXBase_ObjManager script on a GameObject in your scene.");
                    }
                }
                return _instance;
            }
        }
        
        
        // Basics
        public Light SunLight;
        public Camera MainCam;
        public Transform LocalPlayer;
        public static Camera GetMainCam{
            get{
                return NXBase_ObjManager.instance.MainCam;
            }
        }
        public static Light GetSunLight{
            get{
                return NXBase_ObjManager.instance.SunLight;
            }
        }

        [SerializeField]
        public Dictionary<string, Object> GlobalObjects = new Dictionary<string, Object>()
        {
            {"MainCam", null},
            {"SunLight", null}
        };

        public static Dictionary<string, Object> _GlobalObject{
            get{
                return instance.GlobalObjects;
            }
        }

        void Awake()
        {
            NXBase_ObjManager._instance = this;
        }


        public static void SetObject(string ID, Object newObject){
            if(instance.GlobalObjects.ContainsKey(ID)){
                instance.GlobalObjects[ID] = newObject;
            }else{
                instance.GlobalObjects.Add(ID, newObject);
            }
        }

        public static Object FetchObject(string ID){
            instance.GlobalObjects.TryGetValue(ID, out Object feteched);
            if(!feteched)
            {
                Debug.LogError("Object haven't been set");
                return null;
            }
            return feteched;
        }
        

    }
}
