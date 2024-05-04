using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public class ProjectileSpawner : Positionable, ICollidables
    {
        public float lifeSpan;
        public Transform pivot;
        public Projectile projectile;
        public bool followBody = false;
        public Collider[] exceptions;
        [SerializeField] private List<Projectile> projectiles = new();
        public bool debug = false;

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

        public override void SetOrigin(Transform source, Transform target)
        {
            base.SetOrigin(source, target);
        }

        public void Spawn()
        {
            Spawn(new Vector3());
        }

        public void Spawn(Vector3 direction = new Vector3())
        {

            if (isActiveAndEnabled)
            {
                if (debug) { Debug.Log($"{name} spawning projectile in {direction} direction."); }
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

        public void LaunchInstance(Vector3 direction, Transform pInstance, Projectile projectile)
        {
            if (!followBody)
            {
                pInstance.position = target.position + offset;
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

        public IEnumerator CleanupInstance(Transform pInstance, Projectile bInstance)
        {
            yield return new WaitForSeconds(lifeSpan);
            projectiles.Remove(bInstance);
            Destroy(pInstance.gameObject);
        }

        // Collision Exceptions
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