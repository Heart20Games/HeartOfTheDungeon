using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : BaseMonoBehaviour
{
    public Vector3 direction = new Vector3();
    public float speed = 0;
    private new Rigidbody rigidbody;

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidbody.velocity = -transform.forward * speed * Time.fixedDeltaTime;
    }
}
