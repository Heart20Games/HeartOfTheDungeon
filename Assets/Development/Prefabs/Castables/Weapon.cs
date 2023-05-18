using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;

public class Weapon : Castable
{
    public Transform weaponArt;
    private Animator animator;
    public Transform pivot;
    public Transform body;
    public bool swinging = false; // toggled in weapon 
    public float swingLength = 1; // Non-animation swing time
    public float speed = 3f; // speed of the animation
    public int damage = 1;

    private readonly List<IDamageable> others = new();
    private readonly List<IDamageable> ignored = new();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null )
        {
            animator.speed = speed;
        }
        pivot.gameObject.SetActive(false);
    }


    // Castable

    public override void Initialize(Body.Character source)
    {
        base.Initialize(source);
        Transform origin = followBody ? source.body : transform;
        IDamageable damageable = source.body.GetComponent<IDamageable>();
        if (damageable != null)
        {
            ignored.Add(damageable);
        }
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

    public override void Cast(Vector3 direction)
    {
        Swing();
        base.Cast(direction);
    }

    public override bool CanCast() { return !swinging; }


    // Swinging

    public void Swing()
    {
        if (animator != null)
        {
            SwingForAnimation();
        }
        else
        {
            StartCoroutine(SwingForSeconds(swingLength));
        }
    }

    public IEnumerator SwingForSeconds(float seconds)
    {
        StartSwinging();
        yield return new WaitForSeconds(seconds);
        DoneSwinging();
    }

    public void SwingForAnimation()
    {
        StartSwinging();
        animator.SetTrigger("Swing");
    }

    public void HitDamagable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && !ignored.Contains(other) && !others.Contains(other))
        {
            others.Add(other);
            other.TakeDamage(damage);
        }
    }

    public void LeftDamagable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && !ignored.Contains(other) && others.Contains(other))
        {
            others.Remove(other);
        }
    }

    public void StartSwinging()
    {
        swinging = true;
        pivot.gameObject.SetActive(true);
    }

    public void DoneSwinging()
    {
        swinging = false;
        pivot.gameObject.SetActive(false);
        others.Clear();
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
