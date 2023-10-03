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
        public ICastable Castable;
        private Vector3 weapRotation = Vector3.forward;
        [SerializeField] private bool debug;

        [Header("Vector")]
        public Vector2 fallback = new();
        public Vector2 fbOverride = new();
        public Vector2 aimVector = new();
        public Vector2 castVector = new();
        public Vector3 rotationOffset = new();
        public UnityEvent<Vector2> OnSetCastVector;
        public bool useTargetOnly = false;
        public bool UseTargetOnly { get => useTargetOnly; set => useTargetOnly = value; }
        [ReadOnly][SerializeField] private Transform target;

        private void Awake()
        {
            character = GetComponent<Character>();
            artRenderer = character.artRenderer;
            pivot = character.pivot;
        }

        private void FixedUpdate()
        {
            SetTarget(target);
        }

        // Cast Vector
        public void SetTarget(Transform target)
        {
            this.target = target;
            if (target != null)
            {
                if (debug) print($"Set target: {target} (on {character.Name})");
                Vector3 castPoint = character.firingLocation.position;
                Vector3 direction = (target.position + (Vector3.up * 0.5f)) - castPoint;
                direction = Quaternion.Euler(rotationOffset) * direction;
                SetFallback(direction.XZVector(), true);
            }
            else SetFallback(new(), true);
        }
        public void SetFallback(Vector2 fallback, bool setOverride = false)
        {
            if (debug) print($"Set fallback{(setOverride ? " override" : "")}: {fallback} (on {character.Name})");
            if (setOverride) fbOverride = fallback;
            else this.fallback = fallback;
            SetVector(aimVector);
        }
        public void SetVector(Vector2 aimVector)
        {
            this.aimVector = aimVector;
            if (fallback.magnitude > 0 || aimVector.magnitude > 0)
            {
                if (debug) print($"Set vector: {aimVector} (on {character.Name}");
                Vector2 fallback = fbOverride.magnitude > 0 ? fbOverride : this.fallback;
                if (aimVector.magnitude > 0 && !useTargetOnly)
                {
                    castVector = OrientToCamera(character.body, aimVector);
                }
                else
                {
                    castVector = fallback;
                }
                if (castVector.magnitude > 0)
                    OnSetCastVector.Invoke(castVector);
            }
        }

        public Vector3 OrientToCamera(Transform body, Vector2 castVector)
        {
            Debug.DrawRay(body.position, castVector.FullY() * 2f, Color.blue, 0.5f);
            Vector3 cameraDirection = body.position - Camera.main.transform.position;
            castVector = castVector.Orient(cameraDirection.XZVector().normalized).normalized;
            Debug.DrawRay(body.position, castVector.FullY() * 2f, Color.yellow, 0.5f);
            return castVector;
        }


        // Cast
        private Vector3 lastDirection;

        // Cast based on the given Transform's position relative to the Camera.
        public void Cast(Transform body, Vector2 castVector)
        {
            Cast(castVector);
        }

        // Cast using the given vector
        public void Cast(Vector2 castVector)
        {
            SetCastVector(castVector);
            Trigger();
        }

        public void SetCastVector(Vector2 castVector)
        {
            if (Castable != null && Castable.CanCast())
            {
                float pMag = Mathf.Abs(pivot.localScale.x);
                float sign = castVector.x < 0 ? -1 : 1;
                sign *= character.movement.Flip ? 1 : -1;
                pivot.localScale = new Vector3(pMag * sign, pivot.localScale.y, pivot.localScale.z);

                if (Mathf.Abs(castVector.x) > 0.5f || Mathf.Abs(castVector.y) > 0.5f)
                    weapRotation = Vector3.right * -castVector.x + Vector3.forward * -castVector.y;

                if (artRenderer != null)
                    artRenderer.Cast(Castable.GetItem() == null ? 0 : Castable.GetItem().attackIdx);

                if (weapRotation != lastDirection)
                    lastDirection = weapRotation;

                Castable.Direction = weapRotation; // uses last rotation if not moving
            }
        }

        public void Trigger()
        {
            Castable?.Trigger();
        }

        public void Release()
        {
            Castable?.Release();
        }
    }
}