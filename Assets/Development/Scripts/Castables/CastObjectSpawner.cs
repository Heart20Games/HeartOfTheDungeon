using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public interface ISpawner
    {
        public void Spawn();
        public void Spawn(Vector3 direction = new Vector3());
        public void Activate(Vector3 direction);
    }

    public abstract class Spawner : Positionable, ISpawner, ISetCollisionExceptions
    {
        public float lifeSpan;
        public Transform pivot;
        public bool spawnOnEnable;

        public abstract Collider[] Exceptions { get; set; }

        public abstract void Activate(Vector3 direction);
        public abstract void SetExceptions(Collider[] exceptions);
        public abstract void Spawn();
        public abstract void Spawn(Vector3 direction = default);
    }

    public class CastObjectSpawner : Spawner
    {
        public CastedCollider castObject;
        [SerializeField] private bool followBody = false;
        [SerializeField] private Collider[] exceptions;
        [SerializeField] private List<CastedCollider> castObjects = new();
        [SerializeField] private bool debug = false;

        private void Awake()
        {
            if (pivot != null)
            {
                if (castObject == null)
                {
                    if (pivot.TryGetComponent<Pivot>(out var pivotType))
                    {
                        castObject = pivotType.body.GetComponent<CastedCollider>();
                    }
                }
                // Projectile is a prefab
                if (castObject.gameObject.scene.name == null)
                {
                    castObject = Instantiate(castObject, pivot);
                }
                castObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (spawnOnEnable)
            {
                Spawn();
            }
        }

        public override void SetOrigin(Transform source, Transform location)
        {
            base.SetOrigin(source, location);
        }

        public override void Spawn()
        {
            Spawn(false);
        }

        private void Spawn(bool noDirection)
        {
            if (noDirection)
            {
                Spawn(new Vector3());
            }
            else
            {
                Vector3 direction = (TargetPosition - OriginPosition).normalized;
                Spawn(direction);
            }
        }

        public override void Spawn(Vector3 direction = new Vector3())
        {

            if (debug) { Debug.Log($"{name} spawning projectile in {direction} direction.", this); }
            pivot.localPosition = offset;
            Transform pInstance = Instantiate(pivot, source);
            pInstance.gameObject.SetActive(true);
            if (pInstance.TryGetComponent(out Pivot pivotType))
            {
                CastedCollider bInstance = pivotType.body.GetComponentInChildren<CastedCollider>(true);
                if (bInstance != null)
                {
                    bInstance.SetActive(true);
                    castObjects.Add(bInstance);
                    AddExceptionsOn(exceptions, bInstance);
                    LaunchInstance(direction, pInstance.transform, bInstance);
                    bInstance.QueueCleanup(pInstance.transform, bInstance, lifeSpan, castObjects);
                }
                else
                {
                    Debug.LogWarning("Pivot body has no Projectile components.");
                }
            }
        }

        public override void Activate(Vector3 direction)
        {
            LaunchInstance(direction, pivot.transform, castObject);
        }

        private void LaunchInstance(Vector3 direction, Transform pInstance, CastedCollider projectile)
        {
            if (!followBody)
            {
                pInstance.position = OriginPosition + offset;
            }
            else
            {
                pInstance.localPosition = offset;
            }
            pInstance.gameObject.SetActive(true);
            projectile.gameObject.SetActive(true);
            projectile.direction = direction;
            projectile.transform.localPosition = new();
            Vector2 dir = new(direction.x, direction.z);
            Quaternion bRotation = projectile.transform.localRotation;
            pInstance.SetRotationWithVector(dir, rOffset);
            projectile.transform.localRotation = bRotation;
        }

        private IEnumerator CleanupInstance(Transform pInstance, Projectile bInstance)
        {
            Print("Waiting to cleanup Projectile instance.", debug, this);
            yield return new WaitForSeconds(lifeSpan);
            Print("Deleting Projectile instance...", debug, this);
            castObjects.Remove(bInstance);
            Destroy(pInstance.gameObject);
            Print("Cleaned up Projectile instance", debug, this);
        }

        // Collision Exceptions
        public override Collider[] Exceptions
        {
            get => exceptions;
            set => SetExceptions(value);
        }
        public override void SetExceptions(Collider[] exceptions)
        {
            for (int i = 0; i < castObjects.Count; i++)
            {
                CastedCollider pea = castObjects[i];
                RemoveExceptionsOn(this.exceptions, pea);
                AddExceptionsOn(exceptions, pea);
            }
            this.exceptions = exceptions;
        }

        private void AddExceptionsOn(Collider[] exceptions, CastedCollider pea)
        {
            if (debug) print($"Adding Exceptions on Projectile. ({this.exceptions?.Length} -> {exceptions?.Length})");
            
            if (exceptions != null)
            {
                if (debug) print("Actually adding it.");
                pea.AddExceptions(exceptions);
            }
        }

        private void RemoveExceptionsOn(Collider[] exceptions, CastedCollider pea)
        {
            if (debug) print($"Removing Exceptions on Projectile. ({this.exceptions?.Length} -> {exceptions?.Length})");
            if (exceptions != null)
            {
                pea.RemoveExceptions(exceptions);
            }
        }
    }
}