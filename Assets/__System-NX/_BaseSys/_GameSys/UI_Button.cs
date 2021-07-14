using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NAXS.UI
{
    [System.Serializable]
    public class UI_ButtonEvent : UnityEvent<UI_Button> {}
    public class UI_Button : MonoBehaviour
    {
        public UnityEvent<UI_Button> e;
        public bool interactableButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        public virtual void Click()
        {
            e.Invoke(this);
            
            if (interactableButton)
            {
                if (this.GetComponent<Button>().IsInteractable())
                {
                    this.GetComponent<Button>().interactable = false;
                }
            }
        }

        public void OpenUrl(string url){
        #if !UNITY_WEBGL
            Application.OpenURL(url);
        #else
            Application.ExternalEval("window.open('"+url+"');");
        #endif
        }
    }
}
