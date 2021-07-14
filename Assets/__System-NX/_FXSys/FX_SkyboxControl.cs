using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.FX
{

    public enum PropertyMode
    {
        IsFloat, IsColor
    }

    [ExecuteInEditMode]
    public class FX_SkyboxControl : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool PreviewInEditor = true;
#endif
        public string ShaderProperty = "_Tint";
        public PropertyMode _PropertyMode = PropertyMode.IsColor;

        [ShowIf("_PropertyMode", PropertyMode.IsFloat)]
        [SerializeField]private float m_ctrlValue;
        public float CtrlValue{
            get{
                return m_ctrlValue;
            }
            set{
                m_ctrlValue = value;
            }
        }


        [ShowIf("_PropertyMode", PropertyMode.IsColor)]
        public Color color;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !PreviewInEditor)
            {
                return;
            }
#endif

            if (this._PropertyMode == PropertyMode.IsColor)
            {
                RenderSettings.skybox.SetColor(ShaderProperty, color);
            }
            else
            {
                RenderSettings.skybox.SetFloat(ShaderProperty, m_ctrlValue);
            }
        }
    }
}
