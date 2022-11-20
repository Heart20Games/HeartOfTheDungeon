using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Animator animator;
    public bool swinging = false; // toggled in weapon animation
    public float speed = 3f; // speed of the animation
    FMOD.Studio.EventInstance daggerSwing;
    void Start()
    {
        daggerSwing = FMODUnity.RuntimeManager.CreateInstance("event:/Player SFX/DaggerSwing");
        animator = GetComponent<Animator>();
        animator.speed = speed;
    }

    public void Swing(Vector3 direction)
    {
        daggerSwing.start();
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.localRotation = newRotation;
        }
        animator.SetTrigger("Swing");
    }
}
