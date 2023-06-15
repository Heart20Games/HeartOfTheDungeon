using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Damager : BaseMonoBehaviour
{
    private readonly List<IDamageable> others = new();
    private readonly List<IDamageable> ignored = new();
    public int damage = 1;

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
}
