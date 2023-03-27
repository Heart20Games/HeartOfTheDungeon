using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : Castable
{
    public Transform weaponArt;
    private Animator animator;
    public Transform pivot;
    public Transform body;
    public float rotationOffset = 0;
    public bool swinging = false; // toggled in weapon 
    public float swingLength = 1; // Non-animation swing time
    public float speed = 3f; // speed of the animation
    public float instanceLifeSpan = 2; // Lifespan of instances
    public int damage = 1;
    public bool followBody = true;
    public bool instanced = false;

    private readonly List<IDamageable> others = new List<IDamageable>();
    private readonly List<IDamageable> ignored = new List<IDamageable>();

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

    public override void Initialize(Character source)
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

    private Vector3 lastDirection;
    public override void Cast(Vector3 direction)
    {
        if (instanced)
        {
            if (direction != lastDirection)
            {
                lastDirection = direction;
            }
            Transform pInstance = Instantiate(pivot);
            Transform bInstance = pInstance.GetComponent<Pivot>().body;
            CastInstance(direction, pInstance, bInstance);
            StartCoroutine(CleanupInstance(instanceLifeSpan, pInstance, bInstance));
        }
        else
        {
            CastInstance(direction, pivot, body);
        }
        Swing();
        onCast.Invoke();
    }

    public override bool CanCast() { return !swinging; }


    // Casting

    public void CastInstance(Vector3 direction, Transform pInstance, Transform bInstance)
    {
        if (!followBody)
        {
            pInstance.position = source.body.position + source.weaponOffset;
        }
        else
        {
            pInstance.localPosition = source.weaponOffset;
        }
        bInstance.localPosition = new Vector3();
        pInstance.gameObject.SetActive(true);
        Vector2 dir = new Vector2(direction.x, direction.z);
        pInstance.SetRotationWithVector(dir, rotationOffset);
        bInstance.localRotation = Quaternion.identity;
    }

    public IEnumerator CleanupInstance(float lifeSpan, Transform pInstance, Transform bInstance)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(pInstance.gameObject);
    }


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
