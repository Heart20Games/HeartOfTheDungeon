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
    public float rotationOffset = 0;
    public bool swinging = false; // toggled in weapon 
    public float speed = 3f; // speed of the animation
    public int damage = 1;
    public bool followBody = true;
    FMOD.Studio.EventInstance daggerSwing;
    public UnityEvent onAttackComplete;

    private readonly List<IDamageable> others = new List<IDamageable>();
    private readonly List<IDamageable> ignored = new List<IDamageable>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = speed;
        pivot.gameObject.SetActive(false);
    }

    private void Start()
    {
        daggerSwing = FMODUnity.RuntimeManager.CreateInstance("event:/Player SFX/DaggerSwing");
    }


    // Castable

    public void Cast(Vector3 direction)
    {
        daggerSwing.start();
        pivot.gameObject.SetActive(true);
        print("Weapon Swing: " + direction);
        pivot.transform.SetRotationWithVector(direction, rotationOffset);
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
    public void UnEquip() { Destroy(gameObject); }
    

    // Swinging

    public void HitDamagable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && !ignored.Contains(other) && !others.Contains(other))
        {
            Debug.Log("Damage it!");
            others.Add(other);
            other.TakeDamage(damage);
        }
    }

    public void LeftDamagable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && others.Contains(other))
        {
            others.Remove(other);
        }
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
