using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Body;

public class Movement : MonoBehaviour, ITimeScalable
{
    public float speed = 700f;
    public float maxVelocity = 10f;
    public float footstepVelocity = 1f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public bool canMove = true;

    private float timeScale = 1f;
    public float TimeScale { get { return timeScale; } set { SetTimeScale(value); } }

    private Vector2 moveVector = new Vector2(0,0);
    private Vector2 aimVector = new Vector2(0, 0);
    public Vector2 castVector = new Vector2(0, 0);
    
    private bool hasFootsteps = false;
    FMOD.Studio.EventInstance footsteps;

    private Rigidbody myRigidbody;
    private Character character;
    private Animator animator;
    private Transform pivot;

    private readonly Dictionary<string, bool> parameterExists = new();

    public UnityEvent OnSetCastVector;
    public UnityEvent OnSetMoveVector;

    private void Awake()
    {
        footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/Footsteploop");
        character = GetComponent<Character>();
        myRigidbody = character.body.GetComponent<Rigidbody>();
        animator = character.animator;
        pivot = character.pivot;

        parameterExists["run"] = animator.HasParameter("run");
    }


    // Vectors

    public void UpdateCastVector()
    {
        if (moveVector.magnitude > 0 || aimVector.magnitude > 0)
        {
            castVector = aimVector.magnitude > 0 ? aimVector : moveVector;
            if (castVector.magnitude > 0)
            {
                OnSetCastVector.Invoke();
            }
        }
        else
        {
            float absY = Mathf.Abs(castVector.y);
            float absX = Mathf.Abs(castVector.x);
            if (absY >= absX)
            {
                float signY = Mathf.Sign(castVector.y);
                castVector = signY >= 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                float signX = Mathf.Sign(castVector.x);
                castVector = signX >= 0 ? Vector2.up : Vector2.down;
            }
        }
    }

    public void SetAimVector(Vector2 vector)
    {
        aimVector = vector;
        UpdateCastVector();
    }

    public void SetMoveVector(Vector2 vector)
    {
        moveVector = vector;
        myRigidbody.drag = moveVector.magnitude == 0 ? stopDrag : moveDrag;
        UpdateCastVector();
    }


    // Movement
    
    private void FixedUpdate()
    {
        if (canMove)
        {
            myRigidbody.AddRelativeForce(new Vector3(moveVector.x, 0, moveVector.y) * speed * Time.fixedDeltaTime * timeScale, ForceMode.Force);
            if (myRigidbody.velocity.magnitude > maxVelocity)
            {
                myRigidbody.velocity = myRigidbody.velocity.normalized * maxVelocity;
            }

            Vector2 hVelocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.z);
            if (hVelocity.magnitude > footstepVelocity)
            {
                float pMag = Mathf.Abs(pivot.localScale.x);
                float sign = myRigidbody.velocity.x > myRigidbody.velocity.z ? 1 : -1;
                pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

                if (!hasFootsteps)
                {
                    SetAnimBool("run", true);
                    hasFootsteps = true;
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


    // Animation

    private void SetAnimBool(string parameter, bool value)
    {
        if (parameterExists[parameter])
        {
            animator.SetBool(parameter, value);
        }
    }

    // TimeScaling
    private Vector3 tempVelocity;
    public void SetTimeScale(float timeScale)
    {
        if (myRigidbody != null && this.timeScale != timeScale)
        {
            if (timeScale == 0)
            {
                tempVelocity = myRigidbody.velocity;
                myRigidbody.velocity = new Vector3();
            }
            else if (this.timeScale == 0)
            {
                myRigidbody.velocity = tempVelocity;
            }
            else
            {
                float ratio = timeScale / this.timeScale;
                myRigidbody.velocity *= ratio;
            }
        }
        this.timeScale = timeScale;
    }
}
