using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IDamageable
{
    void TakeDamage(int amount, Identity id=Identity.Neutral);
    void SetDamagePosition(Vector3 damagePosition); // Expects world coordinates
}