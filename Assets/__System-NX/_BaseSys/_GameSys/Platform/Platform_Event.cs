using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace NAXS.GameSys
{
    public class Platform_Event : Platform_BaseClass
    {



        [TabGroup("PCVR")]
        public UnityEvent PCVR;

        [TabGroup("WebGL")]
        public UnityEvent WebGL;




        protected override void Setup(Platforms platform)
        {
            switch (platform)
            {
                case Platforms.PCVR:
                    PCVR.Invoke();
                    break;

                case Platforms.WebGL:
                    WebGL.Invoke();
                    break;

                default:
                    break;
            }

        }
    }
}