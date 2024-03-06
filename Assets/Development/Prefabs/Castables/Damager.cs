using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Damager : BaseMonoBehaviour, IDamager
{
    private readonly List<IDamageable> others = new();
    private readonly List<IDamageable> ignored = new();
    public int damage = 1;
    public Identity identity = Identity.Neutral;

    public void SetIdentity(Identity identity)
    {
        this.identity = identity;
    }

    public void Ignore(Transform toIgnore)
    {
        if (toIgnore.TryGetComponent<IDamageable>(out var damageable))
        {
            ignored.Add(damageable);
        }
    }

    public void HitDamageable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && !ignored.Contains(other) && !others.Contains(other))
        {
            others.Add(other);
            other.SetDamagePosition(impactor.impactLocation);
            other.TakeDamage(damage, identity);
        }
    }

    public void LeftDamageable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && !ignored.Contains(other) && others.Contains(other))
        {
            others.Remove(other);
        }
    }
}
