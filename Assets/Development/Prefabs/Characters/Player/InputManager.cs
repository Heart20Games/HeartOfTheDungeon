using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    PlayerControls controls;
    PlayerControls.GroundActions groundMovement;

    Vector2 horizontalInput;

    void Awake()
    {
        controls = new PlayerControls();
        groundMovement = controls.Ground;

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
