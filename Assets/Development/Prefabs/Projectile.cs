using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Colliders;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : BaseMonoBehaviour, ICollidable
{
    public Vector3 direction = new();
    public float speed = 0;
    private new Rigidbody rigidbody;
    private Collider[] colliders;
    private Collider[] Colliders { get { return colliders ?? InitializeColliders(); } }
    public List<GameObject> collidableObjects;

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
        rigidbody.velocity = speed * Time.fixedDeltaTime * -transform.forward;
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
