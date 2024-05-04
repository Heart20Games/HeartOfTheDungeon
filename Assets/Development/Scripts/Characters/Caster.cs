using UnityEngine;
using Body;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class Caster : BaseMonoBehaviour
    {
        public bool canAttack = true;
        private Character character;
        private ArtRenderer artRenderer;
        private Transform pivot;
        [SerializeField] private Transform weaponLocation;
        [SerializeField] private Transform firingLocation;
        public ICastable castable;
        private Vector3 weapRotation = Vector3.forward;
        [SerializeField] private bool debug;

        [Header("Vector")]
        public float aimDeadzone = 0.01f;
        public Vector3 fallback = new();
        public Vector3 directionOverride = new();
        public Vector3 targetOffset = new();
        public Vector3 aimVector = new();
        public Vector3 castVector = new();
        [ReadOnly] public Vector3 appliedVector = new();
        [ReadOnly] public Vector3 castDirection = new();
        [ReadOnly] public Vector3 finalDirection = new();
        public Vector3 rotationOffset = new();
        public UnityEvent<Vector3> OnSetCastVector;
        [ReadOnly] public Transform target;

        private void Awake()
        {
            if (TryGetComponent(out character))
            {
                artRenderer = character.artRenderer;
                pivot = character.pivot;
                if (weaponLocation == null)
                    weaponLocation = character.weaponLocation;
                if (firingLocation == null)
                    firingLocation = character.firingLocation;
            }
        }

        private void FixedUpdate()
        {
            SetTarget(target);
        }

        // Triggering the Castable

        public void AimCaster()
        {
            directionOverride = Crosshair.GetTargetedPosition() - firingLocation.position;
            UpdateVector();
        }

        public void UnAimCaster()
        {
            SetTarget(target);
            UpdateVector();
        }

        public void TriggerCastable(ICastable castable)
        {
            if (enabled)
            {
                AimingMethod method = castable.GetItem().aimingMethod;
                if (method == AimingMethod.OverTheShoulder)
                    AimCaster();
                this.castable = castable;
                Trigger();
                if (method == AimingMethod.OverTheShoulder)
                    UnAimCaster();
            }
        }

        public void ReleaseCastable(ICastable castable)
        {
            if (enabled && this.castable == castable)
            {
                TargetingMethod method = castable.GetItem().targetingMethod;
                if (method == TargetingMethod.AimBased)
                    AimCaster();
                Release();
                if (method == TargetingMethod.AimBased)
                    UnAimCaster();
            }
            else castable?.Release();
        }

        // Target
        public void SetTarget(Transform target)
        {
            this.target = target;
            if (target != null)
            {
                if (debug) print($"Set target: {target} (on {character.Name})");
                Vector3 castPoint = character.firingLocation.position;
                Vector3 direction = (target.position + targetOffset) - castPoint;
                //direction = Quaternion.Euler(rotationOffset) * direction;
                SetFallback(direction, false, true);
            }
            else SetFallback(new(), false, true);
        }

        // Fallback
        public void SetFallback(Vector3 fallback, bool isAimVector=false, bool setOverride = false)
        {
            if (isAimVector) fallback = OrientAimVector(fallback);
            if (debug) print($"Set fallback{(setOverride ? " override" : "")}: {fallback} (on {character.Name})");
            if (setOverride) directionOverride = fallback;
            else this.fallback = fallback;
            SetVector(aimVector);
        }

        // Cast Vector
        public Vector3 SetVector(Vector3 aimVector)
        {
            this.aimVector = aimVector;
            return UpdateVector();
        }
        public Vector3 UpdateVector()
        {
            if (directionOverride.magnitude > 0 || fallback.magnitude > 0 || aimVector.magnitude > 0)
            {
                if (debug) print($"Set vector: {aimVector} (on {character.Name}");
                
                // Prefer aiming, then target override, then fallback (movement).
                if (aimVector.magnitude > aimDeadzone) castVector = OrientAimVector(aimVector);
                else if (directionOverride.magnitude > 0) castVector = directionOverride;
                else castVector = fallback;

                if (castVector.magnitude > 0)
                    OnSetCastVector.Invoke(castVector);
            }
            return castVector;
        }

        // Camera Orientation
        public Vector3 OrientAimVector(Vector3 vector)
        {
            if (character.PlayerControlled) return OrientToCamera(character.body, vector);
            else return vector;
        }

        public Vector3 OrientToCamera(Transform body, Vector3 vector)
        {
            vector = new(vector.x, vector.y, -vector.z);
            Debug.DrawRay(body.position, vector * 2f, Color.blue, 0.5f);
            Vector3 cameraDirection = Camera.main.transform.position - body.position;
            vector = vector.Orient(cameraDirection.XZVector().FullY().normalized);
            Debug.DrawRay(body.position, vector * 2f, Color.yellow, 0.5f);
            return vector;
        }


        // Apply Cast Vector
        private Vector3 lastDirection;
        public void ApplyCastVector()
        {
            appliedVector = castVector;
            castDirection = castVector.normalized;
            if (castable != null && castable.CanCast())
            {
                float pMag = Mathf.Abs(pivot.localScale.x);
                float sign = castVector.x < 0 ? -1 : 1;
                sign *= character.movement.Flip ? 1 : -1;
                pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

                //if (Mathf.Abs(castVector.x) > 0.5f || Mathf.Abs(castVector.y) > 0.5f || Mathf.Abs(castVector.z) > 0.5f)
                weapRotation = castDirection; // Vector3.right * -castVector.x + Vector3.up * -castVector.y + Vector3.forward * -castVector.z;

                if (artRenderer != null)
                    artRenderer.Cast(castable.GetItem() == null ? 0 : castable.GetItem().attackIdx);

                if (weapRotation != lastDirection)
                    lastDirection = weapRotation;

                finalDirection = weapRotation;
                castable.Direction = weapRotation; // uses last rotation if not moving.
            }
        }

        // Trigger
        public void Trigger()
        {
            ApplyCastVector();
            castable?.Equip();
            castable?.Trigger();
        }
        public void Trigger(Vector2 aimVector)
        {
            SetVector(aimVector);
            Trigger();
        }

        // Release
        public void Release()
        {
            ApplyCastVector();
            castable?.Release();
        }
    }
}