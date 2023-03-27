using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
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

    private Vector3 lastDirection;
    private void FixedUpdate()
    {
        if (rigidbody != null)
        {
            if (-transform.forward != lastDirection)
            {
                print("Projectile: " + -transform.forward + " -> " + transform.rotation);
                lastDirection = -transform.forward;
            }
            rigidbody.velocity = -transform.forward * speed * Time.fixedDeltaTime;
        }
    }
}
