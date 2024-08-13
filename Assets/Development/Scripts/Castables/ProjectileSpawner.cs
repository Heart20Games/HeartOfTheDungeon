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

    public class ProjectileSpawner : Positionable, IProjectileSpawner, ICollidables
    {
        public float lifeSpan;
        public Transform pivot;
        public Projectile projectile;
        [SerializeField] private bool followBody = false;
        [SerializeField] private bool spawnOnEnable = true;
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

            if (isActiveAndEnabled)
            {
                if (debug) { Debug.Log($"{name} spawning projectile in {direction} direction.", this); }
                pivot.localPosition = offset;
                Transform pInstance = Instantiate(pivot, source);
                pInstance.gameObject.SetActive(true);
                if (pInstance.TryGetComponent(out Pivot pivotType))
                {
                    if (pivotType.body.TryGetComponent(out Projectile bInstance))
                    {
                        bInstance.SetActive(true);
                        projectiles.Add(bInstance);
                        AddExceptionsOn(exceptions, bInstance);
                        LaunchInstance(direction, pInstance.transform, bInstance);
                        StartCoroutine(CleanupInstance(pInstance.transform, bInstance));
                    }
                    else
                    {
                        Debug.LogWarning("Pivot body should be a Projectile.");
                    }
                }
            }
        }

        public void Activate(Vector3 direction)
        {
            if (isActiveAndEnabled)
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
            yield return new WaitForSeconds(lifeSpan);
            projectiles.Remove(bInstance);
            Destroy(pInstance.gameObject);
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
                AddExceptionsOn(exceptions, pea);
            }
            this.exceptions = exceptions;
        }

        public void AddExceptionsOn(Collider[] exceptions, Projectile pea)
        {
            if (debug) print($"Setting Exceptions on Projectile. ({this.exceptions?.Length} -> {exceptions?.Length})");
            if (this.exceptions != null)
            {
                pea.RemoveExceptions(this.exceptions);
            }
            if (exceptions != null)
            {
                pea.AddExceptions(exceptions);
            }
        }
    }
}