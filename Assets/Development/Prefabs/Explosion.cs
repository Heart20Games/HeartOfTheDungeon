using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Explosion: BaseMonoBehaviour
{
    public Identity identity = Identity.Neutral;
    public float radius = 5f;
    public float force = 10f;
    public int damage = 10;

    public bool triggerExplosion = false;
    public bool debug = false;
    
    private void Update()
    {
        if (triggerExplosion)
        {
            triggerExplosion = false;
            Explode();
        }
    }

    public void Explode()
    {
        Debug.DrawRay(transform.position, Vector3.forward * radius, Color.yellow, 1);
        Debug.DrawRay(transform.position, Vector3.right * radius, Color.yellow, 1);
        Debug.DrawRay(transform.position, Vector3.back * radius, Color.yellow, 1);
        Debug.DrawRay(transform.position, Vector3.left * radius, Color.yellow, 1);
        RaycastHit[] hits = Physics.SphereCastAll(new(transform.position, Vector3.forward), radius);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.TryGetComponent(out Rigidbody rigidbody))
            {
                Print($"Push them! {hit.collider.gameObject.name}", debug);
                rigidbody.AddExplosionForce(force, transform.position, radius);
            }
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                Print($"Damage them! {hit.collider.gameObject.name}", debug);
                damageable.SetDamagePosition(hit.point);
                damageable.TakeDamage(damage, identity);
            }
        }
    }
}
