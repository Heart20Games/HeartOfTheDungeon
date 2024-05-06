using UnityEngine;
using UnityEngine.Events;
using Body;
using UnityEngine.Assertions;
using MyBox;

namespace HotD.Body
{
    public class Movement : BaseMonoBehaviour, ITimeScalable
    {
        [Header("Settings")]
        public MovementSettings settings;
        public bool applyGravity = true;
        public bool canMove = true;
        public bool debug;
        public float Speed { get => settings.speed; }
        public float MaxVelocity { get => settings.maxVelocity; }
        public float FootstepVelocity { get => settings.footstepVelocity; }
        public float MoveDrag { get => settings.moveDrag; }
        public float StopDrag { get => settings.stopDrag; }
        public float NormalForce { get => settings.normalForce; }
        public float GravityForce { get => settings.gravityForce; }
        public float GroundDistance { get => settings.groundDistance; }

        [Header("Scale")]
        public float npcModifier = 0.5f;
        private float timeScale = 1f;
        public float TimeScale { get => timeScale; set => timeScale = SetTimeScale(value); }

        [Foldout("Mechanics", true)]
        [SerializeField] public Vector2 moveVector = new(0, 0);
        private bool onGround = false;

        private Rigidbody myRigidbody;
        [HideInInspector] public Character character;
        public Transform body;
        [HideInInspector] public Transform pivot;
        [HideInInspector] public ArtRenderer artRenderer;

        public UnityEvent<Vector2> OnSetMoveVector;

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
                applyGravity = settings.applyGravity;
            }
            OnSetMoveVector ??= new();
        }


        // Stop Moving

        public void StopMoving()
        {
            myRigidbody.velocity = Vector3.zero;
        }


        // Move Vector

        public void SetMoveVector(Vector2 vector)
        {
            moveVector = vector;
            myRigidbody.drag = moveVector.magnitude == 0 ? StopDrag : MoveDrag;
            OnSetMoveVector.Invoke(moveVector);
        }


        // Movement

        [Foldout("Animation", true)]
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

                    if (myRigidbody.velocity.magnitude > MaxVelocity * modifier)
                        myRigidbody.velocity = MaxVelocity * modifier * myRigidbody.velocity.normalized;

                    if (artRenderer != null && pivot != null)
                    {
                        Vector2 hVelocity = myRigidbody.velocity.XZVector();
                        Vector2 hCamera = cameraDirection.XZVector().normalized;
                        Vector2 right = Vector2.Perpendicular(hCamera);
                        if (hVelocity.magnitude > FootstepVelocity)
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
                    ApplyGravity();
                }
            }
        }

        public void ApplyGravity(float scale=1, bool checkForGround=true)
        {
            float power = GravityForce;
            if (checkForGround)
            {
                onGround = Physics.Raycast(body.transform.position, Vector3.down, GroundDistance);
                power = onGround ? NormalForce : GravityForce;
            }
            myRigidbody.AddForce(power * scale * Speed * Time.fixedDeltaTime * timeScale * Vector3.down, ForceMode.Force);
        }

        public void ApplyPlayerMovement(Vector3 cameraDirection, Vector2 moveVector, float scale=1)
        {
            Print("Character Controlled Movement", debug);
            Vector3 direction = moveVector.Orient(cameraDirection).FullY();
            Debug.DrawRay(body.position, direction * 3, Color.green, Time.fixedDeltaTime);
            myRigidbody.AddRelativeForce(scale * Speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
        }

        public void ApplyNPCMovement(ref float modifier, Vector2 moveVector, float scale=1)
        {
            Print($"NPC Controlled Movement ({character})", debug);
            modifier = npcModifier;
            Vector3 direction = moveVector.FullY();
            Assert.IsFalse(float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z));
            Debug.DrawRay(body.position, direction, Color.green, Time.fixedDeltaTime);
            myRigidbody.AddForce(modifier * timeScale * scale * Speed * Time.fixedDeltaTime * direction, ForceMode.Force);
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
}