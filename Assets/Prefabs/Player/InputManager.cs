using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    PlayerControls controls;
    PlayerControls.GroundMovementActions groundMovement;

    Vector2 horizontalInput;

    void Awake()
    {
        controls = new PlayerControls();
        groundMovement = controls.GroundMovement;

        // groundMovement.[action].performed += context => do something;
        groundMovement.Movement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

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
