using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Explosion: MonoBehaviour
{
    public Identity identity = Identity.Neutral;
    public float radius = 5f;
    public float force = 10f;
    public int damage = 10;

    public bool triggerExplosion = false;
    
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
                print($"Push them! {hit.collider.gameObject.name}");
                rigidbody.AddExplosionForce(force, transform.position, radius);
            }
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                print($"Damage them! {hit.collider.gameObject.name}");
                damageable.TakeDamage(damage, identity);
            }
        }
    }
}
