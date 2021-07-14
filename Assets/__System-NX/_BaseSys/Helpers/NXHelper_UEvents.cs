using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NAXS.NXHelper{

    [System.Serializable]
    public class TransformEvent : UnityEvent<Transform>{}

    [System.Serializable]
    public class FXEvent : UnityEvent<float>{}

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>{}

    [System.Serializable]
    public class FloatEvent : UnityEvent<float>{}
    [System.Serializable]
    public class StringEvent : UnityEvent<string>{}
    [System.Serializable]
    public class IntEvent : UnityEvent<int>{}
}