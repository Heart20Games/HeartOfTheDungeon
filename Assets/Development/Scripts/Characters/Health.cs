using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IHealth : IDamageReceiver
{
    public void SetHealthTotal(int amount);
    public void SetHealthBase(int total);
    public void SetHealthBase(int amount, int total);
    public void SetHealth(int amount);
    public void HealDamage(int amount);
}

public class Health : BaseMonoBehaviour, IHealth
{
    public int healthTotal = 0;
    public int health = 0;

    [Foldout("Events", true)]
    public UnityEvent<int> onSetHealth;
    public UnityEvent<int> onSetHealthTotal;
    public UnityEvent<int> onTakeDamage;
    public UnityEvent<int, Identity> onTakeDamageFrom;
    [Foldout("Events")]
    public UnityEvent onNoHealth;

    public void Refresh()
    {
        SetHealth(healthTotal);
    }

    public virtual void HealDamage(int amount)
    {
        health += amount;
        onSetHealth.Invoke(health);
    }
    public virtual void SetHealth(int amount)
    {
        health = amount;
        onSetHealth.Invoke(health);
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
        onSetHealthTotal.Invoke(healthTotal);
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Identity.Neutral);
    }

    public virtual void TakeDamage(int amount, Identity id = Identity.Neutral)
    {
        int initial = health;
        health -= amount;
        onSetHealth.Invoke(health);
        onTakeDamage.Invoke(amount);
        onTakeDamageFrom.Invoke(amount, id);
        if (health <= 0)
        {
            health = 0;
            if (initial > 0)
                onNoHealth.Invoke();
        }
    }

    public virtual void SetDamagePosition(Vector3 location) { }
}
