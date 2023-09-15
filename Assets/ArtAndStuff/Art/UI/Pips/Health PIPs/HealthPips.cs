using Body.Behavior.ContextSteering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;
using UnityEngine.Events;

public class HealthPips : Pips, IHealth
{
    public int healthTotal = 0;
    public int health = 0;
    public UnityEvent<int> onTakeDamage;
    public UnityEvent<int, Identity> onTakeDamageFrom;
    public UnityEvent onNoHealth;
    
    // Pips
    private void RefreshPips()
    {
        int damage = Mathf.Min(healthTotal - health, healthTotal);
        if (isActiveAndEnabled)
            SetFilled(damage);
    }

    // IHealth
    public virtual void HealDamage(int amount)
    {
        health += amount;
        RefreshPips();
    }
    public virtual void SetHealth(int amount)
    {
        health = amount;
        RefreshPips();
    }
    public virtual void SetHealthBase(int amount, int total)
    {
        health = amount;
        SetHealthTotal(amount);
    }
    public virtual void SetHealthTotal(int amount)
    {
        healthTotal = amount;
        SetPipCount(healthTotal);
        SetHealth(Mathf.Min(healthTotal, health));
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Identity.Neutral);
    }
    public virtual void TakeDamage(int amount, Identity id = Identity.Neutral)
    {
        health -= amount;
        onTakeDamage.Invoke(amount);
        onTakeDamageFrom.Invoke(amount, id);
        if (health <= 0)
        {
            health = Mathf.Max(health, 0);
            onNoHealth.Invoke();
        }
        RefreshPips();
    }
}
