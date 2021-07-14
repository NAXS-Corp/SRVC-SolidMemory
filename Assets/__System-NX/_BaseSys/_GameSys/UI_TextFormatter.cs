using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NAXS.UI{
    
    [ExecuteInEditMode]
    public class UI_TextFormatter : MonoBehaviour
    {
        [TextArea(15,20)]
        public string TextInput;
        [TextArea(15,20)]
        public string TextResult;

        [Button]
        public void ConvertToFirebaseFormat(){
            if(!string.IsNullOrEmpty(TextInput)){
                TextResult= "\""+TextInput;
                TextResult = TextResult.Replace("\n", "\\n");
                TextResult = TextResult.Replace("\r", "\\r");
                TextResult += "\"";
            }
        }
    }

}