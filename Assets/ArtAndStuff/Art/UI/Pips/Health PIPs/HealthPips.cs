using Body.Behavior.ContextSteering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;
using UnityEngine.Events;
using MyBox;
using static UnityEngine.Rendering.DebugUI;

[ExecuteAlways]
public class HealthPips : Pips, IHealth
{
    public int HealthTotal { get => TotalPips; set => TotalPips = value; }
    public int Health 
    {
        get { return FilledToHealth(); }
        set { SetHealth(value); }
    }
    [Foldout("Events", true)] public UnityEvent<int> onTakeDamage;
    public UnityEvent<int, Identity> onTakeDamageFrom;
    [Foldout("Events")] public UnityEvent onNoHealth;

    [Header("Health Pip Settings")]
    public bool useFilledPipsForDamage = false;

    // Pips
    [ButtonMethod]
    private void RefreshPips()
    {
        Health = Health;
    }

    // Filled vs Health Conversions
    public int FilledToHealth()
    {
        return useFilledPipsForDamage? TotalPips - FilledPips : FilledPips;
    }
    public int HealthToFilled(int value)
    {
        return useFilledPipsForDamage ? TotalPips - value : value;
    }

    // IHealth
    public virtual void HealDamage(int amount)
    {
        Health += amount;
    }

    public virtual void SetHealth(int amount) { SetHealth(amount, Mool.Maybe); }
    public virtual void SetHealth(int amount, Mool alwaysReport)
    {
        SetFilled(HealthToFilled(amount), alwaysReport);
    }

    public virtual void SetHealthBase(int total) { SetHealthBase(total, total); }
    public virtual void SetHealthBase(int total, int amount)
    {
        Health = amount;
        SetHealthTotal(amount);
    }
    public virtual void SetHealthTotal(int amount)
    {
        HealthTotal = amount;
        //SetPipCount(HealthTotal);
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
    }
}
