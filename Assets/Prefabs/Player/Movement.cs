using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 800f;
    public bool canMove = true;

    private Vector2 MovementVector = new Vector2(0,0);

    private Rigidbody myRigidbody;

    PlayerControls controls;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        controls = new PlayerControls();
        
        
    }

    private void Update()
    {
        // this.transform.local
        
        //myRigidbody.AddForce(new Vector3(MovementVector.x, 0, MovementVector.y) * speed * Time.deltaTime, ForceMode.Force); // deltatime decouples the framerate from movement.
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            myRigidbody.AddRelativeForce(new Vector3(MovementVector.x, 0, MovementVector.y) * speed * Time.deltaTime, ForceMode.Force);
        }
    }

    public void ReceiveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Movement input");
            Debug.Log(context.valueType);
            Debug.Log(context.ReadValue<Vector2>());

            Vector2 inputVector = context.ReadValue<Vector2>();
            // nowthe actual movement logic
            // myRigidbody.AddForce(new Vector3(inputVector.x, 0, inputVector.y)* speed, ForceMode.Force);
            // time to hack it.
            MovementVector = inputVector;
        }
        //horizontalInput = input;
        //print(horizontalInput);
    }
}
