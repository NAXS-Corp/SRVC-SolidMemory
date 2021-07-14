
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.Event{
    public class NXEventCaller : MonoBehaviour
    {
        public bool AutoStart = true;
        public string EventName = "ChangeServerZoneID";
        public NXEventDataType DataType;

        [ShowIf("DataType", NXEventDataType.Int)]
        public int EventDataInt;

        [ShowIf("DataType", NXEventDataType.Boolean)]
        public bool EventDataBool;

        [ShowIf("DataType", NXEventDataType.String)]
        public string EventDataString;

        void Start(){
            if(AutoStart)
                CallEvent();
        }
        public void CallEvent(){
            switch (DataType)
            {
                case NXEventDataType.Boolean:
                    NXEvent.SetData(EventName, EventDataBool);
                    break;
                case NXEventDataType.Int:
                    NXEvent.SetData(EventName, EventDataInt);
                    break;
                case NXEventDataType.String:
                    NXEvent.SetData(EventName, EventDataString);
                    break;
                default:
                    break;
            }
            NXEvent.EmitEvent(EventName);
        }
    }
}