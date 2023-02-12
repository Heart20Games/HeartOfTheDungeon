using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour, ICastable
{
    public Transform weaponArt;
    private Animator animator;
    public Transform pivot;
    public Transform body;
    public bool swinging = false; // toggled in weapon 
    public float speed = 3f; // speed of the animation
    public int damage = 1;
    public bool followBody = true;
    FMOD.Studio.EventInstance daggerSwing;
    public UnityEvent onAttackComplete;

    private readonly List<Enemy> enemiesHit = new List<Enemy>();

    void Start()
    {
        daggerSwing = FMODUnity.RuntimeManager.CreateInstance("event:/Player SFX/DaggerSwing");
        animator = GetComponent<Animator>();
        animator.speed = speed;
        pivot.gameObject.SetActive(false);
    }

    // Castable

    public void Cast(Vector3 direction)
    {
        daggerSwing.start();
        pivot.gameObject.SetActive(true);
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
            pivot.localRotation = newRotation;
        }
        swinging = true;
        animator.SetTrigger("Swing");
    }
    public UnityEvent OnCasted() { return onAttackComplete; }
    public void Initialize(Character source)
    {
        Transform origin = followBody ? source.body : transform;
        Vector3 pivotLocalPosition = pivot.localPosition;
        pivot.SetParent(origin, false);
        pivot.localPosition = pivotLocalPosition;
        if (source.weaponHand != null && weaponArt)
        {
            Vector3 weaponLocalPosition = weaponArt.localPosition;
            weaponArt.SetParent(source.weaponHand, false);
            weaponArt.localPosition = weaponLocalPosition;
        }
    }
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
        swinging = false;
        pivot.gameObject.SetActive(false);
        if (enemiesHit.Count > 0)
        {
            enemiesHit.Clear();
        }
    }

    // Cleanup

    private void OnDestroy()
    {
        Destroy(pivot.gameObject);
        if (weaponArt != null)
        {
            Destroy(weaponArt.gameObject);
        }
    }
}
