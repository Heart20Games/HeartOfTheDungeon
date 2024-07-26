using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IDamager
{
    public void HitDamageable(Impact impactor);
    public void LeftDamageable(Impact impactor);
}

public class Damager : BaseMonoBehaviour, IDamager
{
    private readonly List<IDamageReceiver> others = new();
    private readonly List<IDamageReceiver> ignored = new();
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
        if (toIgnore.TryGetComponent<IDamageReceiver>(out var damageable))
        {
            ignored.Add(damageable);
            ignoredCount = ignored.Count;
        }
    }

    public void HitDamageable(Impact impactor)
    {
        if (impactor != null)
        {
            IDamageReceiver other = impactor.other.gameObject.GetComponent<IDamageReceiver>();
            
            Print($"Hit Damageable: {other}", debug, this);
            if(impactor._Character != null)
            {
                if (!impactor._Character.Alive) return;
            }

            if (other != null && !ignored.Contains(other) && !others.Contains(other))
            {
                Print("Doing stuff with damageable.", debug, this);
                //others.Add(other);
                otherCount = others.Count;
                other.SetDamagePosition(impactor.other.ImpactLocation);
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
            IDamageReceiver other = impactor.other.gameObject.GetComponent<IDamageReceiver>();

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

        IDamageReceiver other = _impactor.GetComponent<IDamageReceiver>();

        if (_impactor._Character != null)
        {
            if (_impactor._Character.CurrentHealth <= 0) return;
        }

        if (other != null)
        {
            others.Add(other);
            otherCount = others.Count;
            other.SetDamagePosition(_impactor.other.ImpactLocation);
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
