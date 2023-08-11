using UnityEngine;
using UnityEngine.Events;
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
    public float npcModifier = 0.5f;
    private float timeScale = 1f;
    public float TimeScale { get => timeScale; set => timeScale=SetTimeScale(value); }

    [Header("Vectors")]
    [SerializeField] public Vector2 moveVector = new(0,0);
    private bool onGround = false;

    private Rigidbody myRigidbody;
    private Character character;
    private Transform body;
    private Transform pivot;
    private ArtRenderer artRenderer;

    public UnityEvent<Vector2> OnSetMoveVector;

    private void Awake()
    {
        if (TryGetComponent(out character))
        {
            body = character.body;
            pivot = character.pivot;
            artRenderer = character.artRenderer;
        }
        if (body == null)
            body = transform;
        if (body != null)
            myRigidbody = body.GetComponent<Rigidbody>();

    }


    // Move Vector

    public void SetMoveVector(Vector2 vector)
    {
        moveVector = vector;
        myRigidbody.drag = moveVector.magnitude == 0 ? stopDrag : moveDrag;
        OnSetMoveVector.Invoke(moveVector);
    }


    // Movement

    [Header("Animation")]
    public float flipBuffer = 0.01f;
    private bool hasFootsteps = false;
    [ReadOnly] private bool flip = false;
    public UnityEvent startWalking;
    public UnityEvent stopWalking;
    public UnityEvent<bool> onFlip;

    public bool Flip
    {
        get => flip;
        set
        {
            flip = value;
            float pMag = Mathf.Abs(pivot.localScale.x);
            float sign = flip ? -1 : 1; // Mathf.Sign(angle);
            pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);
            onFlip.Invoke(flip);
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 cameraDirection = body.position - Camera.main.transform.position;

            float modifier = 1f;
            if (character != null && character.controllable)
            {
                Vector3 direction = moveVector.Orient(cameraDirection).FullY();
                Debug.DrawRay(body.position, direction * 3, Color.green, Time.fixedDeltaTime);
                myRigidbody.AddRelativeForce(speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
            }
            else
            {
                modifier = npcModifier;
                Vector3 direction = moveVector.FullY();
                Assert.IsFalse(float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z));
                Debug.DrawRay(body.position, direction, Color.green, Time.fixedDeltaTime);
                myRigidbody.AddForce(modifier * speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
            }
            
            if (myRigidbody.velocity.magnitude > maxVelocity * modifier)
                myRigidbody.velocity = maxVelocity * modifier * myRigidbody.velocity.normalized;

            if (artRenderer != null && pivot != null)
            {
                Vector2 hVelocity = myRigidbody.velocity.XZVector();
                Vector2 hCamera = cameraDirection.XZVector().normalized;
                Vector2 right = Vector2.Perpendicular(hCamera);
                if (hVelocity.magnitude > footstepVelocity)
                {
                    float angle = Vector2.Dot(right, hVelocity);

                    if ((Flip && angle > flipBuffer) || (!Flip && angle < -flipBuffer))
                    {
                        Flip = !Flip;
                    }

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
        }

        if (applyGravity)
        {
            onGround = Physics.Raycast(body.transform.position, Vector3.down, groundDistance);
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
