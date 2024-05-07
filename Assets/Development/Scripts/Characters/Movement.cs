using UnityEngine;
using UnityEngine.Events;
using Body;
using UnityEngine.Assertions;
using MyBox;

namespace HotD.Body
{
    public interface IMovement : ITimeScalable
    {
        // Settings
        public MovementSettings Settings { get; set; }
        public bool UseGravity { get; set; }
        public bool CanMove { get; set; }
        
        // Initialization
        public void SetCharacter(Character character);
        
        // Status
        public void StopMoving();
        public Vector2 MoveVector { get; set; }
        public bool ShouldFlip { get; set; }
    }

    public class Movement : BaseMonoBehaviour, IMovement
    {
        [Header("Settings")]
        [SerializeField] protected MovementSettings settings;
        [SerializeField] protected bool useGravity = true;
        [SerializeField] protected bool canMove = true;
        [SerializeField] protected bool debug;
        public MovementSettings Settings { get => settings; set => settings = value; }
        public bool UseGravity { get => useGravity; set => useGravity = value; }
        public bool CanMove { get => canMove; set => canMove = value; }

        [Header("Scaling")]
        public float npcModifier = 0.5f;

        [Foldout("Mechanics", true)]
        private bool onGround = false;

        [SerializeField] private Vector2 moveVector = new(0, 0);
        public Vector2 MoveVector
        {
            get => moveVector;
            set
            {
                moveVector = value;
                myRigidbody.drag = moveVector.magnitude == 0 ? settings.stopDrag : settings.moveDrag;
                onSetMoveVector.Invoke(moveVector);
            }
        }

        protected Rigidbody myRigidbody;
        private Character character;
        [SerializeField] private Transform body;
        private Transform pivot;
        private ArtRenderer artRenderer;

        [SerializeField] private UnityEvent<Vector2> onSetMoveVector;

        public void SetCharacter(Character character)
        {
            this.character = character;
            body = character.body;
            pivot = character.pivot;
            artRenderer = character.artRenderer;
        }

        private void Awake()
        {
            if (body == null) body = transform;
            if (body != null)
            {
                if (!body.TryGetComponent(out myRigidbody))
                {
                    Debug.LogError("Movement expects it's body to have a Rigidbody. None found.", this);
                }
                else
                {
                    myRigidbody.useGravity = false;
                }
            }
            if (settings != null)
            {
                useGravity = settings.useGravity;
            }
            onSetMoveVector ??= new();
        }


        // Stop Moving

        public void StopMoving()
        {
            myRigidbody.velocity = Vector3.zero;
        }


        // Movement

        [Foldout("Animation", true)]
        private float flipBuffer = 0.01f;
        private bool hasFootsteps = false;
        [ReadOnly] private bool flip = false;
        [SerializeField] private UnityEvent startWalking;
        [SerializeField] private UnityEvent stopWalking;
        [SerializeField] private UnityEvent<bool> onFlip;

        public bool ShouldFlip
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
            if (isActiveAndEnabled)
            {
                if (canMove)
                {
                    Vector3 cameraDirection = body.position;
                    if (Camera.main != null)
                    {
                        cameraDirection -= Camera.main.transform.position;
                    }

                    float modifier = 1f;
                    if (character != null && character.PlayerControlled)
                    {
                        ApplyPlayerMovement(cameraDirection, moveVector);
                    }
                    else
                    {
                        ApplyNPCMovement(ref modifier, moveVector);
                    }

                    if (myRigidbody.velocity.magnitude > settings.maxVelocity * modifier)
                        myRigidbody.velocity = settings.maxVelocity * modifier * myRigidbody.velocity.normalized;

                    if (artRenderer != null && pivot != null)
                    {
                        Vector2 hVelocity = myRigidbody.velocity.XZVector();
                        Vector2 hCamera = cameraDirection.XZVector().normalized;
                        Vector2 right = Vector2.Perpendicular(hCamera);
                        if (hVelocity.magnitude > settings.footstepVelocity)
                        {
                            float angle = Vector2.Dot(right, hVelocity);

                            if ((ShouldFlip && angle > flipBuffer) || (!ShouldFlip && angle < -flipBuffer))
                            {
                                ShouldFlip = !ShouldFlip;
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

                if (useGravity)
                {
                    ApplyGravityForce();
                }
            }
        }

        protected void ApplyGravityForce(float scale=1, bool checkForGround=true)
        {
            float power = settings.gravityForce;
            if (checkForGround)
            {
                onGround = Physics.Raycast(body.transform.position, Vector3.down, settings.groundDistance);
                power = onGround ? settings.normalForce : settings.gravityForce;
            }
            myRigidbody.AddForce(power * scale * settings.speed * Time.fixedDeltaTime * timeScale * Vector3.down, ForceMode.Force);
        }

        protected void ApplyPlayerMovement(Vector3 cameraDirection, Vector2 moveVector, float scale=1)
        {
            Print("Character Controlled Movement", debug);
            Vector3 direction = moveVector.Orient(cameraDirection).FullY();
            Debug.DrawRay(body.position, direction * 3, Color.green, Time.fixedDeltaTime);
            myRigidbody.AddRelativeForce(scale * settings.speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
        }

        protected void ApplyNPCMovement(ref float modifier, Vector2 moveVector, float scale=1)
        {
            Print($"NPC Controlled Movement ({character})", debug);
            modifier = npcModifier;
            Vector3 direction = moveVector.FullY();
            Assert.IsFalse(float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z));
            Debug.DrawRay(body.position, direction, Color.green, Time.fixedDeltaTime);
            myRigidbody.AddForce(modifier * timeScale * scale * settings.speed * Time.fixedDeltaTime * direction, ForceMode.Force);
        }


        // TimeScaling
        private float timeScale = 1f;
        public float TimeScale 
        {
            get => timeScale;
            set => timeScale = SetTimeScale(value);
        }
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
}