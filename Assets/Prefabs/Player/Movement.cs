using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 700f;
    public float maxVelocity = 10f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public bool canMove = true;

    private Vector2 MovementVector = new Vector2(0,0);

    private Rigidbody myRigidbody;
    PlayerControls controls;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        controls = new PlayerControls();
    }

    public Vector2 getMoveVector()
    {
        return MovementVector;
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
            if (myRigidbody.velocity.magnitude > maxVelocity)
            {
                Debug.Log("Velocity clamped");
                myRigidbody.velocity = myRigidbody.velocity.normalized * maxVelocity;
            }
            else
            {
                Debug.Log("Velocity below maximum");
            }
        }
    }

    public void ReceiveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            Vector2 inputVector = context.ReadValue<Vector2>();
            MovementVector = inputVector;
            if (MovementVector.magnitude == 0)
            {
                myRigidbody.drag = stopDrag;
            } 
            else
            {
                myRigidbody.drag = moveDrag;
            }
        }
        //horizontalInput = input;
        //print(horizontalInput);
    }
}
