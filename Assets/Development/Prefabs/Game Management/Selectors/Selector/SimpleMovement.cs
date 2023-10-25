using UnityEngine;
using UnityEngine.Events;

public class SimpleMovement : BaseMonoBehaviour, ITimeScalable
{
    [Header("Components")]
    public Rigidbody myRigidbody;

    [Header("Settings")]
    public MovementSettings settings;
    [ReadOnly] public bool canMove = true;

    [Header("Scale")]
    [SerializeField] private float timeScale = 1f;
    public float TimeScale { get => timeScale; set => timeScale = SetTimeScale(value); }

    [Header("Vectors")]
    [SerializeField] private Vector2 moveVector = new(0, 0);
    public Vector2 MoveVector { get { return moveVector; } set { SetMoveVector(value); } }
    private bool onGround = false;
    public bool swapAxes = true;
    public UnityEvent<Vector2> onSetMoveVector;

    // Initialization

    private void Awake()
    {
        if (settings == null) settings = ScriptableObject.CreateInstance<MovementSettings>();
        if (myRigidbody == null) myRigidbody = GetComponent<Rigidbody>();
    }

    // Movement

    public void SetMoveVector(Vector2 vector)
    {
        print("Setting move vector");
        moveVector = vector.SwapAxes(swapAxes) * (swapAxes ? new Vector2(-1, 1) : Vector2.one);
        myRigidbody.drag = moveVector.magnitude == 0 ? settings.stopDrag : settings.moveDrag;
        onSetMoveVector.Invoke(MoveVector);
        //cursor.transform.SetLocalRotationWithVector(MoveVector);
    }

    private void FixedUpdate()
    {
        Vector3 cameraDirection = transform.position - Camera.main.transform.position;
        float speed = settings.speed;

        if (canMove)
        {
            Vector3 direction = moveVector.Orient(cameraDirection).FullY();
            Debug.DrawRay(transform.position, direction * 3, Color.green, Time.fixedDeltaTime);
            myRigidbody.AddRelativeForce(speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
        }

        float maxVelocity = settings.maxVelocity;
        if (myRigidbody.velocity.magnitude > maxVelocity)
            myRigidbody.velocity = myRigidbody.velocity.normalized * maxVelocity;

        if (settings.applyGravity)
        {
            onGround = Physics.Raycast(transform.position, Vector3.down, settings.groundDistance);
            float power = onGround ? settings.normalForce : settings.gravityForce;
            myRigidbody.AddForce(speed * timeScale * power * Time.fixedDeltaTime * Vector3.down);
        }
    }

    // TimeScale

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
