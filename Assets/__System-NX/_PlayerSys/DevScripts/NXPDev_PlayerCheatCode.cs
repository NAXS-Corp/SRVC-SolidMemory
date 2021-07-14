using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAXS.NXPlayer;

public class NXPDev_PlayerCheatCode : MonoBehaviour
{
    public NXPMovementSettings CheatMovementSettings;
    private string[] cheatCode;
    private bool checkcheat = false;
    private int index;
    
    void Start() {
        // Code is "idkfa", user needs to input this in the right order
        cheatCode = new string[] { "g", "o", "d", "2", "0", "5", "0" };
        index = 0;    
    }
    
    void Update() {
        if (!checkcheat){
        // Check if any key is pressed
            if (Input.anyKeyDown) {
                // Check if the next key in the code is pressed
                if (Input.GetKeyDown(cheatCode[index])) {
                    // Add 1 to index to check the next key in the code
                    index++;
                }
                // Wrong key entered, we reset code typing
                else {
                    index = 0;    
                }
            }
        
        // If index reaches the length of the cheatCode string, 
        // the entire code was correctly entered
            if (index == cheatCode.Length) {
                // Cheat code successfully inputted!
                // Unlock crazy cheat code stuff
                var PlayerMovement = this.gameObject.GetComponent<NXP_Movement>();
                PlayerMovement.Settings = CheatMovementSettings;

                checkcheat = true;
            }
        }
    }
}
