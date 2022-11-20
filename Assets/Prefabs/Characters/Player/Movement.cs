using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 700f;
    public float maxVelocity = 10f;
    public float footstepVelocity = 1f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public bool canMove = true;
    private bool hasFootsteps = false;

    private Vector2 MovementVector = new Vector2(0,0);

    private Rigidbody myRigidbody;
    PlayerControls controls;
    FMOD.Studio.EventInstance footsteps;

    private void Awake()
    {
        footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/Player SFX/PlayerFootstep");
        myRigidbody = GetComponent<Rigidbody>();
        controls = new PlayerControls();
    }

    public Vector2 getAttackVector()
    {
        Vector3 center = new Vector3(Screen.width, Screen.height, 0)/2;
        return (Input.mousePosition - center).normalized;
        //return MovementVector;
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

            if (myRigidbody.velocity.magnitude > footstepVelocity)
            {
                if (!hasFootsteps)
                {
                    hasFootsteps = true;
                    footsteps.start();
                }
                
            } 
            else if (hasFootsteps)
            {
                hasFootsteps = false;
                footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
    }
}
