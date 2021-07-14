using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public float movementSpeed = 5f;
    [SerializeField] public float jumpHeight = 2f;

    private InputManager controls = null;
    private void Awake() => controls = new InputManager();
    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();


    // Update is called once per frame
    private void Update() => Move();

    public void Jump(){
            controls.Player.Jump.performed += callback => {
                transform.position = new Vector3
                (
                    transform.position.x,
                    transform.position.y + jumpHeight * Time.deltaTime,
                    transform.position.z
                );
            };
    }

    private void Move() {
        var movementInput = controls.Player.Movement.ReadValue<Vector2>();

        var movement = new Vector3
        {     
            x = movementInput.x,
            z = movementInput.y
        }.normalized;

        transform.Translate(movement * movementSpeed * Time.deltaTime);
        
    }
}
