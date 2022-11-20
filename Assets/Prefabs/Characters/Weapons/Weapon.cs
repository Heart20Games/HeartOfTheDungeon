using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Animator animator;
    public bool swinging = false; // toggled in weapon 
    public float speed = 3f; // speed of the animation
    public int damage = 1;
    FMOD.Studio.EventInstance daggerSwing;

    private readonly List<Enemy> enemiesHit = new List<Enemy>();

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
        Debug.Log("Swinging -- " + swinging);
        swinging = true;
        animator.SetTrigger("Swing");
    }

    public bool HitEnemy(Enemy enemy)
    {
        bool hit = !enemiesHit.Contains(enemy);
        if (hit)
        {
            enemiesHit.Add(enemy);
        }
        return hit;
    }

    public void DoneSwinging()
    {
        Debug.Log("Done Swinging - " + swinging);
        swinging = false;
        if (enemiesHit.Count > 0)
        {
            enemiesHit.Clear();
        }
    }
}
