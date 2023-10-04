using MyBox;
using System.Collections.Generic;
using UnityEngine;
using static Colliders;

namespace HotD.Castables
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : CastedCollider, ICollidable
    {
        [Foldout("Projectile", true)]
        [ReadOnly][SerializeField] private Vector3 localPosition;
        public Vector3 direction = new();
        public float speed = 0;

        [Foldout("Collision", true)]
        private new Rigidbody rigidbody;
        private Collider[] colliders;
        private Collider[] Colliders { get { return colliders ?? InitializeColliders(); } }
        [Foldout("Collision")] public List<GameObject> collidableObjects;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private Collider[] InitializeColliders()
        {
            List<Collider> colliderList = new();
            collidableObjects.Add(gameObject);
            for (int i = 0; i < collidableObjects.Count; i++)
            {
                if (collidableObjects[i] != null)
                {
                    Collider[] components = collidableObjects[i].GetComponents<Collider>();
                    if (components != null)
                    {
                        colliderList.AddRange(components);
                    }
                }
            }
            colliders = colliderList.ToArray();
            return colliders;
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rigidbody.position = transform.position;
            rigidbody.velocity = speed * Time.fixedDeltaTime * direction;
        }

        private void FixedUpdate()
        {
            rigidbody.velocity = speed * Time.fixedDeltaTime * direction;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i].enabled = active;
            }
        }

        // Collision Exceptions
        public void AddException(Collider exception)
        {
            ChangeException(Colliders, exception, true);
        }
        public void RemoveException(Collider exception)
        {
            ChangeException(Colliders, exception, false);
        }
        public void AddExceptions(Collider[] exceptions)
        {
            ChangeExceptions(Colliders, exceptions, true);
        }
        public void RemoveExceptions(Collider[] exceptions)
        {
            ChangeExceptions(Colliders, exceptions, false);
        }
    }
}