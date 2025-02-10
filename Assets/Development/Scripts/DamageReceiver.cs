using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IDestroyable
{
    UnityAction OnDestroyed { get; set; }
}

public interface IDamageReceiver: IDestroyable
{
    /// <summary>
    /// Represents some amount of damage take by some object with the given identity.
    /// </summary>
    /// <param name="amount">Amount of damage to take.</param>
    /// <param name="id">The identity of the thing that applied that damage.</param>
    void TakeDamage(int amount, Identity id = Identity.Neutral);
    /// <summary>
    /// Sets the position at which the is applied. (Used for visual effects and physics interactions and such)
    /// </summary>
    /// <param name="damagePosition">The position at which damage should be applied, in world coordinates.</param>
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
