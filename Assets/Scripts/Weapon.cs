using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Animator animator;
    public bool swinging = false; // toggled in weapon animation
    public float speed = 3f; // speed of the animation
    
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = speed;
    }

    public void Swing(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.localRotation = newRotation;
        }
        animator.SetTrigger("Swing");
    }
}
