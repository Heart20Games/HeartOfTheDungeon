using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Health : BaseMonoBehaviour, IHealth
{
    public int healthTotal = 0;
    public int health = 0;

    [Foldout("Events", true)]
    public UnityEvent<int> onSetHealth;
    public UnityEvent<int> onTakeDamage;
    public UnityEvent<int, Identity> onTakeDamageFrom;
    [Foldout("Events")]
    public UnityEvent onNoHealth;

    public virtual void HealDamage(int amount)
    {
        health += amount;
    }
    public virtual void SetHealth(int amount)
    {
        health = amount;
    }
    public virtual void SetHealthBase(int total) { SetHealthBase(total, total); }
    public virtual void SetHealthBase(int amount, int total)
    {
        health = amount;
        SetHealthTotal(amount);
    }
    public virtual void SetHealthTotal(int amount)
    {
        healthTotal = amount;
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Identity.Neutral);
    }

    public virtual void TakeDamage(int amount, Identity id = Identity.Neutral)
    {
        health -= amount;
        onSetHealth.Invoke(health);
        onTakeDamage.Invoke(amount);
        onTakeDamageFrom.Invoke(amount, id);
        if (health <= 0)
        {
            health = Mathf.Max(health, 0);
            onNoHealth.Invoke();
        }
    }

    public virtual void SetDamagePosition(Vector3 location) { }
}
