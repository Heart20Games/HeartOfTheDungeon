using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.Assertions;

public class Movement : BaseMonoBehaviour, ITimeScalable
{
    [Header("Physics")]
    public float speed = 700f;
    public float maxVelocity = 10f;
    public float footstepVelocity = 1f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public bool canMove = true;
    public bool applyGravity = true;
    public float normalForce = 0.1f;
    public float gravityForce = 1f;
    public float groundDistance = 0.01f;

    [Header("Scale")]
    private float timeScale = 1f;
    public float TimeScale { get => timeScale; set => timeScale=SetTimeScale(value); }
    public float npcModifier = 0.5f;

    [Header("Vectors")]
    [SerializeField] private Vector2 moveVector = new(0,0);
    private Vector2 aimVector = new(0, 0);
    public Vector2 castVector = new(0, 0);
    private bool onGround = false;

    private bool hasFootsteps = false;
    private Rigidbody myRigidbody;
    private Character character;
    private Transform pivot;
    private ArtRenderer artRenderer;

    public UnityEvent OnSetCastVector;
    public UnityEvent OnSetMoveVector;

    private void Awake()
    {
        character = GetComponent<Character>();
        myRigidbody = character.body.GetComponent<Rigidbody>();
        pivot = character.pivot;
        artRenderer = character.artRenderer;
    }


    // Vectors

    public void UpdateCastVector()
    {
        //castVector = new();
        if (moveVector.magnitude > 0 || aimVector.magnitude > 0)
        {
            castVector = aimVector.magnitude > 0 ? aimVector : moveVector;
            if (castVector.magnitude > 0)
                OnSetCastVector.Invoke();
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
    
    public UnityEvent startWalking;
    public UnityEvent stopWalking;

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 cameraDirection = character.body.position - Camera.main.transform.position;

            float modifier = 1f;
            if (character.controllable)
            {
                Vector3 direction = moveVector.Orient(cameraDirection).FullY();
                Debug.DrawRay(character.body.position, direction * 3, Color.green, Time.fixedDeltaTime);
                myRigidbody.AddRelativeForce(speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
            }
            else
            {
                modifier = npcModifier;
                Vector3 direction = moveVector.FullY();
                Assert.IsFalse(float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z));
                Debug.DrawRay(character.body.position, direction, Color.green, Time.fixedDeltaTime);
                myRigidbody.AddForce(modifier * speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
            }
            
            if (myRigidbody.velocity.magnitude > maxVelocity * modifier)
                myRigidbody.velocity = maxVelocity * modifier * myRigidbody.velocity.normalized;

            Vector2 hVelocity = myRigidbody.velocity.XZVector();
            Vector2 hCamera = cameraDirection.XZVector().normalized;
            Vector2 right = Vector2.Perpendicular(hCamera);
            if (hVelocity.magnitude > footstepVelocity)
            {
                float pMag = Mathf.Abs(pivot.localScale.x);
                float sign = Mathf.Sign(Vector2.Dot(right, hVelocity));
                pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

                if (!hasFootsteps)
                {
                    artRenderer.Running = true;
                    hasFootsteps = true;
                }
            } 
            else if (hasFootsteps)
            {
                artRenderer.Running = false;
                hasFootsteps = false;
            }
        }

        if (applyGravity)
        {
            onGround = Physics.Raycast(character.body.transform.position, Vector3.down, groundDistance);
            float power = onGround ? normalForce : gravityForce;
            myRigidbody.AddForce(speed * timeScale * power * Time.fixedDeltaTime * Vector3.down);
        }
    }


    // TimeScaling
    private Vector3 tempVelocity;
    public float SetTimeScale(float timeScale)
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
        return timeScale;
    }
}
