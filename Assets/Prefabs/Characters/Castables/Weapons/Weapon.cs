using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour, ICastable
{
    public Animator animator;
    public bool swinging = false; // toggled in weapon 
    public float speed = 3f; // speed of the animation
    public int damage = 1;
    FMOD.Studio.EventInstance daggerSwing;
    public UnityEvent onAttackComplete;

    private readonly List<Enemy> enemiesHit = new List<Enemy>();

    void Start()
    {
        daggerSwing = FMODUnity.RuntimeManager.CreateInstance("event:/Player SFX/DaggerSwing");
        animator = GetComponent<Animator>();
        animator.speed = speed;
    }

    // Castable

    public void Cast(Vector3 direction)
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
    public UnityEvent OnCasted() { return onAttackComplete; }
    public void Initialize(Character source, Transform effectSource = null) { }
    public void Disable() { }
    public void Enable() { }
    public bool CanCast() { return !swinging; }

    // HItting Enemies

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
