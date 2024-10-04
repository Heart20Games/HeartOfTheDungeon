using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public interface IProjectileSpawner
    {
        public void Spawn();
        public void Spawn(Vector3 direction = new Vector3());
        public void Activate(Vector3 direction);
    }

    public class ProjectileSpawner : Positionable, IProjectileSpawner, ISetCollisionExceptions
    {
        public float lifeSpan;
        public Transform pivot;
        public Projectile projectile;
        [SerializeField] private bool followBody = false;
        public bool spawnOnEnable = true;
        [SerializeField] private Collider[] exceptions;
        [SerializeField] private List<Projectile> projectiles = new();
        [SerializeField] private bool debug = false;

        private void Awake()
        {
            if (pivot != null)
            {
                if (projectile == null)
                {
                    if (pivot.TryGetComponent<Pivot>(out var pivotType))
                    {
                        projectile = pivotType.body.GetComponent<Projectile>();
                    }
                }
                // Projectile is a prefab
                if (projectile.gameObject.scene.name == null)
                {
                    projectile = Instantiate(projectile, pivot);
                }
                projectile.SetActive(false);
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

        public void Spawn()
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

        public void Spawn(Vector3 direction = new Vector3())
        {

            if (debug) { Debug.Log($"{name} spawning projectile in {direction} direction.", this); }
            pivot.localPosition = offset;
            Transform pInstance = Instantiate(pivot, source);
            pInstance.gameObject.SetActive(true);
            if (pInstance.TryGetComponent(out Pivot pivotType))
            {
                Projectile bInstance = pivotType.body.GetComponentInChildren<Projectile>(true);
                if (bInstance != null)
                {
                    bInstance.SetActive(true);
                    projectiles.Add(bInstance);
                    AddExceptionsOn(exceptions, bInstance);
                    LaunchInstance(direction, pInstance.transform, bInstance);
                    bInstance.QueueCleanup(pInstance.transform, bInstance, lifeSpan, projectiles);
                }
                else
                {
                    Debug.LogWarning("Pivot body has no Projectile components.");
                }
            }
        }

        public void Activate(Vector3 direction)
        {
            LaunchInstance(direction, pivot.transform, projectile);
        }

        private void LaunchInstance(Vector3 direction, Transform pInstance, Projectile projectile)
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
            projectiles.Remove(bInstance);
            Destroy(pInstance.gameObject);
            Print("Cleaned up Projectile instance", debug, this);
        }

        // Collision Exceptions
        public Collider[] Exceptions
        {
            get => exceptions;
            set => SetExceptions(value);
        }
        public void SetExceptions(Collider[] exceptions)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                Projectile pea = projectiles[i];
                RemoveExceptionsOn(this.exceptions, pea);
                AddExceptionsOn(exceptions, pea);
            }
            this.exceptions = exceptions;
        }

        private void AddExceptionsOn(Collider[] exceptions, Projectile pea)
        {
            if (debug) print($"Adding Exceptions on Projectile. ({this.exceptions?.Length} -> {exceptions?.Length})");
            
            if (exceptions != null)
            {
                if (debug) print("Actually adding it.");
                pea.AddExceptions(exceptions);
            }
        }

        private void RemoveExceptionsOn(Collider[] exceptions, Projectile pea)
        {
            if (debug) print($"Removing Exceptions on Projectile. ({this.exceptions?.Length} -> {exceptions?.Length})");
            if (exceptions != null)
            {
                pea.RemoveExceptions(exceptions);
            }
        }
    }
}