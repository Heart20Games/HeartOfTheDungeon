using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : BaseMonoBehaviour
{

    PlayerControls controls;
    PlayerControls.GroundMovementActions groundMovement;

    Vector2 horizontalInput;

    void Awake()
    {
        controls = new PlayerControls();
        groundMovement = controls.GroundMovement;
        //toggleFollow = controls.ToggleFollowers();
        // groundMovement.[action].performed += context => do something;
        groundMovement.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        
    }

    void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
