using Body.Behavior.ContextSteering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;
using UnityEngine.Events;
using MyBox;

[ExecuteAlways]
public class HealthPips : Pips, IHealth
{
    public int HealthTotal { get => totalPips; set => totalPips = value; }
    public int Health 
    {
        get { return useFilledPipsForDamage ? totalPips - filledPips : filledPips; }
        set { filledPips = useFilledPipsForDamage ? totalPips - value : value; }
    }
    [Foldout("Events")] public UnityEvent<int> onTakeDamage;
    [Foldout("Events")] public UnityEvent<int, Identity> onTakeDamageFrom;
    [Foldout("Events")] public UnityEvent onNoHealth;

    public bool useFilledPipsForDamage = false;

    // Pips
    [ButtonMethod]
    private void RefreshPips()
    {
        int damage = Mathf.Min(HealthTotal - (HealthTotal-Health), HealthTotal);
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

    public virtual void SetHealthBase(int total) { SetHealthBase(total, total); }
    public virtual void SetHealthBase(int total, int amount)
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
