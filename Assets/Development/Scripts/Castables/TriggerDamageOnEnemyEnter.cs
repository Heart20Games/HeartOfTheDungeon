using UnityEngine;

public class TriggerDamageOnEnemyEnter : BaseMonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.gameObject.TryGetComponent<IDamageReceiver>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
