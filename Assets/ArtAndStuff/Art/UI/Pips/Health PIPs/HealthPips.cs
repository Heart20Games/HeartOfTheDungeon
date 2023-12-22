using Body.Behavior.ContextSteering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;
using UnityEngine.Events;

public class HealthPips : Pips, IHealth
{
    public int HealthTotal { get => totalPips; set => totalPips = value; }
    public int Health { get => filledPips; set => filledPips = value; }
    public UnityEvent<int> onTakeDamage;
    public UnityEvent<int, Identity> onTakeDamageFrom;
    public UnityEvent onNoHealth;

    // Pips
    private void RefreshPips()
    {
        int damage = Mathf.Min(HealthTotal - Health, HealthTotal);
        if (isActiveAndEnabled)
            SetFilled(damage);
    }

    // IHealth
    public virtual void HealDamage(int amount)
    {
        Health += amount;
        RefreshPips();
    }
    public virtual void SetHealth(int amount)
    {
        Health = amount;
        RefreshPips();
    }
    public virtual void SetHealthBase(int amount, int total)
    {
        Health = amount;
        SetHealthTotal(amount);
    }
    public virtual void SetHealthTotal(int amount)
    {
        HealthTotal = amount;
        SetPipCount(HealthTotal);
        SetHealth(Mathf.Min(HealthTotal, Health));
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Identity.Neutral);
    }
    public virtual void TakeDamage(int amount, Identity id = Identity.Neutral)
    {
        Health -= amount;
        onTakeDamage.Invoke(amount);
        onTakeDamageFrom.Invoke(amount, id);
        if (Health <= 0)
        {
            Health = Mathf.Max(Health, 0);
            onNoHealth.Invoke();
        }
        RefreshPips();
    }
}
