using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Damager : BaseMonoBehaviour, IDamager
{
    private readonly List<IDamageable> others = new();
    private readonly List<IDamageable> ignored = new();
    public int damage = 1;
    public Identity identity = Identity.Neutral;
    private Impact _impactor;
    [ReadOnly][SerializeField] private int otherCount = 0;
    [ReadOnly][SerializeField] private int ignoredCount = 0;

    public bool debug = false;

    public Impact _Impactor
    {
        get => _impactor;
        set => _impactor = value;
    }

    public void SetIdentity(Identity identity)
    {
        this.identity = identity;
    }

    public void Ignore(Transform toIgnore)
    {
        if (toIgnore.TryGetComponent<IDamageable>(out var damageable))
        {
            ignored.Add(damageable);
            ignoredCount = ignored.Count;
        }
    }

    public void HitDamageable(Impact impactor)
    {
        if (impactor != null)
        {
            IDamageable other = impactor.other.GetComponent<IDamageable>();
            
            Print($"Hit Damageable: {other}", debug, this);

            if(impactor._Character != null)
            {
                if (!impactor._Character.Alive) return;
            }

            if (other != null && !ignored.Contains(other) && !others.Contains(other))
            {
                Print("Doing stuff with damageable.", debug, this);

                others.Add(other);
                otherCount = others.Count;
                other.SetDamagePosition(impactor.impactLocation);
                other.TakeDamage(damage, identity);
            }
        }
        else
        {
            Debug.LogWarning("Impactor is null.", this);
        }
    }

    public void LeftDamageable(Impact impactor)
    {
        if (impactor != null)
        {
            IDamageable other = impactor.other.GetComponent<IDamageable>();

            Print($"Left Damageable: {other}", debug, this);

            if (other != null && !ignored.Contains(other) && others.Contains(other))
            {
                Print("Removing Damageable.", debug, this);

                others.Remove(other);
                otherCount = others.Count;
            }
        }
        else
        {
            Debug.LogWarning("Impactor is null.", this);
        }
    }

    public void DamageTarget()
    {
        if (_impactor == null) return;

        IDamageable other = _impactor.GetComponent<IDamageable>();

        if (_impactor._Character != null)
        {
            if (_impactor._Character.CurrentHealth <= 0) return;
        }

        if (other != null)
        {
            others.Add(other);
            otherCount = others.Count;
            other.SetDamagePosition(_impactor.impactLocation);
            other.TakeDamage(damage, identity);
        }
    }

    public void SpawnDamagerEffect(GameObject effect)
    {
        Transform childTransform = transform.GetChild(0);

        GameObject go = Instantiate(effect, childTransform);

        go.GetComponent<VisualEffect>().Play();

        Destroy(go, 2f);
    }
}
