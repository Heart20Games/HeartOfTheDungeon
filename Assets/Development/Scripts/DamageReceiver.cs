using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IDestroyable
{
    UnityAction OnDestroyed { get; set; }
}

public interface IDamageReceiver: IDestroyable
{
    void TakeDamage(int amount, Identity id = Identity.Neutral);
    void SetDamagePosition(Vector3 damagePosition); // Expects world coordinates
}

public class DamageReceiver : BaseMonoBehaviour, IDamageReceiver
{
    public UnityEvent<int> onTakeDamage = new();
    public UnityEvent<int, Identity> onTakeDamageFrom = new();
    private UnityAction onDestroyed;

    public virtual UnityAction OnDestroyed { get => onDestroyed; set => onDestroyed = value; }

    public virtual void TakeDamage(int amount, Identity id=Identity.Neutral)
    {
        onTakeDamage?.Invoke(amount);
        onTakeDamageFrom?.Invoke(amount, id);
    }

    public virtual void SetDamagePosition(Vector3 location) { }
}
