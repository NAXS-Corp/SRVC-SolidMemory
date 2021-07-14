using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NAXS.NXPlayer
{
    public static class NX_InputActionExtensions
    {
        public static bool IsPressed(this InputAction inputAction)
        {
            return inputAction.ReadValue<float>() > 0f;
        }
    
        public static bool WasPressedThisFrame(this InputAction inputAction)
        {
            return inputAction.triggered && inputAction.ReadValue<float>() > 0f;
        }
    
        public static bool WasReleasedThisFrame(this InputAction inputAction)
        {
            return inputAction.triggered && inputAction.ReadValue<float>() == 0f;
        }

        // Input Map shortcut
        public static InputAction Jump(this InputActionMap inputActionMap)
        {
            return inputActionMap.FindAction("Jump");
        }
        public static InputAction Move(this InputActionMap inputActionMap)
        {
            return inputActionMap.FindAction("Move");
        }
        public static InputAction Run(this InputActionMap inputActionMap)
        {
            return inputActionMap.FindAction("Run");
        }
        public static InputAction Camera(this InputActionMap inputActionMap)
        {
            return inputActionMap.FindAction("Camera");
        }
    }
}

