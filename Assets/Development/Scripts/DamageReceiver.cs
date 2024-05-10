using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

public class DamageReceiver : BaseMonoBehaviour, IDamageable
{
    public UnityEvent<int> onTakeDamage;
    public UnityEvent<int, Identity> onTakeDamageFrom;

    public virtual void TakeDamage(int amount, Identity id=Identity.Neutral)
    {
        onTakeDamage.Invoke(amount);
        onTakeDamageFrom.Invoke(amount, id);
    }

    public virtual void SetDamagePosition(Vector3 location) { }
}
