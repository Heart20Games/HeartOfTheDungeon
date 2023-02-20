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
    private Character character;
    private Animator animator;
    private Transform pivot;
    FMOD.Studio.EventInstance footsteps;

    private Dictionary<string, bool> parameterExists = new Dictionary<string, bool>();

    private void Awake()
    {
        footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/Player SFX/PlayerFootstep");
        character = GetComponent<Character>();
        myRigidbody = character.body.GetComponent<Rigidbody>();
        animator = character.animator;
        pivot = character.pivot;

        parameterExists["run"] = animator.HasParameter("run");
    }

    public Vector2 getAttackVector()
    {
        Vector3 center = new Vector3(Screen.width, Screen.height, 0)/2;
        return (Input.mousePosition - center).normalized;
    }

    public void SetInputVector(Vector2 inputVector)
    {
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

    private void FixedUpdate()
    {
        if (canMove)
        {
            myRigidbody.AddRelativeForce(new Vector3(MovementVector.x, 0, MovementVector.y) * speed * Time.fixedDeltaTime, ForceMode.Force);
            if (myRigidbody.velocity.magnitude > maxVelocity)
            {
                myRigidbody.velocity = myRigidbody.velocity.normalized * maxVelocity;
            }

            if (myRigidbody.velocity.magnitude > footstepVelocity)
            {
                float pMag = Mathf.Abs(pivot.localScale.x);
                float sign = myRigidbody.velocity.x > myRigidbody.velocity.z ? 1 : -1;
                pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

                if (!hasFootsteps)
                {
                    SetAnimBool("run", true);
                    hasFootsteps = true;
                    footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    footsteps.start();
                }
            } 
            else if (hasFootsteps)
            {
                SetAnimBool("run", false);
                hasFootsteps = false;
                footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    private void SetAnimBool(string parameter, bool value)
    {
        if (parameterExists[parameter])
        {
            animator.SetBool(parameter, value);
        }
    }
}
