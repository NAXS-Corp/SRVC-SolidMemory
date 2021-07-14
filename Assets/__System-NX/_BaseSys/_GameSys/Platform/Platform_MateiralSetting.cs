using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.GameSys
{
    public class Platform_MateiralSetting : Platform_BaseClass
    {
        public Renderer TargetRenderer;

        [TabGroup("Default")]
        public Material Material_Default;

        [TabGroup("PCVR")]
        public Material Material_PCVR;

        [TabGroup("WebGL")]
        public Material Material_WebGL;




        protected override void Setup(Platforms platform)
        {
            switch (platform)
            {
                case Platforms.PCVR:
                    TargetRenderer.material = Material_PCVR;
                    break;

                case Platforms.WebGL:
                    TargetRenderer.material = Material_WebGL;
                    break;

                default:
                    TargetRenderer.material = Material_Default;
                    break;
            }

        }
    }
}