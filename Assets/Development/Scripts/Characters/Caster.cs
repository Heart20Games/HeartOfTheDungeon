using UnityEngine;
using Body;
using UnityEngine.Events;
using HotD.Body;

namespace HotD.Castables
{
    public interface ICaster : IEnableable
    {
        public void TriggerCastable(ICastable castable);
        public void ReleaseCastable(ICastable castable);
        public void SetFallback(Vector3 fallback, bool isAimVector = false, bool setOverride = false);
        public Vector3 SetVector(Vector3 aimVector);
    }

    public class Caster : BaseMonoBehaviour, ICaster
    {
        private bool canAttack = true;
        private Character character;
        private ArtRenderer artRenderer;
        private Transform pivot;
        [SerializeField] private Transform weaponLocation;
        [SerializeField] private Transform firingLocation;
        private ICastable castable;
        private Vector3 weapRotation = Vector3.forward;
        [SerializeField] private bool debug;

        [Header("Vector")]
        [SerializeField] private float aimDeadzone = 0.01f;
        [SerializeField] private Vector3 fallback = new();
        [SerializeField] private Vector3 directionOverride = new();
        [SerializeField] private Vector3 targetOffset = new();
        [SerializeField] private Vector3 aimVector = new();
        [SerializeField] private Vector3 castVector = new();
        [SerializeField][ReadOnly] private Vector3 appliedVector = new();
        [SerializeField][ReadOnly] private Vector3 castDirection = new();
        [SerializeField][ReadOnly] private Vector3 finalDirection = new();
        private Vector3 rotationOffset = new();
        [SerializeField] private UnityEvent<Vector3> OnSetCastVector = new();
        [SerializeField][ReadOnly] private Transform target;

        private void Awake()
        {
            if (TryGetComponent(out character))
            {
                artRenderer = character.artRenderer;
                pivot = character.pivot;
                if (weaponLocation == null)
                    weaponLocation = character.weaponLocation;
                if (firingLocation == null)
                    firingLocation = character.firingLocation != null ? character.firingLocation : character.pivot;
            }
        }

        private void FixedUpdate()
        {
            SetTarget(target);
        }

        // Aiming the Castable

        private void AimCaster()
        {
            directionOverride = Crosshair.GetTargetedPosition() - firingLocation.position;
            UpdateVector();
        }

        private void UnAimCaster()
        {
            SetTarget(target);
            UpdateVector();
        }

        // Triggering the Castable

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
                TargetingMethod method = castable.Item.targetingMethod;
                if (method == TargetingMethod.AimBased)
                    AimCaster();
                Release();
                if (method == TargetingMethod.AimBased)
                    UnAimCaster();
            }
            else castable?.QueueAction(CastAction.Release);
        }

        // Target
        private void SetTarget(Transform target)
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
        private Vector3 UpdateVector()
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
        private Vector3 OrientAimVector(Vector3 vector)
        {
            if (character.PlayerControlled) return OrientToCamera(character.body, vector);
            else return vector;
        }

        private Vector3 OrientToCamera(Transform body, Vector3 vector)
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
        private void ApplyCastVector()
        {
            appliedVector = castVector;
            castDirection = castVector.normalized;
            if (castable != null && castable.CanCast())
            {
                float pMag = Mathf.Abs(pivot.localScale.x);
                float sign = castVector.x < 0 ? -1 : 1;
                sign *= character.movement.ShouldFlip ? 1 : -1;
                pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

                //if (Mathf.Abs(castVector.x) > 0.5f || Mathf.Abs(castVector.y) > 0.5f || Mathf.Abs(castVector.z) > 0.5f)
                weapRotation = castDirection; // Vector3.right * -castVector.x + Vector3.up * -castVector.y + Vector3.forward * -castVector.z;

                if (artRenderer != null)
                    artRenderer.Cast(castable.Item == null ? 0 : castable.Item.attackIdx);

                if (weapRotation != lastDirection)
                    lastDirection = weapRotation;

                finalDirection = weapRotation;
                castable.Direction = weapRotation; // uses last rotation if not moving.
            }
        }

        // Trigger
        private void Trigger()
        {
            ApplyCastVector();
            castable?.QueueAction(CastAction.Equip);
            castable?.QueueAction(CastAction.Trigger);
        }
        private void Trigger(Vector2 aimVector)
        {
            SetVector(aimVector);
            Trigger();
        }

        // Release
        private void Release()
        {
            ApplyCastVector();
            castable?.QueueAction(CastAction.Release);
        }
    }
}