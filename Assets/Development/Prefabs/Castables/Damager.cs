using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Damager : BaseMonoBehaviour
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

    public void HitDamagable(Impact impactor)
    {
        IDamageable other = impactor.other.GetComponent<IDamageable>();
        if (other != null && !ignored.Contains(other) && !others.Contains(other))
        {
            others.Add(other);
            other.TakeDamage(damage, identity);
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
}
