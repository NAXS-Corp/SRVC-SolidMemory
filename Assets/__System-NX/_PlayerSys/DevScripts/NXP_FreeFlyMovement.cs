using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXP_FreeFlyMovement : MonoBehaviour
{
    public CharacterController characterController;
    public float Speed = 0.5f;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)){
            Vector3 dir = Camera.main.transform.forward;
            dir *= Time.deltaTime * Speed;
            characterController.Move(dir);
        }
    }
}
