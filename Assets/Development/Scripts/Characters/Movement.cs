using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using MyBox;
using System.Collections.Generic;

namespace HotD.Body
{
    public interface IMovement : ITimeScalable
    {
        // Settings
        public MoveSettings Settings { get; set; }
        public List<MoveModifier> Modifiers { get; set; }
        public bool UseGravity { get; set; }
        public bool CanMove { get; set; }
        public void AddOrRemoveModifiers(List<MoveModifier> modifiers, bool add);
        
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
        [SerializeField] protected MovementTemplate settingsTemplate;
        [SerializeField] protected MoveSettings settings;
        [SerializeField] protected List<MoveModifier> modifiers = new();
        [SerializeField] protected bool useGravity = true;
        [SerializeField] protected bool canMove = true;
        [SerializeField] protected bool debug;
        protected bool settingsInitialized = false;
        protected bool settingsModified = false;
        public MoveSettings Settings
        {
            get
            {
                if (!settingsInitialized && settingsTemplate != null)
                {
                    modifiers?.AddRange(settingsTemplate.modifiers);
                    settingsModified = false;
                    settingsInitialized = true;
                }
                if (!settingsModified)
                {
                    settings = settingsTemplate.settings.Modified(modifiers);
                    settingsModified = true;
                }
                return settings;
            }
            set => settings = value.Modified(modifiers);
        }
        public List<MoveModifier> Modifiers { get => modifiers; set => modifiers = value; }
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
                MyRigidbody.drag = moveVector.magnitude == 0 ? Settings.stopDrag : Settings.moveDrag;
                onSetMoveVector.Invoke(moveVector);
            }
        }

        protected Rigidbody myRigidbody;
        protected Rigidbody MyRigidbody
        {
            get
            {
                if (myRigidbody == null && !Body.TryGetComponent(out myRigidbody))
                    Warning("Movement expects it's body to have a Rigidbody. None found.", debug, this);
                return myRigidbody;
            }
        }

        private Character character;
        [SerializeField] private Transform body;
        protected Transform Body
        {
            get
            {
                if (body == null) body = transform;
                return body;
            }
        }

        private Transform pivot;
        private ArtRenderer artRenderer;

        [SerializeField] private UnityEvent<Vector2> onSetMoveVector;

        public void SetCharacter(Character character)
        {
            this.character = character;
            body = character.Body;
            pivot = character.Pivot;
            artRenderer = character.ArtRenderer;
        }

        public void AddOrRemoveModifiers(List<MoveModifier> modifiers, bool add)
        {
            if (add)
            {
                Modifiers.AddRange(modifiers);
            }
            else
            {
                foreach (var modifier in modifiers)
                {
                    for (int i = Modifiers.Count-1; i >= 0; i--)
                    {
                        if (Modifiers[i].Equals(modifier))
                        {
                            Modifiers.RemoveAt(i);
                        } 
                    }
                }
            }
            settingsModified = false;
        }

        [ButtonMethod]
        public void ReapplyModifiers()
        {
            settingsModified = false;
        }


        private void Awake()
        {
            if (MyRigidbody != null)
            {
                MyRigidbody.useGravity = false;
            }
            useGravity = Settings.useGravity;
            onSetMoveVector ??= new();
        }


        // Stop Moving

        public void StopMoving()
        {
            MyRigidbody.velocity = Vector3.zero;
            artRenderer.RunVelocity = MyRigidbody.velocity.magnitude;
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
                    Vector3 cameraDirection = Body.position;
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

                    if (MyRigidbody.velocity.magnitude > Settings.maxVelocity * modifier)
                        MyRigidbody.velocity = Settings.maxVelocity * modifier * MyRigidbody.velocity.normalized;

                    if (artRenderer != null && pivot != null)
                    {
                        Vector2 hVelocity = MyRigidbody.velocity.XZVector2();
                        Vector2 hCamera = cameraDirection.XZVector2().normalized;
                        Vector2 right = Vector2.Perpendicular(hCamera);
                        if (hVelocity.magnitude > Settings.footstepVelocity)
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

            if (artRenderer != null && MyRigidbody != null)
            {
                artRenderer.RunVelocity = MyRigidbody.velocity.magnitude;
            }
        }

        protected void ApplyGravityForce(float scale=1, bool checkForGround=true)
        {
            float power = Settings.gravityForce;
            if (checkForGround)
            {
                onGround = Physics.Raycast(Body.transform.position, Vector3.down, Settings.groundDistance);
                power = onGround ? Settings.normalForce : Settings.gravityForce;
            }
            MyRigidbody.AddForce(power * scale * Settings.speed * Time.fixedDeltaTime * timeScale * Vector3.down, ForceMode.Force);
        }

        protected void ApplyPlayerMovement(Vector3 cameraDirection, Vector2 moveVector, float scale=1)
        {
            Print("Character Controlled Movement", debug, this);
            Vector3 direction = moveVector.Orient(cameraDirection).FullY();
            Debug.DrawRay(Body.position, direction * 3, Color.green, Time.fixedDeltaTime);
            MyRigidbody.AddRelativeForce(scale * Settings.speed * Time.fixedDeltaTime * timeScale * direction, ForceMode.Force);
        }

        protected void ApplyNPCMovement(ref float modifier, Vector2 moveVector, float scale=1)
        {
            Print($"NPC Controlled Movement ({character})", debug, this);
            modifier = npcModifier;
            Vector3 direction = moveVector.FullY();
            Assert.IsFalse(float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z));
            Debug.DrawRay(Body.position, direction, Color.green, Time.fixedDeltaTime);
            MyRigidbody.AddForce(modifier * timeScale * scale * Settings.speed * Time.fixedDeltaTime * direction, ForceMode.Force);
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
            if (MyRigidbody != null && this.timeScale != timeScale)
            {
                if (timeScale == 0)
                {
                    tempVelocity = MyRigidbody.velocity;
                    MyRigidbody.velocity = new Vector3();
                }
                else if (this.timeScale == 0)
                {
                    MyRigidbody.velocity = tempVelocity;
                }
                else
                {
                    float ratio = timeScale / this.timeScale;
                    MyRigidbody.velocity *= ratio;
                }
            }
            this.timeScale = timeScale;
            return timeScale;
        }
    }
}